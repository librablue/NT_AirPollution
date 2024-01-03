<template>
	<vxe-modal title="申請單明細" v-model="visible" width="80%" height="90%" :lock-scroll="false" esc-closable resize show-footer>
		<template #default>
			<el-form ref="form" size="small" :model="form" inline>
				<el-form-item label="案件編號">{{form.PaymentID}}</el-form-item>
				<el-form-item prop="ProjectID" label="管制編號">
					<el-input maxlength="50" v-model="form.ProjectID" />
				</el-form-item>
				<el-form-item prop="TotalMoney" label="應繳總金額">
					<el-input style="width:140px" type="number" v-model="form.TotalMoney" />
				</el-form-item>
				<el-form-item>
					<el-button type="warning" size="mini" icon="el-icon-right" circle @click="setReceiveMoney()"></el-button>
				</el-form-item>
				<el-form-item prop="ReceiveMoney" label="已收金額">
					<el-input style="width:140px" type="number" v-model="form.ReceiveMoney" />
				</el-form-item>
				<el-form-item prop="Status" label="審核狀態">
					<el-select style="width:140px" v-model="form.Status">
						<el-option label="審理中" :value="1" v-if="data.Status <= 1"></el-option>
						<el-option label="需補件" :value="2" v-if="data.Status <= 2"></el-option>
						<el-option label="通過待繳費" :value="3" v-if="data.Status <= 3"></el-option>
						<el-option label="已繳費完成" :value="4" v-if="data.Status <= 4"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item prop="FailReason" label="補件原因" v-if="form.Status === 2">
					<el-input type="textarea" :autosize="{ minRows: 2, maxRows: 6 }" v-model="form.FailReason" />
				</el-form-item>
				<table class="modal-table border">
					<tbody>
						<tr>
							<th style="width: 180px">1.工程名稱</th>
							<td colspan="7">
								<el-input v-model="form.ProjectName" />
							</td>
						</tr>
						<tr>
							<th>2.工地地址或地號</th>
							<td colspan="3">
								<el-input v-model="form.ProjectAddr" />
							</td>
							<th style="width: 180px">3.E-Mail</th>
							<td colspan="3">
								<el-input style="width:250px" v-model="form.Email" />
								({{form.IsVerify ? '已驗證' : '未驗證'}})
							</td>
						</tr>
						<tr>
							<th>4.建照字號或合約編號</th>
							<td colspan="3">
								<el-input v-model="form.ContractID" />
							</td>
							<th>5.工程類別代碼</th>
							<td colspan="3">
								<el-select style="width:100%" v-model="form.ProjectCode" @change="projectCodeChange()">
									<el-option label="請選擇" value></el-option>
									<el-option label="1. 建築（房屋）工程-鋼筋混凝土構造" value="1"></el-option>
									<el-option label="2. 建築（房屋）工程-鋼骨結構" value="2"></el-option>
									<el-option label="3. 建築（房屋）工程-拆除" value="3"></el-option>
									<el-option label="4. 道路（隧道）工程-道路" value="4"></el-option>
									<el-option label="5. 道路（隧道）工程-隧道" value="5"></el-option>
									<el-option label="6. 管線開挖工程" value="6"></el-option>
									<el-option label="7. 橋樑工程" value="7"></el-option>
									<el-option label="8. 區域開發工程-社區" value="8"></el-option>
									<el-option label="9. 區域開發工程-工業區" value="9"></el-option>
									<el-option label="A. 區域開發工程-遊樂區" value="A"></el-option>
									<el-option label="B. 疏濬工程" value="B"></el-option>
									<el-option label="Z. 其他工程" value="Z"></el-option>
								</el-select>
							</td>
						</tr>
						<tr>
							<th>6.工程內容概述</th>
							<td colspan="7">
								<el-input v-model="form.ProjectDescription" />
							</td>
						</tr>
						<tr>
							<th>7.營建業主名稱</th>
							<td colspan="3">
								<el-input v-model="form.BizCompany" />
							</td>
							<th>8.營利事業統一編號</th>
							<td colspan="3">
								<el-input v-model="form.BizID" />
							</td>
						</tr>
						<tr>
							<th>9.營業地址</th>
							<td colspan="7">
								<el-input v-model="form.BizAddr1" />
							</td>
						</tr>
						<tr>
							<th>10.聯絡地址</th>
							<td colspan="3">
								<el-input v-model="form.BizAddr2" />
							</td>
							<th>11.聯絡電話</th>
							<td colspan="3">
								<el-input v-model="form.BizTel" />
							</td>
						</tr>
						<tr>
							<th>12.負責人姓名</th>
							<td>
								<el-input v-model="form.BizOwnerName" />
							</td>
							<th style="width: 80px">13.職稱</th>
							<td>
								<el-input v-model="form.BizOwnerJobTitle" />
							</td>
							<th>14.身分證字號</th>
							<td colspan="3">
								<el-input v-model="form.BizOwnerID" />
							</td>
						</tr>
						<tr>
							<th>15.聯絡人姓名</th>
							<td>
								<el-input v-model="form.BizContactName" />
							</td>
							<th>16.職稱</th>
							<td>
								<el-input v-model="form.BizContactJobTitle" />
							</td>
							<th>17.身分證字號</th>
							<td colspan="3">
								<el-input v-model="form.BizContactID" />
							</td>
						</tr>
						<tr>
							<th>18.聯絡人地址</th>
							<td colspan="3">
								<el-input v-model="form.BizContactAddr" />
							</td>
							<th>19.電話</th>
							<td colspan="3">
								<el-input v-model="form.BizContactTel" />
							</td>
						</tr>
						<tr>
							<th>20.承包(造)單位名稱</th>
							<td colspan="3">
								<el-input v-model="form.ExecCompany" />
							</td>
							<th>21.營利事業統一編號</th>
							<td colspan="3">
								<el-input v-model="form.ExecCompany" />
							</td>
						</tr>
						<tr>
							<th>22.營業地址</th>
							<td colspan="7">
								<el-input v-model="form.ExecAddr1" />
							</td>
						</tr>
						<tr>
							<th>23.聯絡地址</th>
							<td colspan="3">
								<el-input v-model="form.ExecAddr2" />
							</td>
							<th>24.聯絡電話</th>
							<td colspan="3">
								<el-input v-model="form.ExecTel1" />
							</td>
						</tr>
						<tr>
							<th>25.負責人姓名</th>
							<td>
								<el-input v-model="form.ExecOwnerName" />
							</td>
							<th>26.職稱</th>
							<td>
								<el-input v-model="form.ExecOwnerJobTitle" />
							</td>
							<th>27.身分證字號</th>
							<td colspan="3">
								<el-input v-model="form.ExecOwnerID" />
							</td>
						</tr>
						<tr>
							<th>28.工務所地址</th>
							<td colspan="3">
								<el-input v-model="form.ExecAddr3" />
							</td>
							<th>29.電話</th>
							<td colspan="3">
								<el-input v-model="form.ExecTel2" />
							</td>
						</tr>
						<tr>
							<th>30.工地主任姓名</th>
							<td>
								<el-input v-model="form.ExecManagerName" />
							</td>
							<th>31.電話</th>
							<td>
								<el-input v-model="form.ExecManagerTel" />
							</td>
							<th>32.工地環保負責人姓名</th>
							<td>
								<el-input v-model="form.ExecEnvManagerName" />
							</td>
							<th style="width: 80px">33.電話</th>
							<td>
								<el-input v-model="form.ExecEnvMangerTel" />
							</td>
						</tr>
						<tr>
							<th>34.工程合約經費</th>
							<td colspan="7">
								<el-input style="width:200px" data-type="integer" type="number" v-model="form.ContractMoney" />
							</td>
						</tr>
						<tr>
							<th>35.工程環保經費</th>
							<td colspan="7">
								<el-input style="width:200px" data-type="integer" type="number" v-model="form.EnvMoney" />佔工程合約經費之
								<el-input style="width:200px" type="number" v-model="form.EnvMoneyPercent" />%
							</td>
						</tr>
						<tr>
							<th>36.工程面積</th>
							<td colspan="7">
								<el-select v-model="form.Area1">
									<el-option label="請選擇" value></el-option>
									<el-option label="建築面積" :value="1"></el-option>
									<el-option label="總樓地板面積" :value="2"></el-option>
									<el-option label="施工面積" :value="3"></el-option>
									<el-option label="隧道面積" :value="4"></el-option>
									<el-option label="橋面面積" :value="5"></el-option>
									<el-option label="開發面積" :value="6"></el-option>
								</el-select>
								<el-input style="width:160px; margin-left:20px;" type="number" v-model="form.Area2" />
								<span v-if="form.ProjectCode === 'B'">
									M
									<sup>3</sup>
								</span>
								<span style="margin-left:20px" v-else>
									<el-radio v-model="form.Area3" :label="1">
										M
										<sup>2</sup>
									</el-radio>
									<el-radio v-model="form.Area3" :label="2">公頃</el-radio>
								</span>
							</td>
						</tr>
						<tr>
							<th>37.預計施工期程</th>
							<td colspan="7">
								<el-date-picker style="width:160px" placement="bottom-start" v-model="form.StartDate" type="date" value-format="yyyy-MM-dd" placeholder="開始日期"></el-date-picker>~
								<el-date-picker style="width:160px" placement="bottom-start" v-model="form.EndDate" type="date" value-format="yyyy-MM-dd" placeholder="開始日期"></el-date-picker>
								共 {{diffDays}} 日曆天
							</td>
						</tr>
						<tr>
							<th>38.是否繳交空氣污染防制書(嘉義市政府公共工程請繳交)</th>
							<td colspan="2">
								<el-switch v-model="form.HasPromiseDoc" active-text="是" inactive-text="否"></el-switch>
							</td>
							<td colspan="5">
								<a :href="`api/Form/Download?f=${form.PromiseDoc}&n=${form.PromiseDocDisplayName}`" target="_blank">{{form.PromiseDocDisplayName}}</a>
							</td>
						</tr>
					</tbody>
				</table>
				<h2 class="modal-header">
					<i class="fa fa-paperclip"></i> 附件
				</h2>
				<ul class="file-list">
					<li v-for="(item, idx) in form.Attachments" :key="idx">
						<div style="width: 100%" class="flex-row-center">
							<span class="filename">
								<a :href="`api/Form/Download?f=${item.FileName}&n=${item.DisplayName}`" target="_blank">{{item.DisplayName}}</a>
							</span>
						</div>
					</li>
				</ul>
				<h2 class="modal-header">
					<i class="fa fa-paperclip"></i> 收據
				</h2>
				<div class="mb-2">
					<a v-if="data.Status === 4" :href="`api/Form/Download?f=Receipt/收據${form.PaymentID}.pdf&n=收據${form.PaymentID}.pdf`" target="_blank">{{`收據${form.PaymentID}.pdf`}}</a>
				</div>
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
import { mapGetters } from 'vuex';
import { dateTime, form } from '@/mixins/filter';
export default {
	name: 'FormModal',
	props: ['show', 'data'],
	mixins: [dateTime, form],
	data() {
		return {
			visible: false,
			loading: false,
			form: {}
		};
	},
	mounted() {},
	computed: {
		...mapGetters(['currentUser']),
		diffDays() {
			if (!this.form.StartDate || !this.form.EndDate) return 0;
			var day1 = new Date(this.form.StartDate);
			var day2 = new Date(this.form.EndDate);

			var difference = Math.abs(day2 - day1);
			var days = Math.round(difference / (1000 * 3600 * 24) + 1);
			return days;
		}
	},
	methods: {
		setReceiveMoney() {
			this.form.ReceiveMoney = this.form.TotalMoney;
		},
		projectCodeChange() {
			if (this.form.ProjectCode === 'B') {
				this.form.Area3 = 3;
			} else {
				// 原本是3的才幫忙修改選項
				if (this.form.Area3 === 3) this.form.Area3 = 1;
			}
		},
		save() {
			if (this.form.Status === 2 && !this.form.FailReason) {
				alert('呃，請輸入補件原因。');
				return false;
			}
			if (this.form.Status === 3) {
				if (!this.form.TotalMoney) {
					alert('呃，請輸入應繳總金額。');
					return false;
				}
				if (!this.form.ProjectID) {
					alert('呃，請輸入管制編號。');
					return false;
				}
			}
			if (this.data.Status !== 3 && this.form.Status === 3) {
				if (!confirm(`你確定要將案件編號 ${this.form.PaymentID}、管制編號 ${this.form.ProjectID}、繳費金額 ${this.form.TotalMoney} 元，通過審查產生繳費單?`)) return false;
			}
			if (this.form.Status === 4 && !this.form.ReceiveMoney) {
				alert('呃，請輸入已收金額。');
				return false;
			}
			this.axios
				.post('api/Form/UpdateForm', this.form)
				.then(res => {
					this.$emit('on-updated');
					this.$message.success('畫面資料已儲存');
					this.visible = false;
				})
				.catch(err => {
					this.$message.error(err.response.data.ExceptionMessage);
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
<style lang="scss">
.el-form-item__label {
	font-weight: 700;
}
.modal-table {
	width: 100%;
	border-collapse: collapse;
	&.border {
		th,
		td {
			border: 1px solid #ccc;
		}
	}
	th,
	td {
		padding: 11px 8px;
		vertical-align: top !important;
	}
	.pure-text {
		line-height: 33px;
		margin: 0 10px;
	}
}
.modal-header {
	margin: 10px 0;
}
.file-list {
	list-style-type: none;
	li {
		display: flex;
		border-bottom: 1px dashed #ccc;
		padding: 4px 0;
		.filename {
			display: block;
			overflow: hidden;
			text-overflow: ellipsis;
			white-space: nowrap;
		}
	}
}
</style>