const dateTime = {
    filters: {
        date: value => {
            if (!value || value === '0001-01-01T00:00:00') return '';
            return moment(value).format('YYYY-MM-DD');
        },
        datetime: value => {
            if (!value || value === '0001-01-01T00:00:00') return '';
            return moment(value).format('YYYY-MM-DD HH:mm');
        }
    }
};

const comma = {
    filters: {
        comma: value => {
            if (!value && value !== 0) return '';
            return ('' + value).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        },
        commaPrecision2: value => {
            if (!value && value !== 0) return '';
            return ('' + new Number(value).toFixed(2)).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        },
        commaPrecision2EmptyIfZero: value => {
            if (!value) return '';
            return ('' + new Number(value).toFixed(2)).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        }
    }
};

const form = {
    filters: {
        formStatus(val) {
            switch (val) {
                case 0:
                    return '未送審';
                case 1:
                    return '審理中';
                case 2:
                    return '待補件';
                case 3:
                    return '通過待繳費';
                case 4:
                    return '繳費完成';
                case 5:
                    return '免繳費';
                default:
                    return '';
            }
        },
        calcStatus: value => {
            switch (value) {
                case 0:
                    return '未申請';
                case 1:
                    return '審理中';
                case 2:
                    return '待補件';
                case 3:
                    return '通過待繳費';
                case 4:
                case 5:
                    return '通過待退費';
                case 6:
                    return '繳退費完成';
                default:
                    return '';
            }
        },
        verifyStage: value => {
            switch (value) {
                case 0:
                    return '未送審';
                case 1:
                    return '申請中';
                case 2:
                    return '初審通過';
                case 3:
                    return '複審通過';
                default:
                    return '';
            }
        },
        area1(val) {
            switch (val) {
                case 1:
                    return '建築面積';
                case 2:
                    return '總樓地板面積';
                case 3:
                    return '施工面積';
                case 4:
                    return '隧道面積';
                case 5:
                    return '橋面面積';
                case 6:
                    return '開發面積';
            }
        },
        area3(val) {
            switch (val) {
                case 1:
                    return 'M2';
                case 2:
                    return '公頃';
            }
        },
        projectCode(val) {
            switch (val) {
                case '1':
                    return '1. 建築（房屋）工程-鋼筋混凝土構造';
                case '2':
                    return '2. 建築（房屋）工程-鋼骨結構';
                case '3':
                    return '3. 建築（房屋）工程-拆除';
                case '4':
                    return '4. 道路（隧道）工程-道路';
                case '5':
                    return '5. 道路（隧道）工程-隧道';
                case '6':
                    return '6. 管線開挖工程';
                case '7':
                    return '7. 橋樑工程';
                case '8':
                    return '8. 區域開發工程-社區';
                case '9':
                    return '9. 區域開發工程-工業區';
                case 'A':
                    return 'A. 區域開發工程-遊樂區';
                case 'B':
                    return 'B. 疏濬工程';
                case 'Z':
                    return 'Z. 其他工程';
            }
        }
    }
};

const status = {
    filters: {
        status: value => {
            if (value === null) return '';
            return value ? '啟用' : '停用';
        }
    }
};

export { dateTime, comma, form, status };
