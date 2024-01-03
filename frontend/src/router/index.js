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
				{ path: '', redirect: '/form' },
				{
					path: '/form',
					name: 'form',
					component: (resolve) => require(['@/components/form/Form'], resolve)
				}
			]
		}
	]
});
