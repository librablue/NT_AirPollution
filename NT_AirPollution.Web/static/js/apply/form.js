document.addEventListener('DOMContentLoaded', () => {
	new Vue({
		el: '#app',
		filters: {
			date: value => {
				if (!value || value === '0001-01-01T00:00:00') return '';
				return moment(value).format('YYYY-MM-DD');
			},
			formStatus: value => {
				switch (value) {
					case 1:
						return '審理中';
					case 2:
						return '需補件';
					case 3:
						return '通過待繳費';
					case 4:
						return '繳費完成';
					default:
						return '';
				}
			},
			calcStatus: value => {
				switch (value) {
					case 0:
						return '未申請';
					case 1:
						return '審理中';
					case 2:
						return '需補件';
					case 3:
						return '通過待繳費';
					case 4:
					case 5:
						return '通過待退費';
					case 6:
						return '繳退費完成';
					default:
						return '';
				}
			}
		},
		data() {
			const checkE_DATE2 = (rule, value, callback) => {
				if (!value) {
					callback(new Error('請輸入預計施工完成日期'));
				}
				if (this.selectRow.B_DATE2 && this.selectRow.E_DATE2 && moment(value).isSameOrBefore(this.selectRow.B_DATE2)) {
					callback(new Error('結束日期不得早於起始日期'));
				}
				callback();
			};
			const checkArea = (rule, value, callback) => {
				if (!this.selectRow.VOLUMEL && !value) {
					callback(new Error('如果非疏濬工程，請輸入施工面積'));
				}
				callback();
			};
			const checkVolumel = (rule, value, callback) => {
				if (!this.selectRow.AREA && !value) {
					callback(new Error('如果為疏濬工程，請輸入清運土石體積'));
				}
				callback();
			};
			return {
				mode: '',
				filter: {
					StartDate: moment().format('YYYY-MM-01'),
					EndDate: moment().format('YYYY-MM-DD'),
					C_NO: null,
					PUB_COMP: null,
					CreateUserName: null,
					COMP_NAM: null,
					FormStatus: 0
				},
				district: Object.freeze([]),
				projectCode: Object.freeze([]),
				attachmentInfo: Object.freeze([]),
				companies: Object.freeze([]),
				selectCompany: null,
				contractors: Object.freeze([]),
				selectContractor: null,
				forms: [],
				selectRow: {
					P_KIND: '一次全繳',
					BUD_DOC2: '無',
					Attachments: [],
					StopWorks: [],
					RefundBank: {},
					PaymentProof: {}
				},
				banks: Object.freeze(banksAry),
				dialogVisible: false,
				failReasonDialogVisible: false,
				bankAccountDialogVisible: false,
				paymentProofModalVisible: false,
				activeTab: 'first',
				rules1: Object.freeze({
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
					E_DATE2: [{ validator: checkE_DATE2 }]
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
			this.getCompanies();
			this.getContractor();
		},
		computed: {
			totalDays() {
				if (!this.selectRow.B_DATE2 || !this.selectRow.E_DATE2) return '';
				var date1 = new Date(this.selectRow.B_DATE2);
				var date2 = new Date(this.selectRow.E_DATE2);

				// 計算毫秒差異
				var diff = Math.abs(date2 - date1 + 1000 * 60 * 60 * 24);
				// 轉換為天數
				var dayDiff = Math.ceil(diff / (1000 * 60 * 60 * 24));

				return dayDiff;
			},
			filterAttachmentInfo() {
				return this.attachmentInfo.filter(item => item.PUB_COMP === this.selectRow.PUB_COMP);
			}
		},
		methods: {
			getDistrict() {
				axios.get('/Option/GetDistrict').then(res => {
					this.district = Object.freeze(res.data);
				});
			},
			getProjectCode() {
				axios.get('/Option/GetProjectCode').then(res => {
					this.projectCode = Object.freeze(res.data);
				});
			},
			getAttachmentInfo() {
				axios.get('/Option/GetAttachmentInfo').then(res => {
					this.attachmentInfo = Object.freeze(res.data);
				});
			},
			getCompanies() {
				axios.post('/Apply/GetMyCompanies', this.filter).then(res => {
					this.companies = Object.freeze(res.data);
				});
			},
			selectCompanyChange() {
				const result = this.companies.find(item => item.ID === this.selectCompany);
				this.selectRow.S_NAME = result.S_NAME;
				this.selectRow.S_G_NO = result.S_G_NO;
				this.selectRow.S_ADDR1 = result.S_ADDR1;
				this.selectRow.S_ADDR2 = result.S_ADDR2;
				this.selectRow.S_TEL = result.S_TEL;
				this.selectRow.S_B_NAM = result.S_B_NAM;
				this.selectRow.S_B_TIT = result.S_B_TIT;
				this.selectRow.S_B_ID = result.S_B_ID;
				this.selectRow.S_B_BDATE = result.S_B_BDATE;
				this.selectRow.S_B_BDATE2 = result.S_B_BDATE2;
				this.selectRow.S_C_NAM = result.S_C_NAM;
				this.selectRow.S_C_TIT = result.S_C_TIT;
				this.selectRow.S_C_ID = result.S_C_ID;
				this.selectRow.S_C_ADDR = result.S_C_ADDR;
				this.selectRow.S_C_TEL = result.S_C_TEL;
			},
			getContractor() {
				axios.post('/Apply/GetMyContractor', this.filter).then(res => {
					this.contractors = Object.freeze(res.data);
				});
			},
			selectContractorChange() {
				const result = this.contractors.find(item => item.ID === this.selectContractor);
				this.selectRow.R_NAME = result.R_NAME;
				this.selectRow.R_G_NO = result.R_G_NO;
				this.selectRow.R_ADDR1 = result.R_ADDR1;
				this.selectRow.R_ADDR2 = result.R_ADDR2;
				this.selectRow.R_TEL = result.R_TEL;
				this.selectRow.R_B_NAM = result.R_B_NAM;
				this.selectRow.R_B_TIT = result.R_B_TIT;
				this.selectRow.R_B_ID = result.R_B_ID;
				this.selectRow.R_B_BDATE = result.R_B_BDATE;
				this.selectRow.R_B_BDATE2 = result.R_B_BDATE2;
				this.selectRow.R_ADDR3 = result.R_ADDR3;
				this.selectRow.R_TEL1 = result.R_TEL1;
				this.selectRow.R_M_NAM = result.R_M_NAM;
				this.selectRow.R_C_NAM = result.R_C_NAM;
			},
			getForms() {
				const loading = this.$loading();
				axios
					.post('/Apply/GetForms', this.filter)
					.then(res => {
						this.forms = res.data;
						loading.close();
					})
					.catch(err => {
						loading.close();
						console.log(err);
					});
			},
			addForm() {
				this.mode = 'Add';
				// this.selectRow = {
				// 	SER_NO: 1,
				// 	P_KIND: '一次全繳',
				// 	KIND_NO: null,
				// 	BUD_DOC2: '無',
				// 	CreateUserName: document.querySelector('#hfUserName').value,
				// 	CreateUserEmail: document.querySelector('#hfUserEmail').value,
				// 	Attachments: [],
				// 	StopWorks: [],
				// 	RefundBank: {},
				// 	PaymentProof: {}
				// };

				this.selectRow = {
					SER_NO: 1,
					P_KIND: '一次全繳',
					BUD_DOC2: '無',
					PUB_COMP: true,
					TOWN_NO: 'M2',
					CreateUserName: document.querySelector('#hfUserName').value,
					CreateUserEmail: document.querySelector('#hfUserEmail').value,
					COMP_NAM: 'COMP_NAM',
					KIND_NO: '1',
					ADDR: 'ADDR',
					B_SERNO: 'B_SERNO',
					UTME: 123,
					UTMN: 456,
					LATLNG: '123456',
					STATE: 'STATE',
					EIACOMMENTS: 'EIACOMMENTS',
					RECCOMMENTS: 'RECCOMMENTS',
					S_NAME: 'S_NAME',
					S_G_NO: 'S_G_NO',
					S_ADDR1: 'S_ADDR1',
					S_ADDR2: 'S_ADDR2',
					S_TEL: 'S_TEL',
					S_B_NAM: 'S_B_NAM',
					S_B_TIT: 'S_B_TIT',
					S_B_ID: 'S_B_ID',
					S_B_BDATE2: '1985-07-14',
					S_C_NAM: 'S_C_NAM',
					S_C_TIT: 'S_C_TIT',
					S_C_ID: 'S_C_ID',
					S_C_ADDR: 'S_C_ADDR',
					S_C_TEL: 'S_C_TEL',
					R_NAME: 'R_NAME',
					R_G_NO: 'R_G_NO',
					R_ADDR1: 'R_ADDR1',
					R_ADDR2: 'R_ADDR2',
					R_TEL: 'R_TEL',
					R_B_NAM: 'R_B_NAM',
					R_B_TIT: 'R_B_TIT',
					R_B_ID: 'R_B_ID',
					R_B_BDATE2: '2024-01-01',
					R_ADDR3: 'R_ADDR3',
					R_M_NAM: 'R_M_NAM',
					R_C_NAM: 'R_C_NAM',
					R_TEL1: 'R_TEL1',
					MONEY: 1,
					C_MONEY: 2,
					PERCENT: 3,
					AREA: 1,
					VOLUMEL: null,
					B_DATE2: '2024-01-01',
					E_DATE2: '2024-01-31',
					StopWorks: [],
					RefundBank: {},
					PaymentProof: {},
					Attachments: []
				};

				this.dialogVisible = true;
				this.$nextTick(() => {
					// 清空附件
					for (let i = 1; i <= 8; i++) {
						const file = document.querySelector(`#file${i}`);
						if (file) file.value = '';
					}
				});
			},
			showModal(row) {
				this.mode = 'Update';
				this.selectRow = JSON.parse(JSON.stringify(row));
				this.dialogVisible = true;
				if (this.selectRow.FormStatus === 2) {
					this.failReasonDialogVisible = true;
				}
			},
			deleteFile(idx) {
				if (!confirm('是否確認刪除?')) return;
				this.selectRow.Attachments[idx].FileName = null;
			},
			sendForm() {
				this.$refs.form1.validate(valid => {
					if (!valid) {
						alert('欄位驗證錯誤，請檢查修正後重新送出');
						return false;
					}

					if (!confirm('是否確認繼續?')) return false;
					const formData = new FormData();
					for (const key in this.selectRow) {
						if (typeof this.selectRow[key] !== 'object') formData.append(key, this.selectRow[key]);
					}

					// 附件
					this.selectRow.Attachments = this.filterAttachmentInfo.map((item, idx) => ({
						ID: this.selectRow.Attachments[idx] ? this.selectRow.Attachments[idx].ID : 0,
						InfoID: item.ID
					}));
					for (let i = 0; i < this.selectRow.Attachments.length; i++) {
						formData.append(`Attachments[${i}].ID`, this.selectRow.Attachments[i].ID);
						formData.append(`Attachments[${i}].InfoID`, this.selectRow.Attachments[i].InfoID);
					}
					for (let i = 0; i < this.filterAttachmentInfo.length; i++) {
						const file = document.querySelector(`#file${i}`);
						if (file && file.files.length > 0) {
							formData.append('files', file.files[0]);
						} else {
							formData.append('files', new Blob([], { type: 'application/octet-stream' }));
						}
					}

					axios
						.post(`/Apply/${this.mode}Form`, formData)
						.then(res => {
							if (!res.data.Status) {
								alert(res.data.Message);
								return;
							}

							alert('申請資料已送出，繳款金額請依人工審核後之繳費單內容為主。');
							this.getForms();
							this.dialogVisible = false;
						})
						.catch(err => {
							alert('系統發生未預期錯誤');
							console.log(err);
						});
				});
			},
			copyRow(row) {
				this.mode = 'Add';
				this.selectRow = JSON.parse(JSON.stringify(row));
				this.selectRow.StopWorks.length = 0;
				const clearAry = ['SER_NO', 'AP_DATE', 'C_DATE'];
				for (const key of clearAry) {
					this.selectRow[key] = null;
				}

				this.dialogVisible = true;
				this.$nextTick(() => {
					// 清空附件
					for (let i = 1; i <= 8; i++) {
						const file = document.querySelector(`#file${i}`);
						if (file) file.value = '';
					}
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
			dialogClose() {
				// 清空附件
				for (let i = 0; i < this.filterAttachmentInfo.length; i++) {
					const file = document.querySelector(`#file${i}`);
					if (file) file.value = '';
				}
			},
			beforeCommand(row, cmd) {
				return {
					row,
					cmd
				};
			},
			handleCommand(arg) {
				const { row, cmd } = arg;
				switch (cmd) {
					case 'VIEW':
						this.showModal(row);
						break;
					case 'COPY':
						this.copyRow(row);
						break;
					case 'DOWNLOAD_PAYMENT':
						this.downloadPayment(row);
						break;
					case 'CALC':
						this.finalCalc(row);
						break;
					case 'DOWNLOAD_REPAYMENT':
						this.downloadRePayment(row);
						break;
					case 'UPLOAD_PAYMENT_PROOF':
						this.showPaymentProofModal(row);
						break;
					case 'BANK_ACCOUNT':
						this.showBankAccountModal(row);
						break;
					case 'DOWNLOAD_PROOF':
						this.downloadProof(row);
						break;
				}
			},
			downloadPayment(row) {
				const loading = this.$loading();
				axios
					.post('/Form/DownloadPayment', row, {
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
			finalCalc(row) {
				if (!confirm('是否確認提出結算申請?')) return;
				axios
					.post('/Form/FinalCalc', row)
					.then(res => {
						if (!res.data.Status) {
							alert(res.data.Message);
							return;
						}

						alert('結算申請已送出，請等候人工審核後 Email 通知');
						row.CalcStatus = 1;
					})
					.catch(err => {
						alert('系統發生未預期錯誤');
						console.log(err);
					});
			},
			downloadRePayment(row) {
				const loading = this.$loading();
				axios
					.post('/Form/DownloadRePayment', row, {
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
			showPaymentProofModal(row) {
				this.selectRow = JSON.parse(JSON.stringify(row));
				this.selectRow.PaymentProof = Object.assign({}, row.PaymentProof, {
					FormID: row.ID,
					File: null
				});
				this.paymentProofModalVisible = true;
			},
			deletePaymentProof() {
				if (!confirm('是否確認刪除?')) return false;
				this.selectRow.PaymentProof.ProofFile = null;
			},
			savePaymentProof() {
				const formData = new FormData();
				// 附件
				const file = document.querySelector(`#filePF`);
				if (file && file.files.length > 0) {
					formData.append('file', file.files[0]);
					this.selectRow.PaymentProof.File = file.files[0].name;
				}

				this.$refs.form3.validate((valid, object) => {
					if (!valid) {
						alert('欄位驗證錯誤，請檢查修正後重新送出');
						return false;
					}

					if (!confirm('是否確認繼續?')) return false;

					const loading = this.$loading();
					for (const key in this.selectRow.PaymentProof) {
						if (typeof this.selectRow.PaymentProof[key] !== 'object') formData.append(key, this.selectRow.PaymentProof[key]);
					}

					axios
						.post('/Form/UploadPaymentProof', formData)
						.then(res => {
							loading.close();
							if (!res.data.Status) {
								alert(res.data.Message);
								return;
							}

							alert('上傳成功');
							this.getForms();
							this.paymentProofModalVisible = false;
						})
						.catch(err => {
							loading.close();
							alert('系統發生未預期錯誤');
							console.log(err);
						});
				});
			},
			showBankAccountModal(row) {
				this.selectRow = JSON.parse(JSON.stringify(row));
				this.selectRow.RefundBank = Object.assign({}, row.RefundBank, {
					FormID: row.ID,
					File: null
				});
				this.bankAccountDialogVisible = true;
			},
			deleteBankPhoto() {
				if (!confirm('是否確認刪除?')) return false;
				this.selectRow.RefundBank.Photo = null;
			},
			saveBankAccount() {
				const formData = new FormData();
				// 附件
				const file = document.querySelector(`#fileBA`);
				if (file && file.files.length > 0) {
					formData.append('file', file.files[0]);
					this.selectRow.RefundBank.File = file.files[0].name;
				}
				this.$refs.form2.validate((valid, object) => {
					if (!valid) {
						alert('欄位驗證錯誤，請檢查修正後重新送出');
						return false;
					}

					if (!confirm('是否確認繼續?')) return false;

					const loading = this.$loading();
					for (const key in this.selectRow.RefundBank) {
						if (typeof this.selectRow.RefundBank[key] !== 'object') formData.append(key, this.selectRow.RefundBank[key]);
					}

					axios
						.post('/Form/UpdateBankAccount', formData)
						.then(res => {
							loading.close();
							if (!res.data.Status) {
								alert(res.data.Message);
								return;
							}

							alert('退款帳戶資料已儲存');
							this.getForms();
							this.bankAccountDialogVisible = false;
						})
						.catch(err => {
							loading.close();
							alert('系統發生未預期錯誤');
							console.log(err);
						});
				});
			},
			downloadProof(row) {
				const loading = this.$loading();
				axios
					.post('/Form/DownloadProof', row, {
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
		}
	});
});
