using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yyx.BS.Models;

namespace Yyx.BS.Library.Services
{
    public class BaseServices
    {
        public BSDATAEntities db = new BSDATAEntities();

        /// <summary>
        /// 获取表ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public string SetID<T>(T t)
        {
            string id = string.Empty;
            string tName = t.GetType().FullName;
            if (true)
            {
                Seq seq = db.Seq.First(o => o.ObjectID == tName.ToString());
                if (seq != null)
                {
                    seq.CurrentValue = seq.CurrentValue + 1;
                    db.Seq.Attach(seq);
                    db.Entry(seq).State = EntityState.Modified;
                }
                else
                {
                    seq = new Seq();
                    seq.SeqID = Guid.NewGuid();
                    seq.ObjectID = tName.ToString();
                    seq.ObjectParm1 = GetParm(tName.ToString());
                    seq.CurrentValue = 1;
                    db.Seq.Add(seq);
                }
                db.SaveChanges();
                id = seq.ObjectParm1 + seq.CurrentValue.ToString().PadLeft(10, '0');
            }

            return id;
        }

        private string GetParm(string tableName)
        {
            string prarm = string.Empty;
            foreach (var item in tableName)
            {
                if (item >= 'A' && item <= 'Z')
                {
                    prarm += item;
                }
            }
            return prarm;
        }
    }
}
