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
    public class StoreServices : BaseServices
    {
        //门店登录
        public StoreOperator StoreLogin(string mobile, string password, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                StoreOperator oper = db.StoreOperator.Where(o => o.StoreOperatorMobile == mobile && o.DelStatus == false).FirstOrDefault();
                if (oper == null)
                {
                    message = "用户或密码有误！";
                    return null;
                }
                if (oper.StoreOperatorPassword != password)
                {
                    message = "用户或密码有误！";
                    return null;
                }

                return oper;
            }
        }

        public Store GetStore(string strorId, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                Store store = db.Store.Where(o => o.StoreID == strorId).FirstOrDefault();
                if (store == null)
                {
                    message = "门店不存在";
                    return null;
                }
                return store;
            }
        }

        //修改门店密码 
        public bool ModifyStorePassword(string storeOperatorId, string password, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                StoreOperator oper = db.StoreOperator.Where(o => o.StoreOperatorID == storeOperatorId && o.DelStatus == false).FirstOrDefault();
                if (oper == null)
                {
                    message = "用户不存在";
                    return false;
                }

                oper.StoreOperatorPassword = password;
                db.StoreOperator.Attach(oper);
                db.Entry(oper).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }

        //修改工作区域
        public Store ModifyStoreArea(string storeId, string area, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                Store store = db.Store.Where(o => o.StoreID == storeId && o.DelStatus == false).FirstOrDefault();
                if (store == null)
                {
                    message = "门店不存在";
                    return null;
                }
                store.Area = area;
                db.Store.Attach(store);
                db.Entry(store).State = EntityState.Modified;
                db.SaveChanges();

                return store;
            }
        }

        public void GetOrderTotalCount(string storeId, out int totalCountPaid, out int totalCountTryCancel)
        {
            using (db = new BSDATAEntities())
            {
                totalCountPaid = db.Order.Where(o => o.StoreID == storeId && o.OrderStatusID == OrderStatusEnum.Paid.ToString() && o.DelStatus == false).Count();
                totalCountTryCancel = db.Order.Where(o => o.StoreID == storeId && o.OrderStatusID == OrderStatusEnum.TryCancel.ToString() && o.DelStatus == false).Count();
            }
        }

        //获取门店订单信息（接单，发货，完成）
        public List<Order> GetOrderList(string storeId, string orderStatus, int pageIndex, int pageSize, out int totalCount)
        {
            using (db = new BSDATAEntities())
            {
                List<Order> orders = db.Order.Where(o => o.StoreID == storeId && o.OrderStatusID == orderStatus && o.DelStatus == false).OrderByDescending(o => o.ServiceDate).ToList();

                totalCount = orders.Count;
                orders = orders.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                return orders;
            }
        }

        //门店-接受订单发货
        public bool ConfirmOrder(string orderId, out string message)
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

                if (order.OrderStatusID != OrderStatusEnum.Paid.ToString())
                {
                    message = "订单状态不对";
                    return false;
                }

                order.OrderStatusID = OrderStatusEnum.ServerSent.ToString();
                db.Order.Attach(order);
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
        }

        //确认取消订单
        public bool ConfirmCancelOrder(string orderId, out string message)
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

                        if (order.OrderStatusID != OrderStatusEnum.TryCancel.ToString())
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
    }
}
