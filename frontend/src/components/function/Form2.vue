<template>
	<div class="main">
		<h1>結算案件管理</h1>
		<el-form size="small" inline>
			<el-form-item label="管制編號">
				<el-input style="width: 140px" v-model="filter.C_NO"></el-input>
			</el-form-item>
			<el-form-item label="Email">
				<el-input v-model="filter.ClientUserEmail"></el-input>
			</el-form-item>
			<el-form-item label="結算進度">
				<el-select style="width: 180px" v-model="filter.CalcStatus">
					<el-option v-for="item in calcStatusList" :key="item.value" :label="item.label" :value="item.value"></el-option>
				</el-select>
			</el-form-item>
			<el-form-item>
				<el-button type="primary" @click="getForms()">
					<i class="fa fa-search"></i> 查 詢
				</el-button>
			</el-form-item>
		</el-form>
		<vxe-table :data="forms" size="small" :loading="loading" max-height="640px" show-overflow border resizable auto-resize :sort-config="{ trigger: 'cell' }">
			<vxe-table-column title="功能" width="140" align="center" fixed="left">
				<template v-slot="{ row }">
					<el-button size="mini" icon="el-icon-search" circle title="查看內容" @click="showDetail(row)"></el-button>
					<el-button type="success" size="mini" icon="el-icon-copy-document" circle title="追加序號" @click="copyRow(row)"></el-button>
					<el-button type="primary" size="mini" icon="el-icon-s-check" circle title="審核案件" @click="showVerifyModal(row)"></el-button>
				</template>
			</vxe-table-column>
			<vxe-table-column field="FormStatus" title="審核進度" width="120" align="center" sortable fixed="left">
				<template v-slot="{ row }">{{row.FormStatus | formStatus}}</template>
			</vxe-table-column>
            <vxe-table-column field="VerifyStage1" title="初/複審" width="120" align="center" sortable fixed="left">
				<template v-slot="{ row }">{{row.VerifyStage1 | verifyStage}}</template>
			</vxe-table-column>
			<vxe-table-column field="CalcStatus" title="結算進度" width="120" align="center" sortable fixed="left">
				<template v-slot="{ row }">{{row.CalcStatus | calcStatus}}</template>
			</vxe-table-column>
            <vxe-table-column field="VerifyStage2" title="初/複審" width="120" align="center" sortable fixed="left">
				<template v-slot="{ row }">{{row.VerifyStage2 | verifyStage}}</template>
			</vxe-table-column>
			<vxe-table-column field="C_NO" title="管制編號" width="140" align="center" sortable fixed="left">
				<template #default="{ row }">
					<span v-if="row.C_NO">{{row.C_NO}}-{{row.SER_NO}}</span>
					<el-button v-else type="text" @click="createC_NO(row)">產生管制編號</el-button>
				</template>
			</vxe-table-column>
			<vxe-table-column field="CreateUserEmail" title="Email" width="160" align="center" fixed="left">
				<template #default="{ row }">{{ row.CreateUserEmail }}</template>
			</vxe-table-column>
			<vxe-table-column field="S_C_NAM" title="業主聯絡人" width="120" align="center"></vxe-table-column>
			<vxe-table-column field="S_C_TEL" title="業主聯絡電話" width="180" align="center"></vxe-table-column>
			<vxe-table-column field="COMP_NAM" title="工程名稱" width="180" align="center"></vxe-table-column>
			<vxe-table-column field="TOWN_NA" title="鄉鎮名稱" width="100" align="center" sortable></vxe-table-column>
			<vxe-table-column field="S_NAME" title="營建業主名稱" width="180" align="center"></vxe-table-column>
			<vxe-table-column field="R_NAME" title="承造單位名稱" width="180" align="center"></vxe-table-column>
			<vxe-table-column field="C_DATE" title="申報日期" width="140" align="center" sortable>
				<template #default="{ row }">{{ row.C_DATE | datetime }}</template>
			</vxe-table-column>
			<vxe-table-column field="M_DATE" title="修改日期" width="140" align="center" sortable>
				<template #default="{ row }">{{ row.M_DATE | datetime }}</template>
			</vxe-table-column>
			<vxe-table-column field="VerifyDate1" title="申報審核日" width="140" align="center" sortable>
				<template #default="{ row }">{{ row.VerifyDate1 | datetime }}</template>
			</vxe-table-column>
			<vxe-table-column field="VerifyDate2" title="結算審核日" width="140" align="center" sortable>
				<template #default="{ row }">{{ row.VerifyDate2 | datetime }}</template>
			</vxe-table-column>
			<vxe-table-column field="FailReason1" title="審核退件原因" width="240" align="center"></vxe-table-column>
			<vxe-table-column field="FailReason2" title="結算退件原因" width="240" align="center"></vxe-table-column>
		</vxe-table>
		<FormModal :show.sync="formModalVisible" :mode="mode" :data="selectRow" @on-updated="onUpdated" />
		<VerifyModal :show.sync="verifyModalVisible" :data="selectRow" @on-updated="onUpdated" />
	</div>
