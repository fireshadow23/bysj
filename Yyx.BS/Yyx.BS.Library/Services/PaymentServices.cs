using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yyx.BS.Models;
using System.Transactions;
using System.Data.Entity;

namespace Yyx.BS.Library.Services
{
    public class PaymentServices : BaseServices
    {
        /// <summary>
        /// 查看用户账户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Account GetUserAccount(string userId)
        {
            using (db = new BSDATAEntities())
            {
                User user = db.User.First(o => o.UserID == userId && o.DelStatus == false);
                if (user == null)
                {
                    return null;
                }

                Account account = db.Account.First(o => o.AccountID == user.AccountID);
                if (account == null)
                {
                    return null;
                }

                return account;
            }
        }

        /// <summary>
        /// 获取用户账户使用情况
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Balance> GetUserBalance(string userId)
        {
            using (db = new BSDATAEntities())
            {
                User user = db.User.First(o => o.UserID == userId && o.DelStatus == false);
                if (user == null)
                {
                    return null;
                }

                List<Balance> balanceList = db.Balance.Where(o => o.AccountID == user.AccountID).ToList();

                return balanceList;
            }
        }


        /// <summary>
        /// 用户充值
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="amount"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool Recharge(string userId, decimal amount, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                User user = db.User.First(o => o.UserID == userId && o.DelStatus == false);
                if (user == null)
                {
                    message = "用户不存在";
                    return false;
                }

                Account account = db.Account.First(o => o.AccountID == user.AccountID);
                if (account == null)
                {
                    account.AccountID = SetID(account);
                    account.Amount = amount;
                    account.FrozenAmount = 0;
                    account.CreateDate = DateTime.Now;
                    account.UpdateDate = DateTime.Now;
                    db.Account.Add(account);
                }
                else
                {
                    account.Amount += amount;
                    account.UpdateDate = DateTime.Now;
                    db.Account.Attach(account);
                    db.Entry(account).State = EntityState.Modified;
                }

                Balance balance = new Balance();
                balance.BalanceID = SetID(balance);
                balance.AccountID = user.AccountID;
                balance.Amount = amount;
                balance.Discription = "用户充值";
                balance.CreateDate = DateTime.Now;
                db.Balance.Add(balance);
                db.SaveChanges();

                return true;
            }
        }
    }
}
