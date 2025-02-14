<template>
	<vxe-modal class="edit-modal" title="編輯視窗" v-model="visible" width="300px" :lock-scroll="false" esc-closable resize show-footer>
		<template #default>
			<el-form ref="form" :model="rate" :rules="rules" size="small" label-position="top">
				<el-form-item prop="Date" label="日期">
					<el-date-picker v-model="rate.Date" type="date" value-format="yyyy-MM-dd" placeholder="請選擇日期"></el-date-picker>
				</el-form-item>
                <el-form-item prop="Rate" label="利率(%)">
					<el-input v-model="rate.Rate"></el-input>
				</el-form-item>
			</el-form>
		</template>
		<template #footer>
			<el-button size="small" @click="visible = false">
				<i class="fa fa-ban"></i> 取 消
			</el-button>
			<el-button type="primary" size="small" @click="save()">
				<i class="fa fa-floppy-o"></i> 儲 存
			</el-button>
		</template>
	</vxe-modal>
</template>

<script>
export default {
	name: 'RateModal',
	props: ['show'],
	data() {
		return {
			visible: false,
			rate: {
				Date: '',
				Rate: null
			},
			rules: {
				Date: [{ required: true, message: '請選擇日期', trigger: 'blur' }],
				Rate: [{ required: true, message: '請輸入利率', trigger: 'blur' }]
			}
		};
	},
	methods: {
		save() {
			this.$refs.form.validate(valid => {
				if (!valid) {
					this.$message.error('呃，欄位驗證錯誤');
					return false;
				}

				this.axios
					.post('api/Option/AddRate', this.rate)
					.then(res => {
						this.$emit('on-success');
						this.$message.success('畫面資料已儲存');
						this.visible = false;
					})
					.catch(err => {
						this.$message.error(err.response.data.ExceptionMessage);
					});
			});
		}
	},
	watch: {
		show: {
			handler(newValue, oldValue) {
				this.visible = newValue;
			}
		},
		visible: {
			handler(newValue, oldValue) {
				this.$emit('update:show', newValue);
			}
		}
	}
};
</script>

<style lang="scss">
</style>