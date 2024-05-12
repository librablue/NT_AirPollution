<template>
    <div class="main">
        <h1>下載專區</h1>
        <el-form inline>
            <el-form-item>
                <el-button type="primary" size="small" @click="handleAdd()"><i class="fa fa-plus"></i> 新 增</el-button>
            </el-form-item>
        </el-form>
        <vxe-table :data="downloads" size="small" :sort-config="{trigger: 'cell', defaultSort: {field: 'CreateDate', order: 'desc'}}">
            <vxe-table-column title="功能" align="center" width="120px">
                <template v-slot="{ row }">
                    <el-button type="primary" size="mini" icon="el-icon-edit" circle @click="handleEdit(row)"></el-button>
                    <el-button type="danger" size="mini" icon="el-icon-delete" circle @click="handleDelete(row)"></el-button>
                </template>
            </vxe-table-column>
            <vxe-table-column field="Id" title="系統編號" width="100px" align="center"></vxe-table-column>
            <vxe-table-column field="Title" title="標題"></vxe-table-column>
            <vxe-table-column title="檔案">
                <template v-slot="{ row }">
                    <a :href="row.Link" target="_blank">{{ row.Filename }}</a>
                </template>
            </vxe-table-column>
            <vxe-table-column field="Sort" title="排序" width="80px" align="center" sortable></vxe-table-column>
            <vxe-table-column field="CreateDate" title="建立日期" width="180px" align="center" sortable>
                <template v-slot="{ row }">{{ row.CreateDate | datetime }}</template>
            </vxe-table-column>
        </vxe-table>
        <DownloadModal :show.sync="dialogVisible" :id="selectId" :mode="mode" @save-success="saveSuccess" />
    </div>
</template>

<script>
import DownloadModal from '@/components/function/child/DownloadModal';
import { dateTime } from '@/mixins/filter';

export default {
    name: 'Download',
    mixins: [dateTime],
    components: {
        DownloadModal
    },
    data() {
        return {
            dialogVisible: false,
            downloads: [],
            mode: '',
            selectId: 0
        };
    },
    mounted() {
        this.getDownloads();
    },
    methods: {
        getDownloads() {
            this.axios.get('api/Download/GetDownloads').then(res => {
                this.downloads = res.data;
            });
        },
        handleAdd() {
            this.mode = '新增';
            this.selectId = 0;
            this.dialogVisible = true;
        },
        handleEdit(row) {
            this.mode = '編輯';
            this.selectId = row.Id;
            this.dialogVisible = true;
        },
        saveSuccess() {
            this.getDownloads();
        },
        handleDelete(row) {
            if (!confirm('刪除後資料無法回復，是否確定繼續?')) return false;
            this.axios
                .post('api/Download/DeleteDownload', {
                    id: row.Id
                })
                .then(res => {
                    this.getDownloads();
                    this.$message.success('資料已刪除。');
                })
                .catch(err => {
                    this.$message.error(err.response.data);
                });
        }
    }
};
</script>
