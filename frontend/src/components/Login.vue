<template>
	<div class="login-form">
		<h1>南投縣營建空污繳費申報系統</h1>
		<div class="form-wrap">
			<div class="form-item">
				<input type="text" placeholder="帳號" v-model="user.Account">
			</div>
			<div class="form-item">
				<input type="password" placeholder="密碼" v-model="user.Password">
			</div>
			<div class="form-item">
				<vue-recaptcha ref="captcha" sitekey="6Lde16sZAAAAAKrHCmXbAXCray0vlqEKblppoTy8" @verify="verify"></vue-recaptcha>
			</div>
			<div class="form-item">
				<button @click="login()">登 入</button>
			</div>
		</div>
	</div>
</template>

<script>
import VueRecaptcha from 'vue-recaptcha';
import { mapActions } from 'vuex';
export default {
	name: 'Login',
	components: { VueRecaptcha },
	data() {
		return {
			user: {
				Account: '',
				Password: '',
				Captcha: ''
			}
		};
	},
	methods: {
		...mapActions(['setCurrentUser']),
		verify(response) {
			this.user.Captcha = response;
		},
		login() {
			this.axios
				.post('api/Auth/SignIn', this.user)
				.then(res => {
					this.setCurrentUser(res.data);
					this.$router.push({ path: '/' });
				})
				.catch(err => {
					this.$message.error('登入失敗');
					this.$refs.captcha.reset();
				});
		}
	}
};
</script>

<style lang="scss" scoped>
.login-form {
	background-color: #fff;
	margin: 80px auto 0;
	h1 {
		text-align: center;
		margin-bottom: 1em;
		color: #566375;
	}
	h2 {
		color: #fff;
		background-color: #607d8b;
		text-align: center;
		padding: 8px 0;
		margin-bottom: 30px;
	}
	.form-wrap {
		width: 90%;
		max-width: 350px;
		margin: 0 auto;
		padding: 20px;
	}
	.form-item {
		margin-bottom: 20px;
	}
	input {
		box-sizing: border-box;
		padding: 12px 20px;
		border: 1px solid #ccc;
		width: 100%;
		&:focus {
			outline: none;
		}
	}
	button {
		width: 100%;
		padding: 10px;
		background-color: #566375;
		border: none;
		color: #fff;
		font-size: 18px;
		cursor: pointer;
		&:focus {
			outline: none;
		}
	}
}
</style>
