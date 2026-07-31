import Vue from 'vue';
import store from '@/store';
import { moduleAuthCheck, functionAuthCheck } from '@/router/auth';

Vue.directive('auth-link', {
	inserted(el, binding, vnode, old) {
		if (el.classList.contains('dropdown')) {
			// 尋找子選單
			const child = el.querySelector('.dropdown-menu');
			// 如果沒有子選單就自我移除(divider是分隔線)
			if (child.querySelectorAll('li:not(.divider)').length === 0) {
				el.remove();
			}
			return;
		}

		const user = store.getters.getCurrentUser;
		const pass = moduleAuthCheck(vnode.componentOptions.propsData.to, user);
		if (!pass) {
			el.remove();
		}
	}
});

Vue.directive('auth', {
	inserted(el, binding, vnode, old) {
		const user = store.getters.getCurrentUser;
		const path = vnode.context.$route.path;
		const key = binding.value;
		const pass = functionAuthCheck(path, key, user);
		if (!pass) {
			el.remove();
		}
	}
});

Vue.directive('auth-else', {
	inserted(el, binding, vnode, old) {
		const user = store.getters.getCurrentUser;
		const path = vnode.context.$route.path;
		const key = binding.value;
		const pass = functionAuthCheck(path, key, user);
		if (pass) {
			el.remove();
		}
	}
});

Vue.directive('uppercase', {
	// 在元素綁定後
	inserted(el) {
		el.addEventListener('change', event => {
			// 將輸入的值轉換為大寫
			event.target.value = event.target.value.toUpperCase();
			// 手動觸發 input 事件，確保 v-model 更新
			event.target.dispatchEvent(new Event('change'));
		});
	}
});

Vue.directive('trim', {
	// 在元素綁定後
	inserted(el) {
		el.addEventListener('change', event => {
			event.target.value = event.target.value.trim(); // 去除空白
			event.target.dispatchEvent(new Event('change')); // 手動觸發事件，確保 v-model 更新
		});
	}
});

// 正數小數
Vue.directive('decimal', {
	bind(el) {
		let input;
		const innerInput = el.querySelector('.el-input__inner');
		if(innerInput) {
			input = innerInput;
		}
		else {
			input = el;
		}

		input.addEventListener('keydown', e => {
			if(e.key === 'e') {
				e.preventDefault();
			}
		});
		input.addEventListener('input', () => {
			// 使用正則表達式過濾輸入的內容，只允許小數數字
			let value = input.value;
			const regex = /^\d*\.?\d{0,2}$/;

			if (!regex.test(value)) {
				// 移除不符合格式的字元
				value = value.replace(/[^0-9.]/g, ''); // 只保留數字和小數點
				value = value.replace(/(\..*)\./g, '$1'); // 只允許一個小數點
				value = value.replace(/^0+(\d)/, '$1'); // 移除前導零

				// 更新 input 的值
				input.value = value;
			}
		});
	}
});

// 可負數小數
Vue.directive('decimal-neg', {
	bind(el) {
		let input;
		const innerInput = el.querySelector('.el-input__inner');
		if(innerInput) {
			input = innerInput;
		}
		else {
			input = el;
		}

		input.addEventListener('keydown', e => {
			if(e.key === 'e') {
				e.preventDefault();
			}
		});
		input.addEventListener('input', () => {
			// 使用正則表達式過濾輸入的內容，允許負數的小數
			let value = input.value;
			const regex = /^-?\d*\.?\d{0,2}$/;

			if (!regex.test(value)) {
				// 移除不符合格式的字元
				value = value.replace(/[^0-9.-]/g, ''); // 只保留數字、小數點和負號
				value = value.replace(/(\..*)\./g, '$1'); // 只允許一個小數點
				value = value.replace(/^-+/, '-'); // 負號只允許在最前面
				value = value.replace(/^-0+(\d)/, '-$1'); // 移除負數的前導零
				value = value.replace(/^0+(\d)/, '$1'); // 移除正數的前導零

				// 更新 input 的值
				input.value = value;
			}
		});
	}
});

// 整數
Vue.directive('integer', {
	bind(el) {
		let input;
		const innerInput = el.querySelector('.el-input__inner');
		if(innerInput) {
			input = innerInput;
		}
		else {
			input = el;
		}

		input.addEventListener('input', () => {
			let value = input.value;
			// 使用正則表達式來限制輸入內容為正整數
			const regex = /^[1-9]\d*$/;

			if (!regex.test(value)) {
				// 移除非數字字符
				value = value.replace(/[^0-9]/g, ''); // 只允許數字
				value = value.replace(/^0+/, ''); // 移除前導零

				// 更新 input 的值
				input.value = value;
			}
		});
	}
});

// 可負數整數
Vue.directive('integer-neg', {
	bind(el) {
		let input;
		const innerInput = el.querySelector('.el-input__inner');
		if(innerInput) {
			input = innerInput;
		}
		else {
			input = el;
		}

		input.addEventListener('input', () => {
			let value = input.value;
			// 使用正則表達式來限制輸入內容為整數（包括負整數）
			const regex = /^-?\d*$/;

			if (!regex.test(value)) {
				// 移除非數字和負號的字符
				value = value.replace(/[^0-9-]/g, ''); // 只允許數字和負號
				value = value.replace(/^-+/, '-'); // 負號只允許在最前面
				value = value.replace(/^0+(\d)/, '$1'); // 移除正數的前導零

				// 更新 input 的值
				input.value = value;
			}
		});
	}
});
