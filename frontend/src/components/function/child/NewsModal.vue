<template>
	<vxe-modal class="edit-modal" title="編輯視窗" v-model="visible" width="50%" height="90%" :lock-scroll="false" esc-closable resize show-footer>
		<template #default>
			<el-form ref="form" :model="news" :rules="rules" size="small" label-position="top">
				<el-form-item prop="Title" label="標題">
					<el-input v-model="news.Title"></el-input>
				</el-form-item>
				<el-form-item prop="PublishDate" label="發佈日期">
					<el-date-picker v-model="news.PublishDate" type="date" value-format="yyyy-MM-dd" placeholder="上架日期"></el-date-picker>
				</el-form-item>
				<el-form-item prop="Content" label="內容">
					<el-tabs v-model="activeName" @tab-click="tabClick">
						<el-tab-pane label="編輯器" name="tab1">
							<vue-editor v-model="news.Content" useCustomImageHandler @image-added="imageAdded" />
						</el-tab-pane>
						<el-tab-pane label="原始碼" name="tab2">
							<el-input type="textarea" :autosize="{ minRows: 4, maxRows: 10}" v-model="SourceCode"></el-input>
						</el-tab-pane>
					</el-tabs>
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
	name: 'NewsModal',
	props: ['show', 'data'],
	data() {
		return {
			visible: false,
			loading: false,
			activeName: 'tab1',
			SourceCode: '',
			news: {
				Title: '',
				Content: '',
				CreateDate: ''
			},
			rules: {
				Title: [{ required: true, message: '請輸入標題', trigger: 'blur' }],
				Content: [{ required: true, message: '請輸入內容', trigger: 'blur' }],
				CreateDate: [{ required: true, message: '請選擇上線日期', trigger: 'change' }]
			}
		};
	},
	methods: {
		imageAdded(file, Editor, cursorLocation, resetUploader) {
			const formData = new FormData();
			formData.append('file', file);

			this.axios
				.post('api/News/UploadFile', formData)
				.then(res => {
					Editor.insertEmbed(cursorLocation, 'image', res.data);
					resetUploader();
				})
				.catch(err => {
					this.$message.error(err.response.data.ExceptionMessage);
				});
		},
		tabClick(tab, event) {
			if (this.activeName === 'tab2') {
				this.SourceCode = this.news.Content;
			}
			if (this.activeName === 'tab1') {
				this.news.Content = this.SourceCode;
			}
		},
		save() {
			this.$refs.form.validate(valid => {
				if (!valid) {
					this.$message.error('呃，欄位驗證錯誤');
					return false;
				}

				// 用有無ID判斷新增or修改
				const url = `api/News/${this.news.ID ? 'UpdateNews' : 'AddNews'}`;
				this.axios
					.post(url, this.news)
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
				if (this.data) {
					this.news = JSON.parse(JSON.stringify(this.data));
				} else {
					this.news = {
						Title: '',
						Content: '',
						CreateDate: ''
					};
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

<style lang="scss">
.ql-editor {
	max-height: 400px;
}
</style>