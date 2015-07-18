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
    public class UserServices : BaseServices
    {
        public User GetUserInfo(string userId)
        {
            using (db = new BSDATAEntities())
            {
                User user = db.User.First(o => o.UserID == userId && o.DelStatus == false);
                return user;
            }
        }

        public User AddNewUser(string mobile, string password, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                List<User> userList = db.User.Where(o => o.CellPhone == mobile && o.DelStatus == false).ToList();
                if (userList != null && userList.Count > 0)
                {
                    message = "手机号已注册";
                    return null;
                }

                Account account = new Account();
                account.AccountID = SetID(account);
                account.Amount = 0;
                account.FrozenAmount = 0;
                account.CreateDate = DateTime.Now;
                account.UpdateDate = DateTime.Now;
                db.Account.Add(account);

                User user = new User();
                user.AccountID = account.AccountID;
                user.UserID = SetID(user);
                user.CellPhone = mobile;
                user.PassWord = password;
                user.DelStatus = false;
                user.CreateDate = DateTime.Now;
                user.UpdateDate = DateTime.Now;
                db.User.Add(user);
                db.SaveChanges();

                return user;
            }
        }

        public User ModifyUserInfo(string userId, User userInfo, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                User user = db.User.First(o => o.UserID == userId && o.DelStatus == false);
                if (user == null)
                {
                    message = "用户不存在";
                    return null;
                }

                user.HeadImagePath = userInfo.HeadImagePath;
                user.UserName = userInfo.UserName;
                user.RealName = userInfo.RealName;
                user.Email = userInfo.Email;
                user.Birthday = userInfo.Birthday;
                user.UpdateDate = DateTime.Now;
                db.User.Attach(user);
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                return user;
            }
        }

        public bool UserLogin(string mobile, string password, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                List<User> userList = db.User.Where(o => o.CellPhone == mobile && o.DelStatus == false).ToList();
                if (userList == null || userList.Count == 0)
                {
                    message = "用户不存在";
                    return false;
                }
                if (userList.First().PassWord != password)
                {
                    message = "密码有误";
                    return false;
                }
                return true;
            }
        }

        public bool ChangePassword(string userId, string password, string newPassword, out string message)
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

                if (user.PassWord != password)
                {
                    message = "旧密码有误";
                    return false;
                }

                user.PassWord = newPassword;
                user.UpdateDate = DateTime.Now;
                db.User.Attach(user);
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }
    }
}
