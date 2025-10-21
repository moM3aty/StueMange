using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StueMange.Data;
using StueMange.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StueMange.Controllers
{
    [Authorize(Roles = "Parent")]
    public class ParentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ParentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var parent = await _context.Parents.FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);

            if (parent == null) return Unauthorized();

            var students = await _context.Students
                .Where(s => s.ParentId == parent.Id)
                .Include(s => s.Class)
                .ToListAsync();

            return View(students);
        }

        public async Task<IActionResult> StudentDetails(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Grades)
                .Include(s => s.Attendances)
                .Include(s => s.Complaints)
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound();

            // Security check: ensure the logged-in parent is the parent of this student
            var user = await _userManager.GetUserAsync(User);
            var parent = await _context.Parents.FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);
            if (student.ParentId != parent.Id)
            {
                return Forbid();
            }

            return View(student);
        }
    }
}

