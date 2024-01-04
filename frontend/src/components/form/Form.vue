<template>
	<div class="main">
		<h1>申請案件管理</h1>
		<el-form size="small" inline>
			<el-form-item label="案件編號">
				<el-input style="width: 140px" v-model="filter.AutoFormID"></el-input>
			</el-form-item>
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
			<vxe-table-column field="AutoFormID" title="案件編號" width="100" align="center" sortable fixed="left"></vxe-table-column>
			<vxe-table-column field="C_NO" title="管制編號" width="100" align="center" sortable fixed="left"></vxe-table-column>
			<vxe-table-column field="Email" title="Email" width="240" align="center" fixed="left">
				<template #default="{ row }">{{ row.Email }}({{ row.IsVerify ? "已驗證" : "未驗證" }})</template>
			</vxe-table-column>
			<vxe-table-column field="ProjectName" title="工程名稱" width="120"></vxe-table-column>
			<vxe-table-column field="ProjectAddr" title="工地地址或地號" width="140"></vxe-table-column>
			<vxe-table-column field="ContractID" title="建照字號或合約編號" width="140" align="center"></vxe-table-column>
			<vxe-table-column field="ProjectCode" title="工程類別代碼" width="200" align="center">
				<template #default="{ row }">{{ row.ProjectCode | projectCode }}</template>
			</vxe-table-column>
			<vxe-table-column field="BizCompany" title="營建業主名稱" width="140" align="center"></vxe-table-column>
			<vxe-table-column field="BizID" title="營利事業統一編號" width="140" align="center"></vxe-table-column>
			<vxe-table-column field="BizAddr1" title="營業地址" width="140"></vxe-table-column>
			<vxe-table-column field="BizAddr2" title="聯絡地址" width="140"></vxe-table-column>
			<vxe-table-column field="BizTel" title="聯絡電話" width="100" align="center"></vxe-table-column>
			<vxe-table-column field="BizOwnerName" title="負責人姓名" width="100" align="center"></vxe-table-column>
			<vxe-table-column field="BizOwnerJobTitle" title="職稱" width="100" align="center"></vxe-table-column>
			<vxe-table-column field="BizOwnerID" title="身分證字號" width="100" align="center"></vxe-table-column>
			<vxe-table-column field="BizContactName" title="聯絡人姓名" width="100" align="center"></vxe-table-column>
			<vxe-table-column field="BizContactJobTitle" title="職稱" width="100" align="center"></vxe-table-column>
			<vxe-table-column field="BizContactID" title="身分證字號" width="100" align="center"></vxe-table-column>
			<vxe-table-column field="BizContactAddr" title="聯絡人地址" width="140"></vxe-table-column>
			<vxe-table-column field="BizContactTel" title="電話" width="100" align="center"></vxe-table-column>
			<vxe-table-column field="ExecCompany" title="承包(造)單位名稱" width="140" align="center"></vxe-table-column>
			<vxe-table-column field="ExecID" title="營利事業統一編號" width="140" align="center"></vxe-table-column>
			<vxe-table-column field="ExecAddr1" title="營業地址" width="140"></vxe-table-column>
			<vxe-table-column field="ExecAddr2" title="聯絡地址" width="140"></vxe-table-column>
			<vxe-table-column field="ExecTel1" title="聯絡電話" width="140" align="center"></vxe-table-column>
			<vxe-table-column field="ExecOwnerName" title="負責人姓名" width="100" align="center"></vxe-table-column>
			<vxe-table-column field="ExecOwnerJobTitle" title="職稱" width="100" align="center"></vxe-table-column>
			<vxe-table-column field="ExecOwnerID" title="身分證字號" width="100" align="center"></vxe-table-column>
			<vxe-table-column field="ExecAddr3" title="工務所地址" width="140"></vxe-table-column>
			<vxe-table-column field="ExecTel2" title="電話" width="100" align="center"></vxe-table-column>
			<vxe-table-column field="ExecManagerName" title="工地主任姓名" width="100" align="center"></vxe-table-column>
			<vxe-table-column field="ExecManagerTel" title="電話" width="100" align="center"></vxe-table-column>
			<vxe-table-column field="ExecEnvManagerName" title="工地環保負責人姓名" width="100" align="center"></vxe-table-column>
			<vxe-table-column field="ExecEnvMangerTel" title="電話" width="100" align="center"></vxe-table-column>
			<vxe-table-column field="ContractMoney" title="工程合約經費" width="100" align="right"></vxe-table-column>
			<vxe-table-column field="EnvMoney" title="工程環保經費" width="100" align="right">
				<template #default="{ row }">{{ row.EnvMoney }} ({{ row.EnvMoneyPercent }}%)</template>
			</vxe-table-column>
			<vxe-table-column field="Area2" title="工程面積" width="140" align="center">
				<template #default="{ row }">{{ row.Area1 | area1 }} {{ row.Area2 }} {{ row.Area3 | area3 }}</template>
			</vxe-table-column>
			<vxe-table-column field="StartDate" title="預計施工期程" width="180" align="center">
				<template #default="{ row }">{{ row.StartDate | date }} ~ {{ row.EndDate | date }}</template>
			</vxe-table-column>
			<vxe-table-column field="CreateDate" title="建立日期" width="100" align="center" sortable>
				<template #default="{ row }">{{ row.CreateDate | date }}</template>
			</vxe-table-column>
			<vxe-table-column field="ModifyDate" title="修改日期" width="100" align="center" sortable>
				<template #default="{ row }">{{ row.ModifyDate | date }}</template>
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
import FormModal from '@/components/form/FormModal';
export default {
	name: 'forms',
	mixins: [dateTime, form],
	components: { FormModal },
	data() {
		return {
			loading: false,
			filter: {
				AutoFormID: '',
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
