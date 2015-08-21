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
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public User GetUserInfo(string userId)
        {
            using (db = new BSDATAEntities())
            {
                User user = db.User.First(o => o.UserID == userId && o.DelStatus == false);
                return user;
            }
        }

        /// <summary>
        /// 添加新用户
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <param name="message"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userInfo"></param>
        /// <param name="message"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <param name="message"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="newPassword"></param>
        /// <param name="message"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取用户地址
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserAddress> GetUserAddress(string userId)
        {
            using (db = new BSDATAEntities())
            {
                List<UserAddress> addrList = db.UserAddress.Where(o => o.UserID == userId && o.DelStatus == false).ToList();
                return addrList;
            }
        }

        /// <summary>
        /// 添加用户地址
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="area"></param>
        /// <param name="street"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public UserAddress AddUserAddress(string userId, string province, string city, string area, string street, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                UserAddress addr = new UserAddress();
                addr.UserAddressID = SetID(addr);
                addr.UserID = userId;
                addr.Province = province;
                addr.City = city;
                addr.Area = area;
                addr.Street = street;
                addr.DelStatus = false;
                addr.CreateDate = DateTime.Now;
                addr.UpdateDate = DateTime.Now;
                db.UserAddress.Add(addr);
                db.SaveChanges();

                return addr;
            }
        }

        /// <summary>
        /// 修改用户地址
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userAddressId"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="area"></param>
        /// <param name="street"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public UserAddress ModifyAddress(string userId, string userAddressId, string province, string city, string area, string street, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                UserAddress addr = db.UserAddress.First(o => o.UserID == userId && o.UserAddressID == userAddressId && o.DelStatus == false);
                if (addr == null)
                {
                    message = "用户不存在";
                    return null;
                }

                addr.Province = province;
                addr.City = city;
                addr.Area = area;
                addr.Street = street;
                addr.UpdateDate = DateTime.Now;
                db.UserAddress.Attach(addr);
                db.Entry(addr).State = EntityState.Modified;
                db.SaveChanges();

                return addr;
            }
        }

        /// <summary>
        /// 删除地址
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userAddressId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool DeleteAddress(string userId, string userAddressId, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                UserAddress addr = db.UserAddress.First(o => o.UserID == userId && o.UserAddressID == userAddressId && o.DelStatus == false);
                if (addr == null)
                {
                    message = "用户不存在";
                    return true;
                }

                addr.DelStatus = true;
                addr.UpdateDate = DateTime.Now;
                db.UserAddress.Attach(addr);
                db.Entry(addr).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }

    }
}
