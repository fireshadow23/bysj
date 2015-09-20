using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Yyx.BS.Library.Services;
using Yyx.BS.Models;
using Yyx.BS.Utils;

namespace Yyx.BS.Controller
{
    public class UserCenterController : System.Web.Mvc.Controller
    {
        [Dependency]
        public OrderServices orderServices { get; set; }

        [Dependency]
        public UserServices userServices { get; set; }

        [Dependency]
        public PaymentServices paymentServices { get; set; }

        public ActionResult Index()
        {
            Models.User user = Session["CurrentUser"] as Yyx.BS.Models.User;
            if (user == null)
            {
                return Redirect("/Login/Index");
            }
            ViewBag.User = user;
            List<Models.UserAddress> addrs = userServices.GetUserAddress(user.UserID);
            ViewBag.UserAddressList = addrs;
            var account = paymentServices.GetUserAccount(user.UserID);
            ViewBag.UserAccount = (account.Amount - account.FrozenAmount).GetValueOrDefault().ToString("0.00");
            return View();
        }

        public ActionResult UpdateUserHeadImg()
        {
            Models.User user = Session["CurrentUser"] as Yyx.BS.Models.User;
            if (user == null)
            {
                return JsonUtils.ErrorResult("请重新登录");
            }
            string ImageSavePath = string.Empty;
            HttpPostedFileBase HeadFile = Request.Files["userheadfile"];
            List<string> allowList = new List<string>() { ".jpg", ".jpeg", ".bmp", ".gif", ".png" };
            bool typeCheck = allowList.Any(endName => HeadFile.FileName.EndsWith(endName));
            if (!typeCheck)
            {
                return JsonUtils.ErrorResult("图片格式不正确");
            }
            string[] name = HeadFile.FileName.Split('.');
            ImageSavePath = AppDomain.CurrentDomain.BaseDirectory + "img\\UserImage\\" + user.UserID + "." + name[name.Length - 1];
            HeadFile.SaveAs(ImageSavePath);
            string returnImagePath = "/img/UserImage/" + user.UserID + "." + name[name.Length - 1];
            StringBuilder buider = new StringBuilder("{");
            buider.AppendFormat("\"{0}\":\"{1}\"", "imagePath", returnImagePath);
            buider.Append('}');
            return JsonUtils.GetJsonResult(buider.ToString());
        }

        public ActionResult UpdateUserInfo(string HeadImagePath, string UserName, string RealName, string Gender, string Birthday, string Email)
        {
            Models.User user = Session["CurrentUser"] as Yyx.BS.Models.User;
            if (user == null)
            {
                return JsonUtils.ErrorResult("请重新登录");
            }
            string message = string.Empty;
            Models.User userinfo = new Models.User();
            userinfo.HeadImagePath = HeadImagePath;
            userinfo.UserName = UserName;
            userinfo.RealName = RealName;
            userinfo.Gender = Gender;
            userinfo.Email = Email;
            if (Birthday == "0001/1/1 0:00:00")
            {
                userinfo.Birthday = null;
            }
            else
            {
                userinfo.Birthday = Convert.ToDateTime(Birthday);
            }

            Models.User newuser = userServices.ModifyUserInfo(user.UserID, userinfo, out message);
            if (!string.IsNullOrEmpty(message))
            {
                return JsonUtils.ErrorResult(message);
            }
            Session["CurrentUser"] = newuser;

            return JsonUtils.SuccessResult();
        }

        public ActionResult Recharge(decimal amount)
        {
            Models.User user = Session["CurrentUser"] as Yyx.BS.Models.User;
            if (user == null)
            {
                return JsonUtils.ErrorResult("请重新登录");
            }
            string message = string.Empty;
            if (paymentServices.Recharge(user.UserID, amount, out message))
            {
                return JsonUtils.SuccessResult();
            }

            return JsonUtils.ErrorResult(message);
        }

