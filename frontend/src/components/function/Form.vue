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
			<el-form-item label="申請進度">
				<el-select style="width: 140px" v-model="filter.FormStatus">
					<el-option label="全部" :value="-1"></el-option>
					<el-option label="審理中" :value="1"></el-option>
					<el-option label="需補件" :value="2"></el-option>
					<el-option label="通過待繳費" :value="3"></el-option>
					<el-option label="已繳費完成" :value="4"></el-option>
				</el-select>
			</el-form-item>
			<el-form-item label="結算進度">
				<el-select style="width: 180px" v-model="filter.CalcStatus">
					<el-option label="全部" :value="-1"></el-option>
					<el-option label="未申請" :value="0"></el-option>
					<el-option label="審理中" :value="1"></el-option>
					<el-option label="需補件" :value="2"></el-option>
					<el-option label="通過待繳費" :value="3"></el-option>
					<el-option label="通過待退費(<4000)" :value="4"></el-option>
					<el-option label="通過待退費(>=4000)" :value="5"></el-option>
					<el-option label="繳退費完成" :value="6"></el-option>
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
			<vxe-table-column field="FormStatus" title="審核進度" width="120" align="center" sortable fixed="left">
				<template #default="{ row }">
					<el-dropdown trigger="click" @command="handleCommand1">
						<a href="javascript:;" class="el-dropdown-link">
							{{ row.FormStatus | formStatus }}
							<i class="el-icon-arrow-down el-icon--right"></i>
						</a>
						<el-dropdown-menu slot="dropdown">
							<el-dropdown-item :command="beforeCommand1(row, 2)">需補件</el-dropdown-item>
							<el-dropdown-item :command="beforeCommand1(row, 3)">通過待繳費</el-dropdown-item>
							<el-dropdown-item :command="beforeCommand1(row, 4)">已繳費完成</el-dropdown-item>
						</el-dropdown-menu>
					</el-dropdown>
				</template>
			</vxe-table-column>
			<vxe-table-column field="CalcStatus" title="結算進度" width="120" align="center" sortable fixed="left">
				<template #default="{ row }">
					<el-dropdown trigger="click" @command="handleCommand2">
						<a href="javascript:;" class="el-dropdown-link">
							{{ row.CalcStatus | calcStatus }}
							<i class="el-icon-arrow-down el-icon--right"></i>
						</a>
						<el-dropdown-menu slot="dropdown">
							<el-dropdown-item :command="beforeCommand2(row, 2)">需補件</el-dropdown-item>
							<el-dropdown-item :command="beforeCommand2(row, 3)">通過待繳費</el-dropdown-item>
							<el-dropdown-item :command="beforeCommand2(row, 4)">通過待退費</el-dropdown-item>
							<!-- <el-dropdown-item :command="beforeCommand2(row, 5)">通過待繳費(大於4000)</el-dropdown-item> -->
							<el-dropdown-item :command="beforeCommand2(row, 6)">繳退費完成</el-dropdown-item>
						</el-dropdown-menu>
					</el-dropdown>
				</template>
			</vxe-table-column>
			<vxe-table-column field="C_NO" title="管制編號" width="120" align="center" sortable fixed="left">
				<template #default="{ row }">{{ row.C_NO ? `${row.C_NO}-${row.SER_NO}` : '(取號中)' }}</template>
			</vxe-table-column>
			<vxe-table-column field="CreateUserEmail" title="Email" width="240" align="center" fixed="left">
				<template #default="{ row }">{{ row.CreateUserEmail }}({{ row.ClientUserID ? '會員' : (row.IsActive ? "已驗證" : "未驗證") }})</template>
			</vxe-table-column>
			<vxe-table-column field="B_SERNO" title="建照字號" width="120" align="center"></vxe-table-column>
			<vxe-table-column field="COMP_NAM" title="工程名稱" width="180" align="center"></vxe-table-column>
			<vxe-table-column field="ADDR" title="工地地址" width="180"></vxe-table-column>
			<vxe-table-column field="S_NAME" title="營建業主名稱" width="180"></vxe-table-column>
			<vxe-table-column field="R_NAME" title="承包(造)名稱" width="180"></vxe-table-column>
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
		<FormModal :show.sync="formModalVisible" :data="selectRow" @on-updated="onUpdated" />
		<FailReasonModal :show.sync="failReasonModalVisible" :data="selectRow" :callback="selectCallBack" @on-confirm="onFailReasonConfirm" />
	</div>
