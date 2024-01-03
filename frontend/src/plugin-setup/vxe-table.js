import XEUtils from 'xe-utils';
import VXETable from 'vxe-table';
import zhTWLocat from 'vxe-table/lib/locale/lang/zh-TW';

VXETable.setup({
    i18n: (key, value) => XEUtils.get(zhTWLocat, key)
});