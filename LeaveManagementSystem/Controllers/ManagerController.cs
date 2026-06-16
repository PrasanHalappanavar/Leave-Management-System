using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.Controllers
{
    public class ManagerController : Controller
    {
        private bool IsManager()
        {
            return HttpContext.Session.GetInt32("UserId") != null
                && HttpContext.Session.GetInt32("Role") == 2;
        }


        public IActionResult Dashboard()
        {
            if (!IsManager()) return RedirectToAction("Login", "Account");
            return View();
        }


        public IActionResult Requests()
        {
            if (!IsManager()) return RedirectToAction("Login", "Account");
            return View();
        }


        public IActionResult History()
        {
            if (!IsManager()) return RedirectToAction("Login", "Account");
            return View();
        }
    }
}
