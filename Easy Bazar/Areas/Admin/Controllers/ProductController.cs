using DataSets.Entity;
using DataSets.Interfaces;
using DataSets.Utility;
using DataSets.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;

namespace Easy_Bazar.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ProjectConstant.Role_Admin)]
    public class ProductController : Controller
    {
        #region Variables
        private readonly IUnitOfWork _uow;
        private readonly IWebHostEnvironment _hostEnvironment;
        #endregion

        #region CTOR
        public ProductController(IUnitOfWork uow, IWebHostEnvironment hostEnvironment)
        {
            _uow = uow;
            _hostEnvironment = hostEnvironment;
        }
        #endregion

        #region Actions
        public IActionResult Index()
        {
            var catname = _uow.Category.GetAll().ToList();
            return View(_uow.Product.GetAll(x => x.CategoryID == x.Category.ID, includeProperties: "Category").ToList());
        }
        #endregion

        #region API CALLS
        public IActionResult GetAll()
        {
            var allObj = _uow.Product.GetAll(includeProperties: "Category");
            return Json(new { data = allObj });
        }


        //[HttpGet]
        //public IActionResult Delete(int id)
        //{
        //    var deleteData = _uow.Product.Get(id);
        //    if (deleteData == null)
        //    {
        //        return NotFound();
        //    }
        //    var listobj = _uow.Category.GetAll().ToList();
        //    var ok = listobj.FirstOrDefault(x => x.ID == deleteData.CategoryID).Name;
        //    //db.CategoryEntities.Where(x=>x.ID==deleteData.CategoryID);
        //    TempData["CategoryName"] = ok;
        //    if (deleteData == null)
        //        return Json(new { success = false, message = "Data Not Found!" });
        //    return View(deleteData); //Json(new { success = true, message = "Delete Operation Successfully" });
        //}

        [HttpDelete]
        [ActionName("Delete")]
        public IActionResult DeleteData(int id)
        {
            var deleteData = _uow.Product.Get(id);
            if (deleteData == null)
                return Json(new { success = false, message = "Data Not Found!" });

            if (deleteData.ImageURL != null)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var imagePath = Path.Combine(webRootPath, deleteData.ImageURL.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            _uow.Product.Remove(deleteData);
            _uow.Save();
            return Json(new { success = true, message = "Delete Operation Successfully" });
        }

        #endregion

        /// <summary>
        /// Create Or Update Get Method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _uow.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.ID.ToString()
                }),
                ProductList = _uow.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.ID.ToString()
                })
            };

            if (id == null)
                return View(productVM);

            productVM.Product = _uow.Product.Get(id.GetValueOrDefault());
            if (productVM.Product == null)
                return NotFound();
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {

                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\products");
                    var extenstion = Path.GetExtension(files[0].FileName);

                    if (productVM.Product.ImageURL != null)
                    {
                        var imagePath = Path.Combine(webRootPath, productVM.Product.ImageURL.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extenstion), FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams);
                    }
                    productVM.Product.ImageURL = @"\images\products\" + fileName + extenstion;
                }
                else
                {
                    if (productVM.Product.ID != 0)
                    {
                        var productData = _uow.Product.Get(productVM.Product.ID);
                        productVM.Product.ImageURL = productData.ImageURL;
                    }
                }

                if (productVM.Product.ID == 0)
                {
                    //Create
                    _uow.Product.Add(productVM.Product);
                }
                else
                {
                    //Update
                    _uow.Product.Update(productVM.Product);
                }
                _uow.Save();
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _uow.Category.GetAll().Select(a => new SelectListItem
                {
                    Text = a.Name,
                    Value = a.ID.ToString()
                });

                productVM.ProductList = _uow.Product.GetAll().Select(a => new SelectListItem
                {
                    Text = a.Name,
                    Value = a.ID.ToString()
                });

                if (productVM.Product.ID != 0)
                {
                    productVM.Product = _uow.Product.Get(productVM.Product.ID);
                }
            }
            return View(productVM.Product);
        }

    }
}