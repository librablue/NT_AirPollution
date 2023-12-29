document.addEventListener('DOMContentLoaded', () => {
	// 數字欄位不能打[-+e]
	document.addEventListener('keydown', function (e) {
		if (e.target.tagName === 'INPUT' && e.target.type === 'number') {
			if (e.type === 'wheel') {
				e.target.blur();
				return;
			}

			var invalidChars = ['-', '+', 'e', 'ArrowUp', 'ArrowDown'];

			// 如果輸入非允許字元停止
			if (invalidChars.indexOf(e.key) > -1) {
				e.preventDefault();
			}

			// 如果已經是負數且輸入負號，清空內容
			if (parseFloat(e.target.value) < 0 && e.key === '-') {
				e.target.value = '';
			}
		}
	});

	document.addEventListener('paste', function (e) {
		if (e.target.tagName === 'INPUT' && e.target.type === 'number') {
			e.preventDefault();
		}
	});
});
