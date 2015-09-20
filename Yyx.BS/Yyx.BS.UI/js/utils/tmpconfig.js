/***************************
 *   配置文件
 ***************************/

; (function () {
    if (typeof yyx === 'undefined' || yyx == null) {
        yyx = {};
    }

    //todo:版本管理统一改为hash文件名
    yyx.templates = {
        //order
        orderDetail: {
            version: '?_v=2015090801',
            path: "/tmpl/userCenter/orderDetail.html"
        },
        orderList: {
            version: '?_v=2015090801',
            path: "/tmpl/store/orderList.html"
        }
    }

    for (var k in yyx.templates) {
        //扩展
        var _obj = yyx.templates[k];
        _obj.fullpath = _obj.path + _obj.version;
    }
})();