        public ActionResult GetUserBalance()
        {
            Models.User user = Session["CurrentUser"] as Yyx.BS.Models.User;
            if (user == null)
            {
                return JsonUtils.ErrorResult("请重新登录");
            }

            var balances = paymentServices.GetUserBalance(user.UserID);
            List<BalanceView> balanceList = new List<BalanceView>();
            foreach (var item in balances)
            {
                BalanceView balance = new BalanceView();
                balance.Amount = item.Amount.ToString("0.00");
                balance.CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
                balance.Discription = item.Discription;
                balanceList.Add(balance);
            }

            return JsonUtils.GetJsonResult(balanceList, "Balances");
        }

        public ActionResult DeleteAddress(string addressID)
        {
            Models.User user = Session["CurrentUser"] as Yyx.BS.Models.User;
            if (user == null)
            {
                return JsonUtils.ErrorResult("请重新登录");
            }
            string message = string.Empty;
            if (userServices.DeleteAddress(user.UserID, addressID, out message))
            {
                return JsonUtils.SuccessResult();
            }

            return JsonUtils.ErrorResult(message);
        }

        public ActionResult ModifyAddress(string ContactName, string ContactPhone, string Province, string City, string Area, string Street, string AddressID)
        {
            Models.User user = Session["CurrentUser"] as Yyx.BS.Models.User;
            if (user == null)
            {
                return JsonUtils.ErrorResult("请重新登录");
            }
            string message = string.Empty;
            if (string.IsNullOrEmpty(AddressID))
            {
                userServices.AddUserAddress(user.UserID, Province, City, Area, Street, ContactName, ContactPhone, out message);
            }
            else
            {
                userServices.ModifyAddress(user.UserID, AddressID, ContactName, ContactPhone, Province, City, Area, Street, out message);
            }
            if (string.IsNullOrEmpty(message))
            {
                return JsonUtils.SuccessResult();
            }
            return JsonUtils.ErrorResult(message);
        }

        public ActionResult MyOrderBook()
        {
            Models.User user = Session["CurrentUser"] as Yyx.BS.Models.User;
            if (user == null)
            {
                return Redirect("/Login/Index");
            }
            ViewBag.User = user;
            ViewBag.OrderBookList = orderServices.GetOrderBook(user.UserID);
            ViewBag.ProductList = orderServices.GetProducts();
            return View();
        }

        public ActionResult CancelOrderBook(string orderBookId)
        {
            Models.User user = Session["CurrentUser"] as Yyx.BS.Models.User;
            if (user == null)
            {
                return Redirect("/Login/Index");
            }
            string message = string.Empty;
            if (orderServices.CancelOrderBook(orderBookId, out message))
            {
                var books = orderServices.GetOrderBook(user.UserID);
                StringBuilder buider = new StringBuilder("{");
                buider.AppendFormat("\"{0}\":\"{1}\"", "OrderBookCount", books == null ? 0 : books.Count);
                buider.Append('}');
                return JsonUtils.GetJsonResult(buider.ToString());
            }
            else
            {
                return JsonUtils.ErrorResult(message);
            }
        }

        public ActionResult MyOrder()
        {
            Models.User user = Session["CurrentUser"] as Yyx.BS.Models.User;
            if (user == null)
            {
                return Redirect("/Login/Index");
            }
            ViewBag.User = user;
            return View();
        }

