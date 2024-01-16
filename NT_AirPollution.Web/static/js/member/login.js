document.addEventListener('DOMContentLoaded', () => {
	new Vue({
		el: '#app',
		data() {
			return {
				form: {
					Email: '',
					Password: '',
					Captcha: ''
				},
				rules: Object.freeze({
					Email: [{ required: true, message: '請輸入Email', trigger: 'blur' }],
					Password: [{ required: true, message: '請輸入密碼', trigger: 'blur' }],
					Captcha: [{ required: true, message: '請勾選我不是機器人', trigger: 'blur' }]
				})
			};
		},
		methods: {
			login() {
				this.form.Captcha = grecaptcha.getResponse();
				this.$refs.form.validate(valid => {
					if (!valid) {
						return false;
					}

					axios
						.post('/Member/Login', this.form)
						.then(res => {
							if (!res.data.Status) {
								grecaptcha.reset();
								this.form.Captcha = '';
								alert(res.data.Message);
								return;
							}

							const url = new URL(location.href);
							const returnUrl = url.searchParams.get('ReturnUrl');
							if (returnUrl) {
								location.href = `${document.baseURI}${returnUrl}`;
								return;
							}

							if (res.data.Message === 1) location.href = `${document.baseURI}Apply/Form`;
							else location.href = `${document.baseURI}Manage/Form`;
						})
						.catch(err => {
							console.log(err);
							grecaptcha.reset();
							this.form.Captcha = '';
							alert('系統發生未預期錯誤');
						});
				});
			}
		}
	});
});
