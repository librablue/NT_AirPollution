<template>
	<div>
		<el-header height="64px">
			<div class="menu-left">
				<h1>
					<router-link to="/">南投縣營建空污繳費申報系統</router-link>
				</h1>
				<el-menu :default-active="activeIndex" mode="horizontal">
					<el-menu-item index="function_form" route="/function/form">申請案件管理</el-menu-item>
				</el-menu>
			</div>
			<div>
				<a href="javascript:;" class="func-link" @click="modalVisible = true">
					<i class="fa fa-user-circle-o"></i>
					{{currentUser.Account}}
				</a>
				<a href="javascript:;" class="func-link ml-1" @click="logout()">
					<i class="fa fa-sign-out"></i> 登 出
				</a>
			</div>
		</el-header>
		<el-container>
			<el-main>
				<transition name="fade" mode="out-in">
					<router-view :key="$route.fullPath"></router-view>
				</transition>
			</el-main>
		</el-container>
	</div>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
export default {
	name: 'Main',
	data() {
		return {
			modalVisible: false
		};
	},
	computed: {
		...mapGetters(['currentUser']),
		activeIndex() {
			return this.$route.name;
		}
	},
	methods: {
		...mapActions(['getCurrentUser', 'setCurrentUser']),
		logout() {
			if (!confirm('是否確認登出?')) return false;
			this.axios.get('api/Admin/Logout').then(res => {
				location.href = '#/login';
			});
		}
	}
};
</script>

<style lang="scss">
@import 'assets/scss/config';
$lineMargin: 20px;
$lineWidth: 28px;

.el-header {
	display: flex;
	justify-content: space-between;
	align-items: center;
	padding: 10px 15px;
	border-bottom: 1px solid #eee;
	font-size: 1.6rem;
	.menu-left {
		display: flex;
		align-items: center;
	}
	h1 {
		font-size: 2rem;
		text-align: center;
		margin-right: 20px;
		a {
			color: #409eff;
		}
	}
	.func-link {
		color: #595260;
	}
}

.el-main {
	overflow: inherit;
	width: 100%;
	padding: 20px 15px;
	h1 {
		font-size: 2.5rem;
		margin-bottom: 2rem;
	}
	.main {
		.el-col {
			padding: 0 10px;
		}
	}
}

.el-menu.el-menu--horizontal {
	border-bottom: none;
	.el-menu-item {
		font-size: 16px;
	}
}
</style>

