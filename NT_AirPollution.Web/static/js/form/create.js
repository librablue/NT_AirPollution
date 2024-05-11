document.addEventListener('DOMContentLoaded', () => {
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
            }
        },
        data() {
            const checkE_DATE2 = (rule, value, callback) => {
                if (!value) {
                    callback(new Error('請輸入結束日期'));
                }
                if (this.form.B_DATE2 && this.form.E_DATE2 && moment(value).isSameOrBefore(this.form.B_DATE2)) {
                    callback(new Error('結束日期不得早於開始日期'));
                }
                callback();
            };
            const checkArea = (rule, value, callback) => {
                if (!this.form.VOLUMEL && !value) {
                    callback(new Error('如果非疏濬工程，請輸入施工面積'));
                }
                callback();
            };
            const checkVolumel = (rule, value, callback) => {
                if (!this.form.AREA && !value) {
                    callback(new Error('如果為疏濬工程，請輸入清運土石體積'));
                }
                callback();
            };
            const checkLATLNG = (rule, value, callback) => {
                if (isNaN(value)) {
                    callback(new Error('經緯度格式錯誤'));
                }
                callback();
            };
            return {
                sendText: '寄送驗證信',
                sending: false,
                district: Object.freeze([]),
                projectCode: Object.freeze([]),
                attachmentInfo: Object.freeze([]),
                form: {
                    SER_NO: 1,
                    P_KIND: '一次全繳',
                    BUD_DOC2: '無',
                    Attachments: []
                },
                activeTab: 'first',
                rules: Object.freeze({
                    PUB_COMP: [{ required: true, message: '請選擇案件類型', trigger: 'change' }],
                    TOWN_NO: [{ required: true, message: '請選擇鄉鎮分類', trigger: 'change' }],
                    CreateUserName: [{ required: true, message: '請輸入申請人姓名', trigger: 'blur' }],
                    CreateUserEmail: [{ required: true, message: '請輸入申請人電子信箱', trigger: 'blur' }],
                    ActiveCode: [{ required: true, message: '請輸入信箱驗證碼', trigger: 'blur' }],
                    COMP_NAM: [{ required: true, message: '請輸入工程名稱', trigger: 'blur' }],
                    KIND_NO: [{ required: true, message: '請選擇工程類別', trigger: 'change' }],
                    ADDR: [{ required: true, message: '請輸入工地地址或地號', trigger: 'blur' }],
                    B_SERNO: [{ required: true, message: '請輸入建照字號或合約編號', trigger: 'blur' }],
                    LAT: [
                        { required: true, message: '請輸入座標(緯度)', trigger: 'blur' },
                        { validator: checkLATLNG, trigger: 'blur' }
                    ],
                    LNG: [
                        { required: true, message: '請輸入座標(經度)', trigger: 'blur' },
                        { validator: checkLATLNG, trigger: 'blur' }
                    ],
                    STATE: [{ required: true, message: '請輸入工程內容概述', trigger: 'blur' }],
                    EIACOMMENTS: [{ required: true, message: '請輸入環評保護對策', trigger: 'blur' }],
                    S_NAME: [{ required: true, message: '請輸入營建業主名稱', trigger: 'blur' }],
                    S_G_NO: [{ required: true, message: '請輸入營利事業統一編號', trigger: 'blur' }],
                    S_ADDR1: [{ required: true, message: '請輸入營利事業營業地址', trigger: 'blur' }],
                    S_ADDR2: [{ required: true, message: '請輸入營利事業聯絡地址', trigger: 'blur' }],
                    S_TEL: [{ required: true, message: '請輸入營利事業主連絡電話', trigger: 'blur' }],
                    S_B_NAM: [{ required: true, message: '請輸入營利事業負責人姓名', trigger: 'blur' }],
                    S_B_TIT: [{ required: true, message: '請輸入營利事業負責人職稱', trigger: 'blur' }],
                    S_B_ID: [{ required: true, message: '請輸入營利事業負責人身分證字號', trigger: 'blur' }],
                    S_B_BDATE2: [{ required: true, message: '請輸入營利事業負責人生日', trigger: 'blur' }],
                    S_C_NAM: [{ required: true, message: '請輸入營利事業聯絡人姓名', trigger: 'blur' }],
                    S_C_TIT: [{ required: true, message: '請輸入營利事業聯絡人職稱', trigger: 'blur' }],
                    S_C_ID: [{ required: true, message: '請輸入營利事業聯絡人身分證字號', trigger: 'blur' }],
                    S_C_ADDR: [{ required: true, message: '請輸入營利事業聯絡人地址', trigger: 'blur' }],
                    S_C_TEL: [{ required: true, message: '請輸入營利事業聯絡人電話', trigger: 'blur' }],
                    R_NAME: [{ required: true, message: '請輸入承造單位名稱', trigger: 'blur' }],
                    R_G_NO: [{ required: true, message: '請輸入承造營利事業統一編號', trigger: 'blur' }],
                    R_ADDR1: [{ required: true, message: '請輸入承造營業地址', trigger: 'blur' }],
                    R_ADDR2: [{ required: true, message: '請輸入承造聯絡地址', trigger: 'blur' }],
                    R_TEL: [{ required: true, message: '請輸入承造連絡電話', trigger: 'blur' }],
                    R_B_NAM: [{ required: true, message: '請輸入承造負責人姓名', trigger: 'blur' }],
                    R_B_TIT: [{ required: true, message: '請輸入承造負責人職稱', trigger: 'blur' }],
                    R_B_ID: [{ required: true, message: '請輸入承造負責人身分證字號', trigger: 'blur' }],
                    R_B_BDATE2: [{ required: true, message: '請輸入承造負責人生日', trigger: 'blur' }],
                    R_ADDR3: [{ required: true, message: '請輸入工務所地址', trigger: 'blur' }],
                    R_M_NAM: [{ required: true, message: '請輸入工地主任姓名', trigger: 'blur' }],
                    R_C_NAM: [{ required: true, message: '請輸入工地環保負責人姓名', trigger: 'blur' }],
                    R_TEL1: [{ required: true, message: '請輸入工務所電話', trigger: 'blur' }],
                    MONEY: [{ required: true, message: '請輸入工程合約經費', trigger: 'blur' }],
                    C_MONEY: [{ required: true, message: '請輸入工程環保經費', trigger: 'blur' }],
                    AREA: [{ validator: checkArea }],
                    VOLUMEL: [{ validator: checkVolumel }],
                    B_DATE2: [{ required: true, message: '請輸入開始日期', trigger: 'blur' }],
                    E_DATE2: [{ validator: checkE_DATE2 }]
                })
            };
        },
        mounted() {
            this.getDistrict();
            this.getProjectCode();
            this.getAttachmentInfo();
            this.form = {
                SER_NO: 1,
                P_KIND: '一次全繳',
                BUD_DOC2: '無',
                PUB_COMP: true,
                TOWN_NO: 'M2',
                CreateUserName: '申請人',
                CreateUserEmail: 'abc@gmail.com',
                COMP_NAM: 'COMP_NAM',
                KIND_NO: '1',
                ADDR: 'ADDR',
                B_SERNO: 'B_SERNO',
                UTME: 123,
                UTMN: 456,
                LAT: 24.1477358,
                LNG: 120.6736482,
                STATE: 'STATE',
                EIACOMMENTS: 'EIACOMMENTS',
                RECCOMMENTS: 'RECCOMMENTS',
                S_NAME: 'S_NAME',
                S_G_NO: 'S_G_NO',
                S_ADDR1: 'S_ADDR1',
                S_ADDR2: 'S_ADDR2',
                S_TEL: 'S_TEL',
                S_B_NAM: 'S_B_NAM',
                S_B_TIT: 'S_B_TIT',
                S_B_ID: 'S_B_ID',
                S_B_BDATE2: '1985-07-14',
                S_C_NAM: 'S_C_NAM',
                S_C_TIT: 'S_C_TIT',
                S_C_ID: 'S_C_ID',
                S_C_ADDR: 'S_C_ADDR',
                S_C_TEL: 'S_C_TEL',
                R_NAME: 'R_NAME',
                R_G_NO: 'R_G_NO',
                R_ADDR1: 'R_ADDR1',
                R_ADDR2: 'R_ADDR2',
                R_TEL: 'R_TEL',
                R_B_NAM: 'R_B_NAM',
                R_B_TIT: 'R_B_TIT',
                R_B_ID: 'R_B_ID',
                R_B_BDATE2: '2024-01-01',
                R_ADDR3: 'R_ADDR3',
                R_M_NAM: 'R_M_NAM',
                R_C_NAM: 'R_C_NAM',
                R_TEL1: 'R_TEL1',
                MONEY: 1,
                C_MONEY: 2,
                PERCENT: 3,
                AREA: 1,
                VOLUMEL: null,
                B_DATE2: '2024-01-01',
                E_DATE2: '2024-01-31',
                Attachments: []
            };
        },
        computed: {
            totalDays() {
                if (!this.form.B_DATE2 || !this.form.E_DATE2) return '';
                var date1 = new Date(this.form.B_DATE2);
                var date2 = new Date(this.form.E_DATE2);

                // 計算毫秒差異
                var diff = Math.abs(date2 - date1 + 1000 * 60 * 60 * 24);
                // 轉換為天數
                var dayDiff = Math.ceil(diff / (1000 * 60 * 60 * 24));

                return dayDiff;
            },
            filterAttachmentInfo() {
                return this.attachmentInfo.filter(item => item.PUB_COMP === this.form.PUB_COMP);
            },
            calcPercent() {
                try {
                    if (!this.form.C_MONEY || !this.form.MONEY) throw '';
                    return +((this.form.C_MONEY / this.form.MONEY) * 100).toFixed(2);
                } catch (err) {
                    return 0;
                }
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
                return this.form.Attachments.filter(item => item.InfoID === infoID);
            },
            sendActiveCode() {
                axios
                    .post('/Form/SendActiveCode', {
                        Email: this.form.CreateUserEmail
                    })
                    .then(res => {
                        if (!res.data.Status) {
                            alert(res.data.Message);
                            return;
                        }

                        alert('系統將於3分鐘內傳送驗證碼給您，請輸入郵件中的驗證碼。');
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
                        alert('系統發生未預期錯誤');
                    });
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

                var pt = new Array(1);
                pt[0] = LM2;
                pt[1] = PH2;
                return pt;
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

                var pt = new Array(1);

                pt[0] = Lat;
                pt[1] = Lon;
                return pt;
            },
            addFile(infoID) {
                const lastID = this.form.Attachments.length === 0 ? 0 : this.form.Attachments[this.form.Attachments.length - 1].ID;
                this.form.Attachments.push({
                    ID: lastID + 1,
                    InfoID: infoID,
                    FileName: null
                });
            },
            deleteFile(row) {
                if (!confirm('是否確認刪除?')) return;
                const findIdx = this.form.Attachments.findIndex((item, idx) => item.ID === row.ID);
                this.form.Attachments.splice(findIdx, 1);
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
                this.form.Captcha = grecaptcha.getResponse();
                this.$refs.form.validate(valid => {
                    if (!valid) {
                        alert('欄位驗證錯誤，請檢查修正後重新送出');
                        return false;
                    }

                    if (!confirm('是否確認繼續?')) return false;
                    const point = this.LatLon2UTM(this.form.LAT, this.form.LNG, 0, 0);
                    this.form.UTME = point[0];
                    this.form.UTMN = point[1];
                    this.form.LATLNG = `${this.form.LAT},${this.form.LNG}`;
                    axios
                        .post('/Form/Create', this.form)
                        .then(res => {
                            if (!res.data.Status) {
                                grecaptcha.reset();
                                alert(res.data.Message);
                                return;
                            }

                            alert('申請資料已送出。\n系統將於3分鐘內傳送認證信給您，請點選郵件中的連結進行驗證。\n完成驗證之案件才會進入審核程序。');
                            alert('繳款金額請依人工審核後之繳費單內容為主。');
                            location.href = `${document.baseURI}/Home/Index`;
                        })
                        .catch(err => {
                            grecaptcha.reset();
                            alert('系統發生未預期錯誤');
                            console.log(err);
                        });
                });
            }
        }
    });
});
