using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yyx.BS.Library.Services;

namespace Yyx.BS.Controller
{
    public class DataUtils
    {
        public static OrderServices orderServices = new OrderServices();
        public static int GetBookNumber(string userId)
        {
            var books = orderServices.GetOrderBook(userId);
            if (books != null)
                return books.Count;
            else
                return 0;
        }
        public static List<Models.ProductCategory> GetProductCategoryList()
        {
            return orderServices.GetProductCategory();
        }
    }
}
