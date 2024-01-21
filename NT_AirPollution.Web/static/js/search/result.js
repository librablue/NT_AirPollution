document.addEventListener('DOMContentLoaded', () => {
    new Vue({
        el: '#app',
        filters: {
            date: value => {
                if (!value || value === '0001-01-01T00:00:00') return '';
                return moment(value).format('YYYY-MM-DD');
            },
            formStatus: value => {
                switch (value) {
                    case 1:
                        return '審理中';
                    case 2:
                        return '需補件';
                    case 3:
                        return '通過待繳費';
                    case 4:
                        return '繳費完成';
                    default:
                        return '';
                }
            },
            calcStatus: value => {
                switch (value) {
                    case 0:
                        return '未申請';
                    case 1:
                        return '審理中';
                    case 2:
                        return '需補件';
                    case 3:
                        return '通過待繳費';
                    case 4:
                    case 5:
                        return '通過待退費';
                    case 6:
                        return '繳退費完成';
                    default:
                        return '';
                }
            }
        },
        data() {
            const checkE_DATE2 = (rule, value, callback) => {
                if (!value) {
                    callback(new Error('請輸入預計施工完成日期'));
                }
                if (this.selectRow.B_DATE2 && this.selectRow.E_DATE2 && moment(value).isSameOrBefore(this.selectRow.B_DATE2)) {
                    callback(new Error('結束日期不得早於起始日期'));
                }
                callback();
            };
            const checkArea = (rule, value, callback) => {
                if (!this.selectRow.VOLUMEL && !value) {
                    callback(new Error('如果非疏濬工程，請輸入施工面積'));
                }
                callback();
            };
            const checkVolumel = (rule, value, callback) => {
                if (!this.selectRow.AREA && !value) {
                    callback(new Error('如果為疏濬工程，請輸入清運土石體積'));
                }
                callback();
            };
            return {
                sendText: '寄送驗證信',
                sending: false,
                district: Object.freeze([]),
                projectCode: Object.freeze([]),
                companies: Object.freeze([]),
                selectCompany: null,
                contractors: Object.freeze([]),
                selectContractor: null,
                forms: [],
                selectRow: {
                    P_KIND: '一次全繳',
                    BUD_DOC2: '無',
                    Attachment: {},
                    StopWorks: [],
                    RefundBank: {}
                },
                banks: Object.freeze(banksAry),
                dialogVisible: false,
                failReasonDialogVisible: false,
                bankAccountDialogVisible: false,
                paymentProofModalVisible: false,
                activeTab: 'first',
                rules1: Object.freeze({
                    PUB_COMP: [{ required: true, message: '請選擇案件類型', trigger: 'change' }],
                    TOWN_NO: [{ required: true, message: '請選擇鄉鎮分類', trigger: 'change' }],
                    CreateUserName: [{ required: true, message: '請輸入申請人姓名', trigger: 'blur' }],
                    CreateUserEmail: [{ required: true, message: '請輸入申請人電子信箱', trigger: 'blur' }],
                    COMP_NAM: [{ required: true, message: '請輸入工程名稱', trigger: 'blur' }],
                    KIND_NO: [{ required: true, message: '請選擇工程類別', trigger: 'change' }],
                    ADDR: [{ required: true, message: '請輸入工地地址或地號', trigger: 'blur' }],
                    B_SERNO: [{ required: true, message: '請輸入建照字號或合約編號', trigger: 'blur' }],
                    UTME: [{ required: true, message: '請輸入座標X', trigger: 'blur' }],
                    UTMN: [{ required: true, message: '請輸入座標Y', trigger: 'blur' }],
                    LATLNG: [{ required: true, message: '請輸入座標(緯度、經度)', trigger: 'blur' }],
                    STATE: [{ required: true, message: '請輸入工程內容概述', trigger: 'blur' }],
                    EIACOMMENTS: [{ required: true, message: '請輸入環評保護對策', trigger: 'blur' }],
                    S_NAME: [{ required: true, message: '請輸入營建業主名稱', trigger: 'blur' }],
                    S_G_NO: [{ required: true, message: '請輸入營利事業統一編號', trigger: 'blur' }],
                    S_ADDR1: [{ required: true, message: '請輸入營利事業營業地址', trigger: 'blur' }],
                    S_ADDR2: [{ required: true, message: '請輸入營利事業聯絡地址', trigger: 'blur' }],
                    S_TEL: [{ required: true, message: '請輸入營利事業連絡電話', trigger: 'blur' }],
                    S_B_NAM: [{ required: true, message: '請輸入營利事業負責人姓名', trigger: 'blur' }],
                    S_B_TIT: [{ required: true, message: '請輸入營利事業負責人職稱', trigger: 'blur' }],
                    S_B_ID: [{ required: true, message: '請輸入營利事業負責人身分證字號', trigger: 'blur' }],
                    S_B_BDATE2: [{ required: true, message: '請輸入營利事業負責人生日', trigger: 'blur' }],
                    S_C_NAM: [{ required: true, message: '請輸入營利事業聯絡人姓名', trigger: 'blur' }],
                    S_C_TIT: [{ required: true, message: '請輸入營利事業聯絡人職稱', trigger: 'blur' }],
                    S_C_ID: [{ required: true, message: '請輸入營利事業聯絡人身分證字號', trigger: 'blur' }],
                    S_C_ADDR: [{ required: true, message: '請輸入營利事業聯絡人地址', trigger: 'blur' }],
                    S_C_TEL: [{ required: true, message: '請輸入營利事業聯絡人電話', trigger: 'blur' }],
                    R_NAME: [{ required: true, message: '請輸入承包(造)單位名稱', trigger: 'blur' }],
                    R_G_NO: [{ required: true, message: '請輸入承包(造)營利事業統一編號', trigger: 'blur' }],
                    R_ADDR1: [{ required: true, message: '請輸入承包(造)營業地址', trigger: 'blur' }],
                    R_ADDR2: [{ required: true, message: '請輸入承包(造)聯絡地址', trigger: 'blur' }],
                    R_TEL: [{ required: true, message: '請輸入承包(造)連絡電話', trigger: 'blur' }],
                    R_B_NAM: [{ required: true, message: '請輸入承包(造)負責人姓名', trigger: 'blur' }],
                    R_B_TIT: [{ required: true, message: '請輸入承包(造)負責人職稱', trigger: 'blur' }],
                    R_B_ID: [{ required: true, message: '請輸入承包(造)負責人身分證字號', trigger: 'blur' }],
                    R_B_BDATE2: [{ required: true, message: '請輸入承包(造)負責人生日', trigger: 'blur' }],
                    R_ADDR3: [{ required: true, message: '請輸入工務所地址', trigger: 'blur' }],
                    R_M_NAM: [{ required: true, message: '請輸入工地主任姓名', trigger: 'blur' }],
                    R_C_NAM: [{ required: true, message: '請輸入工地環保負責人姓名', trigger: 'blur' }],
                    R_TEL1: [{ required: true, message: '請輸入工務所電話', trigger: 'blur' }],
                    MONEY: [{ required: true, message: '請輸入工程合約經費', trigger: 'blur' }],
                    C_MONEY: [{ required: true, message: '請輸入工程環保經費', trigger: 'blur' }],
                    PERCENT: [{ required: true, message: '請輸入工程合約經費比例', trigger: 'blur' }],
                    AREA: [{ validator: checkArea }],
                    VOLUMEL: [{ validator: checkVolumel }],
                    B_DATE2: [{ required: true, message: '請輸入預計施工開始日期', trigger: 'blur' }],
                    E_DATE2: [{ validator: checkE_DATE2 }]
                }),
                rules2: Object.freeze({
                    Code: [{ required: true, message: '請選擇銀行代碼', trigger: 'change' }],
                    Account: [{ required: true, message: '請輸入銀行帳號', trigger: 'blur' }],
                    File: [{ required: true, message: '請上傳存摺照片' }]
                }),
                rules3: Object.freeze({
                    File: [{ required: true, message: '請上傳繳費證明' }]
                })
            };
        },
        mounted() {
            this.getDistrict();
            this.getProjectCode();
            this.getMyForm();
        },
        computed: {
            totalDays() {
                if (!this.selectRow.B_DATE2 || !this.selectRow.E_DATE2) return '';
                var date1 = new Date(this.selectRow.B_DATE2);
                var date2 = new Date(this.selectRow.E_DATE2);

                // 計算毫秒差異
                var diff = Math.abs(date2 - date1 + 1000 * 60 * 60 * 24);
                // 轉換為天數
                var dayDiff = Math.ceil(diff / (1000 * 60 * 60 * 24));

                return dayDiff;
            }
        },
        methods: {
            resend() {
                axios
                    .post('/Search/Resend')
                    .then(res => {
                        this.sending = true;
                        var seconds = 180;
                        this.sendText = '重新寄送(' + seconds + ')';
                        seconds -= 1;
                        var id = setInterval(() => {
                            this.sendText = '重新寄送(' + seconds + ')';
                            seconds--;
                            if (seconds < 0) {
                                clearInterval(id);
                                this.sending = false;
                                this.sendText = '寄送驗證信';
                            }
                        }, 1000);
                    })
                    .catch(err => {
                        console.log(err);
                        alert('發生錯誤');
                    });
            },
            getDistrict() {
                axios.get('/Option/GetDistrict').then(res => {
                    this.district = Object.freeze(res.data);
                });
            },
            getProjectCode() {
                axios.get('/Option/GetProjectCode').then(res => {
                    this.projectCode = Object.freeze(res.data);
                });
            },
            getMyForm() {
                const loading = this.$loading();
                axios
                    .get('/Search/GetMyForm')
                    .then(res => {
                        this.forms = [res.data];
                        this.selectRow = res.data;
                        loading.close();
                    })
                    .catch(err => {
                        loading.close();
                        console.log(err);
                    });
            },
            showModal(row) {
                this.dialogVisible = true;
                if (this.selectRow.FormStatus === 2) {
                    this.failReasonDialogVisible = true;
                }
            },
            deleteFile(idx) {
                if (!confirm('是否確認刪除?')) return;
                this.selectRow.Attachment[`File${idx}`] = null;
            },
            sendForm() {
                this.$refs.form1.validate(valid => {
                    if (!valid) {
                        alert('欄位驗證錯誤，請檢查修正後重新送出');
                        return false;
                    }

                    if (!confirm('是否確認繼續?')) return false;
                    const formData = new FormData();
                    for (const key in this.selectRow) {
                        if (typeof this.selectRow[key] !== 'object') formData.append(key, this.selectRow[key]);
                    }
                    // 附件
                    for (let i = 1; i <= 8; i++) {
                        const file = document.querySelector(`#file${i}`);
                        if (file && file.files.length > 0) formData.append(`file${i}`, file.files[0]);
                    }

                    axios
                        .post('/Search/UpdateForm', formData)
                        .then(res => {
                            if (!res.data.Status) {
                                alert(res.data.Message);
                                return;
                            }

                            alert('申請資料已送出，繳款金額請依人工審核後之繳費單內容為主。');
                            this.getMyForm();
                            this.dialogVisible = false;
                        })
                        .catch(err => {
                            alert('系統發生未預期錯誤');
                            console.log(err);
                        });
                });
            },
            getStopDays(row) {
                if (!row.DOWN_DATE2 || !row.UP_DATE2) return '';
                var date1 = new Date(row.DOWN_DATE2);
                var date2 = new Date(row.UP_DATE2);

                // 計算毫秒差異
                var diff = Math.abs(date2 - date1 + 1000 * 60 * 60 * 24);
                // 轉換為天數
                var dayDiff = Math.ceil(diff / (1000 * 60 * 60 * 24));

                return dayDiff;
            },
            dialogClose() {
                // 清空附件
                for (let i = 1; i <= 8; i++) {
                    const file = document.querySelector(`#file${i}`);
                    if (file) file.value = '';
                }
            },
            beforeCommand(row, cmd) {
                return {
                    row,
                    cmd
                };
            },
            handleCommand(arg) {
                const { row, cmd } = arg;
                switch (cmd) {
                    case 'VIEW':
                        this.showModal(row);
                        break;
                    case 'COPY':
                        this.copyRow(row);
                        break;
                    case 'DOWNLOAD_PAYMENT':
                        this.downloadPayment(row);
                        break;
                    case 'CALC':
                        this.finalCalc(row);
                        break;
                    case 'DOWNLOAD_REPAYMENT':
                        this.downloadRePayment(row);
                        break;
                    case 'UPLOAD_PAYMENT_PROOF':
                        this.showPaymentProofModal(row);
                        break;
                    case 'BANK_ACCOUNT':
                        this.showBankAccountModal(row);
                        break;
                    case 'DOWNLOAD_PROOF':
                        this.downloadProof(row);
                        break;
                }
            },
            downloadPayment(row) {
                const loading = this.$loading();
                axios
                    .post('/Form/DownloadPayment', row, {
                        responseType: 'blob'
                    })
                    .then(res => {
                        loading.close();
                        const url = window.URL.createObjectURL(new Blob([res.data]));
                        const link = document.createElement('a');
                        link.href = url;
                        const fileName = decodeURI(res.headers['file-name']);
                        link.setAttribute('download', fileName);
                        document.body.appendChild(link);
                        link.click();
                        link.remove();
                    })
                    .catch(err => {
                        loading.close();
                        alert('系統發生未預期錯誤');
                        console.log(err);
                    });
            },
            finalCalc(row) {
                if (!confirm('是否確認提出結算申請?')) return;
                axios
                    .post('/Form/FinalCalc', row)
                    .then(res => {
                        if (!res.data.Status) {
                            alert(res.data.Message);
                            return;
                        }

                        alert('結算申請已送出，請等候人工審核後 Email 通知');
                        row.CalcStatus = 1;
                    })
                    .catch(err => {
                        alert('系統發生未預期錯誤');
                        console.log(err);
                    });
            },
            downloadRePayment(row) {
                const loading = this.$loading();
                axios
                    .post('/Form/DownloadRePayment', row, {
                        responseType: 'blob'
                    })
                    .then(res => {
                        loading.close();
                        const url = window.URL.createObjectURL(new Blob([res.data]));
                        const link = document.createElement('a');
                        link.href = url;
                        const fileName = decodeURI(res.headers['file-name']);
                        link.setAttribute('download', fileName);
                        document.body.appendChild(link);
                        link.click();
                        link.remove();
                    })
                    .catch(err => {
                        loading.close();
                        alert('系統發生未預期錯誤');
                        console.log(err);
                    });
            },
            showPaymentProofModal(row) {
                this.selectRow = JSON.parse(JSON.stringify(row));
                this.selectRow.PaymentProof = Object.assign({}, row.PaymentProof, {
                    FormID: row.ID,
                    File: null
                });
                this.paymentProofModalVisible = true;
            },
            deletePaymentProof() {
                if (!confirm('是否確認刪除?')) return false;
                this.selectRow.PaymentProof.ProofFile = null;
            },
            savePaymentProof() {
                const formData = new FormData();
                // 附件
                const file = document.querySelector(`#filePF`);
                if (file && file.files.length > 0) {
                    formData.append('file', file.files[0]);
                    this.selectRow.PaymentProof.File = file.files[0].name;
                }

                this.$refs.form3.validate((valid, object) => {
                    if (!valid) {
                        alert('欄位驗證錯誤，請檢查修正後重新送出');
                        return false;
                    }

                    if (!confirm('是否確認繼續?')) return false;

                    const loading = this.$loading();
                    for (const key in this.selectRow.PaymentProof) {
                        if (typeof this.selectRow.PaymentProof[key] !== 'object') formData.append(key, this.selectRow.PaymentProof[key]);
                    }

                    axios
                        .post('/Form/UploadPaymentProof', formData)
                        .then(res => {
                            loading.close();
                            if (!res.data.Status) {
                                alert(res.data.Message);
                                return;
                            }

                            alert('上傳成功');
                            this.getForms();
                            this.paymentProofModalVisible = false;
                        })
                        .catch(err => {
                            loading.close();
                            alert('系統發生未預期錯誤');
                            console.log(err);
                        });
                });
            },
            showBankAccountModal(row) {
                this.selectRow = JSON.parse(JSON.stringify(row));
                this.selectRow.RefundBank = Object.assign({}, row.RefundBank, {
                    FormID: row.ID,
                    File: null
                });
                this.bankAccountDialogVisible = true;
            },
            deleteBankPhoto() {
                if (!confirm('是否確認刪除?')) return false;
                this.selectRow.RefundBank.Photo = null;
            },
            saveBankAccount() {
                const formData = new FormData();
                // 附件
                const file = document.querySelector(`#fileBA`);
                if (file && file.files.length > 0) {
                    formData.append('file', file.files[0]);
                    this.selectRow.RefundBank.File = file.files[0].name;
                }
                this.$refs.form2.validate((valid, object) => {
                    if (!valid) {
                        alert('欄位驗證錯誤，請檢查修正後重新送出');
                        return false;
                    }

                    if (!confirm('是否確認繼續?')) return false;

                    const loading = this.$loading();
                    for (const key in this.selectRow.RefundBank) {
                        if (typeof this.selectRow.RefundBank[key] !== 'object') formData.append(key, this.selectRow.RefundBank[key]);
                    }

                    axios
                        .post('/Form/UpdateBankAccount', formData)
                        .then(res => {
                            loading.close();
                            if (!res.data.Status) {
                                alert(res.data.Message);
                                return;
                            }

                            alert('退款帳戶資料已儲存');
                            this.getForms();
                            this.bankAccountDialogVisible = false;
                        })
                        .catch(err => {
                            loading.close();
                            alert('系統發生未預期錯誤');
                            console.log(err);
                        });
                });
            },
            downloadProof(row) {
                const loading = this.$loading();
                axios
                    .post('/Form/DownloadProof', row, {
                        responseType: 'blob'
                    })
                    .then(res => {
                        loading.close();
                        const url = window.URL.createObjectURL(new Blob([res.data]));
                        const link = document.createElement('a');
                        link.href = url;
                        const fileName = decodeURI(res.headers['file-name']);
                        link.setAttribute('download', fileName);
                        document.body.appendChild(link);
                        link.click();
                        link.remove();
                    })
                    .catch(err => {
                        loading.close();
                        alert('系統發生未預期錯誤');
                        console.log(err);
                    });
            }
        }
    });
});
