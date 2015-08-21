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
    public class StoreServices : BaseServices
    {
        //门店登录
        public StoreOperator StoreLogin(string mobile, string password, out string message)
        {
            message = string.Empty;
            using (db = new BSDATAEntities())
            {
                StoreOperator oper = db.StoreOperator.First(o => o.StoreOperatorMobile == mobile && o.DelStatus == false);
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
                Store store = db.Store.First(o => o.StoreID == strorId);
                if (store == null)
                {
                    message = "门店不存在";
                    return null;
                }
                return store;
            }
        }

        //修改门店密码 

        //修改工作区域

        //设置门店商品信息

        //获取门店订单信息（接单，发货，完成）

        //门店-接受订单发货
    }
}
