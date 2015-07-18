(function ($) {
    $.validator.addMethod("mobile", function (value, element) {
        var length = value.length;
        var mobile = /^(((13[0-9]{1})|(15[0-9]{1})|(14[0-9]{1})|(18[0-9]{1}))+\d{8})$/;
        return this.optional(element) || (length == 11 && mobile.test(value));
    }, "手机号码格式错误");

    $.validator.addMethod("phone", function (value, element) {
        var tel = /^(0[0-9]{2,3}\-)?([2-9][0-9]{6,7})+(\-[0-9]{1,4})?$/;
        return this.optional(element) || (tel.test(value));
    }, "电话号码格式错误");

    $.validator.addMethod("money", function (value, element) {
        var money = /^-?(?:\d+|\d{1,3}(?:,\d{3})+)?(?:\.\d+)?$/;
        return this.optional(element) || (money.test(value)) && value > 0;
    }, "金额格式不正确");

    $.validator.addMethod("zipCode", function (value, element) {
        var tel = /^[0-9]{6}$/;
        return this.optional(element) || (tel.test(value));
    }, "邮政编码格式错误");

    $.validator.addMethod("qq", function (value, element) {
        var tel = /^[1-9]\d{4,9}$/;
        return this.optional(element) || (tel.test(value));
    }, "qq号码格式错误");

    $.validator.addMethod("ip", function (value, element) {
        var ip = /^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/;
        return this.optional(element) || (ip.test(value) && (RegExp.$1 < 256 && RegExp.$2 < 256 && RegExp.$3 < 256 && RegExp.$4 < 256));
    }, "Ip地址格式错误");

    $.validator.addMethod("chrnum", function (value, element) {
        var chrnum = /^([a-zA-Z0-9]+)$/;
        return this.optional(element) || (chrnum.test(value));
    }, "只能输入数字和字母(字符A-Z, a-z, 0-9)");

    $.validator.addMethod("chinese", function (value, element) {
        var chinese = /^[\u4e00-\u9fa5]+$/;
        return this.optional(element) || (chinese.test(value));
    }, "只能输入中文");

    $.validator.addMethod("selectNone", function (value, element) {
        return value == "请选择";
    }, "必须选择一项");

    $.validator.addMethod("byteRangeLength", function (value, element, param) {
        var length = value.length;
        for (var i = 0; i < value.length; i++) {
            if (value.charCodeAt(i) > 127) {
                length++;
            }
        }
        return this.optional(element) || (length >= param[0] && length <= param[1]);
    }, $.validator.format("请确保输入的值在{0}-{1}个字节之间(一个中文字算2个字节)"));

    $.validator.addMethod("compareDateLarge", function (value, element, param) {
        //var startDate = jQuery(param).val() + ":00";补全yyyy-MM-dd HH:mm:ss格式
        //value = value + ":00";
        var startDate = jQuery(param).val().trim();
        if (startDate.trim() == "" || value.trim() == "") {
            return true;
        }

        var date1 = new Date(Date.parse(startDate.replace("-", "/")));
        var date2 = new Date(Date.parse(value.replace("-", "/")));
        return date1 < date2;
    }, "时间必须大于比较时间");

    $.validator.addMethod("largethan", function (value, element, param) {
        var val = jQuery(param).val();
        var num1 = parseFloat(val);
        var num2 = parseFloat(value);

        if (value.trim() == "" || val.trim() == "") {
            return true;
        }

        return num1 < num2;
    }, "必须大于指定的数");

    /* compareDate
    jQuery("#form1").validate({
        focusInvalid: false,
        rules: {
            "timeStart": {
                required: true
            },
            "timeEnd": {
                required: true,
                compareDate: "#timeStart"
            }
        },
        messages: {
            "timeStart": {
                required: "开始时间不能为空"
            },
            "timeEnd": {
                required: "结束时间不能为空",
                compareDate: "结束日期必须大于开始日期!"
            }
        }
    });
    */

    jQuery.validator.addMethod("isTimeHour", function (value, element) {
        var h = value.substring(0, 2), nowTimHour = (new Date()).getHours();
        return this.optional(element) || (h > nowTimHour);
    }, "请正确选择就餐时间。");

    jQuery.validator.addMethod("isTimeMin", function (value, element) {
        var m = value.substring(0, 2), nowTimMin = (new Date()).getMinutes();
        return this.optional(element) || (m > nowTimMin);
    }, "请正确选择就餐时间。");

})(jQuery);