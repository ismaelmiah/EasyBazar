using DataSets.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Easy_Bazar.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ProjectConstant.Role_Admin)]
    public class AdministrationController : Controller
    {
        public IActionResult Index()
        {
            return View("Index", "_AdminLTE");
        }
        public IActionResult Userlist()
        {
            return PartialView();
        }
    }
}