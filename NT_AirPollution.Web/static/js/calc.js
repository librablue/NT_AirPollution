﻿document.addEventListener('DOMContentLoaded', () => {
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
			const checkE_DATE2 = (rule, value, callback) => {
				if (!value) {
					callback(new Error('請輸入結束日期'));
				}
				if (this.form.B_DATE2 && this.form.E_DATE2 && moment(value).isSameOrBefore(this.form.B_DATE2)) {
					callback(new Error('結束日期不得早於起始日期'));
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
					B_DATE2: null,
					E_DATE2: null,
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
					B_DATE2: [{ required: true, message: '請輸入開始日期', trigger: 'blur' }],
					E_DATE2: [{ validator: checkE_DATE2 }]
				})
			};
		},
		mounted() {
			this.getProjectCode();
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
