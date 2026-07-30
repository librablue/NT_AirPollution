<template>
	<vxe-modal title="檢視附件" v-model="visible" width="50%" :lock-scroll="false" esc-closable resize :show-footer="false">
		<template #default>
			<div class="attach-dialog-wrapper">
				<!-- 提示說明區塊 -->
				<!-- <div class="attach-tip-box">
					<div class="tip-title">
						<i class="el-icon-warning-outline"></i> 上傳注意事項
					</div>
					<ul class="attach-tip-list">
						<li>
							每次上傳新檔案將會
							<b>覆蓋原有檔案</b>。
						</li>
						<li>
							附件請提供
							<b>彙整所有文件的 PDF 檔</b>，作為審查依據。
						</li>
						<li>
							檔案格式限定為
							<b>PDF</b>，單一檔案大小需小於
							<b>100MB</b>。
						</li>
					</ul>
				</div> -->

				<!-- 雙欄上傳區塊 (使用 Grid / Flex 排版) -->
				<el-row :gutter="20" class="attach-row">
					<!-- 首期申報附件 -->
					<el-col :sm="12" :xs="24">
						<div class="upload-card primary-card">
							<div class="card-header">
								<i class="el-icon-upload"></i> 首期申報附件
							</div>
							<div class="card-body">
								<!-- 已上傳檔案下載區塊 -->
								<div v-if="data.FileName1" class="download-box">
									<div class="file-info">
										<i class="el-icon-document text-pdf"></i>
										<span class="file-name" :title="data.DisplayName1">{{ data.DisplayName1 }}</span>
									</div>
									<a :href="`api/Form/Download?f=${data.FileName1}&n=${data.DisplayName1}`" class="download-btn">
										<i class="el-icon-download"></i> 下載
									</a>
								</div>
								<div v-else-if="data.FormStatus > 2" class="empty-file-tip">未上傳首期附件</div>
							</div>
						</div>
					</el-col>

					<!-- 結算申報附件 -->
					<el-col :sm="12" :xs="24">
						<div class="upload-card success-card">
							<div class="card-header">
								<i class="el-icon-finished"></i> 結算申報附件
							</div>
							<div class="card-body">
								<!-- 已上傳檔案下載區塊 -->
								<div v-if="data.FileName2" class="download-box">
									<div class="file-info">
										<i class="el-icon-document text-pdf"></i>
										<span class="file-name" :title="data.DisplayName2">{{ data.DisplayName2 }}</span>
									</div>
									<a :href="`api/Form/Download?f=${data.FileName2}&n=${data.DisplayName2}`" class="download-btn">
										<i class="el-icon-download"></i> 下載
									</a>
								</div>
								<div v-else-if="data.FormStatus <= 2 || data.CalcStatus > 2" class="empty-file-tip">{{ data.FormStatus <= 2 ? '請先完成首期申報' : '未上傳結算附件' }}</div>
							</div>
						</div>
					</el-col>
				</el-row>
			</div>
		</template>
	</vxe-modal>
</template>

<script>
export default {
	name: 'AttachmentModal',
	props: ['show', 'data'],
	data() {
		return {
			visible: false
		};
	},
	methods: {},
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
.attach-dialog-wrapper {
	.attach-tip-box {
		background-color: #fffbe6;
		border: 1px solid #ffe58f;
		border-radius: 6px;
		padding: 12px 16px;
		margin-bottom: 20px;

		.tip-title {
			font-size: 14px;
			font-weight: 700;
			color: #d48806;
			margin-bottom: 6px;

			i {
				margin-right: 4px;
			}
		}

		.attach-tip-list {
			margin: 0;
			padding-left: 20px;
			font-size: 13px;
			color: #595959;
			line-height: 20px;

			b {
				color: #cf1322;
			}
		}
	}

	.attach-row {
		display: flex;
		flex-wrap: wrap;

		.el-col {
			display: flex;
			margin-bottom: 16px;
		}
	}

	.upload-card {
		border: 1px solid #e2e8f0;
		border-radius: 8px;
		overflow: hidden;
		background-color: #ffffff;
		width: 100%;
		display: flex;
		flex-direction: column;

		.card-header {
			padding: 10px 16px;
			font-size: 15px;
			font-weight: 700;
			color: #ffffff;
			display: flex;
			align-items: center;

			i {
				margin-right: 6px;
				font-size: 16px;
			}
		}

		&.primary-card .card-header {
			background-color: #0284c7;
		}

		&.success-card .card-header {
			background-color: #059669;
		}

		.card-body {
			padding: 16px;
			flex: 1;
			display: flex;
			flex-direction: column;
			justify-content: space-between;

			.upload-container {
				flex: 1;
				display: flex;
				flex-direction: column;
				justify-content: center;
			}
		}

		.el-upload {
			width: 100%;

			.el-upload-dragger {
				width: 100%;
				height: 130px;
				border-radius: 6px;

				.el-icon-upload {
					margin: 16px 0 8px;
					font-size: 40px;
				}

				.el-upload__text {
					font-size: 13px;
				}
			}
		}

		.download-box {
			margin-top: 12px;
			padding: 10px 12px;
			background-color: #f8fafc;
			border: 1px solid #e2e8f0;
			border-radius: 6px;
			display: flex;
			align-items: center;
			justify-content: space-between;

			.file-info {
				display: flex;
				align-items: center;
				overflow: hidden;
				margin-right: 8px;

				.text-pdf {
					color: #ef4444;
					font-size: 18px;
					margin-right: 6px;
					flex-shrink: 0;
				}

				.file-name {
					font-size: 13px;
					color: #334155;
					white-space: nowrap;
					overflow: hidden;
					text-overflow: ellipsis;
				}
			}

			.download-btn {
				font-size: 12px;
				color: #0284c7;
				background-color: #e0f2fe;
				padding: 4px 10px;
				border-radius: 4px;
				text-decoration: none;
				font-weight: 600;
				flex-shrink: 0;
				transition: background-color 0.2s;

				&:hover {
					background-color: #bae6fd;
				}
			}
		}

		.empty-file-tip {
			text-align: center;
			color: #94a3b8;
			font-size: 13px;
			padding: 30px 0;
			flex: 1;
			display: flex;
			align-items: center;
			justify-content: center;
		}
	}
}
</style>