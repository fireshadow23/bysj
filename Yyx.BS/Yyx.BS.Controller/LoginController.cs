using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Yyx.BS.Library.Services;
using Yyx.BS.Models;

namespace Yyx.BS.Controller
{
    public class LoginController : System.Web.Mvc.Controller
    {
        [Dependency]
        public UserServices userServices { get; set; }

        public ActionResult Index()
        {
            User user = userServices.GetUserInfo("U0000000001");
            return View();
        }
    }
}
