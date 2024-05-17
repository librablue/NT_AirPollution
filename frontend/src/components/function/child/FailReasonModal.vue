<template>
	<vxe-modal title="退件原因" v-model="visible" width="30%" :lock-scroll="false" esc-closable resize show-footer>
		<template #default>
			<el-form ref="form" :rules="rules" :model="form">
				<el-form-item prop="FailReason" label="退件原因">
					<el-input type="textarea" :autosize="{ minRows: 3, maxRows: 6 }" v-model="form.FailReason" />
				</el-form-item>
			</el-form>
		</template>
		<template #footer>
			<el-button @click="visible = false">
				<i class="fa fa-ban"></i> 取 消
			</el-button>
			<el-button type="primary" @click="sendForm">
				<i class="fa fa-ban"></i> 確 定
			</el-button>
		</template>
	</vxe-modal>
</template>

<script>
export default {
	name: 'FailReasonModal',
	props: ['show', 'callback'],
	data() {
		return {
			visible: false,
			form: {
				FailReason: null
			},
			rules: Object.freeze({
				FailReason: [{ required: true, message: '請輸入退件原因', trigger: 'blur' }]
			})
		};
	},
	methods: {
		sendForm() {
			this.$refs.form.validate(valid => {
				if (!valid) {
					alert('欄位驗證錯誤，請檢查修正後重新送出');
					return false;
				}

				if (!confirm('是否確認繼續?')) return false;
				this.$emit('on-confirm', this.form, this.callback);
				this.visible = false;
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

<style lang="scss" scoped>
.image-list {
	li {
		margin-bottom: 10px;
	}
}
</style>