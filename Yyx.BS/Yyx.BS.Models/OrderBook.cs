//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Yyx.BS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderBook
    {
        public string OrderBookID { get; set; }
        public string UserID { get; set; }
        public string ProductID { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public decimal ActualAmount { get; set; }
        public string OrderMemo { get; set; }
        public Nullable<System.DateTime> ServiceDate { get; set; }
        public bool DelStatus { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
    }
}
