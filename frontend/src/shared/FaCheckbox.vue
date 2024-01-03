<script>
export default {
	name: 'fa-checkbox',
	props: ['value', 'disabled'],
	computed: {
		cssProps() {
			return {
				'--letter-spacing': this.$slots.default ? '6px' : '0'
			};
		}
	},
	methods: {
		handleChange(e) {
			this.$emit('input', e.target.checked);
			this.$emit('change', e.target.checked);
		}
	}
};
</script>
<template>
    <label :style="cssProps">
        <input type="checkbox" :checked="value" @change="handleChange" :disabled="disabled" />
        <span><slot></slot></span>
    </label>
</template>

<style scoped>
input[type='checkbox'] {
    display: none;
}
input[type='checkbox'] + span:before {
    font-family: FontAwesome;
    display: inline-block;
}
input[type='checkbox'] + span:before {
    content: '\f096';
    font-size: 24px;
    font-weight: 500 !important;
    vertical-align: middle;
}
input[type='checkbox'][disabled] + span:before {
    color: #c1c1c1;
}
input[type='checkbox'] + span:before {
    letter-spacing: var(--letter-spacing);
}
input[type='checkbox']:checked + span:before {
    content: '\f046';
}
</style>
