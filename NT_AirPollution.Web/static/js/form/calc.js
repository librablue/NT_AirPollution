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
            const checkE_DATE2 = (rule, value, callback) => {
                if (!value) {
                    callback(new Error('請輸入預計施工完成日期'));
                }
                if (this.form.B_DATE2 && this.form.E_DATE2 && moment(value).isSameOrBefore(this.form.B_DATE2)) {
                    callback(new Error('結束日期不得早於起始日期'));
                }
                callback();
            };
            const checkArea = (rule, value, callback) => {
                const kindAry = ['1', '2', '4', '5', '6', '7', '8', '9', 'A'];
                if (kindAry.includes(this.form.KIND_NO) && !value) {
                    callback(new Error('如果非疏濬工程，請輸入施工面積'));
                }
                callback();
            };
            const checkVolumel = (rule, value, callback) => {
                const kindAry = ['3', 'B'];
                if (kindAry.includes(this.form.KIND_NO) && !value) {
                    callback(new Error('如果為疏濬工程，請輸入清運土石體積'));
                }
                callback();
            };
            const checkMoney = (rule, value, callback) => {
                const kindAry = ['Z'];
                if (kindAry.includes(this.form.KIND_NO) && !value) {
                    callback(new Error('如果為其他營建工程，請輸入工程合約經費'));
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
                    E_DATE2: null
                },
                rules: Object.freeze({
                    KIND_NO: [{ required: true, message: '請選擇工程類別', trigger: 'change' }],
                    MONEY: [{validator: checkMoney}],
                    AREA: [{ validator: checkArea }],
                    VOLUMEL: [{ validator: checkVolumel }],
                    B_DATE2: [{ required: true, message: '請輸入預計施工開始日期', trigger: 'blur' }],
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
            }
        },
        methods: {
            getProjectCode() {
                axios.get('/Option/GetProjectCode').then(res => {
                    this.projectCode = Object.freeze(res.data);
                });
            },
            sendForm() {
                this.$refs.form.validate(valid => {
                    if (!valid) {
                        alert('欄位驗證錯誤，請檢查修正後重新送出');
                        return false;
                    }

                    axios
                        .post('/Form/GetTotalMoney', this.form)
                        .then(res => {
                            if (!res.data.Status) {
                                alert(res.data.Message);
                                return;
                            }

                            const comma=/\B(?<!\.\d*)(?=(\d{3})+(?!\d))/g;
                            const totalMoney = res.data.Message.toString().replace(comma, ',');
                            alert(`核算申報繳納營建空污費為新台幣 ${totalMoney} 元整`);
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
