using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSets.Interfaces;
using DataSets.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Easy_Bazar.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class WidgetsController : Controller
    {
        private readonly IUnitOfWork _uow;
        public WidgetsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public IActionResult Products(bool isLatestProduct, int? categoryid =0)
        {
            var model = new HomeVM
            {
                IsLatestProduct = isLatestProduct
            };

            if (isLatestProduct)
            {
                model.Products = _uow.Product.GetAll(includeProperties: "Category").Take(4).OrderByDescending(x => x.ID).ToList();  
            }
            else if(categoryid.HasValue && categoryid.Value>0)
            {
                model.IsLatestProduct = true;
                model.Products = _uow.Product.GetAll(includeProperties: "Category").Where(x=> x.CategoryID == categoryid.Value).Take(4).OrderByDescending(x => x.ID).ToList();
            }
            else
            {
                model.Products = _uow.Product.GetAll(includeProperties: "Category").Take(8).OrderByDescending(x=>x.ID).ToList();
            }
            return PartialView(model);
        }
    }
}