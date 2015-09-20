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
    public class StoreController : System.Web.Mvc.Controller
    {
        [Dependency]
        public StoreServices storeServices { get; set; }

        [Dependency]
        public OrderServices orderServices { get; set; }

        [Dependency]
        public UserServices userServices { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StoreLogin(string mobile, string password)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                return JsonUtils.ErrorResult("手机不能为空");
            }
            if (string.IsNullOrEmpty(password))
            {
                return JsonUtils.ErrorResult("密码不能为空");
            }

            string message = string.Empty;
            Models.StoreOperator oper = storeServices.StoreLogin(mobile, CryptographyUtils.EncryptString(password), out message);
            if (string.IsNullOrEmpty(message))
            {
                //当前门店
                var store = storeServices.GetStore(oper.StoreID, out message);
                if (string.IsNullOrEmpty(message))
                {
                    Session["CurrentStore"] = store;
                    return JsonUtils.SuccessResult();
                }
            }

            return JsonUtils.ErrorResult(message);

        }

        public ActionResult Order()
        {
            Models.Store store = Session["CurrentStore"] as Models.Store;
            if (store == null)
            {
                return Redirect("/Store/Index");
            }
            return View();
        }

        public ActionResult GetOrderTotalCount()
        {
            Models.Store store = Session["CurrentStore"] as Models.Store;
            if (store == null)
            {
                return JsonUtils.ErrorResult();
            }
            int totalCountPaid = 0;
            int totalCountTryCancel = 0;
            storeServices.GetOrderTotalCount(store.StoreID, out totalCountPaid, out totalCountTryCancel);

            StringBuilder buider = new StringBuilder("{");
            buider.AppendFormat("\"{0}\":\"{1}\",", "totalCountPaid", totalCountPaid);
            buider.AppendFormat("\"{0}\":\"{1}\"", "totalCountTryCancel", totalCountTryCancel);
            buider.Append('}');
            return JsonUtils.GetJsonResult(buider.ToString());
        }

        public ActionResult GetOrderList(int pageIndex, int pageSize, string orderStatus)
        {
            Models.Store store = Session["CurrentStore"] as Models.Store;
            if (store == null)
            {
                return JsonUtils.ErrorResult("请重新登录");
            }

            int totalCount = 0;
            List<Models.Product> products = orderServices.GetProducts();
            List<Models.Order> orders = storeServices.GetOrderList(store.StoreID, orderStatus, pageIndex, pageSize, out totalCount);
            List<Models.OrderView> orderViews = new List<Models.OrderView>();
            foreach (var order in orders)
            {
                Models.OrderView orderView = new Models.OrderView();
                EntityObject.FillEntity(order, orderView);
                orderView.OrderStatusDefCN = orderServices.GetOrderStatus(order.OrderStatusID);
                Models.UserAddress addr = userServices.GetUserAddressByID(order.UserAddressID);
                if (addr != null)
                {
                    orderView.ContactName = addr.ContactName;
                    orderView.ContactPhone = addr.ContactPhone;
                    orderView.ContactAddress = addr.Province + addr.City + addr.Area + addr.Street;
                }

                List<Models.OrderItem> orderItems = orderServices.GetOrderItem(order.OrderID);
                orderView.OrderItemViews = new List<Models.OrderItemView>();
                foreach (var orderItem in orderItems)
                {
                    Models.OrderItemView orderItemView = new Models.OrderItemView();
                    EntityObject.FillEntity(orderItem, orderItemView);
                    var product = products.Find(o => o.ProductID == orderItem.ProductID);
                    orderItemView.ProductName = product.ProductName;
                    orderView.OrderItemViews.Add(orderItemView);
                }
                orderViews.Add(orderView);
            }

            return JsonUtils.GetJsonResult(orderViews, "OrderViews", totalCount);
        }

        public ActionResult ConfirmOrder(string orderId)
        {
            Models.Store store = Session["CurrentStore"] as Models.Store;
            if (store == null)
            {
                return JsonUtils.ErrorResult("请重新登录");
            }

            string message = string.Empty;
            if (storeServices.ConfirmOrder(orderId, out message))
            {
                return JsonUtils.SuccessResult();
            }

            return JsonUtils.ErrorResult(message);
        }

        public ActionResult ConfirmCancelOrder(string orderId)
        {
            Models.Store store = Session["CurrentStore"] as Models.Store;
            if (store == null)
            {
                return JsonUtils.ErrorResult("请重新登录");
            }

            string message = string.Empty;
            if (storeServices.ConfirmCancelOrder(orderId, out message))
            {
                return JsonUtils.SuccessResult();
            }

            return JsonUtils.ErrorResult(message);
        }
    }
}
