define(['jquery', 'jqform', 'jqregx', 'layer'], function (jq) {
    $(function () {
        init();
    })

    function init() {
        //账号密码登录
        $("form[name='mobileRegisterForm']").submit(function () {
            $(this).ajaxSubmit({
                success: function (data) {
                    if (data.status == 'true') {
                        //var redirecturl = $("#j_login_redirect").val();
                        layer.msg("注册成功", 2, 9);
                        window.top.window.location = "/Login/";
                    } else {
                        //提示登录失败
                        if (data.msg != "") {
                            layer.msg(data.msg, 2, 8);
                        }
                        else {
                            layer.msg("注册失败！", 2, 8);
                        }
                    }
                },
                error: function (data) {
                    layer.msg("注册失败!", 2, 8);
                }
            });
            return false;
        });

        GetImageCode();
        $("#btnImageCode,#ImageCode").bind('click', function () {
            GetImageCode();
        });
    }

    function GetImageCode() {
        $("#ImageCode").attr("src", "../Login/GetValidateCode?time=" + Math.random());
    }

})