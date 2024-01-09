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
                forms: []
            };
        },
        methods: {
            GetForms() {
                axios.post('/Manage/GetForms', this.filter).then(res => {
                    this.forms = res.data.Message;
                });
            }
        }
    });
});
