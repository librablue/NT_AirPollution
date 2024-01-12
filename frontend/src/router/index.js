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
				{ path: '', redirect: '/function/form' },
				{
					path: '/function/form',
					name: 'function_form',
					component: (resolve) => require(['@/components/function/Form'], resolve)
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
				}
			]
		}
	]
});
