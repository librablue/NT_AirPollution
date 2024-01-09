document.addEventListener('DOMContentLoaded', () => {
    new Vue({
        el: '#app',
        data() {
            return {
                loading: false,
                filter: {
                    C_NO: '',
                    COMP_NAM: '',
                    WorkStatus: 0,
                    Commitment: 0
                },
                selectRow: {},
                dialogVisible: false
            };
        },
        methods: {
            showEditModal(row) {
                this.selectRow = JSON.parse(JSON.stringify(row));
                this.dialogVisible = true;
            },
            saveForm() {
                if (!confirm('是否確認繼續?')) return false;
                axios
                    .post(`/Apply/${this.mode}Company`, this.selectRow)
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
