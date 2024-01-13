<template>
	<div class="main">
		<h1>道路認養回報</h1>
		<el-form size="small" inline>
			<el-form-item label="管制編號">
				<el-input style="width:120px" v-model="filter.C_NO"></el-input>
			</el-form-item>
			<el-form-item label="工程名稱">
				<el-input style="width:140px" v-model="filter.COMP_NAM"></el-input>
			</el-form-item>
			<el-form-item label="鄉鎮">
				<el-select style="width:100px" v-model="filter.TOWN_NO">
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
			<el-form-item label="年份">
				<el-date-picker style="width:100px" v-model="filter.Year" type="year" value-format="yyyy" placeholder="請選擇年份"></el-date-picker>
			</el-form-item>
			<el-form-item>
				<el-button type="primary" @click="getRoadReport()">
					<i class="fa fa-search"></i> 查 詢
				</el-button>
			</el-form-item>
		</el-form>
		<vxe-grid v-bind="gridOptions"></vxe-grid>
	</div>
</template>

<script>
import { dateTime } from '@/mixins/filter';
export default {
	name: 'Road',
	mixins: [dateTime],
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
				Year: new Date().getFullYear().toString()
			},
			report: [],
			district: Object.freeze([]),
			projectCode: Object.freeze([]),
			gridOptions: {
				border: true,
				resizable: true,
				showOverflow: true,
				maxHeight: 640,
				columns: [],
				data: [
					{ id: 10001, name: 'Test1', nickname: 'T1', role: 'Develop', sex: 'Man', age: 0, address: 'Shenzhen' },
					{ id: 10002, name: 'Test2', nickname: 'T2', role: 'Test', sex: 'Women', age: 22, address: 'Guangzhou' },
					{ id: 10003, name: 'Test3', nickname: 'T3', role: 'PM', sex: 'Man', age: 100, address: 'Shanghai' },
					{ id: 10004, name: 'Test4', nickname: 'T4', role: 'Designer', sex: 'Women', age: 70, address: 'Shenzhen' },
					{ id: 10005, name: 'Test5', nickname: 'T5', role: 'Develop', sex: 'Women', age: 10, address: 'Shanghai' },
					{ id: 10006, name: 'Test6', nickname: 'T6', role: 'Designer', sex: 'Women', age: 90, address: 'Shenzhen' },
					{ id: 10007, name: 'Test7', nickname: 'T7', role: 'Test', sex: 'Man', age: 5, address: 'Shenzhen' },
					{ id: 10008, name: 'Test8', nickname: 'T8', role: 'Develop', sex: 'Man', age: 80, address: 'Shenzhen' }
				]
			}
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
		getRoadReport() {
			this.loading = true;
			this.axios.post('api/Road/GetRoadReport', this.filter).then(res => {
				const colAry = [
					{ field: 'C_NO', title: '管制編號', width: 120, align: 'center' },
					{ field: 'SER_NO', title: '序號', width: 60, align: 'center' },
					{ field: 'COMP_NAM', title: '工程名稱', width: 180, align: 'center' },
					{ field: 'TOWN_NA', title: '鄉鎮名稱', width: 100, align: 'center' },
					{ field: 'KIND', title: '工程類別', width: 200, align: 'center' },
					{ field: 'C_DATE', title: '申請日期', width: 100, align: 'center' },
					{ field: 'S_NAME', title: '認養單位名稱', width: 160, align: 'center' },
					{ field: 'PromiseCreateDate', title: '簽署日期', width: 100, align: 'center' },
					{ field: 'RoadLength', title: '認養公里數', width: 120, align: 'center' },
					{ field: 'RoadName', title: '認養路段', width: 140, align: 'center' },
					{ field: 'StartDate', title: '認養期程(起)', width: 120, align: 'center' },
					{ field: 'EndDate', title: '認養期程(迄)', width: 120, align: 'center' },
					{ field: 'Length1_1', title: '1月洗街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length2_1', title: '1月掃街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length1_2', title: '2月洗街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length2_2', title: '2月掃街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length1_3', title: '3月洗街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length2_3', title: '3月掃街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length1_4', title: '4月洗街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length2_4', title: '4月掃街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length1_5', title: '5月洗街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length2_5', title: '5月掃街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length1_6', title: '6月洗街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length2_6', title: '6月掃街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length1_7', title: '7月洗街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length2_7', title: '7月掃街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length1_8', title: '8月洗街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length2_8', title: '8月掃街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length1_9', title: '9月洗街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length2_9', title: '9月掃街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length1_10', title: '10月洗街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length2_10', title: '10月掃街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length1_11', title: '11月洗街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length2_11', title: '11月掃街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length1_12', title: '12月洗街距離(KM)', width: 140, align: 'center' },
					{ field: 'Length2_12', title: '12月掃街距離(KM)', width: 140, align: 'center' },
					{ field: 'CleanWay1', title: '洗/掃街', width: 100, align: 'center' },
					{ field: 'CleanWay2', title: '作業方式', width: 100, align: 'center' }
				];

				const dataAry = res.data.map(item => {
					let mthCol = {};
					for (let i = 1; i <= 12; i++) {
						const findResult = item.RoadExcelMonth.find(sub => sub.Month === i);
						mthCol[`Length1_${i}`] = item['CleanWay1'] === '洗街' ? (findResult ? findResult.CleanLength1 : 0) : 0;
						mthCol[`Length2_${i}`] = item['CleanWay1'] === '掃街' ? (findResult ? findResult.CleanLength2 : 0) : 0;
					}

					let allCol = {
						C_NO: item.C_NO,
						SER_NO: item.SER_NO,
						COMP_NAM: item.COMP_NAM,
						TOWN_NA: item.TOWN_NA,
						KIND: item.KIND,
						C_DATE: dayjs(item.C_DATE).format('YYYY-MM-DD'),
						S_NAME: item.S_NAME,
						PromiseCreateDate: dayjs(item.PromiseCreateDate).format('YYYY-MM-DD'),
						RoadLength: item.RoadLength,
						RoadName: item.RoadName,
						StartDate: dayjs(item.StartDate).format('YYYY-MM-DD'),
						EndDate: dayjs(item.EndDate).format('YYYY-MM-DD'),
						CleanWay1: item.CleanWay1,
						CleanWay2: item.CleanWay2
					};

					allCol = Object.assign(allCol, mthCol);
					return allCol;
				});

				this.gridOptions.columns = colAry;
				this.gridOptions.data = dataAry;

				this.loading = false;
			});
		}
	}
};
</script>

<style lang="scss">
</style>