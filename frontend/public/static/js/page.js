// 數字欄位不能打[-+e]
$(document).on('keydown wheel', 'input[type="number"]', function(e) {
    if(e.type === 'wheel') {
        $(this).blur();
        return;
    }
    var invalidChars = ['-', '+', 'e', 'ArrowUp', 'ArrowDown'];
    // 如果有 .integer 不可輸入小數
    var isInteger = $(this).data('type') === 'integer';
    if(isInteger) invalidChars.push('.');
    // 如果有 .allow-negative 可以輸入負值
    var allowNegative = $(this).hasClass('negative');
    if(allowNegative) invalidChars.splice(0, 1);

    if (invalidChars.indexOf(e.key) > -1 || (+e.target.value < 0 && e.key === '-')) {
        e.preventDefault();
    }
});