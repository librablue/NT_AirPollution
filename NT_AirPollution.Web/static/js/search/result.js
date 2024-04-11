﻿document.addEventListener('DOMContentLoaded', () => {
    var A1_84 = 6378137.0;
    var B1_84 = 6356752.3141;
    var e2_84 = 0.0066943800355;
    var A1_67 = 6378160.0;
    var B1_67 = 6356774.7192;
    var e2_67 = 0.006694541853;
    var SK = new Array(0.9999, 1.0, 0.9996);
    var Cent = new Array(121.0, 121.0, 123.0);
    var Shift = new Array(250000.0, 350000.0, 500000.0);
    var DegreePI = 180.0 / Math.PI;

    var XM84 = -3033819.548;
    var YM84 = 5071301.969;
    var ZM84 = 2391982.06;
    var DeltaX84 = 754.812;
    var DeltaY84 = 362.233;
    var DeltaZ84 = 187.197;
    var EpsilonX84 = -5.216 / 3600.0 / DegreePI;
    var EpsilonY84 = -8.846 / 3600.0 / DegreePI;
    var EpsilonZ84 = 35.781 / 3600.0 / DegreePI;
    var S84 = 3.161e-6;

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
                district: Object.freeze([]),
                projectCode: Object.freeze([]),
                attachmentInfo: Object.freeze([]),
                companies: Object.freeze([]),
                selectCompany: null,
                contractors: Object.freeze([]),
                selectContractor: null,
                forms: [],
                selectRow: {
                    P_KIND: '一次全繳',
                    BUD_DOC2: '無',
                    Attachments: [],
                    StopWorks: [],
                    RefundBank: {},
                    PaymentProof: {}
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
            this.getAttachmentInfo();
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
            },
            filterAttachmentInfo() {
                return this.attachmentInfo.filter(item => item.PUB_COMP === this.selectRow.PUB_COMP);
            }
        },
        methods: {
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
            getAttachmentInfo() {
                axios.get('/Option/GetAttachmentInfo').then(res => {
                    this.attachmentInfo = Object.freeze(res.data);
                });
            },
            filterAttachments(infoID) {
                return this.selectRow.Attachments.filter(item => item.InfoID === infoID);
            },
            getMyForm() {
                const loading = this.$loading();
                axios
                    .get('/Search/GetMyForm')
                    .then(res => {
                        this.forms = res.data;
                        loading.close();
                    })
                    .catch(err => {
                        loading.close();
                        console.log(err);
                    });
            },
            showModal(row) {
                this.selectRow = row;
                this.dialogVisible = true;
                if (this.selectRow.FormStatus === 2) {
                    this.failReasonDialogVisible = true;
                }
            },
            LatLon2UTMHandler() {
                const latlng = this.selectRow.LATLNG.split(',');
                if (latlng.length !== 2 || !latlng[0] || !latlng[1]) {
                    alert('經緯度座標格式錯誤，格式範例如：121.0000,23.0000');
                    return;
                }
                this.LatLon2UTM(latlng[1], latlng[0], 0, 0);
            },
            UTM2LatLonHandler() {
                if (!this.selectRow.UTME) {
                    alert('請輸入X座標');
                    return;
                }
                if (!this.selectRow.UTMN) {
                    alert('請輸入Y座標');
                    return;
                }
                this.UTM2LatLon(this.selectRow.UTME, this.selectRow.UTMN, 0, 0);
            },
            LatLon2UTM(Lat, Lon, AreaCode, Flag) {
                var E1, E2, PH2, LM2, PH1, LM1, Temp1, Temp2, Temp3, Temp4;
                var Temp5, Temp6, Temp7;
                var RAP, UU, N, T;
                var A, B, C, D, E, PM, PH;
                var A1, B1;

                if (Flag == 0) {
                    A1 = A1_84;
                    B1 = B1_84;
                } else {
                    A1 = A1_67;
                    B1 = B1_67;
                }

                E1 = 1.0 - (B1 / A1) * (B1 / A1);
                E2 = (A1 / B1) * (A1 / B1) - 1.0;

                PH2 = Lat / DegreePI;
                LM2 = Lon / DegreePI;
                LM1 = Cent[AreaCode] / DegreePI;
                PH1 = 0.0;

                Temp1 = Math.sin(PH2);
                Temp2 = Math.cos(PH2);
                RAP = LM2 - LM1;
                UU = E2 * Temp2 * Temp2;
                Temp3 = 1.0 - E1 * Temp1 * Temp1;
                N = A1 / Math.sqrt(Temp3);
                T = Temp1 / Temp2;
                Temp4 = N * Temp2 * RAP;
                Temp5 = (N * (1.0 - T * T + UU) * Math.pow(Temp2, 3.0) * Math.pow(RAP, 3.0)) / 6.0;
                Temp6 = 5.0 - 18.0 * Math.pow(T, 2.0) + Math.pow(T, 4.0) + 14.0 * UU - 58.0 * UU * T * T;
                Temp7 = (N * Temp6 * Math.pow(Temp2, 5.0) * Math.pow(RAP, 5.0)) / 120.0;

                LM2 = Temp4 + Temp5 + Temp7;
                LM2 = SK[AreaCode] * LM2 + Shift[AreaCode];

                Temp4 = Math.pow(E1, 2.0);
                Temp5 = Math.pow(E1, 3.0);
                Temp6 = Math.pow(E1, 4.0);
                A = 1.0 + 0.75 * E1 + (45.0 * Temp4) / 64.0 + (175.0 * Temp5) / 256.0 + (11025.0 * Temp6) / 16384.0;
                B = 0.75 * E1 + (15.0 * Temp4) / 16.0 + (525.0 * Temp5) / 512.0 + (2205.0 * Temp6) / 2048.0;
                C = (15.0 * Temp4) / 64.0 + (105.0 * Temp5) / 256.0 + (2205.0 * Temp6) / 4096.0;
                D = (35.0 * Temp5) / 512.0 + (315.0 * Temp6) / 2048.0;
                E = (315.0 * Temp6) / 16384.0;

                Temp4 = A * (PH2 - PH1) - (B * (Math.sin(2.0 * PH2) - Math.sin(2.0 * PH1))) / 2.0;
                Temp5 = C * (Math.sin(4.0 * PH2) - Math.sin(4.0 * PH1));
                Temp6 = D * (Math.sin(6.0 * PH2) - Math.sin(6.0 * PH1));
                Temp7 = E * (Math.sin(8.0 * PH2) - Math.sin(8.0 * PH1));

                PM = A1 * (1.0 - E1) * (Temp4 + Temp5 / 4.0 + Temp6 / 6.0 + Temp7 / 8.0);

                Temp4 = (N * T * Math.pow(RAP, 2.0) * Math.pow(Temp2, 2.0)) / 2.0;
                Temp5 = 5.0 - Math.pow(T, 2.0) + 9.0 * UU + 4.0 * Math.pow(UU, 2.0);
                Temp6 = (Math.pow(Temp2, 4.0) * Math.pow(RAP, 4.0)) / 24.0;
                Temp7 = 61.0 - 58.0 * T * T + Math.pow(T, 4.0) + 270.0 * UU - 330.0 * UU * T * T;

                PH = PM + Temp4 + N * T * Temp5 * Temp6 + (N * T * Math.pow(PH2, 6.0) * Math.pow(RAP, 6.0) * Temp7) / 720.0;

                PH2 = SK[AreaCode] * PH;

                this.selectRow.UTME = LM2.toFixed(0);
                this.selectRow.UTMN = PH2.toFixed(0);
            },
            UTM2LatLon(X, Y, AreaCode, Flag) {
                var N, E1, CN, CE, PH2, PH1, DS, RM, DPH;
                var Temp1, Temp2, Temp3, T, R, UU;
                var LM2, S, A1, B1;
                var i, IPAT;

                if (Flag == 0) {
                    A1 = A1_84;
                    B1 = B1_84;
                } else {
                    A1 = A1_67;
                    B1 = B1_67;
                }

                E1 = 1.0 - (B1 / A1) * (B1 / A1);
                CN = Y;
                CE = X;
                CN /= SK[AreaCode];
                CE = (CE - Shift[AreaCode]) / SK[AreaCode];
                PH2 = 0.0;
                IPAT = 1200;
                DS = CN / IPAT;

                for (i = 1; i <= IPAT; i++) {
                    Temp1 = Math.pow(Math.sin(PH2), 2.0);
                    Temp2 = 1.0 - E1 * Temp1;
                    Temp3 = Math.pow(Temp2, 1.5);
                    RM = (A1 * (1.0 - E1)) / Temp3;
                    DPH = DS / (2.0 * RM);
                    PH1 = PH2 + DPH;
                    Temp1 = Math.pow(Math.sin(PH1), 2.0);
                    Temp2 = 1.0 - E1 * Temp1;
                    Temp3 = Math.pow(Temp2, 1.5);
                    RM = (A1 * (1.0 - E1)) / Temp3;
                    DPH = DS / RM;
                    PH2 = PH2 + DPH;
                }

                T = Math.sin(PH2) / Math.cos(PH2);
                Temp1 = Math.pow(Math.sin(PH2), 2.0);
                Temp2 = 1.0 - E1 * Temp1;
                Temp3 = Math.pow(Temp2, 1.5);
                R = (A1 * (1.0 - E1)) / Temp3;
                Temp3 = Math.pow(Temp2, 0.5);
                N = A1 / Temp3;
                UU = N / R;
                S = 1.0 / Math.cos(PH2);
                Temp1 = (T * Math.pow(CE, 2.0)) / (2.0 * R * N);
                Temp2 = (T * Math.pow(CE, 4.0) * (-4.0 * Math.pow(UU, 2.0) + 9.0 * (1.0 - T * T) + 12.0 * T * T)) / (24.0 * R * Math.pow(N, 3.0));
                Temp3 =
                    (T * Math.pow(CE, 6.0) * (8.0 * Math.pow(UU, 4.0) * (11.0 - 24.0 * T * T) - 12.0 * Math.pow(UU, 3.0) * (21.0 - 71.0 * T * T) + 15.0 * Math.pow(UU, 2.0) * (15.0 - 98.0 * T * T + 15.0 * Math.pow(T, 4)) + 180.0 * UU * (5.0 * T * T - 3.0 * Math.pow(T, 4.0)) + 360.0 * Math.pow(T, 4.0))) /
                    (720 * R * Math.pow(N, 5.0));
                PH2 = PH2 - Temp1 + Temp2 - Temp3;

                var Lat = PH2 * DegreePI;

                Temp1 = (S * CE) / N;
                Temp2 = (S * Math.pow(CE, 3.0) * (UU + 2.0 * T * T)) / (6 * Math.pow(N, 3.0));
                Temp3 = (S * Math.pow(CE, 5.0) * (-4.0 * Math.pow(UU, 3.0) * (1.0 - 6.0 * T * T) + UU * UU * (9.0 - 68.0 * T * T) + 72.0 * UU * T * T + 24.0 * Math.pow(T, 4.0))) / (120.0 * Math.pow(N, 5.0));
                LM2 = Temp1 - Temp2 + Temp3;
                var Lon = 121 + LM2 * DegreePI;

                this.selectRow.LATLNG = `${Lon.toFixed(5)},${Lat.toFixed(5)}`;
            },
            addFile(infoID) {
                const lastID = this.selectRow.Attachments.length === 0 ? 0 : this.selectRow.Attachments[this.selectRow.Attachments.length - 1].ID;
                this.selectRow.Attachments.push({
                    ID: lastID + 1,
                    InfoID: infoID,
                    FileName: null
                });
            },
            deleteFile(row) {
                if (!confirm('是否確認刪除?')) return;
                const findIdx = this.selectRow.Attachments.findIndex((item, idx) => item.ID === row.ID);
                this.selectRow.Attachments.splice(findIdx, 1);
            },
            uploadAttachment(e, row) {
                if (e.target.files.length === 0) {
                    alert('請選擇檔案');
                    return false;
                }

                const ext = e.target.files[0].name.split('.').pop();
                const allowExt = ['doc', 'docx', 'pdf', 'jpg', 'jpeg', 'png'];
                if (!allowExt.includes(ext)) {
                    alert('附件只允許上傳 doc/docx/pdf/jpg/png 等文件');
                    return false;
                }

                const formData = new FormData();
                formData.append('file', e.target.files[0]);
                axios
                    .post('/Form/UploadAttachment', formData)
                    .then(res => {
                        if (!res.data.Status) {
                            alert(res.data.Message);
                            return;
                        }

                        row.FileName = res.data.Message;
                        this.$message.success('檔案上傳完成');
                    })
                    .catch(err => {
                        alert('系統發生未預期錯誤');
                        console.log(err);
                    });
            },
            sendForm() {
                this.$refs.form1.validate(valid => {
                    if (!valid) {
                        alert('欄位驗證錯誤，請檢查修正後重新送出');
                        return false;
                    }

                    if (!confirm('是否確認繼續?')) return false;
                    axios
                        .post('/Search/UpdateForm', this.selectRow)
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
                for (let i = 0; i < this.filterAttachmentInfo.length; i++) {
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
