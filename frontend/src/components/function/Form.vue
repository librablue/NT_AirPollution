<template>
	<div class="main">
		<h1>申請案件管理</h1>
		<el-form size="small" inline>
			<el-form-item label="管制編號">
				<el-input style="width: 140px" v-model="filter.C_NO"></el-input>
			</el-form-item>
			<el-form-item label="Email">
				<el-input v-model="filter.ClientUserEmail"></el-input>
			</el-form-item>
			<el-form-item label="審核狀態">
				<el-select style="width: 140px" v-model="filter.Status">
					<el-option label="全部" :value="0"></el-option>
					<el-option label="審理中" :value="1"></el-option>
					<el-option label="需補件" :value="2"></el-option>
					<el-option label="通過待繳費" :value="3"></el-option>
					<el-option label="已繳費完成" :value="4"></el-option>
				</el-select>
			</el-form-item>
			<el-form-item>
				<el-button type="primary" @click="getForms()">
					<i class="fa fa-search"></i> 查 詢
				</el-button>
			</el-form-item>
		</el-form>
		<vxe-table :data="forms" size="small" :loading="loading" max-height="640px" show-overflow border resizable auto-resize :sort-config="{ trigger: 'cell' }">
			<vxe-table-column title="明細" width="60" align="center" fixed="left">
				<template v-slot="{ row }">
					<el-button type="primary" size="mini" icon="el-icon-search" circle @click="showDetail(row)"></el-button>
				</template>
			</vxe-table-column>
			<vxe-table-column field="Status" title="審核狀態" width="100" align="center" sortable fixed="left">
				<template #default="{ row }">{{ row.Status | status }}</template>
			</vxe-table-column>
			<vxe-table-column field="C_NO" title="管制編號" width="140" align="center" sortable fixed="left">
				<template #default="{ row }">{{ row.C_NO || '(取號中)' }}</template>
			</vxe-table-column>
			<vxe-table-column field="CreateUserEmail" title="Email" width="240" align="center" fixed="left">
				<template #default="{ row }">{{ row.CreateUserEmail }}({{ row.ClientUserID ? '會員' : (row.IsActive ? "已驗證" : "未驗證") }})</template>
			</vxe-table-column>
			<vxe-table-column field="B_SERNO" title="建照字號" width="120" align="center"></vxe-table-column>
			<vxe-table-column field="COMP_NAM" title="工程名稱" width="180" align="center"></vxe-table-column>
			<vxe-table-column field="ADDR" title="工地地址" width="180"></vxe-table-column>
			<vxe-table-column field="S_NAME" title="營建業主名稱" width="180"></vxe-table-column>
			<vxe-table-column field="R_NAME" title="承包(造)名稱" width="180"></vxe-table-column>
			<vxe-table-column field="Status" title="案件狀態" width="100" align="center" sortable>
				<template #default="{ row }">{{ row.Status | status }}</template>
			</vxe-table-column>
			<vxe-table-column field="C_DATE" title="申報日期" width="100" align="center" sortable>
				<template #default="{ row }">{{ row.C_DATE | date }}</template>
			</vxe-table-column>
			<vxe-table-column field="M_DATE" title="修改日期" width="100" align="center" sortable>
				<template #default="{ row }">{{ row.M_DATE | date }}</template>
			</vxe-table-column>
			<vxe-table-column field="VerifyDate" title="審核日期" width="100" align="center" sortable>
				<template #default="{ row }">{{ row.VerifyDate | date }}</template>
			</vxe-table-column>
		</vxe-table>
		<FormModal :show.sync="modalVisible" :data="selectForm" @on-updated="onUpdated" />
	</div>
</template>
<script>
import { dateTime, form } from '@/mixins/filter';
import FormModal from '@/components/function/child/FormModal';
export default {
	name: 'forms',
	mixins: [dateTime, form],
	components: { FormModal },
	data() {
		return {
			loading: false,
			filter: {
				C_NO: '',
				ClientUserEmail: '',
				Status: 1
			},
			forms: [],
			selectForm: {},
			modalVisible: false
		};
	},
	mounted() {
		this.getForms();
	},
	computed: {},
	methods: {
		getForms() {
			this.loading = true;
			this.axios.post('api/Form/GetForms', this.filter).then(res => {
				this.forms = res.data;
				this.loading = false;
			});
		},
		showDetail(row) {
			this.selectForm = row;
			this.modalVisible = true;
		},
		onUpdated() {
			this.getForms();
		}
	}
};
</script>
