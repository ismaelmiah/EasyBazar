using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataSets.Data;
using DataSets.Entity;
using DataSets.Interfaces;
using DataSets.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Easy_Bazar.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ProjectConstant.Role_Admin)]
    public class CategoryController : Controller
    {
        #region Variables
        private readonly IUnitOfWork _uow;
        private readonly IWebHostEnvironment _hostEnvironment;
        #endregion

        #region CTOR
        public CategoryController(IUnitOfWork uow, IWebHostEnvironment hostEnvironment)
        {
            _uow = uow;
            _hostEnvironment = hostEnvironment;
        }
        #endregion

        #region Actions
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region API CALLS
        public IActionResult GetAll()
        {
            var allObj = _uow.Category.GetAll(includeProperties: "Products");
            return Json(new { data = allObj });
        }

        //[HttpGet]
        //public IActionResult Delete(int id)
        //{
        //    var deleteData = _uow.Category.Get(id);
        //    if (deleteData == null)
        //        return Json(new { success = false, message = "Data Not Found!" });
        //    return Json(new { success = true, message = "Delete Operation Successfully" });
        //}

        //[HttpGet]
        //public IActionResult Delete(int id)
        //{
        //    var deleteData = _uow.Category.Get(id);
        //    if (deleteData == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(deleteData);
        //}

        [HttpDelete]
        [ActionName("Delete")]
        public IActionResult DeleteData(int id)
        {
            var deleteData = _uow.Category.Get(id);
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

            var productinfo = _uow.Product.GetAll().Where(x => x.CategoryID == id).ToList();
            _uow.Product.RemoveRange(productinfo);
            _uow.Save();
            _uow.Category.Remove(deleteData);
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
            Category cat = new Category();
            if (id == null)
            {
                //This for Create
                return View(cat);
            }
            // This for Update
            cat = _uow.Category.Get((int)id);
            if (cat != null)
            {
                return View(cat);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {

                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\category");
                    var extenstion = Path.GetExtension(files[0].FileName);

                    if (category.ImageURL != null)
                    {
                        var imagePath = Path.Combine(webRootPath, category.ImageURL.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extenstion), FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams);
                    }
                    category.ImageURL = @"\images\category\" + fileName + extenstion;
                }
                else
                {
                    if (category.ID != 0)
                    {
                        var categoryData = _uow.Category.Get(category.ID);
                        category.ImageURL = categoryData.ImageURL;
                    }
                }

                if (category.ID == 0)
                {
                    //Create
                    _uow.Category.Add(category);
                }
                else
                {
                    //Update
                    _uow.Category.Update(category);
                }
                _uow.Save();
                return RedirectToAction("Index");
            }
            else
            {
                if (category.ID != 0)
                {
                    category = _uow.Category.Get(category.ID);
                }
            }
            return View(category);
        }

    }
}