using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Yyx.BS.Library.Services;

namespace Yyx.BS.Controller
{
    public class HomeController : System.Web.Mvc.Controller
    {
        [Dependency]
        public OrderServices orderServices { get; set; }
        public ActionResult Index()
        {
            ViewBag.User = Session["CurrentUser"] as Yyx.BS.Models.User;
            ViewBag.ShowCarousel = true;
            ViewBag.ProductList = orderServices.GetProducts().Take(8).ToList();
            return View();
        }

        public ActionResult ShowProduct(string id)
        {
            List<Models.Product> productList = new List<Models.Product>();
            List<Models.ProductCategory> productCategoryList = orderServices.GetProductCategory();
            if (id == null)
            {
                productList = orderServices.GetProducts();
            }
            else
            {
                List<string> ids = new List<string>();
                ids.Add(id);
                var productCategory = productCategoryList.Find(o => o.ProductCategoryID == id);
                if (productCategory != null && productCategory.ParentID == null)
                {
                    foreach (var item in productCategoryList.FindAll(o => o.ParentID == id))
                    {
                        ids.Add(item.ProductCategoryID);
                    }
                }
                productList = orderServices.GetProducts(ids);
            }
            ViewBag.User = Session["CurrentUser"] as Yyx.BS.Models.User;
            ViewBag.ShowCarousel = false;
            ViewBag.ProductList = productList;
            return View("Index");
        }

        public ActionResult ProductDetial(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.User = Session["CurrentUser"] as Yyx.BS.Models.User;
            ViewBag.Product = orderServices.GetProduct(id);
            return View();
        }
    }
}
