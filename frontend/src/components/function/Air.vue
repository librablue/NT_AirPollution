<template>
	<div class="main">
		<h1>空品不良回報</h1>
		<el-form size="small" inline>
			<el-form-item label="管制編號">
				<el-input style="width:140px" v-model="filter.C_NO"></el-input>
			</el-form-item>
			<el-form-item label="工程名稱">
				<el-input style="width:140px" v-model="filter.COMP_NAM"></el-input>
			</el-form-item>
			<el-form-item label="鄉鎮">
				<el-select style="width:140px" v-model="filter.TOWN_NO">
					<el-option label="請選擇" :value="null"></el-option>
					<el-option v-for="item in district" :key="item.Code" :label="item.Name" :value="item.Code"></el-option>
				</el-select>
			</el-form-item>
			<el-form-item label="工程類別">
				<el-select style="width:140px" v-model="filter.KIND_NO">
					<el-option label="請選擇" :value="null"></el-option>
					<el-option v-for="item in projectCode" :key="item.ID" :label="`${item.ID}. ${item.Name}`" :value="item.ID"></el-option>
				</el-select>
			</el-form-item>
			<el-form-item label="營建業主">
				<el-input style="width:140px" v-model="filter.S_NAME"></el-input>
			</el-form-item>
			<el-form-item label="承包(造)">
				<el-input style="width:140px" v-model="filter.R_NAME"></el-input>
			</el-form-item>
			<el-form-item label="預計施工期程">
				<el-date-picker style="width:140px" v-model="filter.StartDate" type="date" value-format="yyyy-MM-dd" placeholder="請選擇日期"></el-date-picker>~
				<el-date-picker style="width:140px" v-model="filter.EndDate" type="date" value-format="yyyy-MM-dd" placeholder="請選擇日期"></el-date-picker>
			</el-form-item>
			<el-form-item>
				<el-button type="primary" @click="getAirs()">
					<i class="fa fa-search"></i> 查 詢
				</el-button>
			</el-form-item>
		</el-form>
		<vxe-table :data="airs" :loading="loading" size="small" max-height="640px" show-overflow border resizable auto-resize>
			<vxe-table-column title="功能" align="center" width="60" fixed="left">
				<template v-slot="{ row }">
					<el-button v-if="row.AirFiles.length > 0" size="mini" icon="el-icon-picture-outline" circle @click="showModal(row)"></el-button>
				</template>
			</vxe-table-column>
			<vxe-table-column field="C_NO" title="管制編號" align="center" width="120">
				<template v-slot="{ row }">{{ row.C_NO }}-{{ row.SER_NO }}</template>
			</vxe-table-column>
			<vxe-table-column field="Position" title="執行地點" align="center" width="160"></vxe-table-column>
			<vxe-table-column title="執行時間" align="center" width="180">
				<template v-slot="{ row }">{{ row.StartDate | date }} - {{ row.EndDate | date }}</template>
			</vxe-table-column>
			<vxe-table-column field="Method" title="執行措施" align="center" width="140"></vxe-table-column>
			<vxe-table-column field="Remark" title="備註" width="140"></vxe-table-column>
			<vxe-table-column field="COMP_NAM" title="工程名稱" align="center" width="140"></vxe-table-column>
			<vxe-table-column field="TOWN_NA" title="鄉鎮" align="center" width="100"></vxe-table-column>
			<vxe-table-column field="KIND" title="工程類別" align="center" width="180"></vxe-table-column>
			<vxe-table-column field="S_NAME" title="營建業主" align="center" width="160"></vxe-table-column>
			<vxe-table-column field="R_NAME" title="承包(造)" align="center" width="160"></vxe-table-column>
			<vxe-table-column title="工程面積" align="center" width="120"></vxe-table-column>
			<vxe-table-column title="合約經費" align="center" width="120"></vxe-table-column>
			<vxe-table-column field="B_DATE" title="預計施工期程(起)" align="center" width="140">
				<template v-slot="{ row }">{{ row.B_DATE2 | date }}</template>
			</vxe-table-column>
			<vxe-table-column field="E_DATE" title="預計施工期程(迄)" align="center" width="140">
				<template v-slot="{ row }">{{ row.E_DATE2 | date }}</template>
			</vxe-table-column>
		</vxe-table>
		<ImageModal :show.sync="modalVisible" :data="airImages"></ImageModal>
	</div>
</template>

<script>
import { dateTime } from '@/mixins/filter';
import ImageModal from '@/components/function/child/ImageModal';
export default {
	name: 'Air',
	mixins: [dateTime],
	components: { ImageModal },
	data() {
		return {
			loading: false,
			filter: {
				C_NO: '',
				COMP_NAM: '',
				TOWN_NO: '',
				KIND_NO: '',
				S_NAME: '',
				R_NAME: '',
				StartDate: '',
				EndDate: ''
			},
			airs: [],
			airImages: [],
			district: Object.freeze([]),
			projectCode: Object.freeze([]),
			modalVisible: false
		};
	},
	mounted() {
		this.getDistrict();
		this.getProjectCode();
	},
	methods: {
		getDistrict() {
			this.axios.get('api/Option/GetDistrict').then(res => {
				this.district = Object.freeze(res.data);
			});
		},
		getProjectCode() {
			this.axios.get('api/Option/GetProjectCode').then(res => {
				this.projectCode = Object.freeze(res.data);
			});
		},
		getAirs() {
			this.loading = true;
			this.axios.post('api/Air/GetAirs', this.filter).then(res => {
				this.airs = res.data;
				this.loading = false;
			});
		},
        showModal(row) {
            this.airImages = row.AirFiles.map(item => item.FileName);
            this.modalVisible = true;
        }
	}
};
</script>

<style lang="scss">
</style>