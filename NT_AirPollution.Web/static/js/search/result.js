document.addEventListener('DOMContentLoaded', () => {
	new Vue({
		el: '#app',
		data() {
			return {
				form: {
					PaymentID: '',
					Email: '',
					Captcha: ''
				},
				sendText: '寄送驗證信',
				sending: false
			};
		},
		methods: {
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
			}
		}
	});
});
