<template>
	<vxe-modal title="申請案件明細" v-model="visible" width="80%" height="90%" :lock-scroll="false" esc-closable resize show-footer>
		<template #default>
			<el-form inline>
				<el-form-item label="管制編號">{{C_NO}}</el-form-item>
				<el-form-item label="申報應繳金額">{{(form.S_AMT === null ? '未結算' : form.S_AMT) | comma}}</el-form-item>
				<el-form-item label="結算應繳金額">{{(form.S_AMT2 === null ? '未結算' : form.S_AMT2) | comma}}</el-form-item>
			</el-form>
			<el-tabs v-model="activeTab">
				<el-tab-pane label="基本資料" name="1">
					<el-form ref="tab1Form" :rules="tab1Rules" :model="form" label-width="auto">
						<el-form-item label="管制編號">{{C_NO}}</el-form-item>
						<el-form-item prop="TOWN_NO" label="鄉鎮分類">
							<el-select v-model="form.TOWN_NO">
								<el-option label="請選擇" :value="undefined"></el-option>
								<el-option v-for="item in district" :key="item.Code" :label="item.Name" :value="item.Code"></el-option>
							</el-select>
						</el-form-item>
						<el-form-item label="申請日期">{{form.C_DATE | date}}</el-form-item>
						<el-form-item prop="CreateUserName" label="申請人">
							<el-input v-model="form.CreateUserName" maxlength="20"></el-input>
						</el-form-item>
						<el-form-item prop="CreateUserEmail" label="申請人電子信箱">
							<el-input type="email" v-model="form.CreateUserEmail" maxlength="50"></el-input>
						</el-form-item>
						<el-form-item prop="COMP_NAM" label="工程名稱">
							<el-input v-model="form.COMP_NAM" maxlength="150"></el-input>
						</el-form-item>
						<el-form-item prop="KIND_NO" label="工程類別">
							<el-select class="w100p" v-model="form.KIND_NO">
								<el-option label="請選擇" :value="null"></el-option>
								<el-option v-for="item in projectCode" :key="item.ID" :label="`${item.ID}. ${item.Name}`" :value="item.ID"></el-option>
							</el-select>
						</el-form-item>
						<el-form-item prop="ADDR" label="工地地址或地號">
							<el-input v-model="form.ADDR" maxlength="100"></el-input>
						</el-form-item>
						<el-form-item prop="B_SERNO" label="建照字號或合約編號">
							<el-input v-model="form.B_SERNO" maxlength="60"></el-input>
						</el-form-item>
						<div class="flex-row">
							<el-form-item prop="LAT" label="座標(緯度)">
								<el-input type="number" v-model="form.LAT" maxlength="20"></el-input>
							</el-form-item>
							<el-form-item prop="LNG" label="座標(經度)">
								<el-input type="number" v-model="form.LNG" maxlength="20"></el-input>
							</el-form-item>
						</div>
						<div class="flex-row">
							<el-form-item prop="UTME" label="座標X">
								<el-input type="number" v-model="form.UTME" disabled></el-input>
							</el-form-item>
							<el-form-item prop="UTMN" label="座標Y">
								<el-input type="number" v-model="form.UTMN" disabled></el-input>
							</el-form-item>
						</div>
						<el-form-item prop="STATE" label="工程內容概述">
							<el-input v-model="form.STATE" maxlength="200"></el-input>
						</el-form-item>
						<el-form-item prop="EIACOMMENTS" label="環評保護對策">
							<el-input v-model="form.EIACOMMENTS"></el-input>
						</el-form-item>
						<el-form-item prop="RECCOMMENTS" label="備註">
							<el-input v-model="form.RECCOMMENTS"></el-input>
						</el-form-item>
					</el-form>
				</el-tab-pane>
				<el-tab-pane label="營建資料" name="2">
					<el-form ref="tab2Form" :rules="tab2Rules" :model="form" label-width="auto">
						<div class="contact-group">
							<div class="flex-row">
								<el-form-item prop="S_NAME" label="營建業主名稱">
									<el-input v-model="form.S_NAME" maxlength="80"></el-input>
								</el-form-item>
								<el-form-item prop="S_G_NO" label="營利事業統一編號">
									<el-input v-model="form.S_G_NO" maxlength="10"></el-input>
								</el-form-item>
								<el-form-item prop="S_TEL" label="營建業主電話">
									<el-input type="tel" v-model="form.S_TEL" maxlength="30"></el-input>
								</el-form-item>
							</div>
							<el-form-item prop="S_ADDR1" label="營業地址">
								<el-input v-model="form.S_ADDR1" maxlength="50"></el-input>
							</el-form-item>
							<el-form-item prop="S_ADDR2" label="聯絡地址">
								<el-input v-model="form.S_ADDR2" maxlength="50"></el-input>
							</el-form-item>
						</div>
						<div class="contact-group">
							<div class="flex-row">
								<el-form-item prop="S_B_NAM" label="負責人姓名">
									<el-input v-model="form.S_B_NAM" maxlength="20"></el-input>
								</el-form-item>
								<el-form-item prop="S_B_TIT" label="職稱">
									<el-input v-model="form.S_B_TIT" maxlength="20"></el-input>
								</el-form-item>
							</div>
							<div class="flex-row">
								<el-form-item prop="S_B_ID" label="負責人身分證字號">
									<el-input v-model="form.S_B_ID" maxlength="20"></el-input>
								</el-form-item>
								<el-form-item prop="S_B_BDATE" label="負責人生日">
									<div class="el-input">
										<input type="text" class="el-input__inner datepicker" data-key="S_B_BDATE" v-model="form.S_B_BDATE" readonly />
									</div>
								</el-form-item>
							</div>
						</div>
						<div class="contact-group">
							<div class="flex-row">
								<el-form-item prop="S_C_NAM" label="聯絡人姓名">
									<el-input v-model="form.S_C_NAM" maxlength="20"></el-input>
								</el-form-item>
								<el-form-item prop="S_C_TIT" label="職稱">
									<el-input v-model="form.S_C_TIT" maxlength="20"></el-input>
								</el-form-item>
							</div>
							<div class="flex-row">
								<el-form-item prop="S_C_ID" label="聯絡人身分證字號">
									<el-input v-model="form.S_C_ID" maxlength="20"></el-input>
								</el-form-item>
								<el-form-item prop="S_C_TEL" label="電話">
									<el-input type="tel" v-model="form.S_C_TEL" maxlength="30"></el-input>
								</el-form-item>
							</div>
							<el-form-item prop="S_C_ADDR" label="聯絡人地址">
								<el-input v-model="form.S_C_ADDR" maxlength="50"></el-input>
							</el-form-item>
						</div>
					</el-form>
				</el-tab-pane>
				<el-tab-pane label="承造資料" name="3">
					<el-form ref="tab3Form" :rules="tab3Rules" :model="form" label-width="auto">
						<div class="contact-group">
							<div class="flex-row">
								<el-form-item prop="R_NAME" label="承造單位名稱">
									<el-input v-model="form.R_NAME" maxlength="60"></el-input>
								</el-form-item>
								<el-form-item prop="R_G_NO" label="營利事業統一編號">
									<el-input v-model="form.R_G_NO" maxlength="10"></el-input>
								</el-form-item>
								<el-form-item prop="R_TEL" label="承造單位電話">
									<el-input type="tel" v-model="form.R_TEL" maxlength="30"></el-input>
								</el-form-item>
							</div>
							<el-form-item prop="R_ADDR1" label="營業地址">
								<el-input v-model="form.R_ADDR1" maxlength="50"></el-input>
							</el-form-item>
							<el-form-item prop="R_ADDR2" label="聯絡地址">
								<el-input v-model="form.R_ADDR2" maxlength="50"></el-input>
							</el-form-item>
						</div>
						<div class="contact-group">
							<div class="flex-row">
								<el-form-item prop="R_B_NAM" label="負責人姓名">
									<el-input v-model="form.R_B_NAM" maxlength="20"></el-input>
								</el-form-item>
								<el-form-item prop="R_B_TIT" label="職稱">
									<el-input v-model="form.R_B_TIT" maxlength="20"></el-input>
								</el-form-item>
							</div>
							<div class="flex-row">
								<el-form-item prop="R_B_ID" label="身分證字號">
									<el-input v-model="form.R_B_ID" maxlength="30"></el-input>
								</el-form-item>
								<el-form-item prop="R_B_BDATE" label="負責人生日">
									<div class="el-input">
										<input type="text" class="el-input__inner datepicker" data-key="R_B_BDATE" v-model="form.R_B_BDATE" readonly />
									</div>
								</el-form-item>
							</div>
						</div>
						<div class="contact-group">
							<div class="flex-row">
								<el-form-item prop="R_M_NAM" label="工地主任姓名">
									<el-input v-model="form.R_M_NAM" maxlength="10"></el-input>
								</el-form-item>
								<el-form-item prop="R_C_NAM" label="工地環保負責人姓名">
									<el-input v-model="form.R_C_NAM" maxlength="10"></el-input>
								</el-form-item>
								<el-form-item prop="R_TEL1" label="電話">
									<el-input type="tel" v-model="form.R_TEL1" maxlength="30"></el-input>
								</el-form-item>
							</div>
							<el-form-item prop="R_ADDR3" label="工務所地址">
								<el-input v-model="form.R_ADDR3" maxlength="50"></el-input>
							</el-form-item>
						</div>
					</el-form>
				</el-tab-pane>
				<el-tab-pane label="經費資料" name="4">
					<el-form ref="tab4Form" :rules="tab4Rules" :model="form" label-width="auto">
						<div class="flex-row">
							<el-form-item prop="MONEY" label="工程合約經費(元)">
								<el-input type="number" v-model="form.MONEY"></el-input>
							</el-form-item>
							<el-form-item prop="C_MONEY" label="工程環保經費(元)">
								<div style="min-width:120px">{{calcC_MONEY | comma}}</div>
							</el-form-item>
							<el-form-item prop="PERCENT" label="工程合約經費比例(%)">{{form.PERCENT}}</el-form-item>
						</div>
						<div v-if="form.KIND_NO === '1' || form.KIND_NO === '2'" class="flex-row">
							<el-form-item prop="AREA_F" label="基地面積">
								<el-input type="number" v-model="form.AREA_F"></el-input>
							</el-form-item>
							<el-form-item prop="AREA_B" label="建築面積">
								<el-input type="number" v-model="form.AREA_B"></el-input>
							</el-form-item>
							<el-form-item prop="PERC_B" label="建蔽率">{{calcPERC_B}}%</el-form-item>
						</div>
						<div v-else>
							<el-form-item prop="AREA" label="工程面積">
								<el-input type="number" v-model="form.AREA"></el-input>
							</el-form-item>
						</div>
						<div v-if="form.KIND_NO === 'B'">
							<div style="margin-bottom:10px">鬆方體積換算表</div>
							<div class="contact-group">
								<div class="flex-row" style="justify-content: flex-start; align-items: flex-end">
									<el-form-item label="鬆方重量(公噸)">
										<el-input type="number" v-model="form.E2"></el-input>
									</el-form-item>
									<el-form-item label="密度">
										<el-input type="number" v-model="form.DENSITYL"></el-input>
									</el-form-item>
									<el-form-item>
										<el-button type="primary" round @click="calcE2">換算</el-button>
									</el-form-item>
								</div>
							</div>
							<div class="hint-message">
								<i class="el-icon-info"></i> 鬆方體積除以實方體積之比值以一‧三一計，鬆方之密度以一‧五一公噸/立方公尺計。營建業主如有現地取樣之實方與鬆方試驗相關數據，得報請地方主管機關同意後，依該數據採計。
							</div>
							<el-form-item prop="VOLUMEL" label="外運土石體積(立方公尺)">
								<el-input type="number" v-model="form.VOLUMEL"></el-input>
							</el-form-item>
						</div>
						<div class="flex-row">
							<el-form-item prop="B_DATE" label="開始日期">
								<div class="el-input">
									<input type="text" class="el-input__inner datepicker" data-key="B_DATE" v-model="form.B_DATE" readonly />
								</div>
							</el-form-item>
							<el-form-item prop="E_DATE" label="結束日期">
								<div class="el-input">
									<input type="text" class="el-input__inner datepicker" data-key="E_DATE" v-model="form.E_DATE" readonly />
								</div>
							</el-form-item>
							<el-form-item label="預計施工天數">{{totalDays}}</el-form-item>
						</div>
						<el-form-item label="規定繳費方式">
							<el-select v-model="form.P_KIND">
								<el-option label="一次全繳" value="一次全繳"></el-option>
								<el-option label="分兩次繳清" value="分兩次繳清"></el-option>
							</el-select>
						</el-form-item>
						<el-form-item label="空汙防制措施計畫書">
							<el-select v-model="form.REC_YN">
								<el-option label="有" value="有"></el-option>
								<el-option label="無" value="無"></el-option>
							</el-select>
						</el-form-item>
					</el-form>
				</el-tab-pane>
				<el-tab-pane label="檢附資料" name="5">
					<div class="attach-row">
						<div>
							<div class="title">首期申報附件</div>
							<a v-if="form.FileName1" :href="`api/Form/Download?f=${form.FileName1}&n=${form.DisplayName1}`" class="link-download">{{form.DisplayName1}}</a>
							<a v-else href="javascript:;" class="link-download">暫無上傳檔案</a>
						</div>
						<div>
							<div class="title">結算申報附件</div>
							<a v-if="form.FileName2" :href="`api/Form/Download?f=${form.FileName2}&n=${form.DisplayName2}`" class="link-download">{{form.DisplayName2}}</a>
							<a v-else href="javascript:;" class="link-download">暫無上傳檔案</a>
						</div>
					</div>
				</el-tab-pane>
				<el-tab-pane label="停復工" name="6">
					<el-button type="primary" icon="el-icon-plus" @click="addStopWork()">新 增</el-button>
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
								<tr v-if="form.StopWorks.length === 0">
									<td colspan="4">暫無資料</td>
								</tr>
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
				<el-tab-pane v-if="form.RefundBank.ID" label="退款帳戶" name="7">
					<el-form-item label="銀行代碼">{{form.RefundBank.Code}}</el-form-item>
					<el-form-item label="銀行帳號">{{form.RefundBank.Account}}</el-form-item>
					<el-form-item label="存摺照片">
						<img style="width:640px" :src="`api/Option/Download?f=${form.RefundBank.Photo}`" />
					</el-form-item>
				</el-tab-pane>
				<!-- <el-tab-pane v-if="form.PaymentProof.ID" label="繳費證明" name="8">
					<el-form-item label="繳費證明">
						<img style="width:640px" :src="`api/Option/Download?f=${form.PaymentProof.ProofFile}`" />
					</el-form-item>
				</el-tab-pane>-->
			</el-tabs>
		</template>
		<template #footer>
			<el-button @click="visible = false">
				<i class="fa fa-ban"></i> 取 消
			</el-button>
			<el-button type="primary" icon="el-icon-arrow-left" :disabled="activeTab === '1'" @click="goPrevTab">上一步</el-button>
			<el-button type="primary" @click="goNextTab">
				{{activeTab === '6' ? (mode === 'Update' ? '儲存' : '複製') : '下一步'}}
				<i class="el-icon-arrow-right el-icon--right"></i>
			</el-button>
		</template>
	</vxe-modal>
