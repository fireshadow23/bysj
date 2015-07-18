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
    
    public partial class Account
    {
        public Account()
        {
            this.Balance = new HashSet<Balance>();
            this.Store = new HashSet<Store>();
            this.User = new HashSet<User>();
        }
    
        public string AccountID { get; set; }
        public decimal Amount { get; set; }
        public Nullable<decimal> FrozenAmount { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
    
        public virtual ICollection<Balance> Balance { get; set; }
        public virtual ICollection<Store> Store { get; set; }
        public virtual ICollection<User> User { get; set; }
    }
}