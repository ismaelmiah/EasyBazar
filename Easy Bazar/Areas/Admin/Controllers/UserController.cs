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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Easy_Bazar.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UserController : Controller
    {
        #region Variables
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
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
            return View();
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
            //var userList = _db.ApplicationUsers.ToList();
            //var userRole = _db.UserRoles.ToList();
            //var roles = _db.Roles.ToList();

            //foreach (var user in userList)
            //{
            //    var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
            //    user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
            //}
            var result = from ur in _db.UserRoles
                join r in _db.Roles on ur.RoleId equals r.Id
                join a in _db.ApplicationUsers on ur.UserId equals a.Id
                select new UserRoleMapping()
                {
                    FullName = a.Name,
                    Phone = a.PhoneNumber,
                    UserName = a.UserName,
                    RoleName = r.Name
                };
            return Json(new { data = result });
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

        public IActionResult GetAllRule()
        {
            var roles = _roleManager.Roles.ToList();
            return Json(new { data = roles });
        }

        #endregion

        public IActionResult Rolelist()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Assign()
        {
            ViewData["UserId"] = new SelectList(_db.ApplicationUsers.Where(x => x.LockoutEnd < DateTime.Now || x.LockoutEnd == null).ToList(), "Id", "UserName");
            ViewData["RoleId"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Assign(RoleUserVm roleUser)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.Id == roleUser.UserId);
            var isExist = await _userManager.IsInRoleAsync(user, roleUser.Role);
            if (isExist)
            {
                ViewBag.msg = "This Role is already assigned for this User";
                ViewData["UserId"] = new SelectList(_db.ApplicationUsers.Where(x => x.LockoutEnd < DateTime.Now || x.LockoutEnd == null).ToList(), "Id", "UserName");
                ViewData["RoleId"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");

                return View();
            }
            var role = await _userManager.AddToRoleAsync(user, roleUser.Role);
            if (role.Succeeded)
            {
                TempData["save"] = "Role has been Assigned";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            await _roleManager.DeleteAsync(role);
            return Json(new { success = true, message = "Delete Operation Successfully" });
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
        public async Task<IActionResult> Upsert(RoleVM roleVm)
        {
            if (ModelState.IsValid)
            {

                var isExist = await _roleManager.RoleExistsAsync(roleVm.rolename);
                if (isExist)
                {
                    ViewBag.msg = "This role is already exist";
                    ViewBag.name = roleVm.rolename; 
                    return View();
                }

                if (roleVm.roleid == null)
                {
                    var role = new IdentityRole
                    {
                        Name = roleVm.rolename
                    }; 
                    var result = await _roleManager.CreateAsync(role);
                    if (result.Succeeded)
                    {
                        TempData["save"] = "Role has been saved successfully";
                        return RedirectToAction(nameof(Rolelist));
                    }
                }
                else
                {
                    var role = await _roleManager.FindByIdAsync(roleVm.roleid);
                    role.Name = roleVm.rolename;
                    var result = await _roleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        TempData["save"] = "Update";
                        return RedirectToAction(nameof(Rolelist));
                    }
                    return View();
                }
            }
            return View(roleVm);
        }

    }
}