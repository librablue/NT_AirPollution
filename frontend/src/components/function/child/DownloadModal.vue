<template>
    <el-dialog :title="`${mode}下載`" :visible.sync="visible">
        <el-form :model="download" :rules="rules" ref="form1" label-width="120px">
            <el-form-item label="建立日期" v-if="download.CreateDate">
                <el-tag size="small">{{ download.CreateDate | datetime }}</el-tag>
            </el-form-item>
            <el-form-item prop="Sort" label="排序">
                <el-input-number v-model="download.Sort" :min="1"></el-input-number>
            </el-form-item>
            <el-form-item prop="Title" label="標題">
                <el-input v-model="download.Title"></el-input>
            </el-form-item>
            <el-form-item prop="Filename" label="檔案上傳">
                <el-upload drag action="api/Download/Upload" :limit="1" :on-success="uploadSuccess" v-if="!download.Filename">
                    <i class="el-icon-upload"></i>
                    <div class="el-upload__text">將文件拖到此處，或<em>點擊上傳</em></div>
                </el-upload>
                <div class="file-list" v-else>
                    <span>
                        <a :href="download.Link" target="_blank">{{ download.Filename }}</a>
                    </span>
                    <span>
                        <a href="javascript:;" @click="deleteFile()"><i class="fa fa-trash-o"></i></a>
                    </span>
                </div>
            </el-form-item>
        </el-form>
        <span slot="footer">
            <el-button @click="visible = false"><i class="fa fa-ban"></i> 取 消</el-button>
            <el-button type="primary" @click="handleSave()"><i class="fa fa-floppy-o"></i> 確 定</el-button>
        </span>
    </el-dialog>
</template>

<script>
import { dateTime } from '@/mixins/filter';

export default {
    name: 'DownloadModal',
    props: ['show', 'id', 'mode'],
    mixins: [dateTime],
    data() {
        return {
            visible: false,
            download: {
                Id: 0,
                Title: '',
                Filename: '',
                Link: '',
                Sort: 1,
                CreateDate: '',
                ModifyDate: ''
            },
            rules: {
                Title: [{ required: true, message: '請輸入標題', trigger: 'blur' }],
                Sort: [{ required: true, message: '請輸入排序', trigger: 'blur' }]
            }
        };
    },
    methods: {
        getDownload() {
            this.axios
                .get('api/Download/GetDownload', {
                    params: {
                        id: this.id
                    }
                })
                .then(res => {
                    this.download = res.data;
                });
        },
        uploadSuccess(response, file, fileList) {
            this.download.Filename = response.filename;
            this.download.Link = response.url;
            this.$message.success('檔案上傳成功。');
        },
        handleSave() {
            this.$refs.form1.validate(valid => {
                if (!valid) return false;

                this.axios
                    .post(`api/Download/${this.mode === '新增' ? 'AddDownload' : 'UpdateDownload'}`, this.download)
                    .then(res => {
                        this.$message.success('畫面資料已儲存。');
                        this.$emit('save-success');
                        this.visible = false;
                    })
                    .catch(err => {
                        this.$message.error(err.response.data);
                    });
            });
        },
        deleteFile() {
            if (!confirm('是否確認刪除檔案?')) return false;

            this.download.Filename = '';
            this.download.Link = '';
        }
    },
    watch: {
        show: {
            handler(newValue, oldValue) {
                this.visible = newValue;
                if (this.visible) {
                    if (this.mode === '新增') {
                        this.download = {
                            Id: 0,
                            Title: '',
                            Filename: '',
                            Sort: 1,
                            CreateDate: '',
                            ModifyDate: ''
                        };
                    } else {
                        this.getDownload();
                    }
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
.file-list {
    span {
        display: inline-block;
        margin-right: 1em;
    }
}
</style>
