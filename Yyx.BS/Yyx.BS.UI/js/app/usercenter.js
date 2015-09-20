define(['jquery', 'mustache', 'lui', 'layer', 'jqpaging', 'jqdatetimepicker', 'jqform', 'jqregx', 'bootstrap', 'tmpconfig'], function (jq) {

    function initIndex() {
        var currDate = new Date();
        $('.form_date').datetimepicker({
            //language: 'zh-CN',
            format: 'yyyy-mm-dd',
            weekStart: 1,
            todayBtn: 1,
            autoclose: 1,
            todayHighlight: 1,
            startView: 2,
            minView: 2,
            forceParse: 0
        });

        if ($("#Gender").val() == "男") {
            $("#gender1").attr('checked', 'checked');
            $("#gender2").removeAttr('checked');
        } else if ($("#Gender").val() == "女") {
            $("#gender1").removeAttr('checked');
            $("#gender2").attr('checked', 'checked');
        } else {
            $("#gender1").removeAttr('checked');
            $("#gender2").removeAttr('checked');
        }

        $("#userheadfile").hide();
        $("#userheadclick").click(function () {
            $("#userheadfile").click();
        });

        $("#userheadfile").change(function () {
            editUserHead($(this)[0]);
        });

        $("#btnEdit").click(function () {
            $("fieldset").removeAttr('disabled');
        });

        $("#btnSave").click(function () {
            if ($("fieldset").attr('disabled') == undefined) {
                $("form[name='updateUserInfo']").submit();
            }
        });

        $("form[name='updateUserInfo']").submit(function () {
            var self = this;

            if (!validateGender()) {
                layer.msg("请选择性别");
                return false;
            }
            $(this).ajaxSubmit({
                success: function (data) {
                    if (data.status == 'true') {
                        layer.msg("保存成功！", 2, 9);
                        $("fieldset").attr('disabled', "disabled");
                    } else {
                        //提示登录失败
                        layer.msg(data.msg, 2, 8);
                    }
                },
                error: function () {
                    layer.msg("保存失败", 2, 8);
                }
            });
            return false;
        });

        function validateGender() {
            if ($("#gender1")[0].checked || $("#gender2")[0].checked)
                return true;
            return false;
        }

        function editUserHead(file) {
            if (file.files && file.files[0]) {
                var reader = new FileReader();
                reader.onload = function (evt) {
                    $("#update-user-head-form").ajaxSubmit({
                        type: 'post',
                        dataType: 'json',
                        beforeSend: function () {
                            var tmp = $("#userheadfile")[0].value;
                            if (tmp.length > 0) {
                                tmp = tmp.toLowerCase();
                                var patten = /(.jpg|.jpeg|.bmp|.png|.gif)$/;
                                if (tmp.search(patten) <= -1) {
                                    layer.msg('只允许上传 jpg/jpeg/bmp/gif/png 格式的文件!', 2, 8);
                                    return false;
                                }
                            }
                        },
                        success: function (data) {
                            if (data.status == "true") {
                                $("#userhead").attr("src", evt.target.result);
                                $("input[name='HeadImagePath']").val(data.imagePath);
                                evt.stopPropagation();
                                //layer.msg("修改头像成功！", 2, 9);
                            } else {
                                layer.msg(data.msg, 2, 8);
                            }
                        },
                        error: function (msg) {
                        }
                    });
                }
                reader.readAsDataURL(file.files[0]);
            }
        }

        $("#btnAddAddr").bind("click", function (ev) {
            ev.stopPropagation();
            popAddr("新增服务地址");

            $("input[name='ContactName']").val("");
            $("input[name='ContactPhone']").val("");
            $("input[name='Street']").val("");
            $("input[name='AddressID']").val("");
        })

        //删除地址
        $(".delAddr").bind("click", function (ev) {
            ev.stopPropagation();
            var addressId = $(this).siblings("input[name='addressId']").val();
            $.layer({
                shade: [0],
                area: ['auto', 'auto'],
                dialog: {
                    msg: '是否确定删除该地址？',
                    btns: 2,
                    type: 4,
                    btn: ['确定', '取消'],
                    yes: function () {
                        jQuery.ajax({
                            url: '/UserCenter/DeleteAddress',
                            type: 'POST',
                            dataType: 'json',
                            async: true,
                            cache: false,
                            data: $.param({ addressID: addressId }),
                            timeout: 1000 * 60,
                            success: function (data) {
                                if (data.status == 'true') {
                                    layer.msg("删除地址成功", 2, 9);
                                    window.location = "/UserCenter/Index";
                                } else {
                                    layer.msg(data.msg, 2, 8);
                                }
                            },
                            error: function (data) {
                            }
                        });
                    }, no: function () {

                    }
                }
            });
        });

        $(".modifyAddr").bind("click", function (ev) {
            ev.stopPropagation();
            popAddr("修改服务地址");
            var values = $(this).parents("tr").find("td");
            $("input[name='ContactName']").val(values[0].innerText);
            $("input[name='ContactPhone']").val(values[1].innerText);
            var province = $(this).siblings("input[name='province']").val();
            var city = $(this).siblings("input[name='city']").val();
            var area = $(this).siblings("input[name='area']").val();
            var street = $(this).siblings("input[name='street']").val();
            var addressId = $(this).siblings("input[name='addressId']").val();
            $("select[name='Province']").val(province);
            $("select[name='City']").val(city);
            $("select[name='Area']").val(area);
            $("input[name='Street']").val(street);
            $("input[name='AddressID']").val(addressId);
        })

        //新增修改地址
        $("form[name='modifyAdderss']").submit(function () {
            if (!validateAddress()) {
                return false;
            }
            $(this).ajaxSubmit({
                success: function (data) {
                    if (data.status == 'true') {
                        layer.msg("地址修改成功！", 2, 9);
                        window.location = "/UserCenter/Index";
                    } else {
                        //提示登录失败
                        layer.msg(data.msg, 2, 8);
                    }
                },
                error: function (data) {
                }
            });
            return false;
        });

        function validateAddress() {
            var ContactName = $("input[name='ContactName']").val();
            var ContactPhone = $("input[name='ContactPhone']").val();
            var Street = $("input[name='Street']").val();

            if ($.isEmptyObject(ContactName)) {
                layer.msg("联系人姓名不能为空！");
                return false;
            }
            if ($.isEmptyObject(ContactPhone)) {
                layer.msg("联系人手机号码不能为空！");
                return false;
            }
            if (!$.isMobile(ContactPhone)) {
                layer.msg("联系人手机号码格式不正确！");
                return false;
            }
            if ($.isEmptyObject(Street)) {
                layer.msg("详细地址不能为空！");
                return false;
            }

            return true;
        }

        function popAddr(name) {
            $.layer({
                type: 1,
                area: ['500px', 'auto'],
                closeBtn: [1, true],
                border: [0],
                title: false,
                //title: [name, 'border:none;background:#feca01;color:#fff;'],
                bgcolor: '#eee',
                page: { dom: '.windows' },
                shift: 'top'
            });
        }

        $("#btnRecharge").click(function () {
            $.layer({
                type: 1,
                area: ['500px', 'auto'],
                closeBtn: [1, true],
                border: [0],
                title: false,
                bgcolor: '#eee',
                page: { dom: '.recharge' },
                shift: 'top'
            });
        });

        $(".paymenttype").bind("click", function () {
            $("#PaymentType").val($(this).html());
        });

        $("form[name='Recharge']").submit(function () {
            var rechargeAmount = $("#rechargeAmount").val();
            var paymentType = $("#PaymentType").val();
            if ($.isEmptyObject(rechargeAmount)) {
                layer.msg("充值金额不能为空！");
                return false;
            }
            if ($.isEmptyObject(paymentType)) {
                layer.msg("请选择支付方式！");
                return false;
            }
            $(this).ajaxSubmit({
                success: function (data) {
                    if (data.status == 'true') {
                        layer.msg("充值成功！", 2, 9);
                        window.location = "/UserCenter/Index";
                    } else {
                        //提示登录失败
                        layer.msg(data.msg, 2, 8);
                    }
                },
                error: function (data) {
                }
            });
            return false;
        });

        $("#btnBalance").click(function () {
            jQuery.ajax({
                url: '/UserCenter/GetUserBalance',
                type: 'POST',
                dataType: 'json',
                async: false,
                cache: false,
                timeout: 1000 * 60,
                success: function (data) {
                    if (data.status == 'true') {

                        $("#balancelist").html("");
                        $.each(data.Balances, function (i) {
                            var tr = "<tr><td>" + data.Balances[i].CreateDate + "</td><td>";
                            tr += data.Balances[i].Amount + "</td><td>";
                            tr += data.Balances[i].Discription + "</td></tr>";
                            $("#balancelist").html(tr + $("#balancelist").html());
                        });

                        $.layer({
                            type: 1,
                            area: ['500px', '400px'],
                            closeBtn: [1, true],
                            border: [0],
                            title: false,
                            bgcolor: '#eee',
                            page: { dom: '.balance' },
                            shift: 'top'
                        });
                    } else {
                        layer.msg(data.msg, 2, 8);
                    }
                },
                error: function (data) {
                }
            });
        });
    }


    function initMyOrder() {
        GetUserOrderList(0, 5);

        var pIndex = 0;
        var pSize = 5;

        function GetUserOrderList(pageIndex, pageSize) {
            pIndex = pageIndex;
            $.ajax({
                url: '/UserCenter/GetUserOrderList',
                data: $.param({ pageIndex: pageIndex, pageSize: pageSize }),
                type: 'POST',
                cache: false,
                dataType: 'json',
                timeout: 1000 * 60,
                success: function (data) {
                    if (data.status == "true") {
                        console.log(data);
                        $$.Template.render("#userOrderList", yyx.templates.orderDetail.fullpath, data, true, function (success, e) {
                            if (!success) return;
                            $.each(data.OrderViews, function (i) {
                                if (data.OrderViews[i].HasComment) {
                                    var orderNo = data.OrderViews[i].OrderNo;
                                    $("#" + orderNo + " .comment-btn").html("查看评价");
                                }
                            });

                            renderPaging(pageIndex, pageSize, data.totalcount);
                            bindOrderListEvent();
                        });
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
            GetUserOrderList(pageIndex, pSize);
        }

        function bindOrderListEvent() {
            BindOrderCompleteEvent();
            BindOrderCancelEvent();
            BindOrderDeleteEvent();
            BindOrderCommentEvent();
        }

        function BindOrderCompleteEvent() {
            $(".complete-btn").click(function () {
                var orderId = $(this).parents("div").eq(0).find("input[name='orderId']").val();
                $.layer({
                    shade: [0],
                    area: ['auto', 'auto'],
                    dialog: {
                        msg: '是否确定完成订单？',
                        btns: 2,
                        type: 4,
                        btn: ['确定', '取消'],
                        yes: function () {
                            $.ajax({
                                url: '/UserCenter/CompleteOrder',
                                data: $.param({ orderId: orderId }),
                                type: 'POST',
                                cache: false,
                                timeout: 1000 * 60,
                                success: function (data) {
                                    if (data.status == "true") {
                                        layer.msg("成功完成订单", 2, 9);
                                        GetUserOrderList(pIndex, pSize);
                                    }
                                    else {
                                        layer.msg(data.msg, 2, 8);
                                    }
                                },
                                error: function () {
                                    layer.msg("完成订单失败", 2, 8);
                                }
                            });
                        }, no: function () {

                        }
                    }
                });
            });
        }
        function BindOrderCancelEvent() {
            $(".cancel-btn").click(function () {
                var orderId = $(this).parents("div").eq(0).find("input[name='orderId']").val();
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
                                url: '/UserCenter/CancelOrder',
                                data: $.param({ orderId: orderId }),
                                type: 'POST',
                                cache: false,
                                timeout: 1000 * 60,
                                success: function (data) {
                                    if (data.status == "true") {
                                        layer.msg("成功受理取消订单", 2, 9);
                                        GetUserOrderList(pIndex, pSize);
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

        function BindOrderDeleteEvent() {
            $(".delete-btn").click(function () {
                var orderId = $(this).parents("div").eq(0).find("input[name='orderId']").val();
                $.layer({
                    shade: [0],
                    area: ['auto', 'auto'],
                    dialog: {
                        msg: '是否确定删除订单？',
                        btns: 2,
                        type: 4,
                        btn: ['确定', '取消'],
                        yes: function () {
                            $.ajax({
                                url: '/UserCenter/DeleteOrder',
                                data: $.param({ orderId: orderId }),
                                type: 'POST',
                                cache: false,
                                timeout: 1000 * 60,
                                success: function (data) {
                                    if (data.status == "true") {
                                        layer.msg("成功删除订单", 2, 9);
                                        GetUserOrderList(pIndex, pSize);
                                    }
                                    else {
                                        layer.msg(data.msg, 2, 8);
                                    }
                                },
                                error: function () {
                                    layer.msg("删除订单失败", 2, 8);
                                }
                            });
                        }, no: function () {

                        }
                    }
                });
            });
        }

        function BindOrderCommentEvent() {
            $(".comment-btn").click(function (ev) {
                ev.stopPropagation();
                var orderId = $(this).parents("div").eq(0).find("input[name='orderId']").val();
                var commentContent = $(this).parents("div").eq(0).find("input[name='orderCommentContent']").val();
                $(".windows input[name='OrderID']").val(orderId);
                if ($(this).html() == "评价订单") {
                    $("#showComment").hide();
                    $("#commentOrder").show();
                }
                else {
                    $("#showComment").show();
                    $("#commentOrder").hide();
                    $("#CommentContent").html(commentContent);
                }
                popComment();
            });
        }

        function popComment() {
            $.layer({
                type: 1,
                area: ['500px', 'auto'],
                closeBtn: [1, true],
                border: [0],
                title: false,
                //title: [name, 'border:none;background:#feca01;color:#fff;'],
                bgcolor: '#eee',
                page: { dom: '.windows' },
                shift: 'top'
            });
        }
    }

    return { initIndex: initIndex, initMyOrder: initMyOrder };
})