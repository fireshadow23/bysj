define(['jquery', 'layer', 'jqform', 'jqregx'], function (jq) {
    $(function () {
        init();
    })

    function init() {
        //账号密码登录
        $("form[name='mobilePswLoginForm']").submit(function () {
            $(this).ajaxSubmit({
                success: function (data) {
                    if (data.status == 'true') {
                        //var redirecturl = $("#j_login_redirect").val();
                        window.top.window.location = "/Home/";
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

        GetImageCode();
        $("#btnImageCode,#ImageCode").bind('click', function () {
            GetImageCode();
        });
    }

    function GetImageCode() {
        $("#ImageCode").attr("src", "../Login/GetValidateCode?time=" + Math.random());
    }
})