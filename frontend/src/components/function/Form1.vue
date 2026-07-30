<template>
	<div class="main">
		<h1>申報案件管理</h1>
		<el-form size="small" inline>
			<el-form-item label="管制編號">
				<el-input style="width: 140px" v-model="filter.C_NO"></el-input>
			</el-form-item>
			<el-form-item label="申報進度">
				<el-select style="width: 140px" v-model="filter.FormStatus">
					<el-option v-for="item in formStatusList" :key="item.value" :label="item.label" :value="item.value"></el-option>
				</el-select>
			</el-form-item>
			<el-form-item>
				<el-button type="primary" @click="getForms()">
					<i class="fa fa-search"></i> 查 詢
				</el-button>
			</el-form-item>
		</el-form>
		<vxe-table :data="forms" size="small" :loading="loading" max-height="640px" show-overflow border resizable auto-resize :sort-config="{ trigger: 'cell' }">
			<vxe-table-column width="60" align="center" fixed="left">
				<template #header>
					檢視
					<br />案件
				</template>
				<template #default="{ row }">
					<el-button size="mini" icon="el-icon-search" circle title="檢視案件" @click="showDetail(row)"></el-button>
				</template>
			</vxe-table-column>
			<vxe-table-column width="60" align="center" fixed="left">
				<template #header>
					檢視
					<br />附件
				</template>
				<template #default="{ row }">
					<el-button size="mini" icon="el-icon-search" circle title="檢視附件" @click="showAttachment(row)"></el-button>
				</template>
			</vxe-table-column>
			<vxe-table-column width="60" align="center" fixed="left">
				<template #header>
					停工
					<br />復工
				</template>
				<template #default="{ row }">
					<el-button size="mini" icon="el-icon-search" circle title="停復工" @click="showStopWork(row)"></el-button>
				</template>
			</vxe-table-column>
			<vxe-table-column width="60" align="center" fixed="left">
				<template #header>
					退款
					<br />帳戶
				</template>
				<template #default="{ row }">
					<el-button size="mini" icon="el-icon-search" circle title="停復工" @click="showRefund(row)"></el-button>
				</template>
			</vxe-table-column>
			<vxe-table-column width="60" align="center" fixed="left">
				<template #header>
					取得
					<br />管編
				</template>
				<template #default="{ row }">
					<el-button type="primary" size="mini" icon="el-icon-edit" circle title="取得管編" :disabled="row.C_NO !== null" @click="createC_NO(row)"></el-button>
				</template>
			</vxe-table-column>
			<vxe-table-column width="60" align="center" fixed="left">
				<template #header>
					刪除
					<br />案件
				</template>
				<template #default="{ row }">
					<el-button type="danger" size="mini" icon="el-icon-delete" circle title="刪除案件" :disabled="row.C_NO !== null" @click="deleteForm(row)"></el-button>
				</template>
			</vxe-table-column>
			<vxe-table-column field="C_NO" title="管制編號" width="140" align="center" sortable>
				<template #default="{ row }">
					<span v-if="row.C_NO">{{row.C_NO}}-{{row.SER_NO}}</span>
				</template>
			</vxe-table-column>
			<vxe-table-column field="COMP_NAM" title="工程名稱" width="240" align="center"></vxe-table-column>
			<vxe-table-column field="C_DATE" title="申報日期" width="140" align="center" sortable>
				<template #default="{ row }">{{ row.C_DATE | datetime }}</template>
			</vxe-table-column>
			<vxe-table-column field="FormStatus" title="首期審核進度" width="140" align="center" sortable>
				<template #default="{ row }">{{row.FormStatus | formStatus}}</template>
			</vxe-table-column>
			<vxe-table-column field="VerifyStage1" title="首期初/複審" width="140" align="center" sortable>
				<template #default="{ row }">{{row.VerifyStage1 | verifyStage}}</template>
			</vxe-table-column>
			<vxe-table-column field="CalcStatus" title="結算審核進度" width="140" align="center" sortable>
				<template #default="{ row }">{{row.CalcStatus | calcStatus}}</template>
			</vxe-table-column>
			<vxe-table-column field="VerifyStage2" title="結算初/複審" width="140" align="center" sortable>
				<template #default="{ row }">{{row.VerifyStage2 | verifyStage}}</template>
			</vxe-table-column>
			<vxe-table-column title="申報表" width="160" align="center">
				<template #default="{ row }">
					<el-button v-if="row.FormStatus > 0" type="primary" size="mini" @click="downloadForm(1, row)">首期</el-button>
					<el-button v-if="row.FormStatus === 4 && row.CalcStatus > 0" type="success" size="mini" @click="downloadForm(2, row)">結算</el-button>
				</template>
			</vxe-table-column>
			<vxe-table-column title="申報證明" width="100" align="center">
				<template #default="{ row }"></template>
			</vxe-table-column>
		</vxe-table>
		<FormModal :show.sync="formModalVisible" :mode="mode" :data="selectRow" @on-updated="onUpdated" />
		<VerifyModal :show.sync="verifyModalVisible" :data="selectRow" @on-updated="onUpdated" />
		<AttachmentModal :show.sync="attachmentModalVisible" :data="selectRow" />
		<StopWorkModal :show.sync="stopWorkModalVisible" :data="selectRow" />
		<RefundModal :show.sync="refundModalVisible" :data="selectRow" />
	</div>