</template>
<script>
import { mapGetters } from 'vuex';
import { dateTime, comma, form } from '@/mixins/filter';
$.datepicker.regional['zh-TW'] = {
	closeText: '關閉',
	prevText: '&#x3C;上月',
	nextText: '下月&#x3E;',
	currentText: '今天',
	monthNames: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
	monthNamesShort: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
	dayNames: ['星期日', '星期一', '星期二', '星期三', '星期四', '星期五', '星期六'],
	dayNamesShort: ['周日', '周一', '周二', '周三', '周四', '周五', '周六'],
	dayNamesMin: ['日', '一', '二', '三', '四', '五', '六'],
	weekHeader: '周',
	dateFormat: 'yy/mm/dd',
	firstDay: 1,
	isRTL: false,
	showMonthAfterYear: true,
	yearSuffix: '年'
};
$.datepicker.setDefaults($.datepicker.regional['zh-TW']);
$.datepicker._phoenixGenerateMonthYearHeader = $.datepicker._generateMonthYearHeader;
$.datepicker._generateMonthYearHeader = function (inst, drawMonth, drawYear, minDate, maxDate, secondary, monthNames, monthNamesShort) {
	var result = $($.datepicker._phoenixGenerateMonthYearHeader(inst, drawMonth, drawYear, minDate, maxDate, secondary, monthNames, monthNamesShort));
	result
		.find('select.ui-datepicker-year')
		.children()
		.each(function () {
			$(this).text($(this).text() - 1911 + '年');
		});

	return result.html();
};
export default {
	name: 'FormModal',
	props: ['show', 'mode', 'data'],
	mixins: [dateTime, comma, form],
	data() {
		const checkE_DATE = (rule, value, callback) => {
			if (!value) {
				callback(new Error('請輸入結束日期'));
			}

			if (!this.form.B_DATE) {
				callback();
			}

			const startDate = `${this.form.B_DATE.substr(0, 3) + 1911}-${this.form.B_DATE.substr(3, 2)}-${this.form.B_DATE.substr(5, 2)}`;
			const endDate = `${this.form.E_DATE.substr(0, 3) + 1911}-${this.form.E_DATE.substr(3, 2)}-${this.form.E_DATE.substr(5, 2)}`;
			if (moment(startDate).isAfter(endDate)) {
				callback(new Error('施工期程起始日期不能大於結束日期'));
			}
			callback();
		};
		const checkArea = (rule, value, callback) => {
			const kindAry = ['1', '2', '4', '5', '6', '7', '8', '9', 'A', 'Z'];
			if (kindAry.includes(this.form.KIND_NO) && !value) {
				callback(new Error('請輸入工程面積'));
			}
			callback();
		};
		const checkAreaF = (rule, value, callback) => {
			const kinds = ['1', '2'];
			if (kinds.includes(this.form.KIND_NO) && !value) {
				callback(new Error('請輸入基地面積'));
			}
			callback();
		};
		const checkVolumel = (rule, value, callback) => {
			if (this.form.KIND_NO === '3' && !value) {
				callback(new Error('請輸入總樓地板面積'));
			}
			if (this.form.KIND_NO === 'B' && !value) {
				callback(new Error('請輸入外運土石體積'));
			}
			callback();
		};
		const checkLATLNG = (rule, value, callback) => {
			if (isNaN(value)) {
				callback(new Error('經緯度格式錯誤'));
			}
			callback();
		};
		const checkS_B_BDATE = (rule, value, callback) => {
			if (!this.PUB_COMP && !value) {
				callback(new Error('請輸入營利事業負責人生日'));
			}
			callback();
		};
		const checkS_C_ID = (rule, value, callback) => {
			if (!this.PUB_COMP && !value) {
				callback(new Error('請輸入營利事業聯絡人身分證字號'));
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
			activeTab: '1',
			tab1Rules: Object.freeze({
				PUB_COMP: [{ required: true, message: '請選擇案件類型', trigger: 'change' }],
				TOWN_NO: [{ required: true, message: '請選擇鄉鎮分類', trigger: 'change' }],
				CreateUserName: [{ required: true, message: '請輸入申請人姓名', trigger: 'blur' }],
				CreateUserEmail: [{ required: true, message: '請輸入申請人電子信箱', trigger: 'blur' }],
				COMP_NAM: [{ required: true, message: '請輸入工程名稱', trigger: 'blur' }],
				KIND_NO: [{ required: true, message: '請選擇工程類別', trigger: 'change' }],
				ADDR: [{ required: true, message: '請輸入工地地址或地號', trigger: 'blur' }],
				B_SERNO: [{ required: true, message: '請輸入建照字號或合約編號', trigger: 'blur' }],
				LAT: [
					{ required: true, message: '請輸入座標(緯度)', trigger: 'blur' },
					{ validator: checkLATLNG, trigger: 'blur' }
				],
				LNG: [
					{ required: true, message: '請輸入座標(經度)', trigger: 'blur' },
					{ validator: checkLATLNG, trigger: 'blur' }
				],
				STATE: [{ required: true, message: '請輸入工程內容概述', trigger: 'blur' }]
			}),
			tab2Rules: Object.freeze({
				S_NAME: [{ required: true, message: '請輸入營建業主名稱', trigger: 'blur' }],
				S_G_NO: [{ required: true, message: '請輸入營利事業統一編號', trigger: 'blur' }],
				S_ADDR1: [{ required: true, message: '請輸入營利事業營業地址', trigger: 'blur' }],
				S_ADDR2: [{ required: true, message: '請輸入營利事業聯絡地址', trigger: 'blur' }],
				S_TEL: [{ required: true, message: '請輸入營利事業主連絡電話', trigger: 'blur' }],
				S_B_NAM: [{ required: true, message: '請輸入營利事業負責人姓名', trigger: 'blur' }],
				S_B_TIT: [{ required: true, message: '請輸入營利事業負責人職稱', trigger: 'blur' }],
				S_B_ID: [{ required: true, message: '請輸入營利事業負責人身分證字號', trigger: 'blur' }],
				S_B_BDATE: [{ validator: checkS_B_BDATE, trigger: 'blur' }],
				S_C_NAM: [{ required: true, message: '請輸入營利事業聯絡人姓名', trigger: 'blur' }],
				S_C_TIT: [{ required: true, message: '請輸入營利事業聯絡人職稱', trigger: 'blur' }],
				S_C_ID: [{ validator: checkS_C_ID, trigger: 'blur' }],
				S_C_ADDR: [{ required: true, message: '請輸入營利事業聯絡人地址', trigger: 'blur' }],
				S_C_TEL: [{ required: true, message: '請輸入營利事業聯絡人電話', trigger: 'blur' }]
			}),
			tab3Rules: Object.freeze({
				R_NAME: [{ required: true, message: '請輸入承造單位名稱', trigger: 'blur' }],
				R_G_NO: [{ required: true, message: '請輸入承造營利事業統一編號', trigger: 'blur' }],
				R_ADDR1: [{ required: true, message: '請輸入承造營業地址', trigger: 'blur' }],
				R_ADDR2: [{ required: true, message: '請輸入承造聯絡地址', trigger: 'blur' }],
				R_TEL: [{ required: true, message: '請輸入承造連絡電話', trigger: 'blur' }],
				R_B_NAM: [{ required: true, message: '請輸入承造負責人姓名', trigger: 'blur' }],
				R_B_TIT: [{ required: true, message: '請輸入承造負責人職稱', trigger: 'blur' }],
				R_B_ID: [{ required: true, message: '請輸入承造負責人身分證字號', trigger: 'blur' }],
				R_B_BDATE2: [{ required: true, message: '請輸入承造負責人生日', trigger: 'blur' }],
				R_ADDR3: [{ required: true, message: '請輸入工務所地址', trigger: 'blur' }],
				R_M_NAM: [{ required: true, message: '請輸入工地主任姓名', trigger: 'blur' }],
				R_C_NAM: [{ required: true, message: '請輸入工地環保負責人姓名', trigger: 'blur' }],
				R_TEL1: [{ required: true, message: '請輸入工務所電話', trigger: 'blur' }]
			}),
			tab4Rules: Object.freeze({
				MONEY: [{ required: true, message: '請輸入工程合約經費', trigger: 'blur' }],
				C_MONEY: [{ required: true, message: '請輸入工程環保經費', trigger: 'blur' }],
				AREA: [{ validator: checkArea, trigger: 'blur' }],
				AREA_F: [{ validator: checkAreaF, trigger: 'blur' }],
				VOLUMEL: [{ validator: checkVolumel, trigger: 'blur' }],
				B_DATE: [{ required: true, message: '請輸入開始日期', trigger: 'blur' }],
				E_DATE: [{ validator: checkE_DATE, trigger: 'blur' }]
			}),
			rules2: Object.freeze({
				Code: [{ required: true, message: '請選擇銀行代碼', trigger: 'change' }],
				Account: [{ required: true, message: '請輸入銀行帳號', trigger: 'blur' }],
				File: [{ required: true, message: '請上傳存摺照片' }]
			}),
			rules3: Object.freeze({
				File: [{ required: true, message: '請上傳繳費證明' }]
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
		C_NO() {
			if (!this.form.C_NO || !this.form.SER_NO) return '待取號';
			return `${this.form.C_NO}-${this.form.SER_NO}`;
		},
		totalDays() {
			if (!this.form.B_DATE || !this.form.E_DATE) return '';
			const startDate = `${+this.form.B_DATE.substr(0, 3) + 1911}-${this.form.B_DATE.substr(3, 2)}-${this.form.B_DATE.substr(5, 2)}`;
			const endDate = `${+this.form.E_DATE.substr(0, 3) + 1911}-${this.form.E_DATE.substr(3, 2)}-${this.form.E_DATE.substr(5, 2)}`;
			var date1 = new Date(startDate);
			var date2 = new Date(endDate);

			// 計算毫秒差異
			var diff = Math.abs(date2 - date1 + 1000 * 60 * 60 * 24);
			// 轉換為天數
			var dayDiff = Math.ceil(diff / (1000 * 60 * 60 * 24));

			return dayDiff;
		},
		filterAttachmentInfo() {
			return this.attachmentInfo.filter(item => item.PUB_COMP === this.form.PUB_COMP);
		},
		calcC_MONEY() {
			try {
				if (!this.form.MONEY) throw '';
				return +((this.form.MONEY * this.form.PERCENT) / 100).toFixed(0);
			} catch (err) {
				return 0;
			}
		},
		calcPERC_B() {
			if (!this.form.AREA_B || !this.form.AREA_F) return 0;
			return ((+this.form.AREA_B / +this.form.AREA_F) * 100).toFixed(2);
		}
	},
	methods: {
		initDatePicker() {
			$('.datepicker').datepicker({
				dateFormat: 'yy/mm/dd',
				yearRange: '-90:+10',
				changeYear: true,
				changeMonth: true,
				beforeShow: function (input, inst) {
					const inputVal = input.value;
					if (inputVal) {
						const year = +inputVal.substr(0, 3) + 1911;
						const month = inputVal.substr(3, 2);
						const day = inputVal.substr(5, 2);
						return {
							defaultDate: `${year}/${month}/${day}`
						};
					}

					return {};
				},
				onSelect: (dateText, inst) => {
					var objDate = {
						y: `${inst.selectedYear - 1911 < 0 ? inst.selectedYear : inst.selectedYear - 1911}`.padStart(3, '0'),
						m: `${inst.selectedMonth + 1}`.padStart(2, '0'),
						d: `${inst.selectedDay}`.padStart(2, '0')
					};

					var dateFormate = `${objDate.y}${objDate.m}${objDate.d}`;
					inst.input.val(dateFormate);
					this.form[inst.input[0].dataset.key] = dateFormate;
				}
			});
		},
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
		isShowAREA() {
			const kindAry = ['1', '2', '4', '5', '6', '7', '8', '9', 'A', 'Z'];
			if (kindAry.includes(this.form.KIND_NO)) {
				return true;
			}
			return false;
		},
		isShowMONEY() {
			const kindAry = ['Z'];
			if (kindAry.includes(this.form.KIND_NO)) {
				return true;
			}
			return false;
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
		},
		goPrevTab() {
			let intActiveTab = +this.activeTab;
			if (intActiveTab > 1) {
				intActiveTab -= 1;
				this.activeTab = intActiveTab.toString();
			}
		},
		goNextTab() {
			// 自動設定公共工程4%、私人工程3%
			if (this.activeTab === '2') {
				if (this.form.PUB_COMP) {
					this.form.PERCENT = 4;
				} else {
					this.form.PERCENT = 3;
				}
			}

			switch (this.activeTab) {
				case '1':
				case '2':
				case '3':
				case '4': {
					this.$refs[`tab${this.activeTab}Form`].validate((valid, obj) => {
						if (!valid) {
							const firstKey = Object.keys(obj)[0];
							alert(obj[firstKey][0].message);
							return false;
						}

						this.activeTab = (+this.activeTab + 1).toString();
					});

					break;
				}
				case '5': {
					this.activeTab = (+this.activeTab + 1).toString();
					break;
				}
				case '6': {
					if (!confirm('是否確認繼續?')) return false;
					this.form.C_MONEY = this.calcC_MONEY;
					// RC或SRC需填寫建築面積&基地面積，AREA跟AREA_B都是建築面積
					if (this.form.KIND_NO === '1' || this.form.KIND_NO === '2') {
						this.form.AREA_B = this.form.AREA;
					}
					this.axios
						.post(`api/Form/${this.mode}Form`, this.form)
						.then(res => {
							this.$emit('on-updated');
							this.$message.success('畫面資料已儲存');
							this.visible = false;
						})
						.catch(err => {
							this.$message.error(err.response.data.ExceptionMessage);
						});
					break;
				}
			}
		},
		calcD2() {
			this.form.VOLUMEL = this.form.D2 * this.form.RATIOLB;
		},
		calcE2() {
			this.form.VOLUMEL = this.form.E2 / this.form.DENSITYL;
		}
	},
	watch: {
		show: {
			handler(newValue, oldValue) {
				this.visible = newValue;
				if (this.visible) {
					this.form = JSON.parse(JSON.stringify(this.data));
					this.activeTab = '1';
					const point = this.form.LATLNG.split(',');
					this.form = {
						...this.form,
						...{
							LAT: point[0] || null,
							LNG: point[1] || null,
							D2: null,
							E2: null
						}
					};

					this.$nextTick(() => {
						this.initDatePicker();
					});
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
.contact-group {
	border: 2px dashed #ccc;
	padding: 10px 10px 10px 0;
	margin-bottom: 10px;
}

.flex-row {
	.el-form-item {
		& ~ .el-form-item {
			margin-left: 10px;
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
.attach-row {
	display: flex;
	flex-wrap: wrap;
	align-items: flex-start;
	& > div {
		padding: 10px;
	}
	.title {
		font-size: 22px;
		font-weight: 700;
		color: #1f2f3d;
		margin-bottom: 10px;
	}
}
.link-download {
	display: block;
	margin: 20px 0;
	font-size: 18px;
	font-weight: 700;
	text-align: center;
	color: #0f67b1;
	border: 1px dashed #0f67b1;
	padding: 6px 0;
}
.hint-message {
	font-size: 14px;
	color: #2a629a;
	margin-bottom: 20px;
}
</style>