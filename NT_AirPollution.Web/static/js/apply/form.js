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

    $.datepicker.regional['zh-TW'] = {
        closeText: '關閉',
        prevText: '&#x3C;上月',
        nextText: '下月&#x3E;',
        currentText: '今天',
        monthNames: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
        monthNamesShort: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
        dayNames: ['星期日', '星期一', '星期二', '星期三', '星期四', '星期五', '星期六'],
        dayNamesShort: ['周日', '周一', '周二', '周三', '周四', '周五', '周六'],
        dayNamesMin: ['日', '一', '二', '三', '四', '五', '六'],
        weekHeader: '周',
        dateFormat: 'yy/mm/dd',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: true,
        yearSuffix: '年'
    };
    $.datepicker.setDefaults($.datepicker.regional['zh-TW']);
    $.datepicker._phoenixGenerateMonthYearHeader = $.datepicker._generateMonthYearHeader;
    $.datepicker._generateMonthYearHeader = function (inst, drawMonth, drawYear, minDate, maxDate, secondary, monthNames, monthNamesShort) {
        var result = $($.datepicker._phoenixGenerateMonthYearHeader(inst, drawMonth, drawYear, minDate, maxDate, secondary, monthNames, monthNamesShort));
        result
            .find('select.ui-datepicker-year')
            .children()
            .each(function () {
                $(this).text($(this).text() - 1911 + '年');
            });

        return result.html();
    };

    new Vue({
        el: '#app',
        filters: {
            comma: value => {
                if (!value && value !== 0) return '';
                return ('' + value).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
            },
            date: value => {
                if (!value || value === '0001-01-01T00:00:00') return '';
                return moment(value).format('YYYY-MM-DD');
            }
        },
        data() {
            const checkVOLUMEL = (rule, value, callback) => {
                const message = this.selectRow.KIND_NO === '3' ? '請輸入總樓地板面積' : '請輸入外運土石體積';
                if (!value) {
                    callback(new Error(message));
                }
                callback();
            };
            const checkE_DATE = (rule, value, callback) => {
                if (!value) {
                    callback(new Error('請輸入結束日期'));
                }

                if (!this.selectRow.B_DATE) {
                    callback();
                }

                const startDate = `${+this.selectRow.B_DATE.substr(0, 3) + 1911}-${this.selectRow.B_DATE.substr(3, 2)}-${this.selectRow.B_DATE.substr(5, 2)}`;
                const endDate = `${+this.selectRow.E_DATE.substr(0, 3) + 1911}-${this.selectRow.E_DATE.substr(3, 2)}-${this.selectRow.E_DATE.substr(5, 2)}`;
                if (moment(startDate).isAfter(endDate)) {
                    callback(new Error('施工期程起始日期不能大於結束日期'));
                }
                callback();
            };
            const checkLATLNG = (rule, value, callback) => {
                if (isNaN(value)) {
                    callback(new Error('經緯度格式錯誤'));
                }
                callback();
            };
            const checkS_B_BDATE = (rule, value, callback) => {
                if (!this.PUB_COMP && !value) {
                    callback(new Error('請輸入營利事業負責人生日'));
                }
                callback();
            };
            const checkS_B_ID = (rule, value, callback) => {
                const idRegx = /^[A-Z][12]\d{8}$/;
                if (!idRegx.test(value)) {
                    callback(new Error('營利事業負責人身分證字號格式錯誤'));
                }
                callback();
            };
            const checkS_C_ID = (rule, value, callback) => {
                const idRegx = /^[A-Z][12]\d{8}$/;
                if (!this.PUB_COMP && !idRegx.test(value)) {
                    callback(new Error('營利事業聯絡人身分證字號格式錯誤'));
                }
                callback();
            };
            const checkR_B_ID = (rule, value, callback) => {
                const idRegx = /^[A-Z][12]\d{8}$/;
                if (!idRegx.test(value)) {
                    callback(new Error('承造負責人身分證字號格式錯誤'));
                }
                callback();
            };
            return {
                loading: null,
                mode: '',
                filter: {
                    StartDate: moment().format('YYYY-MM-01'),
                    EndDate: moment().format('YYYY-MM-DD'),
                    C_NO: null,
                    PUB_COMP: null,
                    CreateUserName: null,
                    COMP_NAM: null,
                    FormStatus: 0
                },
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
                    REC_YN: '無',
                    StopWorks: [],
                    RefundBank: {},
                    PaymentProof: {},
                    RATIOLB: 1.31,
                    DENSITYL: 1.51,
                    D2: null,
                    E2: null
                },
                banks: Object.freeze(banksAry),
                dialogVisible: false,
                failReason1DialogVisible: false,
                failReason2DialogVisible: false,
                bankAccountDialogVisible: false,
                paymentProofModalVisible: false,
                selfCheckModalVisible: false,
                activeTab: '1',
                tab1Rules: Object.freeze({
                    PUB_COMP: [{ required: true, message: '請選擇案件類型', trigger: 'change' }],
                    TOWN_NO: [{ required: true, message: '請選擇鄉鎮分類', trigger: 'change' }],
                    CreateUserName: [{ required: true, message: '請輸入申請人姓名', trigger: 'blur' }],
                    CreateUserEmail: [{ required: true, message: '請輸入申請人電子信箱', trigger: 'blur' }],
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
                    STATE: [{ required: true, message: '請輸入工程內容概述', trigger: 'blur' }]
                }),
                tab2Rules: Object.freeze({
                    S_NAME: [{ required: true, message: '請輸入營建業主名稱', trigger: 'blur' }],
                    S_G_NO: [{ required: true, message: '請輸入營利事業統一編號', trigger: 'blur' }],
                    S_ADDR1: [{ required: true, message: '請輸入營利事業營業地址', trigger: 'blur' }],
                    S_ADDR2: [{ required: true, message: '請輸入營利事業聯絡地址', trigger: 'blur' }],
                    S_TEL: [{ required: true, message: '請輸入營利事業主連絡電話', trigger: 'blur' }],
                    S_B_NAM: [{ required: true, message: '請輸入營利事業負責人姓名', trigger: 'blur' }],
                    S_B_TIT: [{ required: true, message: '請輸入營利事業負責人職稱', trigger: 'blur' }],
                    S_B_ID: [{ validator: checkS_B_ID, trigger: 'blur' }],
                    S_B_BDATE: [{ validator: checkS_B_BDATE, trigger: 'blur' }],
                    S_C_NAM: [{ required: true, message: '請輸入營利事業聯絡人姓名', trigger: 'blur' }],
                    S_C_TIT: [{ required: true, message: '請輸入營利事業聯絡人職稱', trigger: 'blur' }],
                    S_C_ID: [{ validator: checkS_C_ID, trigger: 'blur' }],
                    S_C_ADDR: [{ required: true, message: '請輸入營利事業聯絡人地址', trigger: 'blur' }],
                    S_C_TEL: [{ required: true, message: '請輸入營利事業聯絡人電話', trigger: 'blur' }]
                }),
                tab3Rules: Object.freeze({
                    R_NAME: [{ required: true, message: '請輸入承造單位名稱', trigger: 'blur' }],
                    R_G_NO: [{ required: true, message: '請輸入承造營利事業統一編號', trigger: 'blur' }],
                    R_ADDR1: [{ required: true, message: '請輸入承造營業地址', trigger: 'blur' }],
                    R_ADDR2: [{ required: true, message: '請輸入承造聯絡地址', trigger: 'blur' }],
                    R_TEL: [{ required: true, message: '請輸入承造連絡電話', trigger: 'blur' }],
                    R_B_NAM: [{ required: true, message: '請輸入承造負責人姓名', trigger: 'blur' }],
                    R_B_TIT: [{ required: true, message: '請輸入承造負責人職稱', trigger: 'blur' }],
                    R_B_ID: [{ validator: checkR_B_ID, trigger: 'blur' }],
                    R_B_BDATE: [{ required: true, message: '請輸入承造負責人生日', trigger: 'blur' }],
                    R_ADDR3: [{ required: true, message: '請輸入工務所地址', trigger: 'blur' }],
                    R_M_NAM: [{ required: true, message: '請輸入工地主任姓名', trigger: 'blur' }],
                    R_C_NAM: [{ required: true, message: '請輸入工地環保負責人姓名', trigger: 'blur' }],
                    R_TEL1: [{ required: true, message: '請輸入工務所電話', trigger: 'blur' }]
                }),
                tab4Rules: Object.freeze({
                    MONEY: [{ required: true, message: '請輸入工程合約經費', trigger: 'blur' }],
                    // C_MONEY: [{ required: true, message: '請輸入工程環保經費', trigger: 'blur' }],
                    AREA: [{ required: true, message: '請輸入工程面積', trigger: 'blur' }],
                    AREA_F: [{ required: true, message: '請輸入基地面積', trigger: 'blur' }],
                    AREA_B: [{ required: true, message: '請輸入建築面積', trigger: 'blur' }],
                    VOLUMEL: [{ validator: checkVOLUMEL, trigger: 'blur' }],
                    B_DATE: [{ required: true, message: '請輸入開始日期', trigger: 'blur' }],
                    E_DATE: [{ validator: checkE_DATE, trigger: 'blur' }]
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
            this.getCompanies();
            this.getContractor();
            this.getAttachmentInfo();
        },
        computed: {
            totalDays() {
                if (!this.selectRow.B_DATE || !this.selectRow.E_DATE) return '';
                const startDate = `${+this.selectRow.B_DATE.substr(0, 3) + 1911}-${this.selectRow.B_DATE.substr(3, 2)}-${this.selectRow.B_DATE.substr(5, 2)}`;
                const endDate = `${+this.selectRow.E_DATE.substr(0, 3) + 1911}-${this.selectRow.E_DATE.substr(3, 2)}-${this.selectRow.E_DATE.substr(5, 2)}`;
                var date1 = new Date(startDate);
                var date2 = new Date(endDate);

                // 計算毫秒差異
                var diff = Math.abs(date2 - date1 + 1000 * 60 * 60 * 24);
                // 轉換為天數
                var dayDiff = Math.ceil(diff / (1000 * 60 * 60 * 24));

                return dayDiff;
            },
            filterAttachmentInfo() {
                return this.attachmentInfo.filter(item => item.PUB_COMP === this.selectRow.PUB_COMP);
            },
            calcC_MONEY() {
                try {
                    if (!this.selectRow.MONEY) throw '';
                    return +((this.selectRow.MONEY * this.selectRow.PERCENT) / 100).toFixed(0);
                } catch (err) {
                    return 0;
                }
            },
            calcPERC_B() {
                if (!this.selectRow.AREA_B || !this.selectRow.AREA_F) return 0;
                return ((+this.selectRow.AREA_B / +this.selectRow.AREA_F) * 100).toFixed(2);
            }
        },
        methods: {
            initDatePicker() {
                $('.datepicker').datepicker({
                    dateFormat: 'yy/mm/dd',
                    yearRange: '-90:+10',
                    changeYear: true,
                    changeMonth: true,
                    beforeShow: function (input, inst) {
                        const inputVal = input.value;
                        if (inputVal) {
                            const year = +inputVal.substr(0, 3) + 1911;
                            const month = inputVal.substr(3, 2);
                            const day = inputVal.substr(5, 2);
                            return {
                                defaultDate: `${year}/${month}/${day}`
                            };
                        }

                        return {};
                    },
                    onSelect: (dateText, inst) => {
                        var objDate = {
                            y: `${inst.selectedYear - 1911 < 0 ? inst.selectedYear : inst.selectedYear - 1911}`.padStart(3, '0'),
                            m: `${inst.selectedMonth + 1}`.padStart(2, '0'),
                            d: `${inst.selectedDay}`.padStart(2, '0')
                        };

                        var dateFormate = `${objDate.y}${objDate.m}${objDate.d}`;
                        inst.input.val(dateFormate);
                        this.selectRow[inst.input[0].dataset.key] = dateFormate;
                    }
                });
            },
            getFormStatus(item) {
                switch (item.FormStatus) {
                    case 0:
                        return '未送審';
                    case 1:
                        return '審理中';
                    case 2:
                        return '待補件';
                    case 3:
                        return item.VerifyStage1 === 3 ? '通過待繳費' : '審理中';
                    case 4:
                        return item.VerifyStage1 === 3 ? '繳費完成' : '審理中';
                    case 5:
                        return item.VerifyStage1 === 3 ? '免繳費' : '審理中';
                    default:
                        return '';
                }
            },
            getCalcStatus(item) {
                switch (item.CalcStatus) {
                    case 0:
                        return '未申請';
                    case 1:
                        return '審理中';
                    case 2:
                        return '待補件';
                    case 3:
                        return item.VerifyStage2 === 3 ? '通過待繳費' : '審理中';
                    case 4:
                    case 5:
                        return item.VerifyStage2 === 3 ? '通過待退費' : '審理中';
                    case 6:
                        return item.VerifyStage2 === 3 ? '繳退費完成' : '審理中';
                    default:
                        return '';
                }
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
            getAttachmentInfo() {
                axios.get('/Option/GetAttachmentInfo').then(res => {
                    this.attachmentInfo = Object.freeze(res.data);
                });
            },
            getCompanies() {
                axios.post('/Apply/GetMyCompanies', this.filter).then(res => {
                    this.companies = Object.freeze(res.data);
                });
            },
            selectCompanyChange() {
                const result = this.companies.find(item => item.ID === this.selectCompany);
                if (result) {
                    this.selectRow.S_NAME = result.S_NAME;
                    this.selectRow.S_G_NO = result.S_G_NO;
                    this.selectRow.S_ADDR1 = result.S_ADDR1;
                    this.selectRow.S_ADDR2 = result.S_ADDR2;
                    this.selectRow.S_TEL = result.S_TEL;
                    this.selectRow.S_B_NAM = result.S_B_NAM;
                    this.selectRow.S_B_TIT = result.S_B_TIT;
                    this.selectRow.S_B_ID = result.S_B_ID;
                    this.selectRow.S_B_BDATE = result.S_B_BDATE;
                    this.selectRow.S_C_NAM = result.S_C_NAM;
                    this.selectRow.S_C_TIT = result.S_C_TIT;
                    this.selectRow.S_C_ID = result.S_C_ID;
                    this.selectRow.S_C_ADDR = result.S_C_ADDR;
                    this.selectRow.S_C_TEL = result.S_C_TEL;
                }
            },
            getContractor() {
                axios.post('/Apply/GetMyContractor', this.filter).then(res => {
                    this.contractors = Object.freeze(res.data);
                });
            },
            selectContractorChange() {
                const result = this.contractors.find(item => item.ID === this.selectContractor);
                if (result) {
                    this.selectRow.R_NAME = result.R_NAME;
                    this.selectRow.R_G_NO = result.R_G_NO;
                    this.selectRow.R_ADDR1 = result.R_ADDR1;
                    this.selectRow.R_ADDR2 = result.R_ADDR2;
                    this.selectRow.R_TEL = result.R_TEL;
                    this.selectRow.R_B_NAM = result.R_B_NAM;
                    this.selectRow.R_B_TIT = result.R_B_TIT;
                    this.selectRow.R_B_ID = result.R_B_ID;
                    this.selectRow.R_B_BDATE = result.R_B_BDATE;
                    this.selectRow.R_ADDR3 = result.R_ADDR3;
                    this.selectRow.R_TEL1 = result.R_TEL1;
                    this.selectRow.R_M_NAM = result.R_M_NAM;
                    this.selectRow.R_C_NAM = result.R_C_NAM;
                }
            },
            getForms() {
                const loading = this.$loading();
                axios
                    .post('/Apply/GetForms', this.filter)
                    .then(res => {
                        this.forms = res.data;
                        loading.close();
                    })
                    .catch(err => {
                        loading.close();
                        console.log(err);
                    });
            },
            async addForm() {
                this.mode = 'Add';
                this.activeTab = '1';
                this.selectCompany = null;
                this.selectContractor = null;
                const emptyModel = await axios.get('Apply/GetEmptyFormModel');
                this.selectRow = Object.assign(emptyModel.data.Message, {
                    SER_NO: 1,
                    P_KIND: '一次全繳',
                    KIND_NO: null,
                    REC_YN: '無',
                    CreateUserName: document.querySelector('#hfUserName').value,
                    CreateUserEmail: document.querySelector('#hfUserEmail').value,
                    FormStatus: 1,
                    C_DATE: moment().format('YYYY-MM-DD'),
                    StopWorks: [],
                    RefundBank: {},
                    PaymentProof: {},
                    RATIOLB: 1.31,
                    DENSITYL: 1.51,
                    D2: null,
                    E2: null
                });

                // this.selectRow = {
                //     SER_NO: 1,
                //     P_KIND: '一次全繳',
                //     REC_YN: '無',
                //     PUB_COMP: true,
                //     TOWN_NO: 'M2',
                //     CreateUserName: document.querySelector('#hfUserName').value,
                //     CreateUserEmail: document.querySelector('#hfUserEmail').value,
                //     COMP_NAM: 'COMP_NAM',
                //     KIND_NO: '1',
                //     ADDR: 'ADDR',
                //     B_SERNO: 'B_SERNO',
                //     UTME: 123,
                //     UTMN: 456,
                //     LAT: 24.1477358,
                //     LNG: 120.6736482,
                //     STATE: 'STATE',
                //     EIACOMMENTS: 'EIACOMMENTS',
                //     RECCOMMENTS: 'RECCOMMENTS',
                //     S_NAME: 'S_NAME',
                //     S_G_NO: 'S_G_NO',
                //     S_ADDR1: 'S_ADDR1',
                //     S_ADDR2: 'S_ADDR2',
                //     S_TEL: 'S_TEL',
                //     S_B_NAM: 'S_B_NAM',
                //     S_B_TIT: 'S_B_TIT',
                //     S_B_ID: 'A123456789',
                //     S_B_BDATE: '0740714',
                //     S_C_NAM: 'S_C_NAM',
                //     S_C_TIT: 'S_C_TIT',
                //     S_C_ID: 'A123456789',
                //     S_C_ADDR: 'S_C_ADDR',
                //     S_C_TEL: 'S_C_TEL',
                //     R_NAME: 'R_NAME',
                //     R_G_NO: 'R_G_NO',
                //     R_ADDR1: 'R_ADDR1',
                //     R_ADDR2: 'R_ADDR2',
                //     R_TEL: 'R_TEL',
                //     R_B_NAM: 'R_B_NAM',
                //     R_B_TIT: 'R_B_TIT',
                //     R_B_ID: 'A123456789',
                //     R_B_BDATE: '0740714',
                //     R_ADDR3: 'R_ADDR3',
                //     R_M_NAM: 'R_M_NAM',
                //     R_C_NAM: 'R_C_NAM',
                //     R_TEL1: 'R_TEL1',
                //     MONEY: 1,
                //     C_MONEY: 2,
                //     PERCENT: 3,
                //     AREA: 1,
                //     VOLUMEL: null,
                //     B_DATE: '1130101',
                //     E_DATE: '1130131',
                //     FormStatus: 1,
                //     C_DATE: moment().format('YYYY-MM-DD'),
                //     StopWorks: [],
                //     RefundBank: {},
                //     PaymentProof: {},
                //     RATIOLB: 1.31,
                //     DENSITYL: 1.51,
                //     D2: null,
                //     E2: null
                // };

                this.dialogVisible = true;
                this.$nextTick(() => {
                    this.initDatePicker();
                });
            },
            showModal(row) {
                this.mode = 'Update';
                this.activeTab = '1';
                this.selectRow = JSON.parse(JSON.stringify(row));
                const point = this.selectRow.LATLNG.split(',');
                this.selectRow.LAT = point[0] || null;
                this.selectRow.LNG = point[1] || null;
                this.selectRow.D2 = null;
                this.selectRow.E2 = null;
                this.dialogVisible = true;
                if (this.selectRow.FormStatus === 2) {
                    this.failReason1DialogVisible = true;
                }
                if (this.selectRow.CalcStatus === 2) {
                    this.failReason2DialogVisible = true;
                }
                this.$nextTick(() => {
                    this.initDatePicker();
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
            onUploading() {
                this.loading = this.$loading();
            },
            uploadSuccess1(res, file, fileList) {
                this.loading.close();
                this.$refs.upload1.clearFiles();
                if (!res.Status) {
                    alert(res.Message);
                    return;
                }
                this.selectRow.FileName1 = res.Message;
                this.selectRow.DisplayName1 = file.name;
            },
            uploadSuccess2(res, file, fileList) {
                this.loading.close();
                this.$refs.upload2.clearFiles();
                if (!res.Status) {
                    alert(res.Message);
                    return;
                }
                this.selectRow.FileName2 = res.Message;
                this.selectRow.DisplayName2 = file.name;
            },
            goPrevTab() {
                let intActiveTab = +this.activeTab;
                if (intActiveTab > 1) {
                    intActiveTab -= 1;
                    this.activeTab = intActiveTab.toString();
                }
            },
            goNextTab() {
                // 自動設定公共工程4%、私人工程3%
                if (this.activeTab === '2') {
                    if (this.selectRow.PUB_COMP) {
                        this.selectRow.PERCENT = 4;
                    } else {
                        this.selectRow.PERCENT = 3;
                    }
                }

                switch (this.activeTab) {
                    case '1':
                    case '2':
                    case '3':
                    case '4': {
                        this.$refs[`tab${this.activeTab}Form`].validate((valid, obj) => {
                            if (!valid) {
                                const firstKey = Object.keys(obj)[0];
                                alert(obj[firstKey][0].message);
                                return false;
                            }

                            this.activeTab = (+this.activeTab + 1).toString();
                        });

                        break;
                    }
                    case '5': {
                        this.activeTab = (+this.activeTab + 1).toString();
                        break;
                    }
                    case '6': {
                        if (!confirm('是否確認繼續?')) return false;
                        const point = this.LatLon2UTM(this.selectRow.LAT, this.selectRow.LNG, 0, 0);
                        this.selectRow.UTME = point[0];
                        this.selectRow.UTMN = point[1];
                        this.selectRow.LATLNG = `${this.selectRow.LAT},${this.selectRow.LNG}`;
                        this.selectRow.C_MONEY = this.calcC_MONEY;
                        // 1、2類工程面積=建築面積
                        if (this.selectRow.KIND_NO === '1' || this.selectRow.KIND_NO === '2') {
                            this.selectRow.AREA = this.selectRow.AREA_B;
                        }
                        const loading = this.$loading();
                        axios
                            .post(`/Apply/${this.mode}Form`, this.selectRow)
                            .then(res => {
                                loading.close();
                                if (!res.data.Status) {
                                    alert(res.data.Message);
                                    return;
                                }

                                alert('申請資料已儲存。');
                                this.getForms();
                                this.dialogVisible = false;
                            })
                            .catch(err => {
                                loading.close();
                                alert('系統發生未預期錯誤');
                                console.log(err);
                            });
                        break;
                    }
                }
            },
            showSelfCheckModal(row) {
                this.selectRow = JSON.parse(JSON.stringify(row));
                this.selfCheckModalVisible = true;
            },
            sendFormStatus1() {
                if (!confirm('是否確認提送審查?')) return false;
                axios
                    .post(`/Apply/SendFormStatus1`, this.selectRow)
                    .then(res => {
                        if (!res.data.Status) {
                            alert(res.data.Message);
                            return;
                        }

                        alert('申請資料已送出，繳款金額請依人工審核後之繳費單內容為主。');

                        if (confirm('是否儲存此次營建業主之基本資料，下次可以快速申報。')) {
                            this.addCompany();
                        }
                        if (confirm('是否儲存此次承包商之基本資料，下次可以快速申報。')) {
                            this.addContractor();
                        }

                        this.getForms();
                        this.selfCheckModalVisible = false;
                    })
                    .catch(err => {
                        alert('系統發生未預期錯誤');
                        console.log(err);
                    });
            },
            addCompany() {
                axios.post('/Apply/AddCompany', this.selectRow).then(res => {
                    if (!res.data.Status) {
                        alert(res.data.Message);
                        return;
                    }
                });
            },
            addContractor() {
                axios.post('/Apply/AddContractor', this.selectRow).then(res => {
                    if (!res.data.Status) {
                        alert(res.data.Message);
                        return;
                    }
                });
            },
            copyForm(row) {
                this.mode = 'Add';
                this.selectRow = JSON.parse(JSON.stringify(row));
                const point = this.selectRow.LATLNG.split(',');
                this.selectRow.LAT = point[0] || null;
                this.selectRow.LNG = point[1] || null;
                this.selectRow.FormStatus = 0;
                this.selectRow.FileName1 = null;
                this.selectRow.DisplayName1 = null;
                this.selectRow.FileName2 = null;
                this.selectRow.DisplayName2 = null;
                this.selectRow.StopWorks.length = 0;
                const clearAry = ['C_NO', 'SER_NO', 'AP_DATE', 'C_DATE'];
                for (const key of clearAry) {
                    this.selectRow[key] = null;
                }

                this.dialogVisible = true;
                this.$nextTick(() => {
                    // 清空附件
                    const files = document.querySelectorAll('[type="file"]');
                    files.forEach(item => {
                        item.value = '';
                    });

                    this.initDatePicker();
                });
            },
            deleteForm(row) {
                if (!confirm('是否確認刪除?')) return;
                const loading = this.$loading();
                axios
                    .post('/Apply/DeleteForm', row)
                    .then(res => {
                        loading.close();
                        if (!res.data.Status) {
                            alert(res.data.Message);
                            return;
                        }
                        this.getForms();
                    })
                    .catch(err => {
                        loading.close();
                        alert('系統發生未預期錯誤');
                        console.log(err);
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
                // for (let i = 0; i < this.filterAttachmentInfo.length; i++) {
                //     const file = document.querySelector(`#file${i}`);
                //     if (file) file.value = '';
                // }
            },
            downloadPayment(row) {
                const loading = this.$loading();
                axios
                    .post('/Apply/DownloadPayment', row, {
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
            downloadFreeProof(row) {
                const loading = this.$loading();
                axios
                    .post('/Apply/DownloadFreeProof', row, {
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
            downloadPaymentProof(row) {
                const loading = this.$loading();
                axios
                    .post('/Apply/DownloadPaymentProof', row, {
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
            SendCalcStatus1(row) {
                if (!row.FileName2) {
                    alert('提出結算申請前，需檢視內容並上傳附件');
                    return;
                }
                if (!confirm('是否確認提出結算申請?')) return;
                axios
                    .post('/Apply/SendCalcStatus1', row)
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
                    .post('/Apply/DownloadRePayment', row, {
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
            // showPaymentProofModal(row) {
            //     this.selectRow = JSON.parse(JSON.stringify(row));
            //     this.selectRow.PaymentProof = Object.assign({}, row.PaymentProof, {
            //         FormID: row.ID,
            //         File: null
            //     });
            //     this.paymentProofModalVisible = true;
            // },
            // deletePaymentProof() {
            //     if (!confirm('是否確認刪除?')) return false;
            //     this.selectRow.PaymentProof.ProofFile = null;
            // },
            //savePaymentProof() {
            //    const formData = new FormData();
            //    // 附件
            //    const file = document.querySelector(`#filePF`);
            //    if (file && file.files.length > 0) {
            //        formData.append('file', file.files[0]);
            //        this.selectRow.PaymentProof.File = file.files[0].name;
            //    }

            //    this.$refs.form3.validate((valid, object) => {
            //        if (!valid) {
            //            alert('欄位驗證錯誤，請檢查修正後重新送出');
            //            return false;
            //        }

            //        if (!confirm('是否確認繼續?')) return false;

            //        const loading = this.$loading();
            //        for (const key in this.selectRow.PaymentProof) {
            //            if (typeof this.selectRow.PaymentProof[key] !== 'object') formData.append(key, this.selectRow.PaymentProof[key]);
            //        }

            //        axios
            //            .post('/Apply/UploadPaymentProof', formData)
            //            .then(res => {
            //                loading.close();
            //                if (!res.data.Status) {
            //                    alert(res.data.Message);
            //                    return;
            //                }

            //                alert('上傳成功');
            //                this.getForms();
            //                this.paymentProofModalVisible = false;
            //            })
            //            .catch(err => {
            //                loading.close();
            //                alert('系統發生未預期錯誤');
            //                console.log(err);
            //            });
            //    });
            //},
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
                        .post('/Apply/UpdateBankAccount', formData)
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
            downloadClearProof(row) {
                const loading = this.$loading();
                axios
                    .post('/Apply/DownloadClearProof', row, {
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
            calcD2() {
                this.selectRow.VOLUMEL = this.selectRow.D2 * this.selectRow.RATIOLB;
            },
            calcE2() {
                this.selectRow.VOLUMEL = this.selectRow.E2 / this.selectRow.DENSITYL;
            }
        }
    });
});
