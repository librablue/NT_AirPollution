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
		data() {
			return {
				mode: '',
				loading: false,
				filter: {
					R_G_NO: '',
					R_NAME: '',
					R_B_NAM: '',
					R_M_NAM: '',
					R_C_NAM: ''
				},
				contractors: [],
				selectRow: {},
				dialogVisible: false
			};
		},
		methods: {
            initDatePicker() {
				$('.datepicker').datepicker({
					dateFormat: 'yy/mm/dd',
                    yearRange: '-90:+0',
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
			getContractor() {
				this.loading = true;
				axios.post('/Apply/GetMyContractor', this.filter).then(res => {
					this.contractors = res.data;
					this.loading = false;
				});
			},
            showEditModal(row) {
				if (row) {
					this.mode = 'Update';
					this.selectRow = JSON.parse(JSON.stringify(row));
				} else {
					this.mode = 'Add';
					this.selectRow = {};
				}

				this.dialogVisible = true;
				this.$nextTick(() => {
					this.initDatePicker();
				});
			},
			deleteContractor(row) {
				if (!confirm('是否確認刪除?')) return false;
				axios
					.post(`/Apply/DeleteContractor`, row)
					.then(res => {
						alert('畫面資料已儲存');
						this.getContractor();
					})
					.catch(err => {
						alert('系統發生未預期錯誤');
						console.log(err);
					});
			},
			saveForm() {
				if (!confirm('是否確認繼續?')) return false;
				axios
					.post(`/Apply/${this.mode}Contractor`, this.selectRow)
					.then(res => {
						if (!res.data.Status) {
							alert(res.data.Message);
							return;
						}

						this.getContractor();
						alert('畫面資料已儲存');
						this.dialogVisible = false;
					})
					.catch(err => {
						alert('系統發生未預期錯誤');
						console.log(err);
					});
			}
		}
	});
});
