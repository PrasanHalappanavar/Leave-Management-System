using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.Controllers
{
    public class HRController : Controller
    {
        private bool IsHR()
        {
            return HttpContext.Session.GetInt32("UserId") != null
                && HttpContext.Session.GetInt32("Role") == 3;
        }

        public IActionResult Dashboard()
        {
            if (!IsHR()) return RedirectToAction("Login", "Account");
            return View();
        }

        public IActionResult AddUser()
        {
            if (!IsHR()) return RedirectToAction("Login", "Account");
            return View();
        }

        public IActionResult AllRequests()
        {
            if (!IsHR()) return RedirectToAction("Login", "Account");
            return View();
        }

        public IActionResult LeaveSummary()
        {
            if (!IsHR()) return RedirectToAction("Login", "Account");
            return View();
        }
    }
}
