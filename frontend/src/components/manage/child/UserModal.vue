<template>
	<vxe-modal class="edit-modal" title="建立使用者" v-model="visible" width="33%" :lock-scroll="false" esc-closable resize show-footer>
		<template #default>
			<el-form ref="form" :model="user" :rules="rules" label-position="top">
				<el-row type="flex">
					<el-col :span="12">
						<el-form-item prop="Account" label="帳號">
							<el-input v-model="user.Account"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="12">
						<el-form-item prop="Password" label="密碼">
							<el-input type="password" v-model="user.Password"></el-input>
						</el-form-item>
					</el-col>
				</el-row>
				<el-row type="flex">
					<el-col :span="8">
						<el-form-item prop="UserName" label="姓名">
							<el-input v-model="user.UserName"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8">
                        <el-form-item prop="RoleID" label="權限">
							<el-select class="w100p" v-model="user.RoleID">
								<el-option v-for="(item, idx) in roles" :key="idx" :label="item.RoleName" :value="item.ID"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
                    <el-col :span="8">
                        <el-form-item label="是否啟用">
							<el-switch v-model="user.Enabled"></el-switch>
						</el-form-item>
                    </el-col>
				</el-row>
			</el-form>
		</template>
		<template #footer>
			<el-button @click="visible = false">
				<i class="fa fa-ban"></i> 取 消
			</el-button>
			<el-button type="primary" @click="save()">
				<i class="fa fa-floppy-o"></i> 儲 存
			</el-button>
		</template>
	</vxe-modal>
</template>
<script>
export default {
	name: 'UserModal',
	props: ['show', 'data', 'roles'],
	data() {
		return {
			visible: false,
			loading: false,
			user: {},
			vil: Object.freeze([]),
			rules: {
				Account: [{ required: true, message: '請輸入帳號', trigger: 'blur' }],
				Password: [{ required: true, message: '請輸入密碼', trigger: 'blur' }],
				UserName: [{ required: true, message: '請輸入姓名', trigger: 'blur' }],
                RoleID: [{ required: true, message: '請選擇權限', trigger: 'change' }],
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

				// 用有無ID判斷新增or修改
				const url = `api/Admin/${this.user.ID ? 'UpdateUser' : 'AddUser'}`;
				this.axios
					.post(url, this.user)
					.then(res => {
						if (!res.data.Status) {
							this.$message.error(res.data.Message);
							return;
						}
						this.$emit('on-saved');
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
				if (this.visible) {
					if (this.data) {
						this.user = JSON.parse(JSON.stringify(this.data));
					} else {
						this.user = {
                            Enabled: true
                        };
					}
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