</template>
<script>
import { mapGetters } from 'vuex';
import { dateTime, form } from '@/mixins/filter';
import FormModal from '@/components/function/child/FormModal';
import VerifyModal from '@/components/function/child/VerifyModal';
import AttachmentModal from '@/components/function/child/AttachmentModal';
import StopWorkModal from '@/components/function/child/StopWorkModal';
import RefundModal from '@/components/function/child/RefundModal';

export default {
	name: 'forms',
	mixins: [dateTime, form],
	components: { FormModal, VerifyModal, AttachmentModal, StopWorkModal, RefundModal },
	data() {
		return {
			mode: '',
			loading: false,
			filter: {
				C_NO: '',
				FormStatus: 1,
				CalcStatus: -1,
				VerifyStage1: 1,
				VerifyStage2: -1
			},
			forms: [],
			selectRow: {},
			formModalVisible: false,
			verifyModalVisible: false,
			attachmentModalVisible: false,
			stopWorkModalVisible: false,
			refundModalVisible: false
		};
	},
	mounted() {
		if (this.currentUser.RoleID === 1) {
			this.filter.FormStatus = 1;
			this.filter.VerifyStage1 = 1;
		} else if (this.currentUser.RoleID === 2) {
			this.filter.FormStatus = 3;
			this.filter.VerifyStage1 = -1;
		} else if (this.currentUser.RoleID === 99) {
			this.filter.FormStatus = -1;
		}

		this.getForms();
	},
	computed: {
		...mapGetters(['currentUser']),
		formStatusList() {
			if (this.currentUser.RoleID === 1) {
				return [{ value: 1, label: '審理中' }];
			} else if (this.currentUser.RoleID === 2) {
				return [{ value: 3, label: '通過待繳費' }];
			} else if (this.currentUser.RoleID === 99) {
				return [
					{ value: -1, label: '全部' },
					{ value: 0, label: '未申請' },
					{ value: 1, label: '審理中' },
					{ value: 2, label: '待補件' },
					{ value: 3, label: '通過待繳費' },
					{ value: 4, label: '已繳費完成' },
					{ value: 5, label: '免繳費' }
				];
			}

			return [];
		}
	},
	methods: {
		getForms() {
			this.loading = true;
			this.axios.post('api/Form/GetForms', this.filter).then(res => {
				if (this.currentUser.RoleID === 2) {
					res.data = res.data.filter(item => item.VerifyStage1 >= 2);
				}
				this.forms = res.data;
				this.loading = false;
			});
		},
		showDetail(row) {
			this.mode = 'Update';
			this.selectRow = row;
			this.formModalVisible = true;
		},
		showAttachment(row) {
			this.selectRow = row;
			this.attachmentModalVisible = true;
		},
		showStopWork(row) {
			this.selectRow = row;
			this.stopWorkModalVisible = true;
		},
		showRefund(row) {
			this.selectRow = row;
			this.refundModalVisible = true;
		},
		copyRow(row) {
			if (!confirm('是否確認追加序號?')) return;
			this.mode = 'Copy';
			this.selectRow = JSON.parse(JSON.stringify(row));
			this.selectRow.FormStatus = 0;
			this.selectRow.calcStatus = 0;
			this.selectRow.StopWorks.length = 0;
			const clearAry = ['SER_NO', 'S_AMT', 'S_AMT2', 'C_DATE'];
			for (const key of clearAry) {
				this.selectRow[key] = null;
			}
			this.formModalVisible = true;
		},
		deleteForm(row) {
			if (!confirm('案件刪除後無法回復，是否確認繼續?')) return;
			const loading = this.$loading();
			this.axios
				.post('api/Form/DeleteForm', row)
				.then(res => {
					this.$message.success('案件已刪除');
					loading.close();
				})
				.catch(err => {
					this.$message.error(err.response.data.ExceptionMessage);
					loading.close();
				});
		},
		onUpdated() {
			this.getForms();
		},
		showVerifyModal(row) {
			this.selectRow = row;
			this.verifyModalVisible = true;
		},
		createC_NO(row) {
			if (!confirm('管制編號產生後無法修改，是否確認繼續?')) return;
			const loading = this.$loading();
			this.axios
				.post('api/Form/CreateC_NO', row)
				.then(res => {
					this.$message.success('管制編號已產生');
					loading.close();
					this.getForms();
				})
				.catch(err => {
					this.$message.error(err.response.data.ExceptionMessage);
					loading.close();
				});
		},
		downloadForm(type, row) {
			const loading = this.$loading();
			this.axios
				.post(`api/Form/DownloadForm${type}`, row, {
					responseType: 'blob'
				})
				.then(res => {
					loading.close();
					const url = window.URL.createObjectURL(new Blob([res.data]));
					const link = document.createElement('a');
					link.href = url;
					const fileName = decodeURI(res.headers['file-name']);
					link.setAttribute('download', fileName);
					document.body.appendChild(link);
					link.click();
					link.remove();
				})
				.catch(err => {
					loading.close();
					alert('系統發生未預期錯誤');
					console.log(err);
				});
		}
	}
};
</script>
