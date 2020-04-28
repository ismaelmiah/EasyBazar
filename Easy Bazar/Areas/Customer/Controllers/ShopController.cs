using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataSets.Interfaces;
using DataSets.Utility;
using DataSets.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Easy_Bazar.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShopController : Controller
    {
        private readonly IUnitOfWork _uow;
        public ShopController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public IActionResult Index()
        {
            var model = new HomeVM
            {
                Products = _uow.Product.GetAll(includeProperties: "Category").ToList(),
                Categories = _uow.Category.GetAll().ToList()
            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var shoppingCount = _uow.ShoppingCart.GetAll(a => a.ApplicationUserId == claim.Value).ToList().Count();

                HttpContext.Session.SetInt32(ProjectConstant.shoppingCart, shoppingCount);
            }
            return View(model);
        }
    }
}