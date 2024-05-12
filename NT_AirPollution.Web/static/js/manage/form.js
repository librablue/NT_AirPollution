document.addEventListener('DOMContentLoaded', () => {
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
					BUD_DOC2: '無',
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
				if (!this.selectRow.B_DATE2 || !this.selectRow.E_DATE2) return '';
				var date1 = new Date(this.selectRow.B_DATE2);
				var date2 = new Date(this.selectRow.E_DATE2);

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
