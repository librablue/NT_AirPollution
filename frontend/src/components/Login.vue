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
				<div class="captcha-row">
					<input placeholder="驗證碼" v-model="user.Captcha" @keyup.enter="login()" />
					<img ref="captcha" src="api/Admin/Captcha" @click="refreshCaptcha()" />
				</div>
			</div>
			<div class="form-item">
				<button @click="login()">登 入</button>
			</div>
		</div>
	</div>
</template>

<script>
import { mapActions } from 'vuex';
export default {
	name: 'Login',
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
		login() {
			this.axios
				.post('api/Admin/Login', this.user)
				.then(res => {
                    if(!res.data.Status) {
                        this.$message.error(res.data.Message);
                        this.refreshCaptcha();
                        return;
                    }
					this.setCurrentUser(res.data);
					this.$router.push('/');
				})
				.catch(err => {
					this.refreshCaptcha();
					this.$message.error(err.response.data.ExceptionMessage);
				});
		},
		refreshCaptcha() {
			var id = Math.random();
			this.$refs.captcha.setAttribute('src', 'api/Admin/Captcha?id=' + id);
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
	.captcha-row {
		display: flex;
		align-items: center;
		input {
			flex: 1;
		}
		img {
			width: 100px;
			height: 41px;
		}
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
