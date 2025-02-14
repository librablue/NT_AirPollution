<template>
	<div class="main">
		<h1>郵局利率管理</h1>
		<el-form size="small" inline>
			<el-form-item>
				<el-button type="primary" @click="getRates()">
					<i class="fa fa-search"></i> 查 詢
				</el-button>
			</el-form-item>
			<el-form-item>
				<el-button type="success" @click="addRate()">
					<i class="fa fa-plus"></i> 新 增
				</el-button>
			</el-form-item>
		</el-form>
		<div style="width:320px">
            <vxe-table :data="rates" :loading="loading" size="small" max-height="640px" show-overflow border resizable auto-resize :sort-config="{ trigger: 'cell', defaultSort: { field: 'CreateDate', order: 'desc' }}">
			<vxe-table-column title="功能" align="center" width="60" fixed="left">
				<template v-slot="{ row }">
					<el-button type="danger" size="mini" icon="el-icon-delete" circle @click="deleteNews(row)"></el-button>
				</template>
			</vxe-table-column>
			<vxe-table-column field="Date" title="日期" align="center" width="140">
                <template v-slot="{ row }">{{row.Date | date}}</template>
            </vxe-table-column>
			<vxe-table-column field="Rate" title="利率(%)" align="right" width="100"></vxe-table-column>
		</vxe-table>
        </div>
		<RateModal :show.sync="modalVisible" @on-success="onSuccess" />
	</div>
</template>

<script>
import { dateTime } from '@/mixins/filter';
import RateModal from '@/components/function/child/RateModal';
export default {
	name: 'Rate',
    mixins: [dateTime],
	components: { RateModal },
	data() {
		return {
			loading: false,
			rates: [],
			modalVisible: false
		};
	},
	mounted() {
		this.getRates();
	},
	methods: {
		getRates() {
			this.loading = true;
			this.axios.get('api/Option/GetRates').then(res => {
				this.rates = res.data;
				this.loading = false;
			});
		},
		addRate() {
			this.modalVisible = true;
		},
		onSuccess(data) {
			this.getRates();
		},
		deleteNews(row) {
			if (!confirm('是否確認刪除?')) return false;
			this.axios
				.post('api/Option/DeleteRate', row)
				.then(res => {
					this.getRates();
				})
				.catch(err => {
					this.$message.error(err.response.data.ExceptionMessage);
				});
		}
	}
};
</script>

<style lang="scss">
</style>