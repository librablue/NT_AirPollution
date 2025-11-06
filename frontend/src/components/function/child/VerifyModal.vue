<template>
	<vxe-modal title="案件審核" v-model="visible" width="30%" :lock-scroll="false" esc-closable resize show-footer>
		<template #default>
			<el-form ref="form" :model="form" label-width="80px">
				<el-form-item label="管制編號">{{C_NO}}</el-form-item>
				<div v-if="$route.path === '/function/form1'">
					<el-form-item prop="FormStatus" label="審核進度">
						<el-select v-model="form.FormStatus" placeholder="請選擇">
							<el-option v-for="item in formStatusList" :key="item.value" :label="item.label" :value="item.value"></el-option>
						</el-select>
					</el-form-item>
					<el-form-item v-if="form.FormStatus === 2" prop="FailReason1" label="退件原因">
						<el-input type="textarea" :autosize="{ minRows: 3, maxRows: 6 }" v-model="form.FailReason1" />
					</el-form-item>
					<!-- <el-form-item label="繳費期限">
                        <el-date-picker v-model="form.PayEndDate1" type="date" value-format="yyyy-MM-dd" placeholder="請選擇日期"></el-date-picker>
					</el-form-item>-->
					<el-form-item>
						<el-checkbox v-model="form.IsMailFormStatus" label="郵件通知" border></el-checkbox>
					</el-form-item>
				</div>
				<div v-if="$route.path === '/function/form2'">
					<el-form-item prop="CalcStatus" label="結算進度">
						<el-select v-model="form.CalcStatus" placeholder="請選擇">
							<el-option v-for="item in calcStatusList" :key="item.value" :label="item.label" :value="item.value"></el-option>
						</el-select>
					</el-form-item>
					<el-form-item v-if="form.CalcStatus === 2" prop="FailReason2" label="退件原因">
						<el-input type="textarea" :autosize="{ minRows: 3, maxRows: 6 }" v-model="form.FailReason2" />
					</el-form-item>
					<!-- <el-form-item label="繳費期限">
                        <el-date-picker v-model="form.PayEndDate2" type="date" value-format="yyyy-MM-dd" placeholder="請選擇日期"></el-date-picker>
					</el-form-item>-->
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
import { mapGetters } from 'vuex';
export default {
	name: 'VerifyModal',
	props: ['show', 'data'],
	data() {
		return {
			visible: false,
			form: {}
		};
	},
	computed: {
		...mapGetters(['currentUser']),
        C_NO() {
			if (!this.form.C_NO || !this.form.SER_NO) return '待取號';
			return `${this.form.C_NO}-${this.form.SER_NO}`;
		},
		formStatusList() {
			if (this.currentUser.RoleID === 1) {
				return [
					{ value: 1, label: '審理中' },
					{ value: 2, label: '待補件' },
					{ value: 3, label: '通過待繳費' }
				];
			} else if (this.currentUser.RoleID === 2) {
				return [
					{ value: 2, label: '待補件' },
					{ value: 3, label: '通過待繳費' }
					// { value: 4, label: '已繳費完成' },
					// { value: 5, label: '免繳費' }
				];
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
		},
		calcStatusList() {
			if (this.currentUser.RoleID === 1) {
				return [
					{ value: 1, label: '審理中' },
					{ value: 2, label: '待補件' },
					{ value: 3, label: '通過待繳費' }
				];
			} else if (this.currentUser.RoleID === 2) {
				return [
					{ value: 2, label: '待補件' },
					{ value: 3, label: '通過待繳費' }
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
		updateStatus() {
			if (this.form.FormStatus > 2 && !this.form.C_NO) {
				return alert('若要通過審核，請先產生管制編號');
			}
			if (this.form.FormStatus > 2 && !this.form.S_AMT) {
				return alert('若要通過審核，請先開啟案件瀏覽並儲存，才會產生繳費金額');
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