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
				promise: null,
				rules: Object.freeze({
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
						this.promise = res.data.Message;
					})
					.catch(err => {
						this.loading = false;
						console.log(err);
					});
			},
			addPromise() {
				
			}
		}
	});
});
