document.addEventListener('DOMContentLoaded', () => {
	// 數字欄位不能打[-+e]
	const numberInput = document.querySelectorAll('input[type="number"]');
	numberInput.forEach(item => {
		item.addEventListener('keydown wheel paste', function (e) {
			if (e.type === 'wheel') {
				$(this).blur();
				return;
			}
			var invalidChars = ['-', '+', 'e', 'ArrowUp', 'ArrowDown'];
			// 如果輸入非允許字元停止
			if (invalidChars.indexOf(e.key) > -1) {
				e.preventDefault();
			}
			// 如果已經是負數且輸入負號，清空內容
			if (+e.target.value < 0 && e.key === '-') {
				e.target.value = '';
			}
		});
	});
});
