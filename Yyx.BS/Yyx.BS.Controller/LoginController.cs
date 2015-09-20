using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Yyx.BS.Library.Services;
using Yyx.BS.Models;
using Yyx.BS.Utils;

namespace Yyx.BS.Controller
{
    public class LoginController : System.Web.Mvc.Controller
    {
        [Dependency]
        public UserServices userServices { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserLogin(string mobile, string password, string imageCode)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                return JsonUtils.ErrorResult("手机不能为空");
            }
            if (string.IsNullOrEmpty(password))
            {
                return JsonUtils.ErrorResult("密码不能为空");
            }
            if (string.IsNullOrEmpty(imageCode))
            {
                return JsonUtils.ErrorResult("验证码不能为空");
            }

            if (Session["ValidateCode"].ToString() != imageCode)
            {
                return JsonUtils.ErrorResult("验证码有误");
            }

            string message = string.Empty;
            if (userServices.UserLogin(mobile, CryptographyUtils.EncryptString(password), out message))
            {
                //当前用户
                var user = userServices.GetUserInfoByMobile(mobile);
                Session["CurrentUser"] = user;

                return JsonUtils.SuccessResult();
            }
            else
            {
                return JsonUtils.ErrorResult(message);
            }
        }

        public ActionResult LoginOut()
        {
            Session["CurrentUser"] = null;
            return Redirect("/Home/Index");
        }

        public ActionResult GetValidateCode()
        {
            //生成随机数
            string code = ImageCode.CreateVerifyCode(4);
            Session["ValidateCode"] = code;
            byte[] bytes = ImageCode.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }
    }
}
