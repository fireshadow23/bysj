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
    
    public partial class StoreProduct
    {
        public string StoreProductID { get; set; }
        public string StoreID { get; set; }
        public string ProductID { get; set; }
        public bool DelStatus { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual Store Store { get; set; }
    }
}