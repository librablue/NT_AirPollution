<template>
	<vxe-modal title="案件審核" v-model="visible" width="30%" :lock-scroll="false" esc-closable resize show-footer>
		<template #default>
			<el-form ref="form" :model="form" label-width="80px">
				<div v-if="$route.path === '/function/form1'">
					<el-form-item prop="FormStatus" label="審核進度">
						<el-select v-model="form.FormStatus" placeholder="請選擇">
							<el-option label="未申請" :value="0"></el-option>
							<el-option label="審理中" :value="1"></el-option>
							<el-option label="需補件" :value="2"></el-option>
							<el-option label="審核通過" :value="3"></el-option>
							<el-option label="已繳費完成" :value="4"></el-option>
							<el-option label="免繳費" :value="5"></el-option>
						</el-select>
					</el-form-item>
					<el-form-item v-if="form.FormStatus === 2" prop="FailReason1" label="退件原因">
						<el-input type="textarea" :autosize="{ minRows: 3, maxRows: 6 }" v-model="form.FailReason1" />
					</el-form-item>
					<el-form-item>
						<el-checkbox v-model="form.IsMailFormStatus" label="郵件通知" border></el-checkbox>
					</el-form-item>
				</div>
				<div v-if="$route.path === '/function/form2'">
					<el-form-item prop="CalcStatus" label="結算進度">
						<el-select v-model="form.CalcStatus" placeholder="請選擇">
							<el-option label="未申請" :value="0"></el-option>
							<el-option label="審理中" :value="1"></el-option>
							<el-option label="需補件" :value="2"></el-option>
							<el-option label="審核通過" :value="3"></el-option>
							<el-option label="繳退費完成" :value="6"></el-option>
						</el-select>
					</el-form-item>
					<el-form-item v-if="form.CalcStatus === 2" prop="FailReason2" label="退件原因">
						<el-input type="textarea" :autosize="{ minRows: 3, maxRows: 6 }" v-model="form.FailReason2" />
					</el-form-item>
					<el-form-item>
						<el-checkbox v-model="form.IsMailCalcStatus" label="郵件通知" border></el-checkbox>
					</el-form-item>
				</div>
			</el-form>
		</template>
		<template #footer>
			<el-button @click="visible = false">
				<i class="fa fa-ban"></i> 取 消
			</el-button>
			<el-button type="primary" @click="updateStatus">
				<i class="fa fa-ban"></i> 確 定
			</el-button>
		</template>
	</vxe-modal>
</template>

<script>
export default {
	name: 'VerifyModal',
	props: ['show', 'data'],
	data() {
		return {
			visible: false,
			form: {}
		};
	},
	methods: {
		updateStatus() {
			if (this.form.FormStatus > 2 && !this.form.C_NO) {
				alert('若要通過審核，請先產生管制編號');
				return;
			}
			if (!confirm('是否確認繼續?')) return false;
			const loading = this.$loading();
			this.axios
				.post('api/Form/UpdateStatus', this.form)
				.then(res => {
					loading.close();
					this.$emit('on-updated');
					this.visible = false;
					this.$message.success('審核狀態已更新');
				})
				.catch(err => {
					this.$message.error(err.response.data.ExceptionMessage);
					loading.close();
				});
		}
	},
	watch: {
		show: {
			handler(newValue, oldValue) {
				this.visible = newValue;
				if (this.visible) {
					this.form = JSON.parse(JSON.stringify(this.data));
				}
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

<style lang="scss" scoped>
</style>