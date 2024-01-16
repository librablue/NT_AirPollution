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
				activeName: 'first',
				form: {
					UserType: null,
					Email: '',
					UserName: '',
					CompanyID: '',
					Password: '',
					Password2: ''
				},
				rules1: Object.freeze({
					UserName: [{ required: true, message: '請輸入姓名', trigger: 'blur' }]
				}),
				rules2: Object.freeze({
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
			updateProfile() {
				this.$refs.form1.validate(valid => {
					if (!valid) {
						return false;
					}

					axios
						.post('/Member/UpdateProfile', this.form)
						.then(res => {
							if (!res.data.Status) {
								alert(res.data.Message);
								return;
							}

							alert('基本資料修改成功');
							location.reload();
						})
						.catch(err => {
							console.log(err);
							alert('系統發生未預期錯誤');
						});
				});
			},
			updatePassword() {
				this.$refs.form2.validate(valid => {
					if (!valid) {
						return false;
					}

					axios
						.post('/Member/UpdatePassword', this.form)
						.then(res => {
							if (!res.data.Status) {
								alert(res.data.Message);
								return;
							}

							alert('密碼修改成功，請重新登入');
							location.href = `${document.baseURI}Member/Logout`;
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
