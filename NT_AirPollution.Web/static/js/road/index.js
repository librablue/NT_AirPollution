document.addEventListener('DOMContentLoaded', () => {
	new Vue({
		el: '#app',
		filters: {
			date: value => {
				if (!value || value === '0001-01-01T00:00:00') return '';
				return moment(value).format('YYYY-MM-DD');
			}
		},
		data() {
			const checkCleanWay = (rule, value, callback) => {
				if (!this.promise.CleanWay1 || !this.promise.CleanWay2) {
					callback(new Error('請選擇清掃方式'));
				}
				callback();
			};
			const checkFrequency = (rule, value, callback) => {
				if (!this.promise.Frequency || !this.promise.Times) {
					callback(new Error('請輸入清掃頻率'));
				}
				callback();
			};
			const checkDate = (rule, value, callback) => {
				if (!value) {
					callback(new Error('請選擇執行結束日期'));
				}
				if (this.promise.StartDate && this.promise.EndDate && moment(value).isSameOrBefore(this.promise.StartDate)) {
					callback(new Error('結束日期不得早於起始日期'));
				}
				callback();
			};
			return {
				formID: null,
				mode: '',
				loading: false,
                user: {},
				form: {},
				promise: null,
                roadReports: [],
				report: {},
				promiseDialogVisible: false,
				reportDialogVisible: false,
				promiseRules: Object.freeze({
					CleanWay: [{ validator: checkCleanWay }],
					Frequency: [{ validator: checkFrequency }],
					StartDate: [{ required: true, message: '請選擇執行起始日期', trigger: 'blur' }],
					EndDate: [{ validator: checkDate }]
				})
			};
		},
		mounted() {
			var url = new URL(location.href);
			this.formID = url.searchParams.get('id');
			if (!this.formID) {
				location.href = '/Manage/Form';
				return;
			}
            this.getCurrentUser();
			this.getFormByID();
			this.getPromiseByForm();
            this.getReportByForm();
		},
		methods: {
            getCurrentUser() {
				axios.get('/Member/GetCurrentUser').then(res => {
					if (!res.data.Status) {
						alert(res.data.Message);
						return;
					}

					this.user = res.data.Message;
				});
			},
			getFormByID() {
				axios
					.get(`/Manage/GetFormByID`, {
						params: {
							id: this.formID
						}
					})
					.then(res => {
						if (!res.data.Status) {
							alert(res.data.Message);
							location.href = '/Manage/Form';
							return;
						}
						this.form = res.data.Message;
					})
					.catch(err => {
						console.log(err);
					});
			},
			getPromiseByForm() {
				this.loading = true;
				axios
					.get('/Road/GetPromiseByForm', {
						params: {
							formID: this.formID
						}
					})
					.then(res => {
						this.loading = false;
						if (!res.data.Status) {
							return;
						}
						this.promise = res.data.Message;
					})
					.catch(err => {
						this.loading = false;
						console.log(err);
					});
			},
			getReportByForm() {
				this.loading = true;
				axios
					.get('/Road/GetReportByForm', {
						params: {
							formID: this.formID
						}
					})
					.then(res => {
						this.loading = false;
						if (!res.data.Status) {
							return;
						}
						this.roadReports = res.data.Message;
					})
					.catch(err => {
						this.loading = false;
						console.log(err);
					});
			},
			addPromise() {
				this.promise = {
					FormID: this.formID,
					Roads: []
				};
				this.promiseDialogVisible = true;
			},
			addRoad() {
				this.promise.Roads.push({
					RoadName: null,
					RoadLength: null,
					Times: 1
				});
			},
			deleteRoad(idx) {
				if (!confirm('是否確認刪除?')) return;
				this.promise.Roads.splice(idx, 1);
			},
			sendPromise() {
				this.$refs.form1.validate(valid => {
					if (!valid) {
						alert('欄位驗證錯誤，請檢查修正後重新送出');
						return false;
					}

					if (!confirm('是否確認繼續?')) return false;
					axios
						.post('/Road/AddPromise', this.promise)
						.then(res => {
							if (!res.data.Status) {
								alert(res.data.Message);
								return;
							}

							this.getPromiseByForm();
							this.promiseDialogVisible = false;
							alert('畫面資料已儲存');
						})
						.catch(err => {
							alert('系統發生未預期錯誤');
							console.log(err);
						});
				});
			},
			addReport() {
				this.report = {
					FormID: this.formID,
					YearMth: moment().format('YYYYMM'),
					Roads: JSON.parse(JSON.stringify(this.promise.Roads))
				};
				this.reportDialogVisible = true;
			},
			sendReport() {
				this.$refs.form2.validate(valid => {
					if (!valid) {
						alert('欄位驗證錯誤，請檢查修正後重新送出');
						return false;
					}

					if (!confirm('資料送出後不可修改，是否確認繼續?')) return false;
					axios
						.post('/Road/AddReport', this.report)
						.then(res => {
							if (!res.data.Status) {
								alert(res.data.Message);
								return;
							}

							this.getReportByForm();
							this.reportDialogVisible = false;
							alert('畫面資料已儲存');
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
