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
                        return '需補件';
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
                        return '需補件';
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
                loading: false,
                forms: [],
                sendText: '寄送驗證信',
                sending: false
            };
        },
        mounted() {
            this.getMyForm();
        },
        methods: {
            getMyForm() {
                this.loading = true;
                axios.get('/Search/GetMyForm').then(res => {
                    this.forms = [res.data];
                    this.loading = false;
                });
            },
            resend() {
                axios
                    .post('/Search/Resend')
                    .then(res => {
                        this.sending = true;
                        var seconds = 10;
                        this.sendText = '重新寄送(' + seconds + ')';
                        seconds -= 1;
                        var id = setInterval(() => {
                            this.sendText = '重新寄送(' + seconds + ')';
                            seconds--;
                            if (seconds < 0) {
                                clearInterval(id);
                                this.sending = false;
                                this.sendText = '寄送驗證信';
                            }
                        }, 1000);
                    })
                    .catch(err => {
                        console.log(err);
                        alert('發生錯誤');
                    });
            },
            beforeCommand(row, cmd) {
                return {
                    row,
                    cmd
                };
            },
            handleCommand(arg) {
                const { row, cmd } = arg;
                switch (cmd) {
                    case 'VIEW':
                        this.showModal(row);
                        break;
                    case 'DOWNLOAD_PAYMENT':
                        this.downloadPayment(row);
                        break;
                    case 'CALC':
                        this.finalCalc(row);
                        break;
                    case 'DOWNLOAD_REPAYMENT':
                        this.downloadRePayment(row);
                        break;
                    case 'BANK_ACCOUNT':
                        this.showBankAccountModal(row);
                        break;
                    case 'DOWNLOAD_PROOF':
                        this.downloadProof(row);
                        break;
                }
            }
        }
    });
});
