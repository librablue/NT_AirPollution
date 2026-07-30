<template>
	<vxe-modal :title="readonly ? '停復工紀錄檢視' : '停復工紀錄維護'" v-model="visible" width="640px" :lock-scroll="false" esc-closable resize show-footer>
		<template #default>
			<div class="stopwork-wrapper">
				<!-- 頂部總計天數卡片 -->
				<div class="total-days-card" v-if="form.StopWorks && form.StopWorks.length > 0">
					<div class="card-label">
						<i class="el-icon-date"></i> 累計停工總天數
					</div>
					<div class="card-value">
						<span>{{ totalStopWorkDays }}</span> 天
					</div>
				</div>

				<!-- 新增按鈕 (唯讀時隱藏) -->
				<div class="action-bar" v-if="!readonly">
					<el-button type="primary" icon="el-icon-plus" size="small" @click="addStopWork">新增停復工紀錄</el-button>
				</div>

				<!-- 停復工表格 -->
				<el-table :data="form.StopWorks || []" stripe border header-cell-class-name="table-header-custom" style="width: 100%" empty-text="暫無停復工紀錄">
					<el-table-column label="序號" type="index" width="60" align="center"></el-table-column>

					<!-- 停工日期 -->
					<el-table-column label="停工日期" min-width="180" align="center">
						<template slot-scope="scope">
							<template v-if="!readonly">
								<el-date-picker v-model="scope.row.DOWN_DATE2" type="date" placeholder="選擇停工日期" value-format="yyyy-MM-dd" size="small" style="width: 100%" @change="onDateChange(scope.row)"></el-date-picker>
							</template>
							<template v-else>
								<i class="el-icon-time text-muted"></i>
								<span>{{ scope.row.DOWN_DATE2 | date }}</span>
							</template>
						</template>
					</el-table-column>

					<!-- 復工日期 -->
					<el-table-column label="復工日期" min-width="180" align="center">
						<template slot-scope="scope">
							<template v-if="!readonly">
								<el-date-picker v-model="scope.row.UP_DATE2" type="date" placeholder="選擇復工日期" value-format="yyyy-MM-dd" size="small" style="width: 100%" :picker-options="getUpDatePickerOptions(scope.row.DOWN_DATE2)" @change="onDateChange(scope.row)"></el-date-picker>
							</template>
							<template v-else>
								<i class="el-icon-time text-muted"></i>
								<span>{{ scope.row.UP_DATE2 | date }}</span>
							</template>
						</template>
					</el-table-column>

					<!-- 停工天數 -->
					<el-table-column label="停工天數" width="120" align="center">
						<template slot-scope="scope">
							<el-tag type="danger" effect="plain" size="medium" class="day-tag">{{ scope.row.DOWN_DAY || 0 }} 天</el-tag>
						</template>
					</el-table-column>

					<!-- 操作區 (唯讀時隱藏整個欄位) -->
					<el-table-column v-if="!readonly" label="操作" width="80" align="center">
						<template slot-scope="scope">
							<el-button type="text" class="btn-delete" icon="el-icon-delete" @click="removeStopWork(scope.$index)"></el-button>
						</template>
					</el-table-column>
				</el-table>
			</div>
		</template>
		<template #footer>
			<el-button @click="visible = false">
				<i class="fa fa-ban"></i>
				{{ readonly ? '關 閉' : '取 消' }}
			</el-button>
			<el-button v-if="!readonly" type="primary" @click="handleConfirm">
				<i class="fa fa-check"></i> 確 定
			</el-button>
		</template>
	</vxe-modal>
</template>

<script>
import { dateTime } from '@/mixins/filter';

