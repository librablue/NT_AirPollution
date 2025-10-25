<template>
	<vxe-modal title="申請案件明細" v-model="visible" width="80%" height="90%" :lock-scroll="false" esc-closable resize :show-footer="mode !== 'Read'">
		<template #default>
			<el-form inline>
				<el-form-item label="管制編號">{{C_NO}}</el-form-item>
				<el-form-item label="申報應繳金額">{{(form.S_AMT === null ? '未結算' : form.S_AMT) | comma}}</el-form-item>
				<el-form-item label="結算應繳金額">{{(form.S_AMT2 === null ? '未結算' : form.S_AMT2) | comma}}</el-form-item>
			</el-form>
			<el-tabs v-model="activeTab">
				<el-tab-pane label="工地基本資料" name="1">
					<el-form ref="tab1Form" :rules="tab1Rules" :model="form" label-width="auto">
						<el-form-item label="管制編號">{{C_NO}}</el-form-item>
						<el-form-item prop="TOWN_NO" label="鄉鎮分類">
							<el-select v-model="form.TOWN_NO">
								<el-option label="請選擇" :value="undefined"></el-option>
								<el-option v-for="item in district" :key="item.Code" :label="item.Name" :value="item.Code"></el-option>
							</el-select>
						</el-form-item>
						<el-form-item label="申報日期">{{form.AP_DATE | westDate}}</el-form-item>
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
						<div class="flex-row">
							<el-form-item prop="LAT" label="座標(緯度)">
								<el-input type="number" v-model="form.LAT" maxlength="20"></el-input>
							</el-form-item>
							<el-form-item prop="LNG" label="座標(經度)">
								<el-input type="number" v-model="form.LNG" maxlength="20"></el-input>
							</el-form-item>
						</div>
						<el-form-item prop="B_SERNO" label="建照字號或合約編號">
							<el-input v-model="form.B_SERNO" maxlength="60"></el-input>
						</el-form-item>
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
				<el-tab-pane label="營建業主基本資料" name="2">
					<el-form ref="tab2Form" :rules="tab2Rules" :model="form" label-width="auto" :disabled="form.FormStatus >= 3" class="beauty-form">
						<!-- 營建業主資料 -->
						<div class="contact-group">
							<h3 class="group-title">營建業主資料</h3>
							<el-row :gutter="20">
								<el-col :md="8" :sm="24">
									<el-form-item prop="S_NAME" label="營建業主名稱">
										<el-input v-model="form.S_NAME" maxlength="80"></el-input>
									</el-form-item>
								</el-col>
								<el-col :md="8" :sm="24">
									<el-form-item prop="S_G_NO" label="營利事業統一編號">
										<el-input v-model="form.S_G_NO" maxlength="8"></el-input>
									</el-form-item>
								</el-col>
								<el-col :md="8" :sm="24">
									<el-form-item prop="S_TEL" label="營建業主電話">
										<el-input type="tel" v-model="form.S_TEL" maxlength="30"></el-input>
									</el-form-item>
								</el-col>
								<el-col :span="24">
									<el-form-item prop="S_ADDR1" label="營業地址">
										<el-input v-model="form.S_ADDR1" maxlength="50"></el-input>
									</el-form-item>
								</el-col>
								<el-col :span="24">
									<el-form-item prop="S_ADDR2" label="聯絡地址">
										<el-input v-model="form.S_ADDR2" maxlength="50"></el-input>
									</el-form-item>
								</el-col>
							</el-row>
						</div>

						<!-- 負責人資料 -->
						<div class="contact-group">
							<h3 class="group-title">負責人資料</h3>
							<el-row :gutter="20">
								<el-col :md="12" :sm="24">
									<el-form-item prop="S_B_NAM" label="負責人姓名">
										<el-input v-model="form.S_B_NAM" maxlength="20"></el-input>
									</el-form-item>
								</el-col>
								<el-col :md="12" :sm="24">
									<el-form-item prop="S_B_TIT" label="職稱">
										<el-input v-model="form.S_B_TIT" maxlength="20"></el-input>
									</el-form-item>
								</el-col>

								<el-col :md="12" :sm="24">
									<el-form-item prop="S_B_ID" label="身分證字號" :rules="S_B_IDRules">
										<el-input v-model="form.S_B_ID" maxlength="20"></el-input>
									</el-form-item>
								</el-col>
								<el-col :md="12" :sm="24">
									<el-form-item prop="S_B_BDATE" label="生日">
										<div class="el-input el-input--suffix">
											<input type="text" class="el-input__inner datepicker" data-key="S_B_BDATE" v-model="form.S_B_BDATE" :disabled="form.FormStatus > 2" readonly />
											<span v-if="form.S_B_BDATE && form.FormStatus <= 2" class="el-input__suffix datepicker-suffix" @click="form.S_B_BDATE = ''">
												<i class="fa fa-times-circle"></i>
											</span>
										</div>
									</el-form-item>
								</el-col>
							</el-row>
						</div>

						<!-- 聯絡人資料 -->
						<div class="contact-group">
							<h3 class="group-title">聯絡人資料</h3>
							<el-row :gutter="20">
								<el-col :md="12" :sm="24">
									<el-form-item prop="S_C_NAM" label="聯絡人姓名">
										<el-input v-model="form.S_C_NAM" maxlength="20"></el-input>
									</el-form-item>
								</el-col>
								<el-col :md="12" :sm="24">
									<el-form-item prop="S_C_TIT" label="職稱">
										<el-input v-model="form.S_C_TIT" maxlength="20"></el-input>
									</el-form-item>
								</el-col>

								<el-col :md="12" :sm="24">
									<el-form-item prop="S_C_ID" label="身分證字號" :rules="S_C_IDRules">
										<el-input v-model="form.S_C_ID" maxlength="20"></el-input>
									</el-form-item>
								</el-col>
								<el-col :md="12" :sm="24">
									<el-form-item prop="S_C_TEL" label="電話">
										<el-input type="tel" v-model="form.S_C_TEL" maxlength="30"></el-input>
									</el-form-item>
								</el-col>

								<el-col :span="24">
									<el-form-item prop="S_C_ADDR" label="聯絡人地址">
										<el-input v-model="form.S_C_ADDR" maxlength="50"></el-input>
									</el-form-item>
								</el-col>
							</el-row>
						</div>
					</el-form>
				</el-tab-pane>
				<el-tab-pane label="承(包)造單位基本資料" name="3">
					<el-form ref="tab3Form" :rules="tab3Rules" :model="form" label-width="auto" :disabled="form.FormStatus >= 3" class="beauty-form">
						<!-- 承造單位資料 -->
						<div class="contact-group">
							<h3 class="group-title">承造單位資料</h3>
							<el-row :gutter="20">
								<el-col :md="8" :sm="24">
									<el-form-item prop="R_NAME" label="承造單位名稱">
										<el-input v-model="form.R_NAME" maxlength="60"></el-input>
									</el-form-item>
								</el-col>
								<el-col :md="8" :sm="24">
									<el-form-item prop="R_G_NO" label="營利事業統一編號">
										<el-input v-model="form.R_G_NO" maxlength="8"></el-input>
									</el-form-item>
								</el-col>
								<el-col :md="8" :sm="24">
									<el-form-item prop="R_TEL" label="承造單位電話">
										<el-input type="tel" v-model="form.R_TEL" maxlength="30"></el-input>
									</el-form-item>
								</el-col>
								<el-col :span="24">
									<el-form-item prop="R_ADDR1" label="營業地址">
										<el-input v-model="form.R_ADDR1" maxlength="50"></el-input>
									</el-form-item>
								</el-col>
								<el-col :span="24">
									<el-form-item prop="R_ADDR2" label="聯絡地址">
										<el-input v-model="form.R_ADDR2" maxlength="50"></el-input>
									</el-form-item>
								</el-col>
							</el-row>
						</div>

						<!-- 負責人資料 -->
						<div class="contact-group">
							<h3 class="group-title">負責人資料</h3>
							<el-row :gutter="20">
								<el-col :md="12" :sm="24">
									<el-form-item prop="R_B_NAM" label="負責人姓名">
										<el-input v-model="form.R_B_NAM" maxlength="20"></el-input>
									</el-form-item>
								</el-col>
								<el-col :md="12" :sm="24">
									<el-form-item prop="R_B_TIT" label="職稱">
										<el-input v-model="form.R_B_TIT" maxlength="20"></el-input>
									</el-form-item>
								</el-col>

								<el-col :md="12" :sm="24">
									<el-form-item prop="R_B_ID" label="身分證字號" :rules="R_B_IDRules">
										<el-input v-model="form.R_B_ID" maxlength="30"></el-input>
									</el-form-item>
								</el-col>
								<el-col :md="12" :sm="24">
									<el-form-item prop="R_B_BDATE" label="生日">
										<div class="el-input el-input--suffix">
											<input type="text" class="el-input__inner datepicker" data-key="R_B_BDATE" v-model="form.R_B_BDATE" :disabled="form.FormStatus > 2" readonly />
											<span v-if="form.R_B_BDATE && form.FormStatus <= 2" class="el-input__suffix datepicker-suffix" @click="form.R_B_BDATE = ''">
												<i class="fa fa-times-circle"></i>
											</span>
										</div>
									</el-form-item>
								</el-col>
							</el-row>
						</div>

						<!-- 工地主任 / 工地環保負責人資料 -->
						<div class="contact-group">
							<h3 class="group-title">工地管理人員資料</h3>
							<el-row :gutter="20">
								<el-col :md="8" :sm="24">
									<el-form-item prop="R_M_NAM" label="工地主任姓名">
										<el-input v-model="form.R_M_NAM" maxlength="10"></el-input>
									</el-form-item>
								</el-col>
								<el-col :md="8" :sm="24">
									<el-form-item prop="R_C_NAM" label="工地環保負責人姓名">
										<el-input v-model="form.R_C_NAM" maxlength="10"></el-input>
									</el-form-item>
								</el-col>
								<el-col :md="8" :sm="24">
									<el-form-item prop="R_TEL1" label="電話">
										<el-input type="tel" v-model="form.R_TEL1" maxlength="30"></el-input>
									</el-form-item>
								</el-col>
								<el-col :span="24">
									<el-form-item prop="R_ADDR3" label="工務所地址">
										<el-input v-model="form.R_ADDR3" maxlength="50"></el-input>
									</el-form-item>
								</el-col>
							</el-row>
						</div>
					</el-form>
				</el-tab-pane>
				<el-tab-pane label="經費資料" name="4">
					<el-card shadow="hover" class="form-section">
						<div slot="header" class="clearfix">
							<span class="form-title primary-title">
								<i class="el-icon-document"></i> 初次申報資料
							</span>
						</div>
						<el-form ref="tab4Form" :rules="tab4Rules" :model="form" label-width="auto" :disabled="form.FormStatus > 2">
							<div class="flex-row">
								<el-form-item prop="MONEY" label="工程合約經費(元)">
									<el-input type="number" v-model="form.MONEY"></el-input>
								</el-form-item>
								<el-form-item prop="TAX_MONEY" label="工程合約經費營業稅(元)">
									<el-input type="number" v-model="form.TAX_MONEY"></el-input>
								</el-form-item>
								<el-form-item prop="C_MONEY" label="工程環保經費(元)">
									<div style="min-width:120px">{{calcC_MONEY | comma}}</div>
								</el-form-item>
								<el-form-item prop="PERCENT" label="工程合約經費比例">{{form.PERCENT}}%</el-form-item>
							</div>
							<div v-if="form.KIND_NO === '1' || form.KIND_NO === '2'" class="flex-row">
								<el-form-item prop="AREA_F" label="基地面積">
									<el-input type="number" style="width:120px" v-model="form.AREA_F"></el-input>平方公尺
								</el-form-item>
								<el-form-item prop="AREA_B" label="建築面積">
									<el-input type="number" style="width:120px" v-model="form.AREA_B"></el-input>平方公尺
								</el-form-item>
								<el-form-item prop="PERC_B" label="建蔽率">{{calcPERC_B}}%</el-form-item>
							</div>
							<div v-else-if="form.KIND_NO === '3'">
								<el-form-item prop="AREA2" label="總樓地板面積(平方公尺)">
									<el-input type="number" v-model="form.AREA2"></el-input>
								</el-form-item>
							</div>
							<div v-else>
								<el-form-item prop="AREA" label="工程面積">
									<el-input type="number" v-model="form.AREA" style="width:120px"></el-input>平方公尺
								</el-form-item>
							</div>
							<div v-if="form.KIND_NO === 'B'" v-cloak>
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
									<div class="el-input el-input--suffix">
										<input type="text" class="el-input__inner datepicker" data-key="B_DATE" v-model="form.B_DATE" :disabled="form.FormStatus > 2" readonly />
										<span v-if="form.B_DATE && form.FormStatus <= 2" class="el-input__suffix datepicker-suffix" @click="form.B_DATE = ''">
											<i class="fa fa-times-circle"></i>
										</span>
									</div>
								</el-form-item>
								<el-form-item prop="E_DATE" label="結束日期">
									<div class="el-input el-input--suffix">
										<input type="text" class="el-input__inner datepicker" data-key="E_DATE" v-model="form.E_DATE" :disabled="form.FormStatus > 2" readonly />
										<span v-if="form.E_DATE && form.FormStatus <= 2" class="el-input__suffix datepicker-suffix" @click="form.E_DATE = ''">
											<i class="fa fa-times-circle"></i>
										</span>
									</div>
								</el-form-item>
								<el-form-item label="預計施工天數">{{totalDays(form.B_DATE, form.E_DATE)}}</el-form-item>
							</div>
							<el-form-item label="規定繳費方式">
								<el-select v-model="form.P_KIND" :disabled="form.FormStatus >= 3">
									<el-option label="一次全繳" value="一次全繳"></el-option>
									<el-option label="分兩次繳清" value="分兩次繳清"></el-option>
								</el-select>
							</el-form-item>
							<el-form-item label="空汙防制措施計畫書">
								<el-select v-model="form.REC_YN" :disabled="form.FormStatus >= 3">
									<el-option label="有" value="有"></el-option>
									<el-option label="無" value="無"></el-option>
								</el-select>
							</el-form-item>
						</el-form>
					</el-card>
					<el-card shadow="hover" class="form-section secondary-section" v-if="form.FormStatus >= 4">
						<div slot="header" class="clearfix">
							<span class="form-title secondary-title">
								<i class="el-icon-finished"></i> 結算申報資料
							</span>
						</div>
						<el-form v-if="form.FormB" ref="tab4BForm" :rules="tab4BRules" :model="form.FormB" label-width="auto" :disabled="form.CalcStatus > 2">
							<div class="flex-row">
								<el-form-item prop="MONEY" label="工程合約經費(元)">
									<el-input type="number" v-model="form.FormB.MONEY"></el-input>
								</el-form-item>
								<el-form-item prop="C_MONEY" label="工程環保經費(元)">
									<div style="min-width:120px">{{calcC_MONEYB | comma}}</div>
								</el-form-item>
								<el-form-item prop="PERCENT" label="工程合約經費比例">{{form.PERCENT}}%</el-form-item>
							</div>
							<div v-if="form.FormB.KIND_NO === '1' || form.FormB.KIND_NO === '2'" class="flex-row">
								<el-form-item prop="AREA_F" label="基地面積">
									<el-input type="number" style="width:120px" v-model="form.AREA_F"></el-input>平方公尺
								</el-form-item>
								<el-form-item prop="AREA_B" label="建築面積">
									<el-input type="number" style="width:120px" v-model="form.AREA_B"></el-input>平方公尺
								</el-form-item>
								<el-form-item prop="PERC_B" label="建蔽率">{{calcPERC_B2}}%</el-form-item>
							</div>
							<div v-else-if="form.KIND_NO === '3'">
								<el-form-item prop="AREA2" label="總樓地板面積(平方公尺)">
									<el-input type="number" v-model="form.AREA2" disabled></el-input>
								</el-form-item>
							</div>
							<div v-else>
								<el-form-item prop="AREA" label="工程面積">
									<el-input type="number" v-model="form.FormB.AREA" style="width:120px"></el-input>平方公尺
								</el-form-item>
							</div>
							<div v-if="form.KIND_NO === 'B'" v-cloak>
								<div style="margin-bottom:10px">鬆方體積換算表</div>
								<div class="contact-group">
									<div class="flex-row" style="justify-content: flex-start; align-items: flex-end">
										<el-form-item label="鬆方重量(公噸)">
											<el-input type="number" v-model="form.E2" disabled></el-input>
										</el-form-item>
										<el-form-item label="密度">
											<el-input type="number" v-model="form.FormB.DENSITYL"></el-input>
										</el-form-item>
										<el-form-item>
											<el-button type="primary" round @click="calcE2_B">換算</el-button>
										</el-form-item>
									</div>
								</div>
								<div class="hint-message">
									<i class="el-icon-info"></i> 鬆方體積除以實方體積之比值以一‧三一計，鬆方之密度以一‧五一公噸/立方公尺計。營建業主如有現地取樣之實方與鬆方試驗相關數據，得報請地方主管機關同意後，依該數據採計。
								</div>
								<el-form-item prop="VOLUMEL" label="外運土石體積(立方公尺)">
									<el-input type="number" v-model="form.FormB.VOLUMEL"></el-input>
								</el-form-item>
							</div>
							<div class="flex-row">
								<el-form-item prop="B_DATE" label="開始日期">
									<div class="el-input el-input--suffix">
										<input type="text" class="el-input__inner datepicker" data-key="FormB.B_DATE" v-model="form.FormB.B_DATE" :disabled="form.CalcStatus > 2" readonly />
										<span v-if="form.FormB.B_DATE && form.CalcStatus <= 2" class="el-input__suffix datepicker-suffix" @click="form.FormB.B_DATE = ''">
											<i class="fa fa-times-circle"></i>
										</span>
									</div>
								</el-form-item>
								<el-form-item prop="E_DATE" label="結束日期">
									<div class="el-input el-input--suffix">
										<input type="text" class="el-input__inner datepicker" data-key="FormB.E_DATE" v-model="form.FormB.E_DATE" :disabled="form.CalcStatus > 2" readonly />
										<span v-if="form.FormB.E_DATE && form.CalcStatus <= 2" class="el-input__suffix datepicker-suffix" @click="form.FormB.E_DATE = ''">
											<i class="fa fa-times-circle"></i>
										</span>
									</div>
								</el-form-item>
								<el-form-item label="預計施工天數">{{totalDays(form.FormB.B_DATE, form.FormB.E_DATE)}}</el-form-item>
							</div>
						</el-form>
					</el-card>
				</el-tab-pane>
				<el-tab-pane label="檢附資料" name="5">
					<div class="attach-row">
						<div class="attach-card">
							<div class="attach-title">
								<i class="fa fa-paperclip"></i> 首期申報附件
							</div>
							<a v-if="form.FileName1" :href="`api/Form/Download?f=${form.FileName1}&n=${form.DisplayName1}`" class="link-download">
								<i class="fa fa-download"></i>
								{{ form.DisplayName1 }}
							</a>
							<span v-else class="link-empty">暫無上傳檔案</span>
						</div>

						<div class="attach-card">
							<div class="attach-title">
								<i class="fa fa-paperclip"></i> 結算申報附件
							</div>
							<a v-if="form.FileName2" :href="`api/Form/Download?f=${form.FileName2}&n=${form.DisplayName2}`" class="link-download">
								<i class="fa fa-download"></i>
								{{ form.DisplayName2 }}
							</a>
							<span v-else class="link-empty">暫無上傳檔案</span>
						</div>
					</div>
				</el-tab-pane>
				<el-tab-pane label="停復工" name="6">
					<div class="stopwork-wrapper">
						<div class="toolbar">
							<el-button type="primary" icon="el-icon-plus" @click="addStopWork()">新 增</el-button>
						</div>
						<div class="table-wrapper">
							<table class="table">
								<thead>
									<tr>
										<th style="width:60px">刪除</th>
										<th style="width:180px">停工日期</th>
										<th style="width:180px">復工日期</th>
										<th style="width:120px; white-space: nowrap">停工天數</th>
									</tr>
								</thead>
								<tbody>
									<tr v-if="form.StopWorks.length === 0">
										<td colspan="4" class="no-data">暫無資料</td>
									</tr>
									<tr v-for="(item, idx) in form.StopWorks" :key="idx">
										<td class="text-center">
											<el-button type="danger" size="mini" icon="el-icon-delete" circle @click="deleteStopWork(idx)"></el-button>
										</td>
										<td>
											<el-date-picker class="w100p" v-model="item.DOWN_DATE2" type="date" value-format="yyyy-MM-dd" placeholder="請選擇日期"></el-date-picker>
										</td>
										<td>
											<el-date-picker class="w100p" v-model="item.UP_DATE2" type="date" value-format="yyyy-MM-dd" placeholder="請選擇日期"></el-date-picker>
										</td>
										<td class="days">{{ getStopDays(item) }}</td>
									</tr>
								</tbody>
							</table>
						</div>
					</div>
				</el-tab-pane>
				<el-tab-pane label="退款帳戶" name="7">
					<el-form v-if="form.CalcStatus > 2" inline>
						<el-form-item>
							<el-button type="primary" @click="exportRefundVerify1">結算退費審核表</el-button>
							<el-button type="primary" @click="exportRefundVerify2">結算金額異動原因明細</el-button>
						</el-form-item>
					</el-form>
					<div class="refund-bank-section">
						<el-form v-if="form.RefundBank.ID" label-width="100px" class="refund-bank-form">
							<el-form-item label="銀行代碼">{{ form.RefundBank.Code }}</el-form-item>
							<el-form-item label="銀行帳號">{{ form.RefundBank.Account }}</el-form-item>
							<el-form-item label="存摺照片">
								<div class="photo-card">
									<img :src="`api/Form/Download?f=${form.RefundBank.Photo}`" alt="存摺照片" />
								</div>
							</el-form-item>
						</el-form>
						<div v-else class="no-data">暫無資料</div>
					</div>
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
				{{activeTab === '7' ? (mode === 'Update' ? '儲存' : '複製') : '下一步'}}
				<i class="el-icon-arrow-right el-icon--right"></i>
			</el-button>
		</template>
	</vxe-modal>
