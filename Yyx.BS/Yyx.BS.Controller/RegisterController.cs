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
    public class RegisterController : System.Web.Mvc.Controller
    {
        [Dependency]
        public UserServices userServices { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserRegister(string mobile, string password, string passwordAgain, string imageCode)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                return JsonUtils.ErrorResult("手机不能为空");
            }
            if (string.IsNullOrEmpty(password))
            {
                return JsonUtils.ErrorResult("密码不能为空");
            }
            if (string.IsNullOrEmpty(passwordAgain))
            {
                return JsonUtils.ErrorResult("请再次输入密码");
            }
            if (password != passwordAgain)
            {
                return JsonUtils.ErrorResult("输入的密码不一致");
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
            var user = userServices.AddNewUser(mobile, CryptographyUtils.EncryptString(password), out message);
            if (string.IsNullOrEmpty(message))
            {
                return JsonUtils.SuccessResult();
            }
            else
            {
                return JsonUtils.ErrorResult(message);
            }
        }
    }
}
