document.addEventListener('DOMContentLoaded', () => {
    new Vue({
        el: '#app',
        data() {
            return {
                mode: '',
                loading: false,
                filter: {
                    C_NO: '',
                    StartDate: '',
                    EndDate: '',
                    S_NAME: '',
                    S_G_NO: '',
                    R_NAME: '',
                    R_G_NO: '',
                    COMP_NAM: '',
                    ADDR: '',
                    B_SERNO: '',
                    Status: ''
                },
                forms: [],
                selectRow: {},
                dialogVisible: false
            };
        },
        methods: {
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
                this.selectRow = {};
                this.dialogVisible = true;
            },
            showEditModal(row) {
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
