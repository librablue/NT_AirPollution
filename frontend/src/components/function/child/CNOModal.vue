<template>
	<vxe-modal title="新增管制編號" v-model="visible" width="320px" :lock-scroll="false" esc-closable resize show-footer>
		<template #default>
			<el-form ref="form" :rules="rules" :model="form">
				<el-form-item prop="Year" label="管制編號年度">
					<el-select style="width:100%" v-model="form.CNOYear" placeholder="請選擇年度">
						<el-option label="請選擇" :value="null"></el-option>
						<el-option v-for="year in yearOptions" :key="year.value" :label="year.label" :value="year.value"></el-option>
					</el-select>
				</el-form-item>
			</el-form>
		</template>
		<template #footer>
			<el-button @click="visible = false">
				<i class="fa fa-ban"></i> 取 消
			</el-button>
			<el-button type="primary" @click="createC_NO">
				<i class="fa fa-ban"></i> 確 定
			</el-button>
		</template>
	</vxe-modal>
</template>

<script>
export default {
	name: 'CNOModal',
	props: ['show', 'data'],
	data() {
		return {
			visible: false,
			form: {
				CNOYear: null
			},
			yearOptions: [], // 存放動態產生的 30 年民國年選項
			district: Object.freeze([]),
			projectCode: Object.freeze([]),
			rules: Object.freeze({
				CNOYear: [{ required: true, message: '請選擇管制編號年度', trigger: 'change' }]
			})
		};
	},
	mounted() {
		this.generateYearOptions();
	},
	methods: {
		generateYearOptions() {
			const currentRocYear = new Date().getFullYear() - 1911;
			const options = [];

			// 產生今年至未來 30 年的選單
			for (let i = 0; i < 30; i++) {
				const year = currentRocYear - i; // 若需要「往前 30 年」，改為 currentRocYear - i
				options.push({
					label: `${year} 年`,
					value: year
				});
			}
			this.yearOptions = Object.freeze(options);
		},
		createC_NO() {
			if (!confirm('管制編號產生後無法修改，是否確認繼續?')) return;
			const loading = this.$loading();
			this.axios
				.post('api/Form/CreateC_NO', this.form)
				.then(res => {
					loading.close();
					this.visible = false;
                    this.$emit('on-updated');
					this.$message.success('管制編號已產生');
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
					this.form = JSON.parse(
						JSON.stringify(
							Object.assign({}, this.data, {
								CNOYear: new Date().getFullYear() - 1911
							})
						)
					);
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