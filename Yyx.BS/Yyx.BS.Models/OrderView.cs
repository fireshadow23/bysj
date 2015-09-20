using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yyx.BS.Models
{
    public class OrderView : Order
    {
        public string OrderStatusDefCN { get; set; }
        public string PayStatusDefCN { get; set; }
        public bool HasComment { get; set; }
        public string OrderCommentContent { get; set; }
        public string CreateDateStr
        {
            get
            {
                return CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        public string ServiceDateStr
        {
            get
            {
                return ServiceDate.GetValueOrDefault().ToString("yyyy-MM-dd");
            }
        }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactAddress { get; set; }

        public List<OrderItemView> OrderItemViews { get; set; }
    }

    public class OrderItemView : OrderItem
    {
        public string OrderStatusID { get; set; }
        public string ProductName { get; set; }
        public bool ShowBtn { get; set; }
        public int TotalCount { get; set; }
    }

    public class BalanceView
    {
        public string CreateDate { get; set; }
        public string Amount { get; set; }
        public string Discription { get; set; }
    }
}
