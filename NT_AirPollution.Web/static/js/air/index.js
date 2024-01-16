document.addEventListener('DOMContentLoaded', () => {
    new Vue({
        el: '#app',
        filters: {
            date: value => {
                if (!value || value === '0001-01-01T00:00:00') return '';
                return moment(value).format('YYYY-MM-DD');
            },
            workStatus: value => {
                switch (value) {
                    case 1:
                        return '施工中';
                    case 2:
                        return '停工中';
                    case 3:
                        return '已完工';
                    default:
                        return '';
                }
            }
        },
        data() {
            const checkDate = (rule, value, callback) => {
                if (!value) {
                    callback(new Error('請選擇執行結束日期'));
                }
                if (this.selectRow.StartDate && this.selectRow.EndDate && moment(value).isSameOrBefore(this.selectRow.StartDate)) {
                    callback(new Error('結束日期不得早於起始日期'));
                }
                callback();
            };
            return {
                formID: null,
                mode: '',
                loading: false,
                form: {},
                airs: [],
                selectRow: {},
                newFiles: [],
                dialogVisible: false,
                rules: Object.freeze({
                    StartDate: [{ required: true, message: '請選擇執行起始日期', trigger: 'blur' }],
                    EndDate: [{ validator: checkDate }],
                    Position: [{ required: true, message: '請輸入執行地點', trigger: 'blur' }],
                    Method: [{ required: true, message: '請施入執行措施', trigger: 'blur' }]
                })
            };
        },
        mounted() {
            var url = new URL(location.href);
            this.formID = url.searchParams.get('id');
            if (!this.formID) {
                location.href = `${document.baseURI}Manage/Form`;
                return;
            }
            this.getFormByID();
            this.getAirsByForm();
        },
        methods: {
            getFormByID() {
                axios
                    .get(`/Manage/GetFormByID`, {
                        params: {
                            id: this.formID
                        }
                    })
                    .then(res => {
                        if (!res.data.Status) {
                            alert(res.data.Message);
                            location.href = `${document.baseURI}Manage/Form`;
                            return;
                        }
                        this.form = res.data.Message;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            getAirsByForm() {
                this.loading = true;
                axios
                    .get('/Air/GetAirsByForm', {
                        params: {
                            formID: this.formID
                        }
                    })
                    .then(res => {
                        this.loading = false;
                        if (!res.data.Status) {
                            return;
                        }
                        this.airs = res.data.Message;
                    })
                    .catch(err => {
                        this.loading = false;
                        console.log(err);
                    });
            },
            showModal(row) {
                if (row) {
                    this.mode = 'Update';
                    this.selectRow = JSON.parse(JSON.stringify(row));
                } else {
                    this.mode = 'Add';
                    this.selectRow = {
                        AirFiles: []
                    };
                }

                this.newFiles.length = 0;
                this.dialogVisible = true;
            },
            addFile() {
                this.newFiles.push({});
            },
            deleteFile(idx) {
                if (!confirm('是否確認刪除?')) return;
                this.selectRow.AirFiles.splice(idx, 1);
            },
            sendForm() {
                this.$refs.form.validate(valid => {
                    if (!valid) {
                        alert('欄位驗證錯誤，請檢查修正後重新送出');
                        return false;
                    }

                    if (!confirm('是否確認繼續?')) return false;

                    const formData = new FormData();
                    formData.append('FormID', this.formID);
                    for (const key in this.selectRow) {
                        if (typeof this.selectRow[key] !== 'object') formData.append(key, this.selectRow[key]);
                    }
                    // 附件
                    for (let i = 0; i < this.selectRow.AirFiles.length; i++) {
                        formData.append(`AirFiles[${i}].FileName`, this.selectRow.AirFiles[i].FileName);
                    }
                    for (let i = 0; i < this.newFiles.length; i++) {
                        const file = document.querySelector(`#file${i}`);
                        if (file && file.files.length > 0) formData.append(`file[${i}]`, file.files[0]);
                    }

                    axios
                        .post(`/Air/${this.mode}Air`, formData)
                        .then(res => {
                            if (!res.data.Status) {
                                alert(res.data.Message);
                                return;
                            }

                            this.getAirsByForm();
                            this.dialogVisible = false;
                        })
                        .catch(err => {
                            alert('系統發生未預期錯誤');
                            console.log(err);
                        });
                });
            },
            deleteAir(row) {
                if (!confirm('是否確認刪除?')) return;
                axios
                    .post('/Air/DeleteAir', row)
                    .then(res => {
                        if (!res.data.Status) {
                            alert(res.data.Message);
                            return;
                        }

                        this.getAirsByForm();
                    })
                    .catch(err => {
                        alert('系統發生未預期錯誤');
                        console.log(err);
                    });
            }
        }
    });
});
