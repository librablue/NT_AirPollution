document.addEventListener('DOMContentLoaded', () => {
	new Vue({
		el: '#app',
		filters: {
			date: value => {
				if (!value || value === '0001-01-01T00:00:00') return '';
				return moment(value).format('YYYY-MM-DD');
			},
			datetime: value => {
				if (!value || value === '0001-01-01T00:00:00') return '';
				return moment(value).format('YYYY-MM-DD HH:mm');
			}
		},
		data() {
			return {
				mode: '',
				loading: false,
				filter: {
					StartDate: '',
					EndDate: '',
					C_NO: null,
					PUB_COMP: null,
					CreateUserName: null,
					COMP_NAM: null
				},
				district: Object.freeze([]),
				forms: [],
				selectRow: {
					P_KIND: '一次全繳',
					BUD_DOC2: '無'
				},
				dialogVisible: false
			};
		},
		mounted() {
			this.getDistrict();
		},
		methods: {
			getDistrict() {
				axios.get('/Option/GetDistrict').then(res => {
					this.district = Object.freeze(res.data);
				});
			},
			getForms() {
				this.loading = true;
				axios
					.post('/Member/GetForms', this.filter)
					.then(res => {
						this.forms = res.data;
						this.loading = false;
					})
					.catch(err => {
						this.loading = false;
						console.log(err);
					});
			},
			addForm() {
				this.mode = 'Add';
				this.selectRow = {
					P_KIND: '一次全繳',
					BUD_DOC2: '無'
				};
				this.dialogVisible = true;
			},
			showModal(row) {
				this.mode = 'View';
				this.selectRow = JSON.parse(JSON.stringify(row));
				this.dialogVisible = true;
			},
			saveForm() {
				if (!confirm('是否確認繼續?')) return false;
				axios
					.post(`/Member/AddForm`, this.selectRow)
					.then(res => {
						if (!res.data.Status) {
							alert(res.data.Message);
							return;
						}

						this.getForms();
						alert('畫面資料已儲存。');
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
