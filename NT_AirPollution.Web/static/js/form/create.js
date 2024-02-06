document.addEventListener('DOMContentLoaded', () => {
	new Vue({
		el: '#app',
		filters: {
			date: value => {
				if (!value || value === '0001-01-01T00:00:00') return '';
				return moment(value).format('YYYY-MM-DD');
			}
		},
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
			return {
				district: Object.freeze([]),
				projectCode: Object.freeze([]),
				attachmentInfo: Object.freeze([]),
				form: {
					SER_NO: 1,
					P_KIND: '一次全繳',
					BUD_DOC2: '無',
					Attachments: []
				},
				activeTab: 'first',
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
					E_DATE2: [{ validator: checkE_DATE2 }]
				})
			};
		},
		mounted() {
			this.getDistrict();
			this.getProjectCode();
			this.getAttachmentInfo();
			this.form = {
				SER_NO: 1,
				P_KIND: '一次全繳',
				BUD_DOC2: '無',
				PUB_COMP: true,
				TOWN_NO: 'M2',
				CreateUserName: '申請人',
				CreateUserEmail: 'abc@gmail.com',
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
				Attachments: []
			};
		},
		computed: {
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
			filterAttachmentInfo() {
				return this.attachmentInfo.filter(item => item.PUB_COMP === this.form.PUB_COMP);
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
			filterAttachments(infoID) {
				return this.form.Attachments.filter(item => item.InfoID === infoID);
			},
			addFile(infoID) {
				const lastID = this.form.Attachments.length === 0 ? 0 : this.form.Attachments[this.form.Attachments.length - 1].ID;
				this.form.Attachments.push({
					ID: lastID + 1,
					InfoID: infoID,
                    FileName: null
				});
			},
			deleteFile(row) {
				if (!confirm('是否確認刪除?')) return;
				const findIdx = this.form.Attachments.findIndex((item, idx) => item.ID === row.ID);
				this.form.Attachments.splice(findIdx, 1);
			},
			uploadAttachment(e, row) {
				if (e.target.files.length === 0) {
					alert('請選擇檔案');
					return false;
				}

				const ext = e.target.files[0].name.split('.').pop();
				const allowExt = ['doc', 'docx', 'pdf', 'jpg', 'jpeg', 'png'];
				if (!allowExt.includes(ext)) {
					alert('附件只允許上傳 doc/docx/pdf/jpg/png 等文件');
					return false;
				}

				const formData = new FormData();
				formData.append('file', e.target.files[0]);
				axios
					.post('/Form/UploadAttachment', formData)
					.then(res => {
						if (!res.data.Status) {
							alert(res.data.Message);
							return;
						}

                        row.FileName = res.data.Message;
                        this.$message.success('檔案上傳完成');
					})
					.catch(err => {
						alert('系統發生未預期錯誤');
						console.log(err);
					});
			},
			sendForm() {
				this.form.Captcha = grecaptcha.getResponse();
				this.$refs.form.validate(valid => {
					if (!valid) {
						alert('欄位驗證錯誤，請檢查修正後重新送出');
						return false;
					}

					if (!confirm('是否確認繼續?')) return false;
					axios
						.post('/Form/Create', this.form)
						.then(res => {
							if (!res.data.Status) {
								grecaptcha.reset();
								alert(res.data.Message);
								return;
							}

							alert('申請資料已送出。\n系統將於3分鐘內傳送認證信給您，請點選郵件中的連結進行驗證。\n完成驗證之案件才會進入審核程序。');
							alert('繳款金額請依人工審核後之繳費單內容為主。');
							location.href = `${document.baseURI}Home/Index`;
						})
						.catch(err => {
							grecaptcha.reset();
							alert('系統發生未預期錯誤');
							console.log(err);
						});
				});
			}
		}
	});
});
