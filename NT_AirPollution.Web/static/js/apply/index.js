document.addEventListener('DOMContentLoaded', () => {
    new Vue({
        el: '#app',
        filters: {
            date: value => {
                if (!value || value === '0001-01-01T00:00:00') return '';
                return moment(value).format('YYYY-MM-DD');
            },
            formStatus: value => {
                switch (value) {
                    case 1:
                        return '審理中';
                    case 2:
                        return '待補件';
                    case 3:
                        return '通過待繳費';
                    case 4:
                        return '繳費完成';
                    default:
                        return '';
                }
            },
            calcStatus: value => {
                switch (value) {
                    case 0:
                        return '未申請';
                    case 1:
                        return '審理中';
                    case 2:
                        return '待補件';
                    case 3:
                        return '通過待繳費';
                    case 4:
                    case 5:
                        return '通過待退費';
                    case 6:
                        return '繳退費完成';
                    default:
                        return '';
                }
            }
        },
        data() {
            return {
                filter: {
                    StartDate: moment().format('YYYY-MM-01'),
                    EndDate: moment().format('YYYY-MM-DD'),
                    C_NO: null,
                    PUB_COMP: null,
                    CreateUserName: null,
                    COMP_NAM: null,
                    FormStatus: 0
                },
                forms: [],
            };
        },
        methods: {
            getForms() {
                const loading = this.$loading();
                axios
                    .post('/Apply/GetForms', this.filter)
                    .then(res => {
                        this.forms = res.data;
                        loading.close();
                    })
                    .catch(err => {
                        loading.close();
                        console.log(err);
                    });
            }
        }
    });
});
