using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Easy_Bazar.Models;
using DataSets.Interfaces;
using DataSets.Data;
using DataSets.Entity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using DataSets.Utility;
using Microsoft.AspNetCore.Authorization;
using DataSets.ViewModels;

namespace Easy_Bazar.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _uow;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork uow, ApplicationDbContext db)
        {
            _logger = logger;
            _uow = uow;
            _db = db;
        }

        public IActionResult Index()
        {

            var model = new HomeVM
            {
                FeaturedCategories = _uow.Category.GetAll().Where(x => x.IsFeatured && x.ImageURL != null).ToList(),
                Products = _uow.Product.GetAll(includeProperties: "Category").Take(4).ToList()
            };
            //return View(model);


            //IEnumerable<Product> productList = _uow.Product.GetAll(includeProperties: "Category");

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var shoppingCount = _uow.ShoppingCart.GetAll(a => a.ApplicationUserId == claim.Value).ToList().Count();

                HttpContext.Session.SetInt32(ProjectConstant.shoppingCart, shoppingCount);
            }
            return View(model);
        }
        public IActionResult Details(int id)
        {
            var product = _uow.Product.GetFirstOrDefault(p => p.ID == id, includeProperties: "Category");

            ShoppingCart cart = new ShoppingCart()
            {
                Product = product,
                ProductId = product.ID
            };

            var listobj = _uow.Category.GetAll().ToList();
            var ok = listobj.FirstOrDefault(x => x.ID == product.CategoryID).Name;
            TempData["CatName"] = ok;
            return View(cart);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cartObj)
        {
            cartObj.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartObj.ApplicationUserId = claim.Value;

                ShoppingCart fromDb = _uow.ShoppingCart.GetFirstOrDefault(
                    s => s.ApplicationUserId == cartObj.ApplicationUserId
                    && s.ProductId == cartObj.ProductId,
                    includeProperties: "Product");
                
                if (fromDb == null)
                {
                    //Insert
                    _uow.ShoppingCart.Add(cartObj);
                }
                else
                {
                    //Update
                    fromDb.Count += cartObj.Count;
                }

                _uow.Save();

                var shoppingCount = _uow.ShoppingCart.GetAll(a => a.ApplicationUserId == cartObj.ApplicationUserId).ToList().Count();

                HttpContext.Session.SetInt32(ProjectConstant.shoppingCart, shoppingCount);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var product = _uow.Product.GetFirstOrDefault(p => p.ID == cartObj.ProductId, includeProperties: "Category");

                ShoppingCart cart = new ShoppingCart()
                {
                    Product = product,
                    ProductId = product.ID
                };
                return View(cart);
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
