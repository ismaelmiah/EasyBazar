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
                FeaturedProducts = _uow.Product.GetAll(includeProperties: "Category").Take(8).ToList()
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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
