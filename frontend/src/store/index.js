import Vue from 'vue';
import Vuex from 'vuex';
import axios from 'axios';

Vue.use(Vuex);

const debug = process.env.NODE_ENV !== 'production';


const state = {
    user: Object.freeze({})
};

const getters = {
    currentUser: state => state.user
};

// actions
const actions = {
    getCurrentUser({ commit, state }) {
        return axios.get('api/Admin/GetCurrentUser');
    },
    setCurrentUser({ commit, state }, user) {
        commit('SET_CURRENT_USER', user);
    }
};

// mutations
const mutations = {
    ['SET_CURRENT_USER'](state, user) {
        state.user = Object.freeze(user);
    }
};


export default new Vuex.Store({
    state,
    getters,
    actions,
    mutations
});
