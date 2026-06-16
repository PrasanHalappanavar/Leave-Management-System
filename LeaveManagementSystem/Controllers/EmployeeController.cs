using Microsoft.AspNetCore.Mvc;
using System.Web.Http;

namespace LeaveManagementSystem.Controllers
{
    public class EmployeeController : Controller
    {

        private bool IsEmployee()
        {
            return HttpContext.Session.GetInt32("UserId") != null
                && HttpContext.Session.GetInt32("Role") == 1;
        }


        public IActionResult Dashboard()
        {
            if (!IsEmployee()) return RedirectToAction("Login", "Account");
            return View();
        }



        public IActionResult Apply()
        {
            if (!IsEmployee()) return RedirectToAction("Login", "Account");
            return View();
        }



        public IActionResult MyRequests()
        {
            if (!IsEmployee()) return RedirectToAction("Login", "Account");
            return View();
        }
    }
}
