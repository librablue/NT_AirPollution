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
			return {
				district: Object.freeze([]),
				projectCode: Object.freeze([]),
				form: {
					SER_NO: 1,
					P_KIND: '一次全繳',
					BUD_DOC2: '無',
					Attachment: {}
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
					AREA_F: [{ required: true, message: '請輸入基地面積', trigger: 'blur' }],
					AREA_B: [{ required: true, message: '請輸入建築面積', trigger: 'blur' }],
					AREA2: [{ required: true, message: '請輸入總樓地板面積', trigger: 'blur' }],
					PERC_B: [{ required: true, message: '請輸入遮蔽率', trigger: 'blur' }],
					B_DATE2: [{ required: true, message: '請輸入預計施工開始日期', trigger: 'blur' }],
					E_DATE2: [{ validator: checkE_DATE2 }]
				})
			};
		},
		mounted() {
			this.getDistrict();
			this.getProjectCode();
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
				AREA_F: 4,
				AREA_B: 5,
				AREA2: 6,
				PERC_B: 7,
				B_DATE2: '2024-01-01',
				E_DATE2: '2024-01-31',
				Attachment: {}
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
			deleteFile(idx) {
				if (!confirm('是否確認刪除?')) return;
				this.selectRow.Attachment[`File${idx}`] = null;
			},
			sendForm() {
				this.form.Captcha = grecaptcha.getResponse();
				this.$refs.form.validate(valid => {
					if (!valid) {
						return false;
					}

					if (!confirm('是否確認繼續?')) return false;
					const formData = new FormData();
					for (const key in this.form) {
						if (typeof this.form[key] !== 'object') formData.append(key, this.form[key]);
					}
					// 附件
					for (let i = 1; i <= 8; i++) {
						const file = document.querySelector(`#file${i}`);
						if (file.files.length > 0) formData.append(`file${i}`, file.files[0]);
					}

					axios
						.post(`/Form/Create`, formData)
						.then(res => {
							if (!res.data.Status) {
								alert(res.data.Message);
								return;
							}

							alert('畫面資料已儲存。');
							location.reload();
						})
						.catch(err => {
							alert('系統發生未預期錯誤');
							console.log(err);
						});
				});
			}
		}
	});
});