</template>
<script>
import { mapGetters } from 'vuex';
import { dateTime, form } from '@/mixins/filter';
import FormModal from '@/components/function/child/FormModal';
import VerifyModal from '@/components/function/child/VerifyModal';

export default {
	name: 'forms',
	mixins: [dateTime, form],
	components: { FormModal, VerifyModal },
	data() {
		return {
			mode: '',
			loading: false,
			filter: {
				C_NO: '',
				ClientUserEmail: '',
				FormStatus: -1,
				CalcStatus: -1,
                VerifyStage1: 3,
                VerifyStage2: 1,
			},
			forms: [],
			selectRow: {},
			formModalVisible: false,
			verifyModalVisible: false
		};
	},
	mounted() {
        if (this.currentUser.RoleID === 1) {
            this.filter.CalcStatus = 1;
            this.filter.VerifyStage1 = 3;
            this.filter.VerifyStage2 = 1;
        }
        else if (this.currentUser.RoleID === 2) {
            this.filter.CalcStatus = 3;
            this.filter.VerifyStage1 = 3;
            this.filter.VerifyStage2 = 2;
        }
        else if (this.currentUser.RoleID === 99) {
            this.filter.CalcStatus = -1;
        }

        this.getForms();
	},
	computed: {
		...mapGetters(['currentUser']),
		calcStatusList() {
			if (this.currentUser.RoleID === 1) {
				return [
					{ value: 1, label: '審理中' },
					{ value: 2, label: '待補件' },
                    { value: 3, label: '通過待繳費' },
                    { value: 4, label: '通過待退費(<4000)' },
					{ value: 5, label: '通過待退費(>=4000)' }
				];
			} else if (this.currentUser.RoleID === 2) {
				return [
					{ value: 3, label: '通過待繳費' },
					{ value: 4, label: '通過待退費(<4000)' },
					{ value: 5, label: '通過待退費(>=4000)' },
					{ value: 6, label: '繳退費完成' }
				];
			} else if (this.currentUser.RoleID === 99) {
				return [
					{ value: -1, label: '全部' },
					{ value: 0, label: '未申請' },
					{ value: 1, label: '審理中' },
					{ value: 2, label: '待補件' },
					{ value: 3, label: '通過待繳費' },
					{ value: 4, label: '通過待退費(<4000)' },
					{ value: 5, label: '通過待退費(>=4000)' },
					{ value: 6, label: '繳退費完成' }
				];
			}

			return [];
		}
	},
	methods: {
		getForms() {
			this.loading = true;
			this.axios.post('api/Form/GetForms', this.filter).then(res => {
                // 只有繳費完成或免繳費會顯示在form2
				this.forms = res.data.filter(item => item.FormStatus > 3);
				this.loading = false;
			});
		},
		showDetail(row) {
			this.mode = 'Update';
			this.selectRow = row;
			this.formModalVisible = true;
		},
		copyRow(row) {
			this.mode = 'Copy';
			this.selectRow = JSON.parse(JSON.stringify(row));
			this.selectRow.FormStatus = 0;
			this.selectRow.calcStatus = 0;
			this.selectRow.StopWorks.length = 0;
			const clearAry = ['SER_NO', 'AP_DATE', 'C_DATE', 'S_AMT', 'S_AMT2'];
			for (const key of clearAry) {
				this.selectRow[key] = null;
			}
			this.formModalVisible = true;
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
					row.C_NO = res.data;
					this.$message.success('管制編號已產生');
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
