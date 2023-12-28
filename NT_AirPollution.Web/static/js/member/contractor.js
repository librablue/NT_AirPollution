document.addEventListener('DOMContentLoaded', () => {
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
				contractor: [],
				selectRow: {},
				dialogVisible: false
			};
		},
		methods: {
			getContractor() {
				this.loading = true;
				axios
					.post('/Member/GetMyContractor', this.filter)
					.then(res => {
						this.contractor = res.data;
						this.loading = false;
					})
					.catch(err => {
						this.loading = false;
						console.log(err);
					});
			},
			addContractor() {
				this.mode = 'Add';
				this.selectRow = {};
				this.dialogVisible = true;
			},
			showEditModal(row) {
				this.mode = 'Update';
				this.selectRow = JSON.parse(JSON.stringify(row));
				this.dialogVisible = true;
			},
			deleteContractor(row) {
				if (!confirm('是否確認刪除?')) return false;
				axios
					.post(`/Member/DeleteContractor`, row)
					.then(res => {
						alert('畫面資料已儲存。');
						this.getCompanies();
					})
					.catch(err => {
						alert('系統發生未預期錯誤');
						console.log(err);
					});
			},
			saveForm() {
				if (!confirm('是否確認繼續?')) return false;
				axios
					.post(`/Member/${this.mode}Contractor`, this.selectRow)
					.then(res => {
						alert('畫面資料已儲存。');
						this.getContractor();
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
