<template>
	<vxe-modal title="退款帳戶" v-model="visible" width="640px" :lock-scroll="false" esc-closable resize show-footer>
		<template #default>
			<div v-if="data.RefundBank && data.RefundBank.ID" class="refund-bank-card">
				<div class="bank-info-grid">
					<div class="info-item">
						<span class="info-label">銀行代碼</span>
						<span class="info-value text-highlight">{{ data.RefundBank.Code }}</span>
					</div>
					<div class="info-item">
						<span class="info-label">銀行帳號</span>
						<span class="info-value text-mono">{{ data.RefundBank.Account }}</span>
					</div>
				</div>

				<div class="bank-photo-section">
					<div class="info-label">存摺照片</div>
					<div class="photo-card">
						<img :src="`api/Form/Download?f=${data.RefundBank.Photo}`" alt="存摺照片" />
						<div class="photo-overlay">
							<a :href="`api/Form/Download?f=${data.RefundBank.Photo}`" target="_blank" class="preview-btn">
								<i class="el-icon-view"></i> 查看大圖
							</a>
						</div>
					</div>
				</div>
			</div>

			<div v-else class="refund-bank-empty">
				<i class="el-icon-document-delete empty-icon"></i>
				<span class="empty-text">暫無退費帳戶資料</span>
			</div>
		</template>
		<template #footer>
			<el-button @click="visible = false">
				<i class="fa fa-ban"></i> 取 消
			</el-button>
		</template>
	</vxe-modal>
</template>

<script>
export default {
	name: 'RefundModal',
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
	data() {
		return {
			visible: false
		};
	},
	methods: {},
	watch: {
		show: {
			handler(newValue) {
				this.visible = newValue;
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
.refund-bank-card {
	background-color: #ffffff;
	border: 1px solid #e2e8f0;
	border-radius: 8px;
	padding: 20px;
	box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);

	.bank-info-grid {
		display: grid;
		grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
		gap: 16px;
		padding-bottom: 16px;
		border-bottom: 1px dashed #e2e8f0;
		margin-bottom: 16px;
	}

	.info-item {
		display: flex;
		flex-direction: column;
		gap: 4px;

		.info-value {
			font-size: 16px;
			font-weight: 600;
			color: #1e293b;

			&.text-highlight {
				color: #0284c7;
			}

			&.text-mono {
				font-family: SFMono-Regular, Consolas, 'Liberation Mono', Menlo, monospace;
				letter-spacing: 0.5px;
			}
		}
	}

	.info-label {
		font-size: 13px;
		color: #64748b;
		font-weight: 500;
	}

	.bank-photo-section {
		display: flex;
		flex-direction: column;
		gap: 8px;

		.photo-card {
			position: relative;
			width: 100%;
			max-width: 320px;
			border-radius: 6px;
			overflow: hidden;
			border: 1px solid #cbd5e1;
			background-color: #f8fafc;
			box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);

			img {
				display: block;
				width: 100%;
				height: auto;
				object-fit: cover;
			}

			.photo-overlay {
				position: absolute;
				top: 0;
				left: 0;
				width: 100%;
				height: 100%;
				background-color: rgba(15, 23, 42, 0.55);
				display: flex;
				align-items: center;
				justify-content: center;
				opacity: 0;
				transition: opacity 0.2s ease;

				.preview-btn {
					color: #ffffff;
					background-color: rgba(255, 255, 255, 0.2);
					padding: 6px 14px;
					border-radius: 20px;
					font-size: 13px;
					text-decoration: none;
					backdrop-filter: blur(4px);
					border: 1px solid rgba(255, 255, 255, 0.4);
					transition: background-color 0.2s;

					&:hover {
						background-color: rgba(255, 255, 255, 0.35);
					}

					i {
						margin-right: 4px;
					}
				}
			}

			&:hover .photo-overlay {
				opacity: 1;
			}
		}
	}
}

.refund-bank-empty {
	display: flex;
	flex-direction: column;
	align-items: center;
	justify-content: center;
	padding: 32px 16px;
	background-color: #f8fafc;
	border: 1px dashed #cbd5e1;
	border-radius: 8px;
	color: #94a3b8;

	.empty-icon {
		font-size: 36px;
		margin-bottom: 8px;
	}

	.empty-text {
		font-size: 14px;
	}
}
</style>