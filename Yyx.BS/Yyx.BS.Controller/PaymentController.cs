using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Yyx.BS.Library.Services;
using Yyx.BS.Utils;

namespace Yyx.BS.Controller
{
    public class PaymentController : System.Web.Mvc.Controller
    {
        [Dependency]
        public OrderServices orderServices { get; set; }

        [Dependency]
        public PaymentServices paymentServices { get; set; }

        [Dependency]
        public UserServices userServices { get; set; }

        public ActionResult AddOrderBook(string userid, string productid, decimal quantity)
        {
            Models.User user = Session["CurrentUser"] as Yyx.BS.Models.User;
            if (user == null)
            {
                return Redirect("/Login/Index");
            }
            try
            {
                Models.Product product = orderServices.GetProduct(productid);
                Models.OrderBook book = orderServices.AddOrderBook(userid, product.ProductID, quantity, product.Price, null, DateTime.Now);
                var books = orderServices.GetOrderBook(user.UserID);

                StringBuilder buider = new StringBuilder("{");
                buider.AppendFormat("\"{0}\":\"{1}\",", "OrderBookID", book.OrderBookID);
                buider.AppendFormat("\"{0}\":\"{1}\"", "OrderBookCount", books == null ? 0 : books.Count);
                buider.Append('}');
                return JsonUtils.GetJsonResult(buider.ToString());
            }
            catch (Exception)
            {
                return JsonUtils.ErrorResult();
            }
        }

        public ActionResult Confirm(string id)
        {
            Models.User user = Session["CurrentUser"] as Yyx.BS.Models.User;
            if (user == null)
            {
                return Redirect("/Login/Index");
            }
            List<string> ids = id.Split(',').ToList();
            Models.Account account = paymentServices.GetUserAccount(user.UserID);
            List<Models.UserAddress> addrs = userServices.GetUserAddress(user.UserID);

            ViewBag.User = user;
            ViewBag.UserAccount = account.Amount - account.FrozenAmount;
            ViewBag.UserAddressList = addrs;
            ViewBag.ProductList = orderServices.GetProducts();
            ViewBag.OrderBookList = orderServices.GetOrderBooks(ids);
            return View();
        }

        //支付
        public ActionResult PaidOrder(List<string> OrderBookID, string ServiceDate, string UserAddressID, string OrderMemo)
        {
            Models.User user = Session["CurrentUser"] as Yyx.BS.Models.User;
            if (user == null)
            {
                return Redirect("/Login/Index");
            }
            DateTime date = Convert.ToDateTime(ServiceDate + " 00:00:00");

            string message = string.Empty;
            if (orderServices.PaidOrder(orderServices.GetOrderBooks(OrderBookID), user.UserID, UserAddressID, date, OrderMemo, out message))
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
