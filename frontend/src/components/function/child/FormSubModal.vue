<template>
	<vxe-modal title="合併申報內容" v-model="visible" width="50%" :lock-scroll="false" esc-closable resize show-footer>
		<template #default>
			<div class="formsub-wrapper">
				<div class="action-bar">
					<el-button type="primary" icon="el-icon-plus" size="small" @click="addFormSub">新增工程</el-button>
				</div>

				<el-table :data="form.FormSub || []" size="size" stripe border header-cell-class-name="table-header-custom" style="width: 100%" empty-text="暫無合併申報資料">
					<el-table-column label="序號" type="index" width="55" align="center"></el-table-column>
					<el-table-column label="工程名稱" min-width="160" align="center">
						<template slot-scope="scope">
							<el-input v-model="scope.row.COMP_NAM" placeholder="請輸入工程名稱" size="small" maxlength="150"></el-input>
						</template>
					</el-table-column>
					<el-table-column label="工程地址或地號" min-width="180" align="center">
						<template slot-scope="scope">
							<el-input v-model="scope.row.ADDR" placeholder="請輸入地址或地號" size="small" maxlength="100"></el-input>
						</template>
					</el-table-column>
					<el-table-column label="施工面積(㎡)" width="120" align="center">
						<template slot-scope="scope">
							<el-input v-decimal v-model="scope.row.AREA" size="small"></el-input>
						</template>
					</el-table-column>
					<el-table-column label="施工期程(起日)" width="120" align="center">
						<template slot-scope="scope">
							<div class="el-input el-input--small">
								<input type="text" class="el-input__inner datepicker" v-model="scope.row.B_DATE" readonly />
							</div>
						</template>
					</el-table-column>
					<el-table-column label="施工期程(迄日)" width="120" align="center">
						<template slot-scope="scope">
							<div class="el-input el-input--small">
								<input type="text" class="el-input__inner datepicker" v-model="scope.row.E_DATE" readonly />
							</div>
						</template>
					</el-table-column>
					<el-table-column label="操作" width="70" align="center">
						<template slot-scope="scope">
							<el-button type="text" class="btn-delete" icon="el-icon-delete" @click="removeFormSub(scope.$index)"></el-button>
						</template>
					</el-table-column>
				</el-table>
			</div>
		</template>
		<template #footer>
			<el-button @click="visible = false">
				<i class="fa fa-ban"></i>
				取 消
			</el-button>
			<el-button type="primary" @click="saveFormSub">
				<i class="fa fa-check"></i> 確 定
			</el-button>
		</template>
	</vxe-modal>
</template>

<script>
import { dateTime } from '@/mixins/filter';

export default {
	name: 'FormSubModal',
	props: {
		show: {
			type: Boolean,
			default: false
		},
		data: {
			type: Object,
			default: () => ({})
		}
	},
	mixins: [dateTime],
	mounted() {
		this.initDatePicker();
	},
	data() {
		return {
			visible: false,
			form: {}
		};
	},
	methods: {
		initDatePicker() {
			$('.datepicker').datepicker({
				dateFormat: 'yy/mm/dd',
				yearRange: '-90:+10',
				changeYear: true,
				changeMonth: true,
				beforeShow: function (input, inst) {
					const inputVal = input.value;
					if (inputVal) {
						const year = +inputVal.substr(0, 3) + 1911;
						const month = inputVal.substr(3, 2);
						const day = inputVal.substr(5, 2);
						return {
							defaultDate: `${year}/${month}/${day}`
						};
					}
					return {};
				},
				onSelect: function (dateText, inst) {
					var objDate = {
						y: `${inst.selectedYear - 1911 < 0 ? inst.selectedYear : inst.selectedYear - 1911}`.padStart(3, '0'),
						m: `${inst.selectedMonth + 1}`.padStart(2, '0'),
						d: `${inst.selectedDay}`.padStart(2, '0')
					};

					const dateFormate = `${objDate.y}${objDate.m}${objDate.d}`;

					// 1. 更新 HTML input 上的顯示文字
					inst.input.val(dateFormate);

					// 2. 取得 Vue 的 vnode 綁定資訊並自動更新資料
					const inputEl = inst.input[0];
					const vnode = inputEl._vnode;

					if (vnode && vnode.data && vnode.data.model) {
						// 直接執行 Vue 自動生成的雙向綁定 setter 函式
						vnode.data.model.callback(dateFormate);
					} else {
						// 備用方案：如果不是用 v-model 而是改觸發 input 事件
						inputEl.dispatchEvent(new Event('input', { bubbles: true }));
					}
				}
			});
		},
		// 新增一筆合併申報
		addFormSub() {
			if (!this.form.FormSub) {
				this.$set(this.form, 'FormSub', []);
			}
			this.form.FormSub.push({
				COMP_NAM: '',
				ADDR: '',
				AREA: null,
				B_DATE: '',
				E_DATE: ''
			});

			this.$nextTick(() => {
				this.initDatePicker();
			});
		},
		// 刪除一筆合併申報
		removeFormSub(index) {
			this.form.FormSub.splice(index, 1);
		},
		saveFormSub() {
			const list = this.form.FormSub || [];

			// 1. 檢查是否有資料
			if (list.length === 0) {
				this.$message.warning('請至少新增一筆工程資料');
				return;
			}

			// 2. 逐筆驗證欄位資料
			for (let i = 0; i < list.length; i++) {
				const item = list[i];
				const rowNum = i + 1; // 顯示對應的序號

				// 必填檢查：工程名稱
				if (!item.COMP_NAM || !item.COMP_NAM.trim()) {
					this.$message.warning(`第 ${rowNum} 筆資料：請輸入工程名稱`);
					return;
				}

				// 必填檢查：工程地址或地號
				if (!item.ADDR || !item.ADDR.trim()) {
					this.$message.warning(`第 ${rowNum} 筆資料：請輸入工程地址或地號`);
					return;
				}

				// 數值檢查：施工面積 (不可為空、必須大於 0)
				if (item.AREA === null || item.AREA === '' || isNaN(item.AREA) || Number(item.AREA) <= 0) {
					this.$message.warning(`第 ${rowNum} 筆資料：請輸入有效的施工面積（需大於 0）`);
					return;
				}

				// 必填檢查：施工期程起日與迄日
				if (!item.B_DATE) {
					this.$message.warning(`第 ${rowNum} 筆資料：請選擇施工期程(起日)`);
					return;
				}
				if (!item.E_DATE) {
					this.$message.warning(`第 ${rowNum} 筆資料：請選擇施工期程(迄日)`);
					return;
				}

				// 邏輯檢查：起日不可大於迄日 (格式為民國年 YYMMDD，字串或數字比對皆適用)
				if (item.B_DATE > item.E_DATE) {
					this.$message.warning(`第 ${rowNum} 筆資料：施工期程起日不可大於迄日`);
					return;
				}
			}

			// 驗證通過，回寫資料並關閉彈窗
			this.data.FormSub = JSON.parse(JSON.stringify(this.form.FormSub));
			this.visible = false;
		}
	},
	watch: {
		show: {
			handler(newValue) {
				this.visible = newValue;
				if (this.visible) {
					this.form = JSON.parse(JSON.stringify(this.data || {}));

					this.$nextTick(() => {
						this.initDatePicker();
					});
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
.formsub-wrapper {
	.action-bar {
		margin-bottom: 12px;
		text-align: right;
	}

	.table-header-custom {
		background-color: #f1f5f9 !important;
		color: #334155 !important;
		font-weight: 700 !important;
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
			padding: 6px 0;
		}
	}
}
</style>