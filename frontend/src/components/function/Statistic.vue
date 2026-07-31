<template>
	<div class="main">
		<h1>成果統計</h1>
		<el-form size="small" inline>
			<el-form-item label="開始日期">
				<el-date-picker style="width:140px" v-model="filter.sdate" type="date" value-format="yyyy-MM-dd" placeholder="請選擇日期"></el-date-picker>
			</el-form-item>
			<el-form-item label="結束日期">
				<el-date-picker style="width:140px" v-model="filter.edate" type="date" value-format="yyyy-MM-dd" placeholder="請選擇日期"></el-date-picker>
			</el-form-item>
			<el-form-item>
				<el-button type="primary" @click="getStatistic">
					<i class="fa fa-search"></i> 查 詢
				</el-button>
			</el-form-item>
		</el-form>

		<!-- 數據顯示卡片區域 -->
		<el-row :gutter="20" class="stat-cards">
			<el-col :span="6">
				<el-card shadow="hover">
					<div class="stat-title">用戶數量</div>
					<div class="stat-value">{{ userCount | comma }} 戶</div>
				</el-card>
			</el-col>
			<el-col :span="6">
				<el-card shadow="hover">
					<div class="stat-title">表單數量</div>
					<div class="stat-value">{{ formsCount | comma }} 位</div>
				</el-card>
			</el-col>
			<el-col :span="6">
				<el-card shadow="hover">
					<div class="stat-title">繳費數量</div>
					<div class="stat-value">{{ paymentCount | comma }} 件</div>
				</el-card>
			</el-col>
			<el-col :span="6">
				<el-card shadow="hover">
					<div class="stat-title">減碳量</div>
					<div class="stat-value">{{ carbon | commaPrecision2 }} 公斤</div>
				</el-card>
			</el-col>
		</el-row>
	</div>
</template>

<script>
import { comma } from '@/mixins/filter';
export default {
	name: 'Statistic',
	mixins: [comma],
	data() {
		return {
			loading: false,
			filter: {
				sdate: moment().format('YYYY-MM-01'),
				edate: moment().format('YYYY-MM-DD')
			},
			userCount: 0,
			formsCount: 0,
			paymentCount: 0,
			carbon: 0
		};
	},
	mounted() {
		this.getStatistic();
	},
	methods: {
		getStatistic() {
			this.getUserCount();
			this.getFormsCount();
			this.getPaymentCount();
			this.getCarbon();
		},
		getUserCount() {
			this.axios
				.get('api/Statistic/GetUserCount', {
					params: this.filter
				})
				.then(res => {
					this.userCount = res.data;
				});
		},
		getFormsCount() {
			this.axios
				.get('api/Statistic/GetFormsCount', {
					params: this.filter
				})
				.then(res => {
					this.formsCount = res.data;
				});
		},
		getPaymentCount() {
			this.axios
				.get('api/Statistic/GetPaymentCount', {
					params: this.filter
				})
				.then(res => {
					this.paymentCount = res.data;
				});
		},
		getCarbon() {
			this.axios
				.get('api/Statistic/GetCarbon', {
					params: this.filter
				})
				.then(res => {
					this.carbon = res.data;
				});
		}
	}
};
</script>

<style scoped>
.stat-cards {
	margin-top: 20px;
}
.stat-title {
	font-size: 14px;
	color: #909399;
	margin-bottom: 8px;
}
.stat-value {
	font-size: 24px;
	font-weight: bold;
	color: #303133;
}
</style>