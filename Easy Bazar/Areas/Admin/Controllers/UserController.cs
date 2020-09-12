using DataSets.Data;
using DataSets.Entity;
using DataSets.Utility;
using DataSets.ViewModels;
using Easy_Bazar.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Easy_Bazar.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UserController : Controller
    {
        #region Variables
        RoleManager<IdentityRole> _roleManager;
        UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        #endregion

        #region CTOR
        public UserController(ApplicationDbContext db, SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        #endregion

        #region Actions
        public IActionResult Index()
        {
            return PartialView();
        }
        #endregion

        [HttpGet]
        public IActionResult Profile()
        {
            var userProfile = _userManager.GetUserAsync(User).Result;
            return View(userProfile);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(ApplicationUser user)
        {
            var userProfile = _userManager.GetUserAsync(User).Result;
            userProfile.Name = user.Name;
            userProfile.City = user.City;
            userProfile.PostalCode = user.PostalCode;
            userProfile.StreetAddress = user.StreetAddress;
            userProfile.PhoneNumber = user.PhoneNumber;
            _db.ApplicationUsers.Update(userProfile);
            _db.SaveChanges();
            return View(userProfile);
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.SetInt32(ProjectConstant.shoppingCart, 0);
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }


        #region API CALLS
        public IActionResult GetAll()
        {
            var userList = _db.ApplicationUsers.ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
            }
            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var data = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (data == null)
                return Json(new { success = false, message = "Error while locking/unlocking" });

            if (data.LockoutEnd != null && data.LockoutEnd > DateTime.Now)
                data.LockoutEnd = DateTime.Now;
            else
                data.LockoutEnd = DateTime.Now.AddYears(10);

            _db.SaveChanges();
            return Json(new { success = true, message = "Operation Successfully" });
        }

        #endregion

        public IActionResult Rolelist()
        {
            var roles = _roleManager.Roles.ToList();
            ViewBag.Role = roles;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(string? id)
        {
            RoleVM roleVM = new RoleVM();
            if (string.IsNullOrEmpty(id))
            {
                //Create new role
                return View(roleVM);
            }
            // This for Update
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            roleVM.roleid = role.Id;
            roleVM.rolename = role.Name;
            return View(roleVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(RoleVM roleVM)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole
                {
                    Name = roleVM.rolename
                };
                var isExist = await _roleManager.RoleExistsAsync(role.Name);
                if (isExist)
                {
                    ViewBag.msg = "This role is already exist";
                    ViewBag.name = roleVM.rolename;
                    return View();
                }
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    TempData["save"] = "Role has been saved successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(roleVM);
        }

    }
}