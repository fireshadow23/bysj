
var staticUrl = '';

requirejs.config({
    config: {

    },
    //为了在IE中正确检错，强制define/shim导出检测
    //enforceDefine: true,
    waitSeconds: 120,
    urlArgs: 'v=' + _v,
    baseUrl: '/js/',//暴露js库路径
    paths: {
        app: 'app',

        //***** lib *** //
        jquery: staticUrl + 'lib/jquery-1.9.1',

        //mustache
        mustache: staticUrl + 'lib/mustache/mustache',
        lui: staticUrl + 'lib/mustache/lui',

        //layer
        layer: staticUrl + 'lib/jquery.layer/layer',
        layerExt: staticUrl + 'lib/jquery.layer/extend/layer.ext',

        //jquery
        jqpaging: staticUrl + "lib/jquery/jquery.pagination",
        jqdatetimepicker: staticUrl + 'lib/bootstrap-datetimepicker/bootstrap-datetimepicker',
        jqdatetimepickerlg: staticUrl + 'lib/bootstrap-datetimepicker/locales/bootstrap-datetimepicker.zh-CN',
        jqform: staticUrl + 'lib/jquery/jquery.form',
        jqregx: staticUrl + 'lib/jquery/jquery.regx',
        jqcookie: staticUrl + 'lib/jquery/jquery.cookie',

        //bootstrap
        bootstrap: staticUrl + 'lib/bootstrap/bootstrap',

        //json
        json: staticUrl + 'lib/json/json2',

        //angularjs
        angularjs: staticUrl + 'lib/angularjs/angular-1.4.3',

        //***** app *** //
        login: staticUrl + 'app/login',
        register: staticUrl + 'app/register',
        home: staticUrl + 'app/home',
        payment: staticUrl + 'app/payment',
        usercenter: staticUrl + 'app/usercenter',
        store: staticUrl + 'app/store',

        //***** utils *** //
        tmpconfig: staticUrl + 'utils/tmpconfig'

    },
    bundles: {

    },
    shim: {
        jquery: {
            deps: ['json']
        },
        jqdatetimepicker: {
            deps: ['jquery']
        },
        jqform: {
            deps: ['jquery']
        },
        jqregx: {
            deps: ['jquery']
        },
        jqpaging: {
            deps: ['jquery']
        },
        layer: {
            deps: ['jquery'],
            exports: 'layer'
        },
        layerExt: {
            deps: ['jquery'],
            exports: 'layer'
        },
        bootstrap: {
            deps: ['jquery']
        },
        xyjconfig: {
            deps: ['jquery']
        }
    }
});

//hack console.log()
(function () {
    if (typeof window.console === undefined) {
        window.console = {};
        window.console.log = function () { };
    } else if (typeof window.console.log !== 'function') {
        window.console.log = function () { }
    }

});

//define global error handler
requirejs.onError = function (err) {
    //   throw err;
};

require(['common'])
