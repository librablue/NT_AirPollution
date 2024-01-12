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
			return {
				formID: null,
				form: {},
				promise: {},
				rules: Object.freeze({
					StartDate: [{ required: true, message: '請選擇執行起始日期', trigger: 'blur' }],
					Option1: [{ required: true, message: '請選擇執行起始日期', trigger: 'blur' }]
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
			this.getFormByID();
			this.getPromiseByForm();
		},
		methods: {
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
					.get('/Soil/GetPromiseByForm', {
						params: {
							formID: this.formID
						}
					})
					.then(res => {
						this.loading = false;
						if (!res.data.Status) {
							return;
						}
						this.promise = res.data.Message || {
							Option1: null,
							Option2: null,
							Other1: null,
							Other2: null,
							Reason: null
						};
					})
					.catch(err => {
						this.loading = false;
						console.log(err);
					});
			},
			option1Change(val) {
				switch (val) {
					case 1:
						this.promise.Reason = null;
						this.promise.Other2 = null;
						break;
					case 2:
						this.promise.Other1 = null;
						break;
				}
				this.promise.Option2 = null;
			},
			option2Change(val) {
				switch (val) {
					case 1:
					case 2:
						this.promise.Other1 = null;
						this.promise.Reason = null;
						this.promise.Other2 = null;
						break;
					case 3:
					case 4:
						this.promise.Reason = null;
						this.promise.Other2 = null;
						break;
					case 5:
						this.promise.Other1 = null;
						this.promise.Other2 = null;
						break;
					case 6:
						this.promise.Other1 = null;
						this.promise.Reason = null;
						break;
				}
			},
			addPromise() {
				if(!this.promise.StartDate) {
					alert('請選擇開挖日期');
					return;
				}
				if(!this.promise.DigDays) {
					alert('請輸入開挖天數');
					return;
				}
				if (!this.promise.Option1) {
					alert('請勾選是否配合廢土不落地作業');
					return;
				}
				if (this.promise.Option2 === 3 && !this.promise.Other1) {
					alert('請填寫其他內容');
					return;
				}
				if (this.promise.Option2 === 5 && !this.promise.Reason) {
					alert('請填寫請說明工法與原因');
					return;
				}
				if (this.promise.Option2 === 6 && !this.promise.Other2) {
					alert('請填寫其他內容');
					return;
				}

				this.promise.FormID = this.formID;
				if (!confirm('資料送出後不可修改，是否確認繼續?')) return false;
				axios
					.post('/Soil/AddPromise', this.promise)
					.then(res => {
						if (!res.data.Status) {
							alert(res.data.Message);
							return;
						}

						this.getPromiseByForm();
						alert('畫面資料已儲存');
						location.reload();
					})
					.catch(err => {
						alert('系統發生未預期錯誤');
						console.log(err);
					});
			}
		}
	});
});
