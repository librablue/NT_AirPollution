<template>
	<vxe-modal title="申請單明細" v-model="visible" width="80%" height="90%" :lock-scroll="false" esc-closable resize show-footer>
		<template #default>
			<el-form ref="form" size="small" :model="form" inline>
				<el-form-item label="案件編號">{{form.AutoFormID}}</el-form-item>
				<el-form-item prop="ProjectID" label="管制編號">{{form.C_NO}}</el-form-item>
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
				<table class="table">
					<tbody>
						<tr>
							<th style="width:180px">管制編號</th>
							<td>{{form.C_NO || '待取號'}}</td>
							<th style="width:180px">序號</th>
							<td>{{form.SER_NO || '1'}}</td>
							<th style="width:180px">鄉鎮分類</th>
							<td>
								<el-form-item prop="TOWN_NO">
									<el-select v-model="form.TOWN_NO">
										<el-option label="請選擇" :value="undefined"></el-option>
										<el-option v-for="item in district" :key="item.Code" :label="item.Name" :value="item.Code"></el-option>
									</el-select>
								</el-form-item>
							</td>
						</tr>
						<tr>
							<th>申請日期</th>
							<td>{{form.C_DATE | date}}</td>
							<th>申請人</th>
							<td>
								<el-form-item prop="CreateUserName">
									<el-input v-model="form.CreateUserName" maxlength="20"></el-input>
								</el-form-item>
							</td>
							<th>申請人電子信箱</th>
							<td>
								<el-form-item prop="CreateUserEmail">
									<el-input type="email" v-model="form.CreateUserEmail" maxlength="50"></el-input>
								</el-form-item>
							</td>
						</tr>
						<tr>
							<th>1.工程名稱</th>
							<td colspan="2">
								<el-form-item prop="COMP_NAM">
									<el-input v-model="form.COMP_NAM" maxlength="150"></el-input>
								</el-form-item>
							</td>
							<th>2.工程類別</th>
							<td colspan="2">
								<el-form-item prop="KIND_NO">
									<el-select class="w100p" v-model="form.KIND_NO">
										<el-option label="請選擇" :value="null"></el-option>
										<el-option v-for="item in projectCode" :key="item.ID" :label="`${item.ID}. ${item.Name}`" :value="item.ID"></el-option>
									</el-select>
								</el-form-item>
							</td>
						</tr>
						<tr>
							<th>3.工地地址或地號</th>
							<td colspan="3">
								<el-form-item prop="ADDR">
									<el-input v-model="form.ADDR" maxlength="100"></el-input>
								</el-form-item>
							</td>
							<th>4.建照字號或合約編號</th>
							<td>
								<el-form-item prop="B_SERNO">
									<el-input v-model="form.B_SERNO" maxlength="60"></el-input>
								</el-form-item>
							</td>
						</tr>
						<tr>
							<th>座標X</th>
							<td>
								<el-form-item prop="UTME">
									<el-input type="number" v-model="form.UTME"></el-input>
								</el-form-item>
							</td>
							<th>座標Y</th>
							<td>
								<el-form-item prop="UTMN">
									<el-input type="number" v-model="form.UTMN"></el-input>
								</el-form-item>
							</td>
							<th>座標(緯度、經度)</th>
							<td>
								<el-form-item prop="LATLNG">
									<el-input v-model="form.LATLNG" maxlength="200"></el-input>
								</el-form-item>
							</td>
						</tr>
						<tr>
							<th>6.工程內容概述</th>
							<td colspan="5">
								<el-form-item prop="STATE">
									<el-input v-model="form.STATE" maxlength="200"></el-input>
								</el-form-item>
							</td>
						</tr>
						<tr>
							<th>環評保護對策</th>
							<td colspan="5">
								<el-form-item prop="EIACOMMENTS">
									<el-input v-model="form.EIACOMMENTS"></el-input>
								</el-form-item>
							</td>
						</tr>
						<tr>
							<th>記錄註記</th>
							<td colspan="5">
								<el-form-item prop="RECCOMMENTS">
									<el-input v-model="form.RECCOMMENTS"></el-input>
								</el-form-item>
							</td>
						</tr>
					</tbody>
				</table>
				<el-tabs v-model="activeTab">
					<el-tab-pane label="檢附資料" name="first">
						<div class="table-responsive">
							<table class="table">
								<thead>
									<tr>
										<th>檢附資料名稱</th>
										<th>說明</th>
										<th>檢附資料上傳</th>
									</tr>
								</thead>
								<tbody>
									<tr>
										<th>空氣污染防制費申報表</th>
										<td>建照起造人為公司行號請加蓋公司大小章，建照起造人為私人請加蓋個人私章</td>
										<td>
											<a :href="`/api/Form/Download?f=${form.Attachment.File1}`" v-if="form.Attachment.File1">{{form.Attachment.File1}}</a>
										</td>
									</tr>
									<tr>
										<th>建築執照影印本</th>
										<td></td>
										<td>
											<a :href="`/api/Form/Download?f=${form.Attachment.File2}`" v-if="form.Attachment.File2">{{form.Attachment.File2}}</a>
										</td>
									</tr>
									<tr>
										<th>營建業主身分證影本</th>
										<td>業主為建設公司檢附建設公司執照或營業事業登記證，若無營利事業登記證可用公司登記函取代</td>
										<td>
											<a :href="`/api/Form/Download?f=${form.Attachment.File3}`" v-if="form.Attachment.File3">{{form.Attachment.File3}}</a>
										</td>
									</tr>
									<tr>
										<th>簡易位置圖</th>
										<td>附註路名或大地標</td>
										<td>
											<a :href="`/api/Form/Download?f=${form.Attachment.File4}`" v-if="form.Attachment.File4">{{form.Attachment.File4}}</a>
										</td>
									</tr>
									<tr>
										<th>承包商營利事業登記證</th>
										<td>承包商第一次申報需檢附。若無營利事業登記證可用公司登記函取代</td>
										<td>
											<a :href="`/api/Form/Download?f=${form.Attachment.File5}`" v-if="form.Attachment.File5">{{form.Attachment.File5}}</a>
										</td>
									</tr>
									<tr>
										<th>承包商負責人身分證影本</th>
										<td>承包商第一次申報需檢附。空污費二萬元以上，請配合本局辦理道路認養。</td>
										<td>
											<a :href="`/api/Form/Download?f=${form.Attachment.File6}`" v-if="form.Attachment.File6">{{form.Attachment.File6}}</a>
										</td>
									</tr>
									<tr>
										<th>其它文件</th>
										<td></td>
										<td>
											<a :href="`/api/Form/Download?f=${form.Attachment.File7}`" v-if="form.Attachment.File7">{{form.Attachment.File7}}</a>
										</td>
									</tr>
									<tr>
										<th>免徵案件證明</th>
										<td>免徵案件需上傳免徵證明</td>
										<td>
											<a :href="`/api/Form/Download?f=${form.Attachment.File8}`" v-if="form.Attachment.File8">{{form.Attachment.File8}}</a>
										</td>
									</tr>
								</tbody>
							</table>
						</div>
					</el-tab-pane>
					<el-tab-pane label="停復工" name="second">停復工</el-tab-pane>
				</el-tabs>
			</el-form>
		</template>
		<template #footer>
			<el-button size="small" @click="visible = false">
				<i class="fa fa-ban"></i> 取 消
			</el-button>
			<el-button type="primary" size="small" @click="saveForm()">
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
			form: {},
			district: Object.freeze([]),
			projectCode: Object.freeze([]),
			activeTab: 'first'
		};
	},
	mounted() {
		this.getDistrict();
		this.getProjectCode();
	},
	computed: {
		...mapGetters(['currentUser']),
		diffDays() {
			if (!this.form.B_DATE2 || !this.form.E_DATE2) return 0;
			var day1 = new Date(this.form.B_DATE2);
			var day2 = new Date(this.form.E_DATE2);

			var difference = Math.abs(day2 - day1);
			var days = Math.round(difference / (1000 * 3600 * 24) + 1);
			return days;
		}
	},
	methods: {
		getDistrict() {
			this.axios.get('api/Option/GetDistrict').then(res => {
				this.district = Object.freeze(res.data);
			});
		},
		getProjectCode() {
			this.axios.get('api/Option/GetProjectCode').then(res => {
				this.projectCode = Object.freeze(res.data);
			});
		},
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
		saveForm() {
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
				if (!confirm(`你確定要將案件編號 ${this.form.AutoFormID}、管制編號 ${this.form.ProjectID}、繳費金額 ${this.form.TotalMoney} 元，通過審查產生繳費單?`)) return false;
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
.table-responsive {
	overflow-x: auto;
}
.table {
	width: 100%;
	margin-bottom: 20px;
	border-collapse: collapse;
	th {
		font-weight: 700;
	}
	td,
	th {
		border: 1px solid #ddd;
		padding: 8px;
		vertical-align: middle;
		line-height: 1.428571429;
	}
}
.modal-header {
	margin: 10px 0;
}
</style>