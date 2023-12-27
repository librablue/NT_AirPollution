document.addEventListener('DOMContentLoaded', () => {
    new Vue({
        el: '#app',
        data() {
            const checkEmail = (rule, value, callback) => {
                const regexMail =
                    /(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])/;
                if (!regexMail.test(value)) {
                    callback(new Error('Email格式錯誤'));
                }
                callback();
            };
            const checkActiveCode = (rule, value, callback) => {
                if (this.step === 'SUBMIT' && !value) {
                    callback(new Error('密碼複雜度不符合要求'));
                }
                callback();
            };
            const checkPassword = (rule, value, callback) => {
                const regexPwd = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20}$/;
                if (this.step === 'SUBMIT' && !regexPwd.test(value)) {
                    callback(new Error('密碼複雜度不符合規則'));
                }
                callback();
            };
            const checkPassword2 = (rule, value, callback) => {
                if (this.step === 'SUBMIT' && value !== this.form.Password) {
                    callback(new Error('二次密碼輸入不相等'));
                }
                callback();
            };
            return {
                step: 'VERIFY_MAIL',
                btnSendCodeText: '寄送驗證碼',
                btnSendCodeDisabled: false,
                form: {
                    Email: '',
                    ActiveCode: '',
                    Password: '',
                    Password2: '',
                    Captcha: ''
                },
                rules: Object.freeze({
                    Email: [{ validator: checkEmail }],
                    ActiveCode: [{ validator: checkActiveCode }],
                    Password: [{ validator: checkPassword }],
                    Password2: [{ validator: checkPassword2 }]
                })
            };
        },
        methods: {
            sendCode() {
                this.step = 'VERIFY_MAIL';
                this.$refs.form.validate(valid => {
                    if (!valid) {
                        return false;
                    }

                    axios
                        .post('/Member/SendCode', this.form)
                        .then(res => {
                            if (!res.data.Status) {
                                alert(res.data.Message);
                                return;
                            }

                            this.btnSendCodeDisabled = true;
                            this.btnSendCodeText = '寄送驗證碼';
                            let countDown = 300;
                            const interval = setInterval(() => {
                                this.btnSendCodeText = `寄送驗證碼(${countDown})`;
                                countDown -= 1;
                                if (countDown <= 0) {
                                    clearInterval(interval);
                                    this.btnSendCodeDisabled = false;
                                    this.btnSendCodeText = '寄送驗證碼';
                                }
                            }, 1000);

                            alert(res.data.Message);
                        })
                        .catch(err => {
                            console.log(err.response.data.ExceptionMessage);
                            alert('系統發生未預期錯誤');
                        });
                });
            },
            signUp() {
                this.step = 'SUBMIT';
                this.$refs.form.validate(valid => {
                    if (!valid) {
                        return false;
                    }

                    if (!confirm('是否確認送出?')) return;
                    axios
                        .post('/Member/SignUp', this.form)
                        .then(res => {
                            if (!res.data.Status) {
                                alert(res.data.Message);
                                return;
                            }

                            alert('註冊完成，請重新登入');
                            location.href = 'SignIn';
                        })
                        .catch(err => {
                            console.log(err.response.data.ExceptionMessage);
                            alert('系統發生未預期錯誤');
                        });
                });
            }
        }
    });
});
