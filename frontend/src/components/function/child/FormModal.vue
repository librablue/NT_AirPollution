<template>
	<vxe-modal title="申請單明細" v-model="visible" width="80%" height="90%" :lock-scroll="false" esc-closable resize show-footer>
		<template #default>
			<el-form class="modal-form" ref="form" size="small" :rules="rules" :model="form">
				<div class="form-item-inline">
					<div class="form-item-col">
						<el-form-item prop="C_NO" label="管制編號">{{form.C_NO}}</el-form-item>
					</div>
					<div class="form-item-col">
						<el-form-item label="申報應繳金額">{{form.S_AMT | comma}}</el-form-item>
					</div>
					<div v-if="form.FormStatus === 1 || form.FormStatus === 2" class="form-item-col">
						<el-link type="primary" style="line-height: 32px;" @click="finalCalc('S_AMT')">產生申報金額</el-link>
					</div>
					<div class="form-item-col">
						<el-form-item label="結算應繳金額">{{form.S_AMT2 | comma}}</el-form-item>
					</div>
					<div v-if="form.CalcStatus === 1 || form.CalcStatus === 2" class="form-item-col">
						<el-link type="primary" style="line-height: 32px;" @click="finalCalc('S_AMT2')">產生結算金額</el-link>
					</div>
				</div>

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
					<el-tab-pane label="檢附資料" name="tab1">
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
									<tr v-for="(item, idx) in filterAttachmentInfo" :key="idx">
										<th>{{item.FileTitle}}</th>
										<td>{{item.Description}}</td>
										<td>
											<ul class="file-list">
												<li v-for="sub in filterAttachments(item.ID)" :key="sub.ID">
													<a :href="`/Option/Download?f=${sub.FileName}`" class="link-download">{{sub.FileName}}</a>
												</li>
											</ul>
										</td>
									</tr>
								</tbody>
							</table>
						</div>
					</el-tab-pane>
					<el-tab-pane label="停復工" name="tab2">
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
					<el-tab-pane label="收款金額" name="tab3">
						<div class="form-item-inline">
							<div class="form-item-col">
								<el-input type="number" size="small" v-model="newPayment"></el-input>
							</div>
							<div class="form-item-col">
								<el-button type="primary" size="small" icon="el-icon-plus" @click="addPayment()">新 增</el-button>
							</div>
						</div>
						<div class="table-responsive" style="max-width:300px; margin-top:10px">
							<table class="table stopwork-table">
								<thead>
									<tr>
										<th style="width:50px">刪除</th>
										<th style="width:120px">收款金額</th>
										<th style="width:120px;">日期</th>
									</tr>
								</thead>
								<tbody>
									<tr v-for="(item, idx) in form.Payments" :key="idx">
										<td style="width: 50px">
											<el-button type="danger" size="mini" icon="el-icon-delete" circle @click="deletePayment(idx)"></el-button>
										</td>
										<td>{{item.Amount | comma}}</td>
										<td>{{item.CreateDate | date}}</td>
									</tr>
								</tbody>
								<tfoot>
									<tr>
										<td>合計</td>
										<td>{{totalPayment | comma}}</td>
										<td></td>
									</tr>
								</tfoot>
							</table>
						</div>
					</el-tab-pane>
					<el-tab-pane v-if="form.RefundBank.ID" label="退款帳戶" name="tab4">
						<el-form-item label="銀行代碼">{{form.RefundBank.Code}}</el-form-item>
						<el-form-item label="銀行帳號">{{form.RefundBank.Account}}</el-form-item>
						<el-form-item label="存摺照片">
							<img style="width:640px" :src="`api/Option/Download?f=${form.RefundBank.Photo}`" />
						</el-form-item>
					</el-tab-pane>
					<el-tab-pane v-if="form.PaymentProof.ID" label="繳費證明" name="tab5">
						<el-form-item label="繳費證明">
							<img style="width:640px" :src="`api/Option/Download?f=${form.PaymentProof.ProofFile}`" />
						</el-form-item>
					</el-tab-pane>
				</el-tabs>
			</el-form>
		</template>
		<template #footer>
			<el-button @click="visible = false">
				<i class="fa fa-ban"></i> 取 消
			</el-button>
			<el-button type="primary" @click="saveForm()">
				<i class="fa fa-floppy-o"></i> 儲 存
			</el-button>
		</template>
	</vxe-modal>
