import Vue from 'vue';
import App from './App.vue';
import ElementUI from 'element-ui';
import 'element-ui/lib/theme-chalk/index.css';
import './plugin-setup/element-ui';
import XEUtils from 'xe-utils';
import VXETable from 'vxe-table';
import 'vxe-table/lib/index.css';
import './plugin-setup/vxe-table';
import axios from 'axios';
import VueAxios from 'vue-axios';
import Vue2Editor from 'vue2-editor';
import router from './router';
import store from './store';
import authCheck from './router/auth';
import faCheckbox from './shared/FaCheckbox';

Vue.config.productionTip = false;

router.beforeEach(async (to, from, next) => {
    if (to.path === '/login') {
        next();
    }

    try {
        let user;
        // 如果記憶體中無使用者資料則重撈
        if (!store.getters.currentUser.ID) {
            const userPromise = await store.dispatch('getCurrentUser');
            if(!userPromise.data.ID) throw '';
            user = userPromise.data;
            store.commit('SET_CURRENT_USER', user);
        } else {
            user = store.getters.currentUser;
        }

        const pass = authCheck(to.name, user);
        if (pass) {
            next();
        } else {
            alert('呃，您無本功能使用權限。');
            next('/function/news');
        }
    } catch (err) {
        console.log(err);
        next('/login');
    }
});

Vue.use(ElementUI);
Vue.use(VXETable);
Vue.use(VueAxios, axios);
Vue.use(Vue2Editor);
Vue.component('fa-checkbox', faCheckbox);

new Vue({
    store,
    router,
    render: h => h(App)
}).$mount('#app');
