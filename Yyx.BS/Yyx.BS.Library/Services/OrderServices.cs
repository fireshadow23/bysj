using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yyx.BS.Models;
using System.Transactions;
using System.Data.Entity;
using Yyx.BS.Utils;
using System.Text.RegularExpressions;

namespace Yyx.BS.Library.Services
{
    public class OrderServices : BaseServices
    {
        //获取商品分类信息
        public List<ProductCategory> GetProductCategory()
        {
            using (db = new BSDATAEntities())
            {
                List<ProductCategory> productCategoryList = db.ProductCategory.Where(o => o.DelStatus == false).ToList();
                return productCategoryList;
            }
        }

        //获取商品信息
        public List<Product> GetProduct(string productCategoryId)
        {
            using (db = new BSDATAEntities())
            {
                List<Product> productList = db.Product.Where(o => o.ProductCategoryID == productCategoryId).ToList();
                return productList;
            }
        }

        //获取订单列表（待支付订单和支付订单）
        public List<OrderBook> GetOrderBook(string userId)
        {
            using (db = new BSDATAEntities())
            {
                List<OrderBook> orderBookList = db.OrderBook.Where(o => o.UserID == userId && o.DelStatus == false).ToList();
                return orderBookList;
            }
        }

        public List<Order> GetOrder(string userId)
        {
            using (db = new BSDATAEntities())
            {
                List<Order> orderList = db.Order.Where(o => o.UserID == userId && o.DelStatus == false).ToList();
                return orderList;
            }
        }

        public List<OrderItem> GetOrderItem(string orderId)
        {
            using (db = new BSDATAEntities())
            {
                List<OrderItem> orderItemList = db.OrderItem.Where(o => o.OrderID == orderId && o.DelStatus == false).ToList();
                return orderItemList;
            }
        }

        //添加待支付订单
        public OrderBook AddOrderBook(string userId, string storeId, string productId, decimal quantity, decimal amount, string orderMemo, DateTime serviceDate)
        {
            using (db = new BSDATAEntities())
            {
                OrderBook orderBook = new OrderBook();
                orderBook.OrderBookID = SetID(orderBook);
                orderBook.UserID = userId;
                orderBook.ProductID = productId;
                orderBook.Quantity = quantity;
                orderBook.Amount = amount;
                orderBook.ActualAmount = quantity * amount;
                orderBook.OrderMemo = orderMemo;
                orderBook.ServiceDate = serviceDate;
                orderBook.DelStatus = false;
                orderBook.UpdateDate = DateTime.Now;
                orderBook.CreateDate = DateTime.Now;
                db.OrderBook.Add(orderBook);
                db.SaveChanges();

                return orderBook;
            }
        }

        //订单支付(账户支付，可扩展第三方支付)
        public bool PaidOrder(List<OrderBook> orderBookList, string userId, string userAddressId, out string message)
        {
            message = string.Empty;
            if (orderBookList.Exists(o => o.UserID != userId))
            {
                message = "订单商品有误，请重新选择";
                return false;
            }
            using (db = new BSDATAEntities())
            {
                User user = db.User.First(o => o.UserID == userId && o.DelStatus == false);
                if (user == null)
                {
                    message = "用户不存在";
                    return false;
                }

                Account account = db.Account.First(o => o.AccountID == user.AccountID);
                decimal totalAmount = orderBookList.Sum(o => o.ActualAmount);
                if (account == null || account.Amount < totalAmount)
                {
                    message = "账户余额不足";
                    return false;
                }

                //根据用户地址对商品进行门店选择，并分组生成订单

                Order order = new Order();
                order.OrderID = SetID(order);
                order.OrderNo = Id2No(order.OrderID);
                order.UserID = userId;
                order.UserAddressID = userAddressId;
                order.OrderAmount = totalAmount;
                order.ActualAmount = totalAmount;
                order.PaymentAmount = totalAmount;
                order.OrderStatusID = OrderStatusEnum.Paid.ToString();
                order.PayStatusID = PayStatusEnum.Paid.ToString();
                //order.StoreID = 
                //order.OrderMemo = ""; todo
                //order.ServiceDate = DateTime.Now;
                order.DelStatus = false;
                order.CreateDate = DateTime.Now;
                order.UpdateDate = DateTime.Now;
                db.Order.Add(order);

                foreach (var item in orderBookList)
                {
                    OrderItem orderItem = new OrderItem();
                    orderItem.OrderItemID = SetID(orderItem);
                    orderItem.OrderID = order.OrderID;
                    orderItem.ProductID = item.ProductID;
                    orderItem.Quantity = item.Quantity;
                    orderItem.Amount = item.Amount;
                    orderItem.ActualAmount = item.ActualAmount;
                    orderItem.DelStatus = false;
                    orderItem.CreateDate = DateTime.Now;
                    orderItem.UpdatDate = DateTime.Now;
                    db.OrderItem.Add(orderItem);
                }

                account.Amount -= totalAmount;
                account.UpdateDate = DateTime.Now;
                db.Account.Attach(account);
                db.Entry(account).State = EntityState.Modified;

                Balance balance = new Balance();
                balance.BalanceID = SetID(balance);
                balance.AccountID = user.AccountID;
                balance.Amount = -totalAmount;
                balance.Discription = "支付订单:" + order.OrderNo;
                balance.CreateDate = DateTime.Now;
                db.Balance.Add(balance);

                return true;
            }
        }

        public static string Id2No(string id)
        {
            id = Regex.Replace(id, "[a-zA-Z]+", "");//去除字母

            uint a = uint.Parse(id) ^ 0x8fffffff;
            uint b = uint.Parse(id) & 0x7777777f;

            uint x = a * 7100390 + b;
            return x.ToString("0000000000");
        }

        //取消订单（1.待支付取消 2.支付取消 3.发货中取消）
        public bool CancelOrderBook(string orderBookId, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                OrderBook orderBook = db.OrderBook.First(o => o.OrderBookID == orderBookId && o.DelStatus == false);
                if (orderBook == null)
                {
                    return false;
                }
                orderBook.DelStatus = true;
                db.OrderBook.Attach(orderBook);
                db.Entry(orderBook).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }

        public bool CancelOrder(string orderId, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                Order order = db.Order.First(o => o.OrderID == orderId && o.DelStatus == false);
                if (order == null)
                {
                    return false;
                }
                order.DelStatus = true;
                db.Order.Attach(order);
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }

        //完成订单
        public bool CompleteOrder(string orderId, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                Order order = db.Order.First(o => o.OrderID == orderId && o.DelStatus == false);
                if (order == null)
                {
                    message = "订单不存在";
                    return false;
                }
                if (order.OrderStatusID != OrderStatusEnum.ServerSent.ToString() ||
                    order.OrderStatusID != OrderStatusEnum.TryCancel.ToString())
                {
                    message = "订单状态不对";
                    return false;
                }

                order.OrderStatusID = OrderStatusEnum.Complete.ToString();
                db.Order.Attach(order);
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }

        //订单评价
        public bool CommentOrder(string orderId, string commentContent, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                Order order = db.Order.First(o => o.OrderID == orderId && o.DelStatus == false);
                if (order == null)
                {
                    message = "订单不存在";
                    return false;
                }

                OrderComment orderComment = new OrderComment();
                orderComment.OrderID = orderId;
                orderComment.CommentContent = commentContent;
                orderComment.DelStatus = false;
                orderComment.UpdateDate = DateTime.Now;
                orderComment.CreateDate = DateTime.Now;
                db.OrderComment.Add(orderComment);
                db.SaveChanges();

                return true;
            }
        }
    }
}
