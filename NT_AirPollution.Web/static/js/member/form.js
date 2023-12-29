document.addEventListener('DOMContentLoaded', () => {
	new Vue({
		el: '#app',
		filters: {
			date: value => {
				if (!value || value === '0001-01-01T00:00:00') return '';
				return moment(value).format('YYYY-MM-DD');
			},
			datetime: value => {
				if (!value || value === '0001-01-01T00:00:00') return '';
				return moment(value).format('YYYY-MM-DD HH:mm');
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
			return {
				mode: '',
				loading: false,
				filter: {
					StartDate: '',
					EndDate: '',
					C_NO: null,
					PUB_COMP: null,
					CreateUserName: null,
					COMP_NAM: null
				},
				district: Object.freeze([]),
				forms: [],
				selectRow: {
					P_KIND: '一次全繳',
					BUD_DOC2: '無'
				},
				dialogVisible: false,
                activeTab: 'first',
				rules: Object.freeze({
					PUB_COMP: [{ required: true, message: '請選擇案件類型', trigger: 'change' }],
					TOWN_NO: [{ required: true, message: '請選擇鄉鎮分類', trigger: 'change' }],
					CreateUserName: [{ required: true, message: '請輸入申請人姓名', trigger: 'blur' }],
					CreateUserEmail: [{ required: true, message: '請輸入申請人電子信箱', trigger: 'blur' }],
					COMP_NAM: [{ required: true, message: '請輸入工程名稱', trigger: 'blur' }],
                    ProjectCode: [{ required: true, message: '請選擇工程類別', trigger: 'change' }],
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
			}
		},
		methods: {
			getDistrict() {
				axios.get('/Option/GetDistrict').then(res => {
					this.district = Object.freeze(res.data);
				});
			},
			getForms() {
				this.loading = true;
				axios
					.post('/Member/GetForms', this.filter)
					.then(res => {
						this.forms = res.data;
						this.loading = false;
					})
					.catch(err => {
						this.loading = false;
						console.log(err);
					});
			},
			addForm() {
				this.mode = 'Add';
				this.selectRow = {
					P_KIND: '一次全繳',
					BUD_DOC2: '無'
				};
				this.dialogVisible = true;
			},
			showModal(row) {
				this.mode = 'View';
				this.selectRow = JSON.parse(JSON.stringify(row));
				this.dialogVisible = true;
			},
			saveForm() {
				if (!confirm('是否確認繼續?')) return false;
				axios
					.post(`/Member/AddForm`, this.selectRow)
					.then(res => {
						if (!res.data.Status) {
							alert(res.data.Message);
							return;
						}

						this.getForms();
						alert('畫面資料已儲存。');
						this.dialogVisible = false;
					})
					.catch(err => {
						alert('系統發生未預期錯誤');
						console.log(err);
					});
			}
		}
	});
});
