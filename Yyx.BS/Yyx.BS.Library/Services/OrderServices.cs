using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yyx.BS.Models;
using System.Transactions;
using System.Data.Entity;
using Yyx.BS.Utils;

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
        public Product GetProduct(string productId)
        {
            using (db = new BSDATAEntities())
            {
                Product product = db.Product.Where(o => o.ProductID == productId).FirstOrDefault();
                return product;
            }
        }

        public List<Product> GetProducts()
        {
            using (db = new BSDATAEntities())
            {
                List<Product> productList = db.Product.Where(o => 1 == 1).ToList();
                return productList;
            }
        }
        public List<Product> GetProducts(List<string> Ids)
        {
            using (db = new BSDATAEntities())
            {
                List<Product> productList = db.Product.Where(o => Ids.Contains(o.ProductCategoryID)).ToList();
                return productList;
            }
        }
        public List<Product> GetProducts(string productCategoryId)
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

        public List<OrderBook> GetOrderBooks(List<string> orderBookIds)
        {
            using (db = new BSDATAEntities())
            {
                List<OrderBook> orderBookList = db.OrderBook.Where(o => o.DelStatus == false && orderBookIds.Contains(o.OrderBookID)).ToList();
                return orderBookList;
            }
        }

        public Order GetOrder(string orderId)
        {
            using (db = new BSDATAEntities())
            {
                Order order = db.Order.Where(o => o.OrderID == orderId && o.DelStatus == false).FirstOrDefault();

                return order;
            }
        }

        public List<Order> GetOrder(string userId, int pageIndex, int pageSize, out int totalCount)
        {
            using (db = new BSDATAEntities())
            {
                totalCount = 0;
                List<Order> orderList = db.Order.Where(o => o.UserID == userId && o.DelStatus == false).OrderByDescending(o => o.CreateDate).ToList();
                if (orderList != null)
                {
                    totalCount = orderList.Count;
                    return orderList.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                }
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

        public string GetOrderStatus(string orderStatusId)
        {
            using (db = new BSDATAEntities())
            {
                OrderStatus orderstatus = db.OrderStatus.Where(o => o.OrderStatusID == orderStatusId).FirstOrDefault();
                if (orderstatus != null)
                {
                    return orderstatus.OrderStatusDefCN;
                }
                return string.Empty;
            }
        }

        public string GetPayStatus(string payStatusId)
        {
            using (db = new BSDATAEntities())
            {
                PayStatus payStatus = db.PayStatus.Where(o => o.PayStatusID == payStatusId).FirstOrDefault();
                if (payStatus != null)
                {
                    return payStatus.PayStatusDefCN;
                }
                return string.Empty;
            }
        }

        //添加待支付订单
        public OrderBook AddOrderBook(string userId, string productId, decimal quantity, decimal amount, string orderMemo, DateTime serviceDate)
        {
            using (db = new BSDATAEntities())
            {
                OrderBook orderBook = db.OrderBook.Where(o => o.UserID == userId && o.ProductID == productId && o.DelStatus == false).FirstOrDefault();
                if (orderBook != null)
                {
                    orderBook.Quantity += quantity;
                    orderBook.Amount = amount;
                    orderBook.ActualAmount = orderBook.Quantity.GetValueOrDefault() * amount;
                    orderBook.OrderMemo = orderMemo;
                    orderBook.ServiceDate = serviceDate;
                    orderBook.UpdateDate = DateTime.Now;
                    db.OrderBook.Attach(orderBook);
                    db.Entry(orderBook).State = EntityState.Modified;
                }
                else
                {
                    orderBook = new OrderBook();
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
                }

                db.SaveChanges();

                return orderBook;
            }
        }

        //订单支付(账户支付，可扩展第三方支付)
        public bool PaidOrder(List<OrderBook> orderBookList, string userId, string userAddressId, DateTime serviceDate, string orderMemo, out string message)
        {
            message = string.Empty;
            if (orderBookList.Exists(o => o.UserID != userId))
            {
                message = "订单商品有误，请重新选择";
                return false;
            }

            using (db = new BSDATAEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        User user = db.User.Where(o => o.UserID == userId && o.DelStatus == false).FirstOrDefault();
                        if (user == null)
                        {
                            message = "用户不存在";
                            return false;
                        }

                        Account account = db.Account.Where(o => o.AccountID == user.AccountID).FirstOrDefault();
                        decimal totalAmount = orderBookList.Sum(o => o.ActualAmount);
                        if (account == null || account.Amount < totalAmount)
                        {
                            message = "账户余额不足";
                            return false;
                        }

                        UserAddress addr = db.UserAddress.Where(o => o.UserAddressID == userAddressId && o.DelStatus == false).FirstOrDefault();
                        if (addr == null)
                        {
                            message = "地址不存在";
                            return false;
                        }

                        AddressLibrary addrlibcity = db.AddressLibrary.Where(o => o.AddressName == addr.City && o.AddressLevel == "2").FirstOrDefault();
                        if (addrlibcity == null)
                        {
                            message = "非常抱歉，地址所在城市暂时没有提供服务";
                            return false;
                        }

                        AddressLibrary addrlibarea = db.AddressLibrary.Where(o => o.AddressName == addr.Area && o.AddressLevel == "3").FirstOrDefault();
                        if (addrlibarea == null)
                        {
                            message = "非常抱歉，地址所在地区暂时没有提供服务";
                            return false;
                        }

                        Store store = db.Store.Where(o => o.CityID == addrlibcity.AddressLibraryID && o.WorkArea.Contains(addrlibarea.AddressLibraryID) && o.DelStatus == false).FirstOrDefault();
                        if (store == null)
                        {
                            message = "非常抱歉，地址所在城市地区暂时没有提供服务";
                            return false;
                        }

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
                        //todo根据用户地址对商品进行门店选择
                        order.StoreID = store.StoreID;
                        order.OrderMemo = orderMemo;
                        order.ServiceDate = serviceDate;
                        order.DelStatus = false;
                        order.CreateDate = DateTime.Now;
                        order.UpdateDate = DateTime.Now;
                        db.Order.Add(order);

                        foreach (var item in orderBookList)
                        {
                            OrderBook book = db.OrderBook.Where(o => o.OrderBookID == item.OrderBookID).FirstOrDefault();
                            if (book != null)
                            {
                                book.DelStatus = true;
                                db.OrderBook.Attach(book);
                                db.Entry(book).State = EntityState.Modified;

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
                        }

                        //打钱给平台
                        Payment payment = new Payment();
                        payment.PaymentID = SetID(payment);
                        payment.OrderID = order.OrderID;
                        payment.PaymentNo = Id2No(payment.PaymentID);
                        payment.Amount = totalAmount;
                        payment.ActualAmount = totalAmount;
                        payment.PaymentDate = DateTime.Now;
                        payment.PaymentTypeID = PaymentTypeEnum.AccountPay.ToString();
                        payment.PayStatusID = PayStatusEnum.Paid.ToString();
                        payment.FromAccountID = account.AccountID;
                        payment.ToAccountID = ConfigHelper.GetPlatformAccount();
                        payment.CreateDate = DateTime.Now;
                        payment.UpdateDate = DateTime.Now;
                        db.Payment.Add(payment);

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

                        Balance balancePlat = new Balance();
                        balancePlat.BalanceID = SetID(balancePlat);
                        balancePlat.AccountID = ConfigHelper.GetPlatformAccount();
                        balancePlat.Amount = totalAmount;
                        balancePlat.Discription = "支付订单:" + order.OrderNo;
                        balancePlat.CreateDate = DateTime.Now;
                        db.Balance.Add(balancePlat);

                        db.SaveChanges();
                        scope.Complete();
                        return true;
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }
            }

        }

        //取消订单（1.待支付取消 2.支付取消 3.发货中取消）
        public bool CancelOrderBook(string orderBookId, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                OrderBook orderBook = db.OrderBook.Where(o => o.OrderBookID == orderBookId && o.DelStatus == false).FirstOrDefault();
                if (orderBook == null)
                {
                    message = "订单不存在";
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
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {

                        Order order = db.Order.Where(o => o.OrderID == orderId && o.DelStatus == false).FirstOrDefault();
                        if (order == null)
                        {
                            message = "订单不存在";
                            return false;
                        }

                        if (order.OrderStatusID != OrderStatusEnum.Paid.ToString())
                        {
                            message = "订单状态不对";
                            return false;
                        }
                        order.OrderStatusID = OrderStatusEnum.UserCancel.ToString();
                        db.Order.Attach(order);
                        db.Entry(order).State = EntityState.Modified;

                        User user = db.User.Where(o => o.UserID == order.UserID && o.DelStatus == false).FirstOrDefault();
                        if (user == null)
                        {
                            message = "用户不存在";
                            return false;
                        }

                        //打钱给用户
                        Payment payment = new Payment();
                        payment.PaymentID = SetID(payment);
                        payment.OrderID = order.OrderID;
                        payment.PaymentNo = Id2No(payment.PaymentID);
                        payment.Amount = order.ActualAmount;
                        payment.ActualAmount = order.ActualAmount;
                        payment.PaymentDate = DateTime.Now;
                        payment.PaymentTypeID = PaymentTypeEnum.AccountPay.ToString();
                        payment.PayStatusID = PayStatusEnum.Paid.ToString();
                        payment.FromAccountID = ConfigHelper.GetPlatformAccount();
                        payment.ToAccountID = user.AccountID;
                        payment.CreateDate = DateTime.Now;
                        payment.UpdateDate = DateTime.Now;
                        db.Payment.Add(payment);

                        Account account = db.Account.Where(o => o.AccountID == user.AccountID).FirstOrDefault();
                        if (account == null)
                        {
                            account.AccountID = SetID(account);
                            account.Amount = order.ActualAmount;
                            account.FrozenAmount = 0;
                            account.CreateDate = DateTime.Now;
                            account.UpdateDate = DateTime.Now;
                            db.Account.Add(account);
                        }
                        else
                        {
                            account.Amount += order.ActualAmount;
                            account.UpdateDate = DateTime.Now;
                            db.Account.Attach(account);
                            db.Entry(account).State = EntityState.Modified;
                        }

                        Balance balance = new Balance();
                        balance.BalanceID = SetID(balance);
                        balance.AccountID = user.AccountID;
                        balance.Amount = order.ActualAmount;
                        balance.Discription = "取消订单:" + order.OrderNo;
                        balance.CreateDate = DateTime.Now;
                        db.Balance.Add(balance);

                        Balance balancePlat = new Balance();
                        balancePlat.BalanceID = SetID(balance);
                        balancePlat.AccountID = ConfigHelper.GetPlatformAccount();
                        balancePlat.Amount = -order.ActualAmount;
                        balancePlat.Discription = "取消订单:" + order.OrderNo;
                        balancePlat.CreateDate = DateTime.Now;
                        db.Balance.Add(balancePlat);

                        db.SaveChanges();
                        scope.Complete();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
        }

        public bool DeleteOrder(string orderId, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                Order order = db.Order.Where(o => o.OrderID == orderId && o.DelStatus == false).FirstOrDefault();
                if (order == null)
                {
                    message = "订单不存在";
                    return false;
                }
                order.DelStatus = true;
                db.Order.Attach(order);
                db.Entry(order).State = EntityState.Modified;

                db.SaveChanges();

                return true;
            }
        }

        //申请取消
        public bool TryCancelOrder(string orderId, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                Order order = db.Order.Where(o => o.OrderID == orderId && o.DelStatus == false).FirstOrDefault();
                if (order == null)
                {
                    message = "订单不存在";
                    return false;
                }
                order.OrderStatusID = OrderStatusEnum.TryCancel.ToString();
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
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {

                        Order order = db.Order.Where(o => o.OrderID == orderId && o.DelStatus == false).FirstOrDefault();
                        if (order == null)
                        {
                            message = "订单不存在";
                            return false;
                        }
                        if (order.OrderStatusID != OrderStatusEnum.ServerSent.ToString() &&
                            order.OrderStatusID != OrderStatusEnum.TryCancel.ToString())
                        {
                            message = "订单状态不对";
                            return false;
                        }

                        order.OrderStatusID = OrderStatusEnum.Complete.ToString();
                        db.Order.Attach(order);
                        db.Entry(order).State = EntityState.Modified;

                        Store store = db.Store.Where(o => o.StoreID == order.StoreID && o.DelStatus == false).FirstOrDefault();
                        if (store == null)
                        {
                            message = "无法完成订单，请联系客服";
                            return false;
                        }

                        //打钱给门店
                        Payment payment = new Payment();
                        payment.PaymentID = SetID(payment);
                        payment.OrderID = order.OrderID;
                        payment.PaymentNo = Id2No(payment.PaymentID);
                        payment.Amount = order.ActualAmount;
                        payment.ActualAmount = order.ActualAmount;
                        payment.PaymentDate = DateTime.Now;
                        payment.PaymentTypeID = PaymentTypeEnum.AccountPay.ToString();
                        payment.PayStatusID = PayStatusEnum.Paid.ToString();
                        payment.FromAccountID = ConfigHelper.GetPlatformAccount(); ;
                        payment.ToAccountID = store.AccountID;
                        payment.CreateDate = DateTime.Now;
                        payment.UpdateDate = DateTime.Now;
                        db.Payment.Add(payment);

                        Account account = db.Account.Where(o => o.AccountID == store.AccountID).FirstOrDefault();
                        if (account == null)
                        {
                            account.AccountID = SetID(account);
                            account.Amount = order.ActualAmount;
                            account.FrozenAmount = 0;
                            account.CreateDate = DateTime.Now;
                            account.UpdateDate = DateTime.Now;
                            db.Account.Add(account);
                        }
                        else
                        {
                            account.Amount += order.ActualAmount;
                            account.UpdateDate = DateTime.Now;
                            db.Account.Attach(account);
                            db.Entry(account).State = EntityState.Modified;
                        }

                        Balance balance = new Balance();
                        balance.BalanceID = SetID(balance);
                        balance.AccountID = store.AccountID;
                        balance.Amount = order.ActualAmount;
                        balance.Discription = "完成订单:" + order.OrderNo;
                        balance.CreateDate = DateTime.Now;
                        db.Balance.Add(balance);

                        Balance balancePlat = new Balance();
                        balancePlat.BalanceID = SetID(balance);
                        balancePlat.AccountID = ConfigHelper.GetPlatformAccount();
                        balancePlat.Amount = -order.ActualAmount;
                        balancePlat.Discription = "完成订单:" + order.OrderNo;
                        balancePlat.CreateDate = DateTime.Now;
                        db.Balance.Add(balancePlat);

                        db.SaveChanges();
                        scope.Complete();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
        }

        public OrderComment GetOrderComment(string orderId)
        {
            using (db = new BSDATAEntities())
            {
                Order order = db.Order.Where(o => o.OrderID == orderId && o.DelStatus == false).FirstOrDefault();
                if (order == null)
                {
                    return null;
                }

                OrderComment comment = db.OrderComment.Where(o => o.OrderID == orderId && o.DelStatus == false).FirstOrDefault();
                if (comment == null)
                {
                    return null;
                }

                return comment;
            }
        }
        //订单评价
        public bool CommentOrder(string orderId, string commentContent, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                Order order = db.Order.Where(o => o.OrderID == orderId && o.DelStatus == false).FirstOrDefault();
                if (order == null)
                {
                    message = "订单不存在";
                    return false;
                }

                OrderComment orderComment = new OrderComment();
                orderComment.OrderCommentID = SetID(orderComment);
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
