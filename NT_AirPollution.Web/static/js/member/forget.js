document.addEventListener('DOMContentLoaded', () => {
    new Vue({
        el: '#app',
        data() {
            return {
                form: {
                    Email: '',
                    Password: ''
                }
            };
        }
    });
});