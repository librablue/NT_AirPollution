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
            const checkDate = (rule, value, callback) => {
                if (!value) {
                    callback(new Error('請選擇執行結束日期'));
                }
                if (this.promise.StartDate && this.promise.EndDate && moment(value).isSameOrBefore(this.promise.StartDate)) {
                    callback(new Error('結束日期不得早於起始日期'));
                }
                callback();
            };
            return {
                formID: null,
                mode: '',
                loading: false,
                form: {},
                promise: null,
                promiseDialogVisible: false,
                promiseRules: Object.freeze({
                    StartDate: [{ required: true, message: '請選擇執行起始日期', trigger: 'blur' }],
                    EndDate: [{ validator: checkDate }]
                })
            };
        },
        mounted() {
            var url = new URL(location.href);
            this.formID = url.searchParams.get('id');
            if (!this.formID) {
                location.href = '/Manage/Form';
                return;
            }
            this.getFormByID();
            this.getPromiseByForm();
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
                            location.href = '/Manage/Form';
                            return;
                        }
                        this.form = res.data.Message;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            },
            getPromiseByForm() {
                this.loading = true;
                axios
                    .get('/Road/GetPromiseByForm', {
                        params: {
                            formID: this.formID
                        }
                    })
                    .then(res => {
                        this.loading = false;
                        if (!res.data.Status) {
                            return;
                        }
                        this.promise = res.data.Message;
                    })
                    .catch(err => {
                        this.loading = false;
                        console.log(err);
                    });
            },
            addPromise() {
                this.promise = {
                    FormID: this.formID,
                    Times: 1
                };
                this.promiseDialogVisible = true;
            },
            sendPromise() {},
            sendForm() {
                this.$refs.form.validate(valid => {
                    if (!valid) {
                        alert('欄位驗證錯誤，請檢查修正後重新送出');
                        return false;
                    }

                    if (!confirm('是否確認繼續?')) return false;

                    var url = new URL(location.href);
                    var id = url.searchParams.get('id');
                    const formData = new FormData();
                    formData.append('FormID', id);
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

                            this.getAirsByForm(id);
                            this.dialogVisible = false;
                        })
                        .catch(err => {
                            alert('系統發生未預期錯誤');
                            console.log(err);
                        });
                });
            }
            // deleteAir(row) {
            //     if (!confirm('是否確認刪除?')) return;
            //     axios
            //         .post('/Air/DeleteAir', row)
            //         .then(res => {
            //             if (!res.data.Status) {
            //                 alert(res.data.Message);
            //                 return;
            //             }

            //             var url = new URL(location.href);
            //             var id = url.searchParams.get('id');
            //             const formData = new FormData();
            //             this.getAirsByForm(id);
            //         })
            //         .catch(err => {
            //             alert('系統發生未預期錯誤');
            //             console.log(err);
            //         });
            // }
        }
    });
});
