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
			date: value => {
				if (!value || value === '0001-01-01T00:00:00') return '';
				return moment(value).format('YYYY-MM-DD');
			},
			workStatus: value => {
				switch (value) {
					case 1:
						return '施工中';
					case 2:
						return '停工中';
					case 3:
						return '已完工';
					default:
						return '';
				}
			}
		},
		data() {
			return {
				loading: false,
				filter: {
					C_NO: '',
					COMP_NAM: '',
					WorkStatus: 0,
					Commitment: 0
				},
				forms: [],
				district: Object.freeze([]),
				projectCode: Object.freeze([]),
				selectRow: {
					P_KIND: '一次全繳',
					REC_YN: '無',
					Attachment: {}
				},
				dialogVisible: false,
				activeTab: 'first'
			};
		},
		mounted() {
			this.getDistrict();
			this.getProjectCode();
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
            calcPercent() {
                try {
                    if (!this.selectRow.C_MONEY || !this.selectRow.MONEY) throw '';
                    return +((this.selectRow.C_MONEY / this.selectRow.MONEY) * 100).toFixed(2);
                } catch (err) {
                    return 0;
                }
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
			getForms() {
				this.loading = true;
				axios
					.post('/Manage/GetForms', this.filter)
					.then(res => {
						this.forms = res.data.Message;
						this.loading = false;
					})
					.catch(err => {
						this.loading = false;
						console.log(err);
					});
			},
			showModal(row) {
				this.selectRow = JSON.parse(JSON.stringify(row));
				this.dialogVisible = true;
                this.$nextTick(() => {
                    this.initDatePicker();
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
			goManage(row, act) {
				window.open(`${document.baseURI}/${act}/Index?id=${row.ID}`);
			}
		}
	});
});
