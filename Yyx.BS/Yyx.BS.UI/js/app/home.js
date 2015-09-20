define(['jquery', 'layer', 'bootstrap'], function (jq) {

    function initIndex() {

    }

    function initDetail() {
        $("#addOrderBook").click(function () {
            newOrder(false);
        });

        $("#orderPayment").click(function () {
            newOrder(true);
        });

        function newOrder(type) {
            var userid = $("input[name='CurrentUserID']").val();
            var quantity = $("input[name='Quantity']").val();
            if (userid == undefined || undefined == "") {
                layer.msg("请先登录", 2, 8);
            }
            else if (parseInt(quantity) <= 0) {
                layer.msg("商品数量必须大于0", 2, 8);
            }
            else {
                var productid = $("input[name='ProductID']").val();
                var quantity = $("input[name='Quantity']").val();
                var message = type ? "无法购买此商品" : "无法添加此商品";
                $.ajax({
                    url: '/Payment/AddOrderBook',
                    type: 'POST',
                    data: $.param({ userid: userid, productid: productid, quantity: quantity }),
                    cache: false,
                    timeout: 1000 * 60,
                    success: function (data) {
                        if (data.status != "true") {
                            layer.msg(message, 2, 8);
                        }
                        else {
                            if (type) {
                                window.location.href = "/Payment/Confirm?id=" + data.OrderBookID;
                            }
                            else {
                                $("#bookNum").html(parseInt(data.OrderBookCount));
                                layer.msg("成功添加购物车", 2, 9);
                            }
                        }
                    },
                    error: function () {
                        layer.msg(message, 2, 8);
                    }
                });
            }
        }

        $("#minusQuantity").click(function () {
            var quantity = $("input[name='Quantity']").val();
            if (parseInt(quantity) <= 1) {
                quantity = 0;
            }
            else {
                quantity = parseInt(quantity) - 1;
            }
            $("input[name='Quantity']").val(quantity);
        });

        $("#plusQuantity").click(function () {
            var quantity = $("input[name='Quantity']").val();
            if (parseInt(quantity) <= 0) {
                quantity = 1;
            }
            else {
                quantity = parseInt(quantity) + 1;
            }
            $("input[name='Quantity']").val(quantity);
        });

    }

    return { initIndex: initIndex, initDetail: initDetail };
})