define(['jquery', 'mustache', 'lui', 'layer', 'jqpaging', 'jqform', 'jqregx', 'bootstrap', 'tmpconfig'], function (jq) {

    function initIndex() {
        //账号密码登录
        $("form[name='mobilePswLoginForm']").submit(function () {
            $(this).ajaxSubmit({
                success: function (data) {
                    if (data.status == 'true') {
                        window.top.window.location = "/Store/Order";
                    } else {
                        //提示登录失败
                        if (data.msg != "") {
                            layer.msg(data.msg, 2, 8);
                        }
                        else {
                            layer.msg("登陆失败！", 2, 8);
                        }
                    }
                },
                error: function (data) {
                    layer.msg("登录失败!", 2, 8);
                }
            });
            return false;
        });
    }

    function initOrder() {
        //定时加载
        setInterval(function () {
            GetOrderTotalCount();
        }, 5000);

        function GetOrderTotalCount() {
            $.ajax({
                url: '/Store/GetOrderTotalCount',
                type: 'POST',
                cache: false,
                dataType: 'json',
                timeout: 1000 * 60,
                success: function (data) {
                    if (data.status == "true") {
                        $("#totalCountPaid").html(data.totalCountPaid);
                        $("#totalCountTryCancel").html(data.totalCountTryCancel);
                    }
                }
            });
        }

        var pIndex = 0;
        var pSize = 5;
        var pStatus = "Paid";

        $("#labpaid").click(function () {
            GetOrderList(0, pSize, "Paid");
            $(this).parent().siblings().removeClass("active");
            $(this).parent().addClass("active");
        });
        $("#labserversent").click(function () {
            GetOrderList(0, pSize, "ServerSent");
            $(this).parent().siblings().removeClass("active");
            $(this).parent().addClass("active");
        });
        $("#labtrycancel").click(function () {
            GetOrderList(0, pSize, "TryCancel");
            $(this).parent().siblings().removeClass("active");
            $(this).parent().addClass("active");
        });
        $("#labcomplete").click(function () {
            GetOrderList(0, pSize, "Complete");
            $(this).parent().siblings().removeClass("active");
            $(this).parent().addClass("active");
        });


        function GetOrderList(pageIndex, pageSize, orderStatus) {
            pIndex = pageIndex;
            pStatus = orderStatus;
            $.ajax({
                url: '/Store/GetOrderList',
                data: $.param({ pageIndex: pageIndex, pageSize: pageSize, orderStatus: orderStatus }),
                type: 'POST',
                cache: false,
                dataType: 'json',
                timeout: 1000 * 60,
                success: function (data) {
                    if (data.status == "true") {
                        console.log(data);
                        $$.Template.render("#orderList", yyx.templates.orderList.fullpath, data, true, function (success, e) {
                            if (!success) return;

                            renderPaging(pageIndex, pageSize, data.totalcount);
                            bindOrderListEvent();
                        });
                        if (orderStatus == "Paid") {
                            $("#totalCountPaid").html(data.totalcount);
                        }
                        else if (orderStatus == "TryCancel") {
                            $("#totalCountTryCancel").html(data.totalcount);
                        }
                    }
                }
            });
        }

        function renderPaging(pageIndex, pageSize, totalcount) {
            $("#Pagination").pagination(totalcount, {
                callback: PageCallback,
                prev_text: '« 上一页',
                next_text: '下一页 »',
                items_per_page: pageSize,
                num_display_entries: 6,//连续分页主体部分分页条目数  
                current_page: pageIndex,//当前页索引  
                num_edge_entries: 2//两侧首尾分页条目数  
            });
        }

        function PageCallback(pageIndex) {
            GetUserOrderList(pageIndex, pSize, pStatus);
        }

        GetOrderList(0, pSize, pStatus);

        function bindOrderListEvent() {
            //派单
            $(".sent-btn").click(function () {
                var orderId = $(this).siblings("input[name='orderId']").val();
                $.layer({
                    shade: [0],
                    area: ['auto', 'auto'],
                    dialog: {
                        msg: '是否确定派送订单？',
                        btns: 2,
                        type: 4,
                        btn: ['确定', '取消'],
                        yes: function () {
                            $.ajax({
                                url: '/Store/ConfirmOrder',
                                data: $.param({ orderId: orderId }),
                                type: 'POST',
                                cache: false,
                                timeout: 1000 * 60,
                                success: function (data) {
                                    if (data.status == "true") {
                                        layer.msg("成功派送订单", 2, 9);
                                        GetOrderList(pIndex, pSize, pStatus);
                                    }
                                    else {
                                        layer.msg(data.msg, 2, 8);
                                    }
                                },
                                error: function () {
                                    layer.msg("派送订单失败", 2, 8);
                                }
                            });
                        }, no: function () {

                        }
                    }
                });
            });
            //取消
            $(".cancel-btn").click(function () {
                var orderId = $(this).siblings("input[name='orderId']").val();
                $.layer({
                    shade: [0],
                    area: ['auto', 'auto'],
                    dialog: {
                        msg: '是否确定取消订单？',
                        btns: 2,
                        type: 4,
                        btn: ['确定', '取消'],
                        yes: function () {
                            $.ajax({
                                url: '/Store/ConfirmCancelOrder',
                                data: $.param({ orderId: orderId }),
                                type: 'POST',
                                cache: false,
                                timeout: 1000 * 60,
                                success: function (data) {
                                    if (data.status == "true") {
                                        layer.msg("成功取消订单", 2, 9);
                                        GetOrderList(pIndex, pSize, pStatus);
                                    }
                                    else {
                                        layer.msg(data.msg, 2, 8);
                                    }
                                },
                                error: function () {
                                    layer.msg("取消订单失败", 2, 8);
                                }
                            });
                        }, no: function () {

                        }
                    }
                });
            });
        }
    }

    return { initIndex: initIndex, initOrder: initOrder };
})