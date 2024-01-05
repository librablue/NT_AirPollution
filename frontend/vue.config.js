const path = require('path');
function resolve(dir) {
    return path.join(__dirname, dir);
}

module.exports = {
    lintOnSave: false,
    publicPath: process.env.NODE_ENV === 'production' ? '/NT_AirPollutionMgr' : '/',
    assetsDir: 'static',
    chainWebpack: config => {
        config.resolve.alias
            .set('@', resolve('src'))
            .set('assets', resolve('assets'))
            .set('public', resolve('public'));
    },
    devServer: {
        proxy: {
            '^/api': {
                target: 'http://localhost:14350/'
            }           
        }
    }
};