export default {
	name: 'StopWorkModal',
	props: {
		show: {
			type: Boolean,
			default: false
		},
		data: {
			type: Object,
			default: () => ({})
		},
		readonly: {
			type: Boolean,
			default: false
		}
	},
	mixins: [dateTime],
	data() {
		return {
			visible: false,
			form: {
				StopWorks: []
			}
		};
	},
	computed: {
		totalStopWorkDays() {
			if (!this.form.StopWorks || !Array.isArray(this.form.StopWorks)) return 0;
			return this.form.StopWorks.reduce((prev, current) => {
				return prev + (Number(current.DOWN_DAY) || 0);
			}, 0);
		}
	},
	methods: {
		addStopWork() {
			if (this.readonly) return;
			if (!this.form.StopWorks) {
				this.$set(this.form, 'StopWorks', []);
			}
			this.form.StopWorks.push({
				DOWN_DATE: '',
				DOWN_DATE2: '',
				UP_DATE: '',
				UP_DATE2: '',
				DOWN_DAY: 0
			});
		},
		removeStopWork(index) {
			if (this.readonly) return;
			this.form.StopWorks.splice(index, 1);
		},
		getUpDatePickerOptions(downDate) {
			return {
				disabledDate(time) {
					if (!downDate) return false;
					return time.getTime() < new Date(downDate).getTime() - 86400000;
				}
			};
		},
		onDateChange(row) {
			row.DOWN_DATE = row.DOWN_DATE2;
			row.UP_DATE = row.UP_DATE2;
			row.DOWN_DAY = this.getStopDays(row);
		},
		getStopDays(row) {
			if (!row.DOWN_DATE2 || !row.UP_DATE2) return 0;

			var date1 = new Date(row.DOWN_DATE2);
			var date2 = new Date(row.UP_DATE2);

			if (date2 < date1) return 0;

			var diff = Math.abs(date2 - date1);
			var dayDiff = Math.floor(diff / (1000 * 60 * 60 * 24));

			return dayDiff;
		},
		handleConfirm() {
			if (this.readonly) return;
			const loading = this.$loading();
			this.axios
				.post(`api/Form/UpdateForm`, this.form)
				.then(res => {
					loading.close();
					this.$emit('on-updated');
					this.$message.success('畫面資料已儲存');
					this.visible = false;
				})
				.catch(err => {
					this.$message.error(err.response.data.ExceptionMessage);
					loading.close();
				});
		}
	},
	watch: {
		show: {
			handler(newValue) {
				this.visible = newValue;
				if (this.visible) {
					this.form = JSON.parse(JSON.stringify(this.data || {}));
					if (!this.form.StopWorks) {
						this.$set(this.form, 'StopWorks', []);
					} else {
						this.form.StopWorks.forEach(item => {
							item.DOWN_DAY = this.getStopDays(item);
						});
					}
				}
			}
		},
		visible: {
			handler(newValue) {
				this.$emit('update:show', newValue);
			}
		}
	}
};
</script>

<style lang="scss" scoped>
.stopwork-wrapper {
	.total-days-card {
		display: flex;
		justify-content: space-between;
		align-items: center;
		background: linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 100%);
		border: 1px solid #bae6fd;
		border-radius: 6px;
		padding: 12px 20px;
		margin-bottom: 16px;

		.card-label {
			font-size: 15px;
			font-weight: 600;
			color: #0369a1;

			i {
				margin-right: 4px;
			}
		}

		.card-value {
			font-size: 14px;
			color: #0c4a6e;

			span {
				font-size: 22px;
				font-weight: 700;
				color: #e11d48;
				margin-right: 2px;
			}
		}
	}

	.action-bar {
		margin-bottom: 12px;
		text-align: right;
	}

	.table-header-custom {
		background-color: #f1f5f9 !important;
		color: #334155 !important;
		font-weight: 700 !important;
	}

	.text-muted {
		color: #94a3b8;
		margin-right: 6px;
	}

	.day-tag {
		font-weight: 700;
		font-size: 13px;
	}

	.btn-delete {
		color: #ef4444;
		font-size: 16px;

		&:hover {
			color: #dc2626;
		}
	}

	.el-table {
		border-radius: 6px;
		overflow: hidden;

		td,
		th {
			padding: 8px 0;
		}
	}
}
</style>