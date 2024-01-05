document.addEventListener('DOMContentLoaded', () => {
    new Vue({
        el: '#app',
        data() {
            return {
                form: {
                    C_NO: '',
                    CreateUserEmail: '',
                    Captcha: ''
                },
                rules: Object.freeze({
                    C_NO: [{ required: true, message: '請輸入管制編號', trigger: 'blur' }],
                    CreateUserEmail: [{ required: true, message: '請輸入Email', trigger: 'blur' }],
                    Captcha: [{ required: true, message: '請勾選我不是機器人' }]
                })
            };
        },
        methods: {
            formSubmit() {
                this.form.Captcha = grecaptcha.getResponse();
                this.$refs.form.validate(valid => {
                    if (!valid) {
                        return false;
                    }

                    axios
                        .post('/Search/Index', this.form)
                        .then(res => {
                            if (!res.data.Status) {
                                grecaptcha.reset();
                                alert(res.data.Message);
                                return;
                            }

                            location.href = '/Search/Result';
                        })
                        .catch(err => {
                            console.log(err);
                            grecaptcha.reset();
                            alert('系統發生未預期錯誤');
                        });
                });
            }
        }
    });
});
