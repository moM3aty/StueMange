using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StueMange.Data;
using StueMange.Models;
using StueMange.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace StueMange.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "محاولة تسجيل دخول غير صالحة.");
            }
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // GET: /Account/RegisterTeacher
        [HttpGet]
        public IActionResult RegisterTeacher()
        {
            return View();
        }

        // POST: /Account/RegisterTeacher
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterTeacher(RegisterTeacherViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Username, FullName = model.FullName };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Teacher");

                    var teacher = new Teacher
                    {
                        ApplicationUserId = user.Id,
                        Subject = model.Subject
                    };
                    _context.Teachers.Add(teacher);
                    await _context.SaveChangesAsync();

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // GET: /Account/RegisterParent
        [HttpGet]
        public async Task<IActionResult> RegisterParent()
        {
            // Fetch students who do not have a parent assigned yet
            ViewBag.StudentsList = new SelectList(await _context.Students.Where(s => s.ParentId == null).ToListAsync(), "Id", "FullName");
            return View();
        }

        // POST: /Account/RegisterParent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterParent(RegisterParentViewModel model)
        {
            ModelState.Remove("StudentsList");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.PhoneNumber, FullName = model.FullName, PhoneNumber = model.PhoneNumber };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Parent");

                    // 1. Create the parent record
                    var parent = new Parent
                    {
                        ApplicationUserId = user.Id,
                    };
                    _context.Parents.Add(parent);
                    await _context.SaveChangesAsync(); // Save to get the new parent's ID

                    // 2. Find the selected student and link them to the new parent
                    var student = await _context.Students.FindAsync(model.StudentId);
                    if (student != null)
                    {
                        student.ParentId = parent.Id;
                        _context.Update(student);
                        await _context.SaveChangesAsync();
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.StudentsList = new SelectList(await _context.Students.Where(s => s.ParentId == null).ToListAsync(), "Id", "FullName", model.StudentId);
            return View(model);
        }
    }
}

