using Microsoft.AspNetCore.Mvc;
using StueMange.ViewModels;
using System.Diagnostics;

namespace StueMange.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // إذا كان المستخدم مسجل دخوله، قم بتوجيهه إلى لوحة التحكم الخاصة به
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (User.IsInRole("Teacher"))
                {
                    return RedirectToAction("Index", "Teacher");
                }
                else if (User.IsInRole("Parent"))
                {
                    return RedirectToAction("Index", "Parent");
                }
            }
            // إذا لم يكن مسجل دخوله، اعرض صفحة تسجيل الدخول
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

