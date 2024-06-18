import Vue from 'vue';
import Router from 'vue-router';

Vue.use(Router);

export default new Router({
	routes: [
		{
			path: '/login',
			name: 'login',
			component: (resolve) => require(['@/components/Login'], resolve)
		},
		{
			path: '/',
			component: (resolve) => require(['@/components/Main'], resolve),
			children: [
				{ path: '', redirect: '/function/news' },
				{
					path: '/function/form1',
					name: 'function_form1',
					component: (resolve) => require(['@/components/function/Form1'], resolve)
				},
                {
					path: '/function/form2',
					name: 'function_form2',
					component: (resolve) => require(['@/components/function/Form2'], resolve)
				},
                {
					path: '/function/news',
					name: 'function_news',
					component: (resolve) => require(['@/components/function/News'], resolve)
				},
                {
					path: '/function/air',
					name: 'function_air',
					component: (resolve) => require(['@/components/function/Air'], resolve)
				},
                {
					path: '/function/road',
					name: 'function_road',
					component: (resolve) => require(['@/components/function/Road'], resolve)
				},
                {
					path: '/function/rate',
					name: 'function_rate',
					component: (resolve) => require(['@/components/function/Rate'], resolve)
				},
                {
					path: '/function/download',
					name: 'function_download',
					component: (resolve) => require(['@/components/function/Download'], resolve)
				},
                {
					path: '/manage/user',
					name: 'manage_user',
					component: (resolve) => require(['@/components/manage/User'], resolve)
				}
			]
		}
	]
});
