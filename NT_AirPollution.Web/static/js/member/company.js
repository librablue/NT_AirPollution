document.addEventListener('DOMContentLoaded', () => {
    new Vue({
        el: '#app',
        data() {
            return {
                mode: '',
                loading: false,
                filter: {
                    S_G_NO: '',
                    S_NAME: '',
                    S_B_NAM: '',
                    S_C_NAM: ''
                },
                companies: [],
                selectRow: {},
                dialogVisible: false
            };
        },
        methods: {
            getCompanies() {
                this.loading = true;
                axios.post('/Member/GetMyCompanies', this.filter).then(res => {
                    this.companies = res.data;
                    this.loading = false;
                });
            },
            addCompany() {
                this.mode = 'Add';
                this.selectRow = {};
                this.dialogVisible = true;
            },
            showEditModal(row) {
                this.mode = 'Update';
                this.selectRow = JSON.parse(JSON.stringify(row));
                this.dialogVisible = true;
            },
            deleteCompany(row) {
                if (!confirm('是否確認刪除?')) return false;
                axios
                    .post(`/Member/DeleteCompany`, row)
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
                    .post(`/Member/${this.mode}Company`, this.selectRow)
                    .then(res => {
                        if (!res.data.Status) {
                            alert(res.data.Message);
                            return;
                        }

                        this.getCompanies();
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