</template>
<script>
import { mapGetters } from 'vuex';
import { dateTime, comma, form } from '@/mixins/filter';
var A1_84 = 6378137.0;
var B1_84 = 6356752.3141;
var e2_84 = 0.0066943800355;
var A1_67 = 6378160.0;
var B1_67 = 6356774.7192;
var e2_67 = 0.006694541853;
var SK = new Array(0.9999, 1.0, 0.9996);
var Cent = new Array(121.0, 121.0, 123.0);
var Shift = new Array(250000.0, 350000.0, 500000.0);
var DegreePI = 180.0 / Math.PI;

var XM84 = -3033819.548;
var YM84 = 5071301.969;
var ZM84 = 2391982.06;
var DeltaX84 = 754.812;
var DeltaY84 = 362.233;
var DeltaZ84 = 187.197;
var EpsilonX84 = -5.216 / 3600.0 / DegreePI;
var EpsilonY84 = -8.846 / 3600.0 / DegreePI;
var EpsilonZ84 = 35.781 / 3600.0 / DegreePI;
var S84 = 3.161e-6;
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
	filters: {
		westDate(val) {
			if (!val) return '';

			// 將民國年轉成西元年
			const rocYear = parseInt(val.substring(0, 3), 10) + 1911;
			const month = val.substring(3, 5);
			const day = val.substring(5, 7);

			// 用 moment 組成日期
			return moment(`${rocYear}-${month}-${day}`, 'YYYY-MM-DD').format('YYYY-MM-DD');
		}
	},
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
		const checkE_DATE2 = (rule, value, callback) => {
			if (!value) {
				callback(new Error('請輸入結束日期'));
			}

			if (!this.form.FormB.B_DATE) {
				callback();
			}

			const startDate = `${+this.form.FormB.B_DATE.substr(0, 3) + 1911}-${this.form.FormB.B_DATE.substr(3, 2)}-${this.form.FormB.B_DATE.substr(5, 2)}`;
			const endDate = `${+this.form.FormB.E_DATE.substr(0, 3) + 1911}-${this.form.FormB.E_DATE.substr(3, 2)}-${this.form.FormB.E_DATE.substr(5, 2)}`;
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
		const checkS_B_ID = (rule, value, callback) => {
			const idRegx = /^[A-Z][12]\d{8}$/;
			if (!this.form.PUB_COMP && !idRegx.test(value)) {
				callback(new Error('營利事業負責人身分證字號格式錯誤'));
			}
			callback();
		};
		const checkS_C_ID = (rule, value, callback) => {
			const idRegx = /^[A-Z][12]\d{8}$/;
			if (!this.form.PUB_COMP && !idRegx.test(value)) {
				callback(new Error('營利事業聯絡人身分證字號格式錯誤'));
			}
			callback();
		};
		const checkR_B_ID = (rule, value, callback) => {
			const idRegx = /^[A-Z][12]\d{8}$/;
			if (!this.form.PUB_COMP && !idRegx.test(value)) {
				callback(new Error('承造負責人身分證字號格式錯誤'));
			}
			callback();
		};
		return {
			visible: false,
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
				S_ADDR1: [{ required: true, message: '請輸入營利事業營業地址', trigger: 'blur' }],
				S_ADDR2: [{ required: true, message: '請輸入營利事業聯絡地址', trigger: 'blur' }],
				S_TEL: [{ required: true, message: '請輸入營利事業主連絡電話', trigger: 'blur' }],
				S_B_NAM: [{ required: true, message: '請輸入營利事業負責人姓名', trigger: 'blur' }],
				S_B_TIT: [{ required: true, message: '請輸入營利事業負責人職稱', trigger: 'blur' }],
				// S_B_ID: [{ validator: checkS_B_ID, trigger: 'blur' }],
				S_C_NAM: [{ required: true, message: '請輸入營利事業聯絡人姓名', trigger: 'blur' }],
				S_C_TIT: [{ required: true, message: '請輸入營利事業聯絡人職稱', trigger: 'blur' }],
				// S_C_ID: [{ validator: checkS_C_ID, trigger: 'blur' }],
				S_C_ADDR: [{ required: true, message: '請輸入營利事業聯絡人地址', trigger: 'blur' }],
				S_C_TEL: [{ required: true, message: '請輸入營利事業聯絡人電話', trigger: 'blur' }]
			}),
			tab3Rules: Object.freeze({
				R_NAME: [{ required: true, message: '請輸入承造單位名稱', trigger: 'blur' }],
				R_ADDR1: [{ required: true, message: '請輸入承造營業地址', trigger: 'blur' }],
				R_ADDR2: [{ required: true, message: '請輸入承造聯絡地址', trigger: 'blur' }],
				R_TEL: [{ required: true, message: '請輸入承造連絡電話', trigger: 'blur' }],
				R_B_NAM: [{ required: true, message: '請輸入承造負責人姓名', trigger: 'blur' }],
				R_B_TIT: [{ required: true, message: '請輸入承造負責人職稱', trigger: 'blur' }],
				// R_B_ID: [{ validator: checkR_B_ID, trigger: 'blur' }],
				R_ADDR3: [{ required: true, message: '請輸入工務所地址', trigger: 'blur' }],
				R_M_NAM: [{ required: true, message: '請輸入工地主任姓名', trigger: 'blur' }],
				R_C_NAM: [{ required: true, message: '請輸入工地環保負責人姓名', trigger: 'blur' }],
				R_TEL1: [{ required: true, message: '請輸入工務所電話', trigger: 'blur' }]
			}),
			tab4Rules: Object.freeze({
				MONEY: [{ required: true, message: '請輸入工程合約經費', trigger: 'blur' }],
				TAX_MONEY: [{ required: true, message: '請輸入工程合約經費營業稅', trigger: 'blur' }],
				C_MONEY: [{ required: true, message: '請輸入工程環保經費', trigger: 'blur' }],
				AREA: [{ validator: checkArea, trigger: 'blur' }],
				AREA_F: [{ validator: checkAreaF, trigger: 'blur' }],
				VOLUMEL: [{ validator: checkVolumel, trigger: 'blur' }],
				B_DATE: [{ required: true, message: '請輸入開始日期', trigger: 'blur' }],
				E_DATE: [{ validator: checkE_DATE, trigger: 'blur' }]
			}),
			tab4BRules: Object.freeze({
				MONEY: [{ required: true, message: '請輸入工程合約經費', trigger: 'blur' }],
				AREA: [{ required: true, message: '請輸入工程面積', trigger: 'blur' }],
				AREA2: [{ required: true, message: '請輸入總樓地板面積', trigger: 'blur' }],
				AREA_F: [{ required: true, message: '請輸入基地面積', trigger: 'blur' }],
				AREA_B: [{ required: true, message: '請輸入建築面積', trigger: 'blur' }],
				VOLUMEL: [{ required: true, message: '請輸入外運土石體積', trigger: 'blur' }],
				B_DATE: [{ required: true, message: '請輸入開始日期', trigger: 'blur' }],
				E_DATE: [{ validator: checkE_DATE2, trigger: 'blur' }]
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
		S_B_IDRules() {
			const rules = [{ validator: this.checkS_B_ID, trigger: 'blur' }];
			if (!this.form.PUB_COMP) {
				rules.unshift({ required: true, message: '營利事業負責人身分證字號格式錯誤', trigger: 'blur' });
			}
			return rules;
		},
		S_C_IDRules() {
			const rules = [{ validator: this.checkS_C_ID, trigger: 'blur' }];
			if (!this.form.PUB_COMP) {
				rules.unshift({ required: true, message: '營利事業聯絡人身分證字號格式錯誤', trigger: 'blur' });
			}
			return rules;
		},
		R_B_IDRules() {
			const rules = [{ validator: this.checkR_B_ID, trigger: 'blur' }];
			if (!this.form.PUB_COMP) {
				rules.unshift({ required: true, message: '承造負責人身分證字號格式錯誤', trigger: 'blur' });
			}
			return rules;
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
		calcC_MONEYB() {
			try {
				if (!this.form.FormB.MONEY) throw '';
				return +((this.form.FormB.MONEY * this.form.FormB.PERCENT) / 100).toFixed(0);
			} catch (err) {
				return 0;
			}
		},
		calcPERC_B() {
			if (!this.form.AREA_B || !this.form.AREA_F) return 0;
			return ((+this.form.AREA_B / +this.form.AREA_F) * 100).toFixed(2);
		},
		calcPERC_B2() {
			if (!this.form.FormB.AREA_B || !this.form.FormB.AREA_F) return 0;
			return ((+this.form.FormB.AREA_B / +this.form.FormB.AREA_F) * 100).toFixed(2);
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

					const key = inst.input[0].dataset.key; // 例如 'FormB.S_DATE' 或 'S_DATE'
					const dateFormate = `${objDate.y}${objDate.m}${objDate.d}`;
					inst.input.val(dateFormate);
					// 根據 key 的值來決定要設值的位置
					if (key.includes('.')) {
						// 有階層結構，例如 'FormB.S_DATE'
						const [parent, child] = key.split('.');
						this.form[parent][child] = dateFormate;
					} else {
						// 沒有階層，直接設值
						this.form[key] = dateFormate;
					}
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
				case '5':
				case '6': {
					this.activeTab = (+this.activeTab + 1).toString();
					break;
				}
				case '7': {
					if (!confirm('是否確認繼續?')) return false;
					const loading = this.$loading();
					const point = this.LatLon2UTM(this.form.LAT, this.form.LNG, 0, 0);
					this.form.UTME = point[0];
					this.form.UTMN = point[1];
					this.form.LATLNG = `${this.form.LAT},${this.form.LNG}`;
					this.form.C_MONEY = this.calcC_MONEY;
					// 1、2類工程面積=建築面積
					if (this.form.KIND_NO === '1' || this.form.KIND_NO === '2') {
						this.form.AREA = this.form.AREA_B;
					}
					// 3類工程面積=總樓地板面積
					else if (this.form.KIND_NO === '3') {
						this.form.AREA = this.form.AREA2;
					}
					this.axios
						.post(`api/Form/${this.mode}Form`, this.form)
						.then(res => {
							this.$emit('on-updated');
							this.$message.success('畫面資料已儲存');
							this.visible = false;
							loading.close();
						})
						.catch(err => {
							this.$message.error(err.response.data.ExceptionMessage);
							loading.close();
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
		},
		calcE2_B() {
			this.form.FormB.VOLUMEL = this.form.E2 / this.form.FormB.DENSITYL;
		},
		totalDays(sdate, edate) {
			if (!sdate || !edate) return '';
			const startDate = `${+sdate.substr(0, 3) + 1911}-${sdate.substr(3, 2)}-${sdate.substr(5, 2)}`;
			const endDate = `${+edate.substr(0, 3) + 1911}-${edate.substr(3, 2)}-${edate.substr(5, 2)}`;
			var date1 = new Date(startDate);
			var date2 = new Date(endDate);

			// 計算毫秒差異
			var diff = Math.abs(date2 - date1 + 1000 * 60 * 60 * 24);
			// 轉換為天數
			var dayDiff = Math.ceil(diff / (1000 * 60 * 60 * 24));

			return dayDiff;
		},
		LatLon2UTM(Lat, Lon, AreaCode, Flag) {
			var E1, E2, PH2, LM2, PH1, LM1, Temp1, Temp2, Temp3, Temp4;
			var Temp5, Temp6, Temp7;
			var RAP, UU, N, T;
			var A, B, C, D, E, PM, PH;
			var A1, B1;

			if (Flag == 0) {
				A1 = A1_84;
				B1 = B1_84;
			} else {
				A1 = A1_67;
				B1 = B1_67;
			}

			E1 = 1.0 - (B1 / A1) * (B1 / A1);
			E2 = (A1 / B1) * (A1 / B1) - 1.0;

			PH2 = Lat / DegreePI;
			LM2 = Lon / DegreePI;
			LM1 = Cent[AreaCode] / DegreePI;
			PH1 = 0.0;

			Temp1 = Math.sin(PH2);
			Temp2 = Math.cos(PH2);
			RAP = LM2 - LM1;
			UU = E2 * Temp2 * Temp2;
			Temp3 = 1.0 - E1 * Temp1 * Temp1;
			N = A1 / Math.sqrt(Temp3);
			T = Temp1 / Temp2;
			Temp4 = N * Temp2 * RAP;
			Temp5 = (N * (1.0 - T * T + UU) * Math.pow(Temp2, 3.0) * Math.pow(RAP, 3.0)) / 6.0;
			Temp6 = 5.0 - 18.0 * Math.pow(T, 2.0) + Math.pow(T, 4.0) + 14.0 * UU - 58.0 * UU * T * T;
			Temp7 = (N * Temp6 * Math.pow(Temp2, 5.0) * Math.pow(RAP, 5.0)) / 120.0;

			LM2 = Temp4 + Temp5 + Temp7;
			LM2 = SK[AreaCode] * LM2 + Shift[AreaCode];

			Temp4 = Math.pow(E1, 2.0);
			Temp5 = Math.pow(E1, 3.0);
			Temp6 = Math.pow(E1, 4.0);
			A = 1.0 + 0.75 * E1 + (45.0 * Temp4) / 64.0 + (175.0 * Temp5) / 256.0 + (11025.0 * Temp6) / 16384.0;
			B = 0.75 * E1 + (15.0 * Temp4) / 16.0 + (525.0 * Temp5) / 512.0 + (2205.0 * Temp6) / 2048.0;
			C = (15.0 * Temp4) / 64.0 + (105.0 * Temp5) / 256.0 + (2205.0 * Temp6) / 4096.0;
			D = (35.0 * Temp5) / 512.0 + (315.0 * Temp6) / 2048.0;
			E = (315.0 * Temp6) / 16384.0;

			Temp4 = A * (PH2 - PH1) - (B * (Math.sin(2.0 * PH2) - Math.sin(2.0 * PH1))) / 2.0;
			Temp5 = C * (Math.sin(4.0 * PH2) - Math.sin(4.0 * PH1));
			Temp6 = D * (Math.sin(6.0 * PH2) - Math.sin(6.0 * PH1));
			Temp7 = E * (Math.sin(8.0 * PH2) - Math.sin(8.0 * PH1));

			PM = A1 * (1.0 - E1) * (Temp4 + Temp5 / 4.0 + Temp6 / 6.0 + Temp7 / 8.0);

			Temp4 = (N * T * Math.pow(RAP, 2.0) * Math.pow(Temp2, 2.0)) / 2.0;
			Temp5 = 5.0 - Math.pow(T, 2.0) + 9.0 * UU + 4.0 * Math.pow(UU, 2.0);
			Temp6 = (Math.pow(Temp2, 4.0) * Math.pow(RAP, 4.0)) / 24.0;
			Temp7 = 61.0 - 58.0 * T * T + Math.pow(T, 4.0) + 270.0 * UU - 330.0 * UU * T * T;

			PH = PM + Temp4 + N * T * Temp5 * Temp6 + (N * T * Math.pow(PH2, 6.0) * Math.pow(RAP, 6.0) * Temp7) / 720.0;

			PH2 = SK[AreaCode] * PH;

			var pt = new Array(1);
			pt[0] = LM2;
			pt[1] = PH2;
			return pt;
		},
		UTM2LatLon(X, Y, AreaCode, Flag) {
			var N, E1, CN, CE, PH2, PH1, DS, RM, DPH;
			var Temp1, Temp2, Temp3, T, R, UU;
			var LM2, S, A1, B1;
			var i, IPAT;

			if (Flag == 0) {
				A1 = A1_84;
				B1 = B1_84;
			} else {
				A1 = A1_67;
				B1 = B1_67;
			}

			E1 = 1.0 - (B1 / A1) * (B1 / A1);
			CN = Y;
			CE = X;
			CN /= SK[AreaCode];
			CE = (CE - Shift[AreaCode]) / SK[AreaCode];
			PH2 = 0.0;
			IPAT = 1200;
			DS = CN / IPAT;

			for (i = 1; i <= IPAT; i++) {
				Temp1 = Math.pow(Math.sin(PH2), 2.0);
				Temp2 = 1.0 - E1 * Temp1;
				Temp3 = Math.pow(Temp2, 1.5);
				RM = (A1 * (1.0 - E1)) / Temp3;
				DPH = DS / (2.0 * RM);
				PH1 = PH2 + DPH;
				Temp1 = Math.pow(Math.sin(PH1), 2.0);
				Temp2 = 1.0 - E1 * Temp1;
				Temp3 = Math.pow(Temp2, 1.5);
				RM = (A1 * (1.0 - E1)) / Temp3;
				DPH = DS / RM;
				PH2 = PH2 + DPH;
			}

			T = Math.sin(PH2) / Math.cos(PH2);
			Temp1 = Math.pow(Math.sin(PH2), 2.0);
			Temp2 = 1.0 - E1 * Temp1;
			Temp3 = Math.pow(Temp2, 1.5);
			R = (A1 * (1.0 - E1)) / Temp3;
			Temp3 = Math.pow(Temp2, 0.5);
			N = A1 / Temp3;
			UU = N / R;
			S = 1.0 / Math.cos(PH2);
			Temp1 = (T * Math.pow(CE, 2.0)) / (2.0 * R * N);
			Temp2 = (T * Math.pow(CE, 4.0) * (-4.0 * Math.pow(UU, 2.0) + 9.0 * (1.0 - T * T) + 12.0 * T * T)) / (24.0 * R * Math.pow(N, 3.0));
			Temp3 =
				(T *
					Math.pow(CE, 6.0) *
					(8.0 * Math.pow(UU, 4.0) * (11.0 - 24.0 * T * T) -
						12.0 * Math.pow(UU, 3.0) * (21.0 - 71.0 * T * T) +
						15.0 * Math.pow(UU, 2.0) * (15.0 - 98.0 * T * T + 15.0 * Math.pow(T, 4)) +
						180.0 * UU * (5.0 * T * T - 3.0 * Math.pow(T, 4.0)) +
						360.0 * Math.pow(T, 4.0))) /
				(720 * R * Math.pow(N, 5.0));
			PH2 = PH2 - Temp1 + Temp2 - Temp3;

			var Lat = PH2 * DegreePI;

			Temp1 = (S * CE) / N;
			Temp2 = (S * Math.pow(CE, 3.0) * (UU + 2.0 * T * T)) / (6 * Math.pow(N, 3.0));
			Temp3 =
				(S * Math.pow(CE, 5.0) * (-4.0 * Math.pow(UU, 3.0) * (1.0 - 6.0 * T * T) + UU * UU * (9.0 - 68.0 * T * T) + 72.0 * UU * T * T + 24.0 * Math.pow(T, 4.0))) / (120.0 * Math.pow(N, 5.0));
			LM2 = Temp1 - Temp2 + Temp3;
			var Lon = 121 + LM2 * DegreePI;

			var pt = new Array(1);

			pt[0] = Lat;
			pt[1] = Lon;
			return pt;
		},
		exportRefundVerify1() {
			const loading = this.$loading();
			this.axios
				.post('api/Form/ExportRefundVerify1', this.form, {
					responseType: 'blob'
				})
				.then(res => {
					loading.close();
					const url = window.URL.createObjectURL(new Blob([res.data]));
					const link = document.createElement('a');
					link.href = url;
					const fileName = decodeURI(res.headers['file-name']);
					link.setAttribute('download', fileName);
					document.body.appendChild(link);
					link.click();
					link.remove();
				})
				.catch(err => {
					loading.close();
					alert('系統發生未預期錯誤');
					console.log(err);
				});
		},
		exportRefundVerify2() {
			const loading = this.$loading();
			this.axios
				.post('api/Form/ExportRefundVerify2', this.form, {
					responseType: 'blob'
				})
				.then(res => {
					loading.close();
					const url = window.URL.createObjectURL(new Blob([res.data]));
					const link = document.createElement('a');
					link.href = url;
					const fileName = decodeURI(res.headers['file-name']);
					link.setAttribute('download', fileName);
					document.body.appendChild(link);
					link.click();
					link.remove();
				})
				.catch(err => {
					loading.close();
					alert('系統發生未預期錯誤');
					console.log(err);
				});
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

.stopwork-wrapper {
	max-width: 720px;
	background: #fff;
	border-radius: 10px;
	padding: 20px 25px;
	.toolbar {
		display: flex;
		justify-content: flex-end;
		margin-bottom: 12px;
	}
	.table-wrapper {
		overflow-x: auto;
	}
	.table {
		width: 100%;
		border-collapse: separate;
		border-spacing: 0;
		border: 1px solid #ebeef5;
		border-radius: 8px;
		overflow: hidden;
		font-size: 14px;
		thead {
			th {
				background-color: #f9fafc;
				color: #606266;
				text-align: center;
				padding: 10px;
				border-bottom: 1px solid #ebeef5;
				font-weight: 600;
			}
		}
		tbody {
			tr {
				transition: background-color 0.2s ease;

				&:hover {
					background-color: #f5f7fa;
				}

				td {
					padding: 8px 10px;
					border-bottom: 1px solid #ebeef5;
					vertical-align: middle;

					&.text-center {
						text-align: center;
					}

					&.days {
						text-align: center;
						color: #409eff;
						font-weight: bold;
					}

					.w100p {
						width: 100%;
					}
				}
			}
			.no-data {
				text-align: center;
				color: #999;
				padding: 15px 0;
				background: #fafafa;
				font-style: italic;
			}
		}
	}
}

.refund-bank-section {
	max-width: 700px;
	margin-top: 20px;
	background: #fff;
	border-radius: 10px;
	padding: 20px 25px;
	box-shadow: 0 3px 10px rgba(0, 0, 0, 0.08);

	.refund-bank-form {
		.el-form-item {
			margin-bottom: 18px;

			.el-form-item__content {
				font-weight: 500;
				color: #333;
			}
		}

		.photo-card {
			padding: 10px;
			border: 1px solid #ebeef5;
			border-radius: 8px;
			display: inline-block;
			max-width: 100%;
			box-shadow: 0 2px 6px rgba(0, 0, 0, 0.05);

			img {
				width: 100%;
				max-width: 640px;
				display: block;
				border-radius: 5px;
			}
		}
	}

	.no-data {
		text-align: center;
		color: #999;
		padding: 20px;
		font-style: italic;
		background: #fafafa;
		border-radius: 8px;
	}
}

.modal-header {
	margin: 10px 0;
}
/* === 共用區 === */
.form-title {
	font-size: 18px;
	font-weight: bold;
	display: flex;
	align-items: center;

	i {
		margin-right: 6px;
	}

	&.primary-title {
		color: #409eff; /* Element 主藍 */
	}
	&.secondary-title {
		color: #67c23a; /* Element 綠色 */
	}
}

/* === 表單分組標題 === */
.group-title {
	font-size: 16px;
	font-weight: 600;
	color: #555;
	margin: 10px 0 15px 10px;
	border-left: 3px solid #67c23a;
	padding-left: 8px;
}

/* === 美化表單 === */
.beauty-form {
	.el-form-item {
		margin-bottom: 16px;

		.el-form-item__label {
			font-weight: 500;
			color: #555;
		}
	}

	.el-input__inner,
	.el-select .el-input__inner {
		border-radius: 8px;
		border-color: #dcdfe6;
		transition: all 0.2s;

		&:focus {
			border-color: #409eff;
			box-shadow: 0 0 0 2px rgba(64, 158, 255, 0.15);
		}
	}
}

/* === 快速選取 === */
.quick-select {
	margin-bottom: 20px;
	display: flex;
	align-items: center;
	gap: 10px;

	.el-form-item__label {
		font-weight: 500;
		color: #666;
	}
}

/* === 表單分組區塊 === */
.contact-group {
	margin-bottom: 25px;
	background: #f9fafb;
	border-radius: 12px;
	padding: 20px 25px;
	border: 1px solid #ebeef5;

	@media (max-width: 768px) {
		padding: 15px;
	}
}

/* === 表單區塊 === */
.form-section {
	margin-bottom: 25px;
	border-radius: 10px;
	overflow: hidden;
	background: #fff;

	&.secondary-section {
		background: #f9fff9;
		border: 1px solid #e1f3d8;
	}
}

.attach-row {
	display: flex;
	flex-wrap: wrap;
	gap: 1rem;
	margin-top: 1rem;
}

.attach-card {
	flex: 1 1 300px;
	background: #fff;
	border: 1px solid #e0e0e0;
	border-radius: 10px;
	padding: 1rem 1.25rem;
	box-shadow: 0 2px 6px rgba(0, 0, 0, 0.05);
	transition: all 0.3s ease;
	&:hover {
		box-shadow: 0 4px 10px rgba(0, 0, 0, 0.08);
		transform: translateY(-2px);
	}
}

.attach-title {
	font-weight: 600;
	font-size: 1.05rem;
	color: #333;
	margin-bottom: 0.75rem;
	display: flex;
	align-items: center;
	gap: 6px;
	i {
		color: #007bff;
	}
}

.link-download {
	display: inline-block;
	color: #007bff;
	text-decoration: none;
	font-weight: 500;
	word-break: break-all;
	&:hover {
		text-decoration: underline;
	}
}

.link-empty {
	color: #999;
	font-style: italic;
}

/* === 表單細節 === */
.flex-row {
	display: flex;
	flex-wrap: wrap;
	align-items: flex-end;
	gap: 12px 24px;
}

.hint-message {
	font-size: 13px;
	color: #909399;
	background: #f5f7fa;
	padding: 8px 10px;
	border-radius: 6px;
	line-height: 1.5;
	margin: 10px 0;
	display: flex;
	align-items: flex-start;

	i {
		margin-right: 6px;
		color: #409eff;
		margin-top: 2px;
	}
}

.contact-group {
	border: 2px dashed #ccc;
	padding: 10px 10px 10px 0;
	margin-bottom: 10px;
}

/* 日期選單清除按鈕 */
.datepicker-suffix {
	position: absolute;
	right: 10px;
	top: 50%;
	transform: translateY(-50%);
	cursor: pointer;
	pointer-events: auto;
	z-index: 2;
	i {
		color: #bbb;
		font-size: 16px;
	}
}
</style>