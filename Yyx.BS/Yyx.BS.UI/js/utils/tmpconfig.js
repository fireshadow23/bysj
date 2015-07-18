/***************************
 *   配置文件
 ***************************/

; (function () {
    if (typeof xyj === 'undefined' || xyj == null) {
        xyj = {};
    }

    //todo:版本管理统一改为hash文件名
    xyj.templates = {
        //order
        orderDetail: {
            version: '?_v=2015010603',
            path: "/tmpl/userCenter/orderDetail.html"
        },
        orderInfo: {
            version: '?_v=2014120801',
            path: "/tmpl/userCenter/orderInfo.html"
        },
        paymentDetail: {
            version: '?_v=2014120801',
            path: "/tmpl/userCenter/paymentDetail.html"
        }
    }

    for (var k in xyj.templates) {
        //扩展
        var _obj = xyj.templates[k];
        _obj.fullpath = _obj.path + _obj.version;
    }
})();
