<template>
    <div id="app">
        <transition name="fade" mode="out-in">
            <router-view :key="$route.fullPath"></router-view>
        </transition>
    </div>
</template>

<script>
export default {
    name: 'App'
};
</script>

<style lang="scss">
@import 'assets/scss/config';
</style>
