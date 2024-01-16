<template>
	<vxe-modal title="申請單明細" v-model="visible" width="80%" height="90%" :lock-scroll="false" esc-closable resize show-footer>
		<template #default>
			<el-form ref="form" size="small" :model="form" inline>
				<el-form-item label="案件編號">{{form.AutoFormID}}</el-form-item>
				<el-form-item prop="ProjectID" label="管制編號">{{form.C_NO}}</el-form-item>
				<el-form-item prop="FormStatus" label="審核狀態">
					<el-select style="width:140px" v-model="form.FormStatus">
						<el-option label="審理中" :value="1" v-if="data.FormStatus <= 1"></el-option>
						<el-option label="需補件" :value="2" v-if="data.FormStatus <= 2"></el-option>
						<el-option label="通過待繳費" :value="3" v-if="data.FormStatus <= 3"></el-option>
						<el-option label="已繳費完成" :value="4" v-if="data.FormStatus <= 4"></el-option>
					</el-select>
				</el-form-item>
				<el-form-item prop="FailReason" label="補件原因" v-if="form.FormStatus === 2">
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
									<el-select v-model="form.KIND_NO">
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
				<h3>營建資料</h3>
				<div class="table-responsive">
					<table class="table form-table">
						<tbody>
							<tr>
								<th style="width:180px">7.營建業主名稱</th>
								<td colspan="3">
									<el-form-item prop="S_NAME">
										<el-input v-model="form.S_NAME" maxlength="80"></el-input>
									</el-form-item>
								</td>
								<th style="width:180px">8.營利事業統一編號</th>
								<td>
									<el-form-item prop="S_G_NO">
										<el-input v-model="form.S_G_NO" maxlength="10"></el-input>
									</el-form-item>
								</td>
							</tr>
							<tr>
								<th>9.營業地址</th>
								<td colspan="5">
									<el-form-item prop="S_ADDR1">
										<el-input v-model="form.S_ADDR1" maxlength="50"></el-input>
									</el-form-item>
								</td>
							</tr>
							<tr>
								<th>10.聯絡地址</th>
								<td colspan="3">
									<el-form-item prop="S_ADDR2">
										<el-input v-model="form.S_ADDR2" maxlength="50"></el-input>
									</el-form-item>
								</td>
								<th>11.連絡電話</th>
								<td>
									<el-form-item prop="S_TEL">
										<el-input type="tel" v-model="form.S_TEL" maxlength="30"></el-input>
									</el-form-item>
								</td>
							</tr>
							<tr>
								<th>12.負責人姓名</th>
								<td>
									<el-form-item prop="S_B_NAM">
										<el-input v-model="form.S_B_NAM" maxlength="20"></el-input>
									</el-form-item>
								</td>
								<th style="width:180px">13.職稱</th>
								<td>
									<el-form-item prop="S_B_TIT">
										<el-input v-model="form.S_B_TIT" maxlength="20"></el-input>
									</el-form-item>
								</td>
								<th>14.身分證字號</th>
								<td>
									<el-form-item prop="S_B_ID">
										<el-input v-model="form.S_B_ID" maxlength="20"></el-input>
									</el-form-item>
								</td>
							</tr>
							<tr>
								<th>生日</th>
								<td colspan="5">
									<el-form-item prop="S_B_BDATE2">
										<el-date-picker v-model="form.S_B_BDATE2" type="date" value-format="yyyy-MM-dd" placeholder="請選擇日期"></el-date-picker>
									</el-form-item>
								</td>
							</tr>
							<tr>
								<th>15.聯絡人姓名</th>
								<td>
									<el-form-item prop="S_C_NAM">
										<el-input v-model="form.S_C_NAM" maxlength="20"></el-input>
									</el-form-item>
								</td>
								<th>16.職稱</th>
								<td>
									<el-form-item prop="S_C_TIT">
										<el-input v-model="form.S_C_TIT" maxlength="20"></el-input>
									</el-form-item>
								</td>
								<th>17.身分證字號</th>
								<td>
									<el-form-item prop="S_C_ID">
										<el-input v-model="form.S_C_ID" maxlength="20"></el-input>
									</el-form-item>
								</td>
							</tr>
							<tr>
								<th>18.聯絡人地址</th>
								<td colspan="3">
									<el-form-item prop="S_C_ADDR">
										<el-input v-model="form.S_C_ADDR" maxlength="50"></el-input>
									</el-form-item>
								</td>
								<th>19.電話</th>
								<td>
									<el-form-item prop="S_C_TEL">
										<el-input type="tel" v-model="form.S_C_TEL" maxlength="30"></el-input>
									</el-form-item>
								</td>
							</tr>
						</tbody>
					</table>
				</div>
				<h3>承包(造)資料</h3>
				<div class="table-responsive">
					<table class="table form-table">
						<tbody>
							<tr>
								<th style="width:180px">20.承包(造)單位名稱</th>
								<td colspan="3">
									<el-form-item prop="R_NAME">
										<el-input v-model="form.R_NAME" maxlength="60"></el-input>
									</el-form-item>
								</td>
								<th style="width:180px">21.營利事業統一編號</th>
								<td>
									<el-form-item prop="R_G_NO">
										<el-input v-model="form.R_G_NO" maxlength="10"></el-input>
									</el-form-item>
								</td>
							</tr>
							<tr>
								<th>22.營業地址</th>
								<td colspan="5">
									<el-form-item prop="R_ADDR1">
										<el-input v-model="form.R_ADDR1" maxlength="50"></el-input>
									</el-form-item>
								</td>
							</tr>
							<tr>
								<th>23.聯絡地址</th>
								<td colspan="3">
									<el-form-item prop="R_ADDR2">
										<el-input v-model="form.R_ADDR2" maxlength="50"></el-input>
									</el-form-item>
								</td>
								<th>24.連絡電話</th>
								<td>
									<el-form-item prop="R_TEL">
										<el-input type="tel" v-model="form.R_TEL" maxlength="30"></el-input>
									</el-form-item>
								</td>
							</tr>
							<tr>
								<th>25.負責人姓名</th>
								<td>
									<el-form-item prop="R_B_NAM">
										<el-input v-model="form.R_B_NAM" maxlength="20"></el-input>
									</el-form-item>
								</td>
								<th style="width:180px">26.職稱</th>
								<td>
									<el-form-item prop="R_B_TIT">
										<el-input v-model="form.R_B_TIT" maxlength="20"></el-input>
									</el-form-item>
								</td>
								<th>27.身分證字號</th>
								<td>
									<el-form-item prop="R_B_ID">
										<el-input v-model="form.R_B_ID" maxlength="30"></el-input>
									</el-form-item>
								</td>
							</tr>
							<tr>
								<th>生日</th>
								<td colspan="5">
									<el-form-item prop="R_B_BDATE2">
										<el-date-picker v-model="form.R_B_BDATE2" type="date" value-format="yyyy-MM-dd" placeholder="請選擇日期"></el-date-picker>
									</el-form-item>
								</td>
							</tr>
							<tr>
								<th>28.工務所地址</th>
								<td colspan="5">
									<el-form-item prop="R_ADDR3">
										<el-input v-model="form.R_ADDR3" maxlength="50"></el-input>
									</el-form-item>
								</td>
							</tr>
							<tr>
								<th>29.工地主任姓名</th>
								<td>
									<el-form-item prop="R_M_NAM">
										<el-input v-model="form.R_M_NAM" maxlength="10"></el-input>
									</el-form-item>
								</td>
								<th>30.工地環保負責人姓名</th>
								<td>
									<el-form-item prop="R_C_NAM">
										<el-input v-model="form.R_C_NAM" maxlength="10"></el-input>
									</el-form-item>
								</td>
								<th>31.電話</th>
								<td>
									<el-form-item prop="R_TEL1">
										<el-input type="tel" v-model="form.R_TEL1" maxlength="30"></el-input>
									</el-form-item>
								</td>
							</tr>
						</tbody>
					</table>
				</div>
				<h3>經費資料</h3>
				<div class="table-responsive">
					<table class="table form-table">
						<tbody>
							<tr>
								<th style="width:180px">32.工程合約經費</th>
								<td>
									<el-form-item prop="MONEY">
										<el-input type="number" v-model="form.MONEY"></el-input>
									</el-form-item>
								</td>
								<th style="width:180px">33.工程環保經費</th>
								<td>
									<el-form-item prop="C_MONEY">
										<el-input type="number" v-model="form.C_MONEY"></el-input>
									</el-form-item>
								</td>
								<th style="width:180px">工程合約經費比例</th>
								<td>
									<el-form-item prop="PERCENT">
										<el-input type="number" v-model="form.PERCENT"></el-input>
									</el-form-item>
								</td>
							</tr>
							<tr>
								<th>施工面積</th>
								<td>
									<el-form-item prop="AREA">
										<el-input type="number" v-model="form.AREA" placeholder="非疏濬工程"></el-input>
									</el-form-item>
								</td>
								<th>清運土石體積</th>
								<td>
									<el-form-item prop="VOLUMEL">
										<el-input type="number" v-model="form.VOLUMEL" placeholder="疏濬工程"></el-input>
									</el-form-item>
								</td>
							</tr>
							<tr>
								<th>34.預計施工開始日期</th>
								<td>
									<el-form-item prop="B_DATE2">
										<el-date-picker v-model="form.B_DATE2" type="date" value-format="yyyy-MM-dd" placeholder="請選擇日期"></el-date-picker>
									</el-form-item>
								</td>
								<th>預計施工完成日期</th>
								<td>
									<el-form-item prop="E_DATE2">
										<el-date-picker v-model="form.E_DATE2" type="date" value-format="yyyy-MM-dd" placeholder="請選擇日期"></el-date-picker>
									</el-form-item>
								</td>
								<th>預計施工天數</th>
								<td>{{totalDays}}</td>
							</tr>
							<tr>
								<th>規定繳費方式</th>
								<td>
									<el-select v-model="form.P_KIND">
										<el-option label="一次全繳" value="一次全繳"></el-option>
										<el-option label="分兩次繳清" value="分兩次繳清"></el-option>
									</el-select>
								</td>
								<th>空汙防制措施計畫書</th>
								<td colspan="3">
									<el-select v-model="form.BUD_DOC2">
										<el-option label="有" value="有"></el-option>
										<el-option label="無" value="無"></el-option>
									</el-select>
								</td>
							</tr>
						</tbody>
					</table>
				</div>
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
					<el-tab-pane label="停復工" name="second">
						<el-button type="primary" size="small" icon="el-icon-plus" @click="addStopWork()">新 增</el-button>
						<div class="table-responsive" style="max-width:500px; margin-top:10px">
							<table class="table stopwork-table">
								<thead>
									<tr>
										<th style="width:50px">刪除</th>
										<th style="width:160px">停工日期</th>
										<th style="width:160px">復工日期</th>
										<th style="width:120px; white-space: nowrap">停工天數</th>
									</tr>
								</thead>
								<tbody>
									<tr v-for="(item, idx) in form.StopWorks" :key="idx">
										<td style="width: 50px">
											<el-button type="danger" size="mini" icon="el-icon-delete" circle @click="deleteStopWork(idx)"></el-button>
										</td>
										<td>
											<el-date-picker class="w100p" v-model="item.DOWN_DATE2" type="date" value-format="yyyy-MM-dd" placeholder="請選擇日期"></el-date-picker>
										</td>
										<td>
											<el-date-picker class="w100p" v-model="item.UP_DATE2" type="date" value-format="yyyy-MM-dd" placeholder="請選擇日期"></el-date-picker>
										</td>
										<td>{{getStopDays(item)}}</td>
									</tr>
								</tbody>
							</table>
						</div>
					</el-tab-pane>
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
		totalDays() {
			if (!this.form.B_DATE2 || !this.form.E_DATE2) return '';
			var date1 = new Date(this.form.B_DATE2);
			var date2 = new Date(this.form.E_DATE2);

			// 計算毫秒差異
			var diff = Math.abs(date2 - date1 + 1000 * 60 * 60 * 24);
			// 轉換為天數
			var dayDiff = Math.ceil(diff / (1000 * 60 * 60 * 24));

			return dayDiff;
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
			// if (this.form.ProjectCode === 'B') {
			// 	this.form.Area3 = 3;
			// } else {
			// 	// 原本是3的才幫忙修改選項
			// 	if (this.form.Area3 === 3) this.form.Area3 = 1;
			// }
		},
		getStopDays(row) {
			if (!row.DOWN_DATE2 || !row.UP_DATE2) return '';
			var date1 = new Date(row.DOWN_DATE2);
			var date2 = new Date(row.UP_DATE2);

			// 計算毫秒差異
			var diff = Math.abs(date2 - date1 + 1000 * 60 * 60 * 24);
			// 轉換為天數
			var dayDiff = Math.ceil(diff / (1000 * 60 * 60 * 24));

			return dayDiff;
		},
		addStopWork() {
			this.form.StopWorks.push({
				DOWN_DATE: '',
				DOWN_DATE2: '',
				UP_DATE: '',
				UP_DATE2: ''
			});
		},
		deleteStopWork(idx) {
			if (!confirm('是否確認刪除?')) return;
			this.form.StopWorks.splice(idx, 1);
		},
		saveForm() {
			if (this.form.FormStatus === 2 && !this.form.FailReason) {
				alert('呃，請輸入補件原因。');
				return false;
			}
			if (this.form.FormStatus === 3) {
				if (!this.form.TotalMoney) {
					alert('呃，請輸入應繳總金額。');
					return false;
				}
				if (!this.form.ProjectID) {
					alert('呃，請輸入管制編號。');
					return false;
				}
			}
			if (this.data.FormStatus !== 3 && this.form.FormStatus === 3) {
				if (!confirm(`你確定要將案件編號 ${this.form.AutoFormID}、管制編號 ${this.form.ProjectID}、繳費金額 ${this.form.TotalMoney} 元，通過審查產生繳費單?`)) return false;
			}
			if (this.form.FormStatus === 4 && !this.form.ReceiveMoney) {
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
.stopwork-table {
	th,td {
		text-align: center;
	}
}
.modal-header {
	margin: 10px 0;
}
</style>