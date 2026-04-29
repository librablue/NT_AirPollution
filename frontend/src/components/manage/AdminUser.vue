<template>
	<div class="main">
		<h1>使用者管理</h1>
		<el-form size="small" inline>
			<el-form-item label="帳號">
				<el-input v-model="filter.Text"></el-input>
			</el-form-item>
			<el-form-item label="權限">
				<el-select style="width:160px" v-model="filter.RoleID">
					<el-option label="全部" :value="null"></el-option>
					<el-option v-for="(item, idx) in roles" :key="idx" :label="item.RoleName" :value="item.ID"></el-option>
				</el-select>
			</el-form-item>
			<el-form-item label="狀態">
				<el-select style="width:100px" v-model="filter.Enabled">
					<el-option label="全部" :value="null"></el-option>
					<el-option label="啟用" :value="true"></el-option>
					<el-option label="停用" :value="false"></el-option>
				</el-select>
			</el-form-item>
			<el-form-item>
				<el-button type="primary" @click="getAdminUsers">
					<i class="fa fa-search"></i> 查 詢
				</el-button>
			</el-form-item>
			<el-form-item>
				<el-button type="success" @click="showDetail(null)">
					<i class="fa fa-plus"></i> 新 增
				</el-button>
			</el-form-item>
		</el-form>
		<vxe-table size="small" :data="users" :loading="loading" max-height="640px" show-overflow border resizable auto-resize :sort-config="{ trigger: 'cell' }">
			<vxe-table-column title="功能" align="center" width="80" fixed="left">
				<template v-slot="{ row }">
					<el-button type="primary" size="mini" icon="el-icon-edit" circle @click="showDetail(row)"></el-button>
				</template>
			</vxe-table-column>
			<vxe-table-column field="Account" title="帳號" align="center" width="200" sortable></vxe-table-column>
			<vxe-table-column field="UserName" title="名稱" align="center" width="200" sortable></vxe-table-column>
			<vxe-table-column field="RoldID" title="權限" align="center" width="100" sortable>
				<template v-slot="{ row }">{{ getRole(row.RoleID) }}</template>
			</vxe-table-column>
			<vxe-table-column field="Enabled" title="狀態" align="center" width="100" sortable>
				<template v-slot="{ row }">{{ row.Enabled | status }}</template>
			</vxe-table-column>
		</vxe-table>
		<AdminUserModal :show.sync="modalVisible" :data="selectUser" :roles="roles" @on-saved="onSaved" />
	</div>
</template>
<script>
import { dateTime, status } from '@/mixins/filter';
import AdminUserModal from '@/components/manage/child/AdminUserModal';
export default {
	name: 'AdminUser',
	mixins: [dateTime, status],
	components: { AdminUserModal },
	data() {
		return {
			loading: false,
			filter: {
				Text: '',
				RoleID: null,
				Enabled: null
			},
			roles: Object.freeze([]),
			users: [],
			selectUser: null,
			modalVisible: false
		};
	},
	mounted() {
		this.getAdminRoles();
		this.getAdminUsers();
	},
	methods: {
		getAdminRoles() {
			this.axios.get('api/Option/GetAdminRoles').then(res => {
				this.roles = Object.freeze(res.data);
			});
		},
		getRole(value) {
			if (!value) return '';
			const result = this.roles.find(item => item.ID === value);
			if (result) return result.RoleName;
			return '';
		},
		getAdminUsers() {
			this.loading = true;
			this.axios
				.post('api/Admin/GetAdminUsers', this.filter)
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
		onSaved() {
			this.getAdminUsers();
		}
	}
};
</script>