        public ActionResult GetUserOrderList(int pageIndex, int pageSize)
        {
            Models.User user = Session["CurrentUser"] as Yyx.BS.Models.User;
            if (user == null)
            {
                return JsonUtils.ErrorResult("请重新登录");
            }

            int totalCount = 0;
            List<Models.Product> products = orderServices.GetProducts();
            List<Models.Order> orders = orderServices.GetOrder(user.UserID, pageIndex, pageSize, out totalCount);
            List<Models.OrderView> orderViews = new List<Models.OrderView>();
            foreach (var order in orders)
            {
                Models.OrderView orderView = new Models.OrderView();
                EntityObject.FillEntity(order, orderView);
                orderView.OrderAmount = decimal.Round(orderView.OrderAmount.GetValueOrDefault(), 2);
                orderView.OrderStatusDefCN = orderServices.GetOrderStatus(order.OrderStatusID);
                orderView.PayStatusDefCN = orderServices.GetPayStatus(order.PayStatusID);
                Models.OrderComment comment = orderServices.GetOrderComment(order.OrderID);
                orderView.HasComment = comment == null ? false : true;
                orderView.OrderCommentContent = comment == null ? "" : comment.CommentContent;

                List<Models.OrderItem> orderItems = orderServices.GetOrderItem(order.OrderID);
                orderView.OrderItemViews = new List<Models.OrderItemView>();
                bool isShowBtn = true;
                foreach (var orderItem in orderItems)
                {
                    Models.OrderItemView orderItemView = new Models.OrderItemView();
                    EntityObject.FillEntity(orderItem, orderItemView);
                    var product = products.Find(o => o.ProductID == orderItem.ProductID);
                    orderItemView.ProductName = product.ProductName;
                    orderItemView.TotalCount = orderItems.Count;
                    if (isShowBtn)
                    {
                        orderItemView.OrderStatusID = order.OrderStatusID;
                    }
                    orderItemView.ShowBtn = isShowBtn;
                    isShowBtn = false;
                    orderView.OrderItemViews.Add(orderItemView);
                }
                orderViews.Add(orderView);
            }

            return JsonUtils.GetJsonResult(orderViews, "OrderViews", totalCount);
        }


        public ActionResult CompleteOrder(string orderId)
        {
            string message = string.Empty;
            Models.User user = Session["CurrentUser"] as Yyx.BS.Models.User;
            if (user == null)
            {
                message = "请重新登录";
                return JsonUtils.ErrorResult(message);
            }

            if (orderServices.CompleteOrder(orderId, out message))
            {
                return JsonUtils.SuccessResult();
            }

            return JsonUtils.ErrorResult(message);
        }

        public ActionResult CancelOrder(string orderId)
        {
            string message = string.Empty;
            Models.User user = Session["CurrentUser"] as Yyx.BS.Models.User;
            if (user == null)
            {
                message = "请重新登录";
                return JsonUtils.ErrorResult(message);
            }

            Models.Order order = orderServices.GetOrder(orderId);
            if (order == null)
            {
                message = "订单不存在";
                return JsonUtils.ErrorResult(message);
            }
            if (order.OrderStatusID == OrderStatusEnum.Paid.ToString())
            {
                if (orderServices.CancelOrder(orderId, out message))
                {
                    return JsonUtils.SuccessResult();
                }
            }
            else if (order.OrderStatusID == OrderStatusEnum.ServerSent.ToString())
            {
                if (orderServices.TryCancelOrder(orderId, out message))
                {
                    return JsonUtils.SuccessResult();
                }
            }

            return JsonUtils.ErrorResult(message);
        }

        public ActionResult DeleteOrder(string orderId)
        {
            string message = string.Empty;
            Models.User user = Session["CurrentUser"] as Yyx.BS.Models.User;
            if (user == null)
            {
                message = "请重新登录";
                return JsonUtils.ErrorResult(message);
            }

            if (orderServices.DeleteOrder(orderId, out message))
            {
                return JsonUtils.SuccessResult();
            }

            return JsonUtils.ErrorResult(message);
        }

        public ActionResult CommentOrder(string OrderID, string CommentContent)
        {
            string message = string.Empty;
            Models.User user = Session["CurrentUser"] as Yyx.BS.Models.User;
            if (user == null)
            {
                message = "请重新登录";
                return JsonUtils.ErrorResult(message);
            }

            if (orderServices.CommentOrder(OrderID, CommentContent, out message))
            {
                return JsonUtils.SuccessResult();
            }

            return JsonUtils.ErrorResult(message);
        }
    }
}
