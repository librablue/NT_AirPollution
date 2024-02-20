<template>
	<div class="main">
		<h1>最新消息管理</h1>
		<el-form size="small" inline>
			<el-form-item>
				<el-button type="primary" @click="getNews()">
					<i class="fa fa-search"></i> 查 詢
				</el-button>
			</el-form-item>
			<el-form-item>
				<el-button type="success" @click="addNews()">
					<i class="fa fa-plus"></i> 新 增
				</el-button>
			</el-form-item>
		</el-form>
		<vxe-table :data="news" :loading="loading" size="small" max-height="640px" show-overflow border resizable auto-resize :sort-config="{ trigger: 'cell', defaultSort: { field: 'CreateDate', order: 'desc' }}">
			<vxe-table-column title="功能" align="center" width="100" fixed="left">
				<template v-slot="{ row }">
					<el-button size="mini" icon="el-icon-edit" circle @click="showDetail(row)"></el-button>
					<el-button type="danger" size="mini" icon="el-icon-delete" circle @click="deleteNews(row)"></el-button>
				</template>
			</vxe-table-column>
			<vxe-table-column field="ID" title="系統編號" align="center" width="100"></vxe-table-column>
			<vxe-table-column field="Title" title="標題"></vxe-table-column>
            <vxe-table-column field="PublishDate" title="發佈日期" align="center" width="140" sortable>
				<template v-slot="{ row }">{{ row.PublishDate | date }}</template>
			</vxe-table-column>
			<vxe-table-column field="CreateDate" title="建立日期" align="center" width="140" sortable>
				<template v-slot="{ row }">{{ row.CreateDate | date }}</template>
			</vxe-table-column>
		</vxe-table>
		<NewsModal :show.sync="modalVisible" :data="selectNews" @on-success="onSuccess" />
	</div>
</template>

<script>
import { dateTime } from '@/mixins/filter';
import NewsModal from '@/components/function/child/NewsModal';
export default {
	name: 'News',
	mixins: [dateTime],
	components: { NewsModal },
	data() {
		return {
			loading: false,
			news: [],
			selectNews: {},
			modalVisible: false
		};
	},
	mounted() {
		this.getNews();
	},
	methods: {
		getNews() {
			this.loading = true;
			this.axios.get('api/News/GetNews').then(res => {
				this.news = res.data;
				this.loading = false;
			});
		},
		addNews() {
			this.selectNews = null;
			this.modalVisible = true;
		},
		showDetail(row) {
			this.selectNews = row;
			this.modalVisible = true;
		},
		onSuccess(data) {
			this.getNews();
		},
		deleteNews(row) {
			if (!confirm('是否確認刪除?')) return false;
			this.axios
				.post('api/News/DeleteNews', row)
				.then(res => {
					this.getNews();
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