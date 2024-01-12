<template>
	<vxe-modal title="預覽圖片" v-model="visible" width="50%" height="90%" :lock-scroll="false" esc-closable resize show-footer>
		<template #default>
			<ul class="image-list">
                <li v-for="(item, idx) in data" :key="idx">
                    <img class="w100p" :src="`/api/Form/Download?f=${item}`">
                </li>
            </ul>
		</template>
		<template #footer>
			<el-button size="small" @click="visible = false">
				<i class="fa fa-ban"></i> 關 閉
			</el-button>
		</template>
	</vxe-modal>
</template>

<script>
export default {
	name: 'ImageModal',
	props: ['show', 'data'],
	data() {
		return {
			visible: false,
		};
	},
	methods: {
		
	},
	watch: {
		show: {
			handler(newValue, oldValue) {
				this.visible = newValue;
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

<style lang="scss" scoped>
.image-list {
    li {
        margin-bottom: 10px;
    }
}
</style>