</template>
<script>
import { dateTime, form } from '@/mixins/filter';
import FormModal from '@/components/function/child/FormModal';
import FailReasonModal from '@/components/function/child/FailReasonModal';

export default {
	name: 'forms',
	mixins: [dateTime, form],
	components: { FormModal, FailReasonModal },
	data() {
		return {
			loading: false,
			filter: {
				C_NO: '',
				ClientUserEmail: '',
				FormStatus: 1,
				CalcStatus: -1
			},
			forms: [],
			selectRow: {},
			selectCallBack: null,
			formModalVisible: false,
			failReasonModalVisible: false
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
			this.selectRow = row;
			this.formModalVisible = true;
		},
		onUpdated() {
			this.getForms();
		},
		onFailReasonConfirm(val, callback) {
			this.selectRow.FailReason = val.FailReason;
			if (callback.name === 'bound updateFormStatus') {
				this.selectRow.FormStatus = 2;
			}
			if (callback.name === 'bound updateCalcStatus') {
				this.selectRow.CalcStatus = 2;
			}
			callback(this.selectRow);
		},
		beforeCommand1(row, cmd) {
			return {
				row,
				cmd
			};
		},
		handleCommand1(arg) {
			const { row, cmd } = arg;
			this.selectRow = row;
			switch (cmd) {
				case 2:
					if (!confirm(`管制編號 ${row.C_NO} 進度改成需補件，是否確認繼續?`)) return false;
					this.selectCallBack = this.updateFormStatus;
					this.failReasonModalVisible = true;
					return;
				case 3:
					if (!confirm(`管制編號 ${row.C_NO} 進度改成通過待繳費，是否確認繼續?`)) return false;
					break;
				case 4:
					if (!confirm(`管制編號 ${row.C_NO} 進度改成已繳費完成，是否確認繼續?`)) return false;
					break;
			}

			row.FormStatus = cmd;
			this.updateFormStatus(row);
		},
		beforeCommand2(row, cmd) {
			return {
				row,
				cmd
			};
		},
		handleCommand2(arg) {
			const { row, cmd } = arg;
			this.selectRow = row;
			if (!this.selectRow.S_AMT2) {
				alert('請先結算並儲存後才可修改狀態');
				return false;
			}
			switch (cmd) {
				case 2:
					if (!confirm(`管制編號 ${row.C_NO} 進度改成需補件，是否確認繼續?`)) return false;
					this.selectCallBack = this.updateCalcStatus;
					this.failReasonModalVisible = true;
					return;
				case 3:
					if (!confirm(`管制編號 ${row.C_NO} 進度改成通過待繳費，是否確認繼續?`)) return false;
					break;
				case 4:
				case 5:
					if (!confirm(`管制編號 ${row.C_NO} 進度改成通過待退費，是否確認繼續?`)) return false;
					break;
				case 6:
					if (!confirm(`管制編號 ${row.C_NO} 進度改成繳退費完成，是否確認繼續?`)) return false;
					break;
			}

			this.selectRow.CalcStatus = cmd;
			// 4.5指令共用，用退費金額<4000判斷4，>=4000判斷5
			const paidAmount = this.selectRow.Payments.reduce((prev, current) => {
				return prev + current.Amount;
			}, 0);
			if (cmd === 4 && paidAmount - this.selectRow.S_AMT2 >= 4000) {
				this.selectRow.CalcStatus = 5;
			}

			this.updateCalcStatus(this.selectRow);
		},
		updateFormStatus(row) {
			const loading = this.$loading();
			this.axios
				.post('api/Form/UpdateFormStatus', row)
				.then(res => {
					this.getForms();
					this.$message.success('畫面資料已儲存');
					loading.close();
				})
				.catch(err => {
					this.$message.error(err.response.data.ExceptionMessage);
					loading.close();
				});
		},
		updateCalcStatus(row) {
			const loading = this.$loading();
			this.axios
				.post('api/Form/UpdateCalcStatus', row)
				.then(res => {
					this.getForms();
					this.$message.success('畫面資料已儲存');
					loading.close();
				})
				.catch(err => {
					this.$message.error(err.response.data.ExceptionMessage);
					loading.close();
				});
		}
	}
};
</script>
