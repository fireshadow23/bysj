(function($){

    //校验是否全由数字组成
    $.isDigit = function (s) {
        var patrn=/^[0-9]{1,20}$/;
        if (!patrn.exec(s)) return false
        return true
    };

    //校验登录名：只能输入5-20个以字母开头、可带数字、“_”、“.”的字串
    $.isRegisterUserName = function(s)
    {
        var patrn=/^[a-zA-Z]{1}([a-zA-Z0-9]|[._]){4,19}$/;
        if (!patrn.exec(s)) return false
        return true
    };

    
    //校验用户姓名：只能输入1-30个以字母开头的字串
    $.isTrueName = function (s)
    {
        var patrn=/^[a-zA-Z]{1,30}$/;
        if (!patrn.exec(s)) return false
        return true
    };

    //校验邮政编码
    $.isPostalCode = function (s)
    {
        //var patrn=/^[a-zA-Z0-9]{3,12}$/;
        var patrn=/^[a-zA-Z0-9 ]{3,12}$/;
        if (!patrn.exec(s)) return false
        return true
    }


    $.isIP = function (s) //by zergling
    {
        var patrn=/^[0-9.]{1,20}$/;
        if (!patrn.exec(s)) return false
        return true
    };

    //校验手机号码：必须以数字开头，除数字外，可含有“-”
    $.isMobile = function(s)
    {
        //var patrn = /^(((13[0-9]{1})|(15[0-9]{1})|(14[0-9]{1})|(18[0-9]{1}))+\d{8})$/;
        var patrn = /^\d{11}$/;
        if (!patrn.exec(s)) return false
        return true
    }

    //校验普通电话、传真号码：可以“+”开头，除数字外，可含有“-”
    $.isTel = function (s)
    {
        //var patrn=/^[+]{0,1}(/d){1,3}[ ]?([-]?(/d){1,12})+$/;
        var patrn = /^(0[0-9]{2,3}\-)?([2-9][0-9]{6,7})+(\-[0-9]{1,4})?$/;
        if (!patrn.exec(s)) return false
        return true
    }

    //邮箱验证
    $.isEmail = function (s)
    {
        var patrn = /^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/;
        if (!patrn.exec(s)) return false
        return true
    }


})(jQuery);
