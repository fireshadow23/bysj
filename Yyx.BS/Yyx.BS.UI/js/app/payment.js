define(['jquery', 'layer', 'jqdatetimepicker', 'jqform', 'bootstrap'], function (jq) {

    function initOrderBook() {

        $(".delbook").bind("click", function () {

            var id = $(this).parent().find("input[name='OrderBookID']").val()
            var $tr = $(this).parent().parent();
            $.ajax({
                url: '/UserCenter/CancelOrderBook',
                type: 'POST',
                data: $.param({ orderBookId: id }),
                cache: false,
                timeout: 1000 * 60,
                success: function (data) {
                    if (data.status == "true") {
                        //删除节点
                        $tr.remove();

                        //计算数量
                        $("#bookNum").html(parseInt(data.OrderBookCount));

                        //计算金额
                        calcAmount();
                    }
                    else {
                        if (data.msg != "") {
                            layer.msg(data.msg, 2, 8);
                        }
                        else {
                            layer.msg("删除失败", 2, 8);
                        }
                    }
                },
                error: function () {
                    layer.msg("删除失败", 2, 8);
                }
            });
        });

        function calcAmount() {
            var totalAmount = 0;
            $.each($("input[name='blankCheckbox']"), function (i) {
                if ($("input[name = 'blankCheckbox']").get(i).checked == true) {
                    var amout = $(this).parent().parent().find("input[name='totalAmount']").val()
                    totalAmount += parseFloat(amout);
                }
            });
            totalAmount = totalAmount.toFixed(2);
            $("#totalAmount").html("￥" + totalAmount);
        }

        $("input[name='blankCheckbox']").bind("click", function () {
            calcAmount();
        });

        calcAmount();

        $("#btnBookPaid").click(function () {
            var totalAmount = 0;
            var ids = new Array();
            $.each($("input[name='blankCheckbox']"), function (i) {
                if ($("input[name = 'blankCheckbox']").get(i).checked == true) {
                    var amout = $(this).parent().parent().find("input[name='totalAmount']").val()
                    var id = $(this).parent().parent().find("input[name='OrderBookID']").val()
                    totalAmount += parseFloat(amout);
                    ids.push(id);
                }
            });
            if (totalAmount == 0) {
                layer.msg("请勾选商品后进行购买");
                return;
            }

            window.top.window.location = "/Payment/Confirm?id=" + ids;
        })
    }

    function initConfirm() {
        var currDate = new Date();
        $('.form_date').datetimepicker({
            //language: 'zh-CN',
            format: 'yyyy-mm-dd',
            weekStart: 1,
            startDate: currDate,
            todayBtn: 1,
            autoclose: 1,
            todayHighlight: 1,
            startView: 2,
            minView: 2,
            forceParse: 0
        });

        $(".useraddr").bind("click", function () {
            var addr = $(this).attr('data-addrid');
            $("input[name='UserAddressID']").val(addr);
            $("#UserAddress").val($(this).html());
        });

        $(".delbook").bind("click", function () {
            $(this).parent().parent().remove();
            calcAmount();
        });

        function calcAmount() {
            var totalAmount = 0;
            $.each($("input[name=totalAmount]"), function (i) {
                var amount = $(this).val();
                totalAmount += parseFloat(amount);
            });
            $("#totalAmount").html("￥" + totalAmount.toFixed(2));

            if (totalAmount > parseFloat($("#UserAccount").val())) {
                $("#userAccountError").show();
                $("#btnsubmit").attr({ "disabled": "disabled" });
            }
            else {
                $("#userAccountError").hide();
                $("#btnsubmit").removeAttr("disabled");
            }
        }

        calcAmount();

        $("form[name='OrderPaid']").submit(function () {
            if ($("input[name='ServiceDate']").val() == "") {
                layer.msg("请选择服务时间", 2, 8);
                return false;
            }
            if ($("input[name='UserAddressID']").val() == "") {
                layer.msg("请选择服务地址", 2, 8);
                return false;
            }
            $(this).ajaxSubmit({
                success: function (data) {
                    if (data.status == 'true') {
                        layer.msg("支付成功", 3, 8);
                        window.top.window.location = "/UserCenter/MyOrder";
                    } else {
                        //提示登录失败
                        if (data.msg != "") {
                            layer.msg(data.msg, 2, 8);
                        }
                        else {
                            layer.msg("支付失败！", 2, 8);
                        }
                    }
                },
                error: function (data) {
                    layer.msg("支付失败!", 2, 8);
                }
            });
            return false;
        });
    }

    return { initOrderBook: initOrderBook, initConfirm: initConfirm };
})