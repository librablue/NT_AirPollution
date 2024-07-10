document.addEventListener('DOMContentLoaded', () => {
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
        const yearAry = [];
        for (let i = new Date().getFullYear(); i >= 1934; i--) {
            yearAry.push(i);
        }
        result
            .find('select.ui-datepicker-year')
            .empty()
            .append(yearAry.map(year => `<option value="${year}" ${year === drawYear? 'selected' : ''}>${(year - 1911).toString().padStart(3, '0')}</option>`).join(''));

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
            const checkE_DATE = (rule, value, callback) => {
                if (!value) {
                    callback(new Error('請輸入結束日期'));
                }

                if(!this.form.B_DATE) {
                    callback();
                }

                const startDate = `${+this.form.B_DATE.substr(0, 3) + 1911}-${this.form.B_DATE.substr(3, 2)}-${this.form.B_DATE.substr(5, 2)}`;
                const endDate = `${+this.form.E_DATE.substr(0, 3) + 1911}-${this.form.E_DATE.substr(3, 2)}-${this.form.E_DATE.substr(5, 2)}`;
                if (moment(startDate).isAfter(endDate)) {
                    callback(new Error('施工期程起始日期不能大於結束日期'));
                }
                callback();
            };
			const checkArea = (rule, value, callback) => {
				const kindAry = ['1', '2', '4', '5', '6', '7', '8', '9', 'A'];
				if (kindAry.includes(this.form.KIND_NO) && !value) {
					callback(new Error('請輸入施工面積'));
				}
				callback();
			};
			const checkVolumel = (rule, value, callback) => {
                if (this.form.KIND_NO === '3' && !value) {
					callback(new Error('請輸入總樓地板面積'));
				}
				if (this.form.KIND_NO === 'B' && !value) {
					callback(new Error('請輸入外運土石體積'));
				}
				callback();
			};
			const checkMoney = (rule, value, callback) => {
				const kindAry = ['Z'];
				if (kindAry.includes(this.form.KIND_NO) && !value) {
					callback(new Error('請輸入工程合約經費'));
				}
				callback();
			};
			return {
				projectCode: Object.freeze([]),
				form: {
					KIND_NO: null,
					MONEY: null,
					AREA: null,
					VOLUMEL: null,
					B_DATE: null,
					E_DATE: null,
                    RATIOLB: 1.31,
                    DENSITYL: 1.51,
                    D2: null,
                    E2: null
				},
                CodeBType: 1,
				calcResult: null,
				rules: Object.freeze({
					KIND_NO: [{ required: true, message: '請選擇工程類別', trigger: 'change' }],
					MONEY: [{ validator: checkMoney }],
					AREA: [{ validator: checkArea }],
					VOLUMEL: [{ validator: checkVolumel }],
					B_DATE: [{ required: true, message: '請輸入開始日期', trigger: 'blur' }],
					E_DATE: [{ validator: checkE_DATE }]
				})
			};
		},
		mounted() {
			this.getProjectCode();
            this.initDatePicker();
		},
		computed: {
            totalDays() {
                if (!this.form.B_DATE || !this.form.E_DATE) return '';
                const startDate = `${+(this.form.B_DATE.substr(0, 3)) + 1911}-${this.form.B_DATE.substr(3, 2)}-${this.form.B_DATE.substr(5, 2)}`;
                const endDate = `${+(this.form.E_DATE.substr(0, 3)) + 1911}-${this.form.E_DATE.substr(3, 2)}-${this.form.E_DATE.substr(5, 2)}`;
                var date1 = new Date(startDate);
                var date2 = new Date(endDate);

                // 計算毫秒差異
                var diff = Math.abs(date2 - date1 + 1000 * 60 * 60 * 24);
                // 轉換為天數
                var dayDiff = Math.ceil(diff / (1000 * 60 * 60 * 24));

                return dayDiff;
            },
			projectCodeText() {
				switch (this.form.KIND_NO) {
					case '1':
					case '2':
						return '建築面積(平方公尺)';
					case '3':
						return '總樓地板面積(平方公尺)';
					case '4':
					case '6':
						return '施工面積(平方公尺)';
					case '5':
						return '隧道平面面積(平方公尺)';
					case '7':
						return '橋面面積(平方公尺)';
					case '8':
					case '9':
					case 'A':
						return '施工面積(公頃)';
					default:
						return '施工面積(平方公尺)';
				}
			}
		},
		methods: {
            initDatePicker() {
                $('.datepicker').datepicker({
					dateFormat: 'yy/mm/dd',
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
						this.form[inst.input[0].dataset.key] = dateFormate;
					}
				});
            },
			getProjectCode() {
				axios.get('/Option/GetProjectCode').then(res => {
					this.projectCode = Object.freeze(res.data);
				});
			},
            getProjectCodeItem(id) {
                return this.projectCode.find(item => item.ID === id);
            },
			isShowAREA() {
				const kindAry = ['1', '2', '4', '5', '6', '7', '8', '9', 'A'];
				if (kindAry.includes(this.form.KIND_NO)) {
					return true;
				}
				return false;
			},
			isShowMONEY() {
				const kindAry = ['Z'];
				if (kindAry.includes(this.form.KIND_NO)) {
					return true;
				}
				return false;
			},
            calcD2() {
                this.form.VOLUMEL = this.form.D2 * this.form.RATIOLB;
            },
            calcE2() {
                this.form.VOLUMEL = this.form.E2 / this.form.DENSITYL;
            },
			sendForm() {
				this.$refs.form.validate(valid => {
					if (!valid) {
						alert('欄位驗證錯誤，請檢查修正後重新送出');
						return false;
					}

                    this.calcResult = null;
					axios
						.post('/Home/GetTotalMoney', this.form)
						.then(res => {
							if (!res.data.Status) {
								alert(res.data.Message);
								return;
							}
                            this.calcResult = res.data.Message;
						})
						.catch(err => {
							alert('系統發生未預期錯誤');
							console.log(err);
						});
				});
			}
		}
	});
});
