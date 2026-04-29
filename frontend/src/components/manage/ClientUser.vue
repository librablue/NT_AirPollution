<template>
	<div class="main">
		<h1>使用者管理</h1>
		<el-form size="small" inline>
			<el-form-item label="帳號">
				<el-input v-model="filter.Email"></el-input>
			</el-form-item>
			<el-form-item label="姓名">
				<el-input v-model="filter.UserName"></el-input>
			</el-form-item>
			<el-form-item>
				<el-button type="primary" @click="getUsers">
					<i class="fa fa-search"></i> 查 詢
				</el-button>
			</el-form-item>
			<el-form-item>
				<el-button type="success" @click="showDetail(null)">
					<i class="fa fa-plus"></i> 新 增
				</el-button>
			</el-form-item>
		</el-form>
		<vxe-table :data="users" size="small" :loading="loading" max-height="640px" show-overflow border resizable auto-resize :sort-config="{ trigger: 'cell' }">
			<vxe-table-column title="功能" align="center" width="80" fixed="left">
				<template v-slot="{ row }">
					<el-button type="primary" size="mini" icon="el-icon-edit" circle @click="showDetail(row)"></el-button>
				</template>
			</vxe-table-column>
			<vxe-table-column field="Email" title="帳號" align="center" width="300" sortable></vxe-table-column>
			<vxe-table-column field="Password" title="密碼" align="center" width="200" sortable></vxe-table-column>
			<vxe-table-column field="UserName" title="姓名" align="center" width="200" sortable></vxe-table-column>
			<vxe-table-column field="CreateDate" title="建立日期" align="center" width="100" sortable>
				<template v-slot="{ row }">{{ row.CreateDate | date }}</template>
			</vxe-table-column>
			<vxe-table-column field="Enabled" title="狀態" align="center" width="100" sortable>
				<template v-slot="{ row }">{{ row.Enabled | status }}</template>
			</vxe-table-column>
		</vxe-table>
		<ClientUserModal :show.sync="modalVisible" :data="selectUser" @on-saved="onSaved" />
	</div>
</template>
<script>
import { dateTime, status } from '@/mixins/filter';
import ClientUserModal from '@/components/manage/child/ClientUserModal';
export default {
	name: 'ClientUser',
	mixins: [dateTime, status],
	components: { ClientUserModal },
	data() {
		return {
			loading: false,
			filter: {
				Email: '',
				UserName: null
			},
			users: [],
			selectUser: null,
			modalVisible: false
		};
	},
	mounted() {
		this.getUsers();
	},
	methods: {
		getUsers() {
			this.loading = true;
			this.axios
				.post('api/Admin/GetClientUsers', this.filter)
				.then(res => {
					this.users = res.data;
					this.loading = false;
				})
				.catch(err => {
					this.loading = false;
					this.$message.error(err.response.data.ExceptionMessage);
				});
		},
		showDetail(row) {
			this.selectUser = row;
			this.modalVisible = true;
		},
		deleteUser(row) {
			if (!confirm('是否確認刪除?')) return;
			this.axios
				.post('api/Admin/DeleteClientUser', row)
				.then(res => {
					this.getUsers();
				})
				.catch(err => {
					this.$message.error(err.response.data.ExceptionMessage);
				});
		},
		onSaved() {
			this.getUsers();
		}
	}
};
</script>