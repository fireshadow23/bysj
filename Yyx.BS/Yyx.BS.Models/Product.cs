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
    
    public partial class Product
    {
        public Product()
        {
            this.StoreProduct = new HashSet<StoreProduct>();
        }
    
        public string ProductID { get; set; }
        public string ProductCategoryID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Discription { get; set; }
        public string Notice { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
    
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ICollection<StoreProduct> StoreProduct { get; set; }
    }
}
