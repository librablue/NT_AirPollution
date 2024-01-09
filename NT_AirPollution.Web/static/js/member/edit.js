document.addEventListener('DOMContentLoaded', () => {
	new Vue({
		el: '#app',
		data() {
			const checkPassword2 = (rule, value, callback) => {
				if (value !== this.form.Password) {
					callback(new Error('二次輸入的密碼不一致'));
				}
				callback();
			};
			return {
				form: {
					UserType: null,
					Email: '',
					UserName: '',
					CompanyID: '',
					Password: '',
					Password2: ''
				},
				rules: Object.freeze({
					Password: [{ required: true, message: '請輸入密碼', trigger: 'blur' }],
					Password2: [{ validator: checkPassword2 }]
				})
			};
		},
		mounted() {
			this.getCurrentUser();
		},
		methods: {
			getCurrentUser() {
				axios.get('/Member/GetCurrentUser').then(res => {
					if (!res.data.Status) {
						alert(res.data.Message);
						return;
					}

					this.form = res.data.Message;
				});
			},
			updatePassword() {
				this.$refs.form.validate(valid => {
					if (!valid) {
						return false;
					}

					axios
						.post('/Member/Edit', this.form)
						.then(res => {
							if (!res.data.Status) {
								alert(res.data.Message);
								return;
							}

							alert('密碼修改成功，請重新登入');
							location.href = '/Member/Logout';
						})
						.catch(err => {
							console.log(err);
							alert('系統發生未預期錯誤');
						});
				});
			}
		}
	});
});