</template>
<script>
import { mapGetters } from 'vuex';
import { dateTime, comma, form } from '@/mixins/filter';
export default {
	name: 'FormModal',
	props: ['show', 'data'],
	mixins: [dateTime, comma, form],
	data() {
		const checkE_DATE2 = (rule, value, callback) => {
			if (!value) {
				callback(new Error('請輸入預計施工完成日期'));
			}
			if (this.form.B_DATE2 && this.form.E_DATE2 && moment(value).isSameOrBefore(this.form.B_DATE2)) {
				callback(new Error('結束日期不得早於起始日期'));
			}
			callback();
		};
		const checkArea = (rule, value, callback) => {
			if (!this.form.VOLUMEL && !value) {
				callback(new Error('如果非疏濬工程，請輸入施工面積'));
			}
			callback();
		};
		const checkVolumel = (rule, value, callback) => {
			if (!this.form.AREA && !value) {
				callback(new Error('如果為疏濬工程，請輸入清運土石體積'));
			}
			callback();
		};
		const checkFailReason = (rule, value, callback) => {
			if (this.form.FormStatus === 2 && !value) {
				callback(new Error('請輸入補件原因'));
			}
			callback();
		};
		return {
			visible: false,
			loading: false,
			form: {},
			district: Object.freeze([]),
			projectCode: Object.freeze([]),
			attachmentInfo: Object.freeze([]),
			activeTab: 'tab1',
			newPayment: null,
			rules: Object.freeze({
				PUB_COMP: [{ required: true, message: '請選擇案件類型', trigger: 'change' }],
				TOWN_NO: [{ required: true, message: '請選擇鄉鎮分類', trigger: 'change' }],
				CreateUserName: [{ required: true, message: '請輸入申請人姓名', trigger: 'blur' }],
				CreateUserEmail: [{ required: true, message: '請輸入申請人電子信箱', trigger: 'blur' }],
				COMP_NAM: [{ required: true, message: '請輸入工程名稱', trigger: 'blur' }],
				KIND_NO: [{ required: true, message: '請選擇工程類別', trigger: 'change' }],
				ADDR: [{ required: true, message: '請輸入工地地址或地號', trigger: 'blur' }],
				B_SERNO: [{ required: true, message: '請輸入建照字號或合約編號', trigger: 'blur' }],
				UTME: [{ required: true, message: '請輸入座標X', trigger: 'blur' }],
				UTMN: [{ required: true, message: '請輸入座標Y', trigger: 'blur' }],
				LATLNG: [{ required: true, message: '請輸入座標(緯度、經度)', trigger: 'blur' }],
				STATE: [{ required: true, message: '請輸入工程內容概述', trigger: 'blur' }],
				EIACOMMENTS: [{ required: true, message: '請輸入環評保護對策', trigger: 'blur' }],
				S_NAME: [{ required: true, message: '請輸入營建業主名稱', trigger: 'blur' }],
				S_G_NO: [{ required: true, message: '請輸入營利事業統一編號', trigger: 'blur' }],
				S_ADDR1: [{ required: true, message: '請輸入營利事業營業地址', trigger: 'blur' }],
				S_ADDR2: [{ required: true, message: '請輸入營利事業聯絡地址', trigger: 'blur' }],
				S_TEL: [{ required: true, message: '請輸入營利事業連絡電話', trigger: 'blur' }],
				S_B_NAM: [{ required: true, message: '請輸入營利事業負責人姓名', trigger: 'blur' }],
				S_B_TIT: [{ required: true, message: '請輸入營利事業負責人職稱', trigger: 'blur' }],
				S_B_ID: [{ required: true, message: '請輸入營利事業負責人身分證字號', trigger: 'blur' }],
				S_B_BDATE2: [{ required: true, message: '請輸入營利事業負責人生日', trigger: 'blur' }],
				S_C_NAM: [{ required: true, message: '請輸入營利事業聯絡人姓名', trigger: 'blur' }],
				S_C_TIT: [{ required: true, message: '請輸入營利事業聯絡人職稱', trigger: 'blur' }],
				S_C_ID: [{ required: true, message: '請輸入營利事業聯絡人身分證字號', trigger: 'blur' }],
				S_C_ADDR: [{ required: true, message: '請輸入營利事業聯絡人地址', trigger: 'blur' }],
				S_C_TEL: [{ required: true, message: '請輸入營利事業聯絡人電話', trigger: 'blur' }],
				R_NAME: [{ required: true, message: '請輸入承包(造)單位名稱', trigger: 'blur' }],
				R_G_NO: [{ required: true, message: '請輸入承包(造)營利事業統一編號', trigger: 'blur' }],
				R_ADDR1: [{ required: true, message: '請輸入承包(造)營業地址', trigger: 'blur' }],
				R_ADDR2: [{ required: true, message: '請輸入承包(造)聯絡地址', trigger: 'blur' }],
				R_TEL: [{ required: true, message: '請輸入承包(造)連絡電話', trigger: 'blur' }],
				R_B_NAM: [{ required: true, message: '請輸入承包(造)負責人姓名', trigger: 'blur' }],
				R_B_TIT: [{ required: true, message: '請輸入承包(造)負責人職稱', trigger: 'blur' }],
				R_B_ID: [{ required: true, message: '請輸入承包(造)負責人身分證字號', trigger: 'blur' }],
				R_B_BDATE2: [{ required: true, message: '請輸入承包(造)負責人生日', trigger: 'blur' }],
				R_ADDR3: [{ required: true, message: '請輸入工務所地址', trigger: 'blur' }],
				R_M_NAM: [{ required: true, message: '請輸入工地主任姓名', trigger: 'blur' }],
				R_C_NAM: [{ required: true, message: '請輸入工地環保負責人姓名', trigger: 'blur' }],
				R_TEL1: [{ required: true, message: '請輸入工務所電話', trigger: 'blur' }],
				MONEY: [{ required: true, message: '請輸入工程合約經費', trigger: 'blur' }],
				C_MONEY: [{ required: true, message: '請輸入工程環保經費', trigger: 'blur' }],
				PERCENT: [{ required: true, message: '請輸入工程合約經費比例', trigger: 'blur' }],
				AREA: [{ validator: checkArea }],
				VOLUMEL: [{ validator: checkVolumel }],
				B_DATE2: [{ required: true, message: '請輸入預計施工開始日期', trigger: 'blur' }],
				E_DATE2: [{ validator: checkE_DATE2 }],
				FailReason: [{ validator: checkFailReason }]
			})
		};
	},
	mounted() {
		this.getDistrict();
		this.getProjectCode();
		this.getAttachmentInfo();
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
		},
		totalPayment() {
			return this.form.Payments.reduce((prev, current) => {
				return prev + +current.Amount;
			}, 0);
		},
		filterAttachmentInfo() {
			return this.attachmentInfo.filter(item => item.PUB_COMP === this.form.PUB_COMP);
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
		getAttachmentInfo() {
			this.axios.get('api/Option/GetAttachmentInfo').then(res => {
				this.attachmentInfo = Object.freeze(res.data);
			});
		},
		filterAttachments(infoID) {
			return this.form.Attachments.filter(item => item.InfoID === infoID);
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
		addPayment() {
			if (!this.newPayment) {
				alert('請輸入收款金額');
				return;
			}
			this.form.Payments.push({
				Amount: this.newPayment,
				CreateDate: moment().format('YYYY-MM-DD')
			});

			this.newPayment = null;
		},
		deletePayment(idx) {
			if (!confirm('是否確認刪除?')) return false;
			this.form.Payments.splice(idx, 1);
		},
		saveForm() {
			this.$refs.form.validate(valid => {
				if (!valid) {
					alert('欄位驗證錯誤，請檢查修正後重新送出');
					return false;
				}

				const isStopWorksError = this.form.StopWorks.some(item => !item.DOWN_DATE2 || !item.UP_DATE2);
				if (isStopWorksError) {
					alert('停復工日期未輸入完整，請檢查修正後重新送出');
					return false;
				}

				if (!confirm('是否確認繼續?')) return false;
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
			});
		},
		finalCalc(key) {
			this.$refs.form.validate(valid => {
				if (!valid) {
					alert('欄位驗證錯誤，請檢查修正後重新送出');
					return false;
				}

				this.axios
					.post('api/Form/GetFinalCalc', this.form)
					.then(res => {
						this.form[key] = res.data;
						this.$message.success('已更新應繳金額');
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
.modal-form {
	.el-form-item__label {
		font-weight: 700;
	}
	.el-form-item__error {
		line-height: 4px;
	}
	.form-item-inline {
		display: flex;
		align-items: center;
		.form-item-col {
			margin: 0 8px;
		}
		.el-form-item__label,
		.el-form-item__content {
			display: inline-block;
		}
		.el-form-item {
			margin-bottom: 0;
		}
	}
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
	th,
	td {
		text-align: center;
	}
}
.modal-header {
	margin: 10px 0;
}
.file-list {
	list-style-type: none;
}
</style>