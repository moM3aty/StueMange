using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StueMange.Data;
using StueMange.Models;
using StueMange.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace StueMange.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminDashboardViewModel
            {
                StudentCount = await _context.Students.CountAsync(),
                ClassCount = await _context.Classes.CountAsync(),
                ComplaintCount = await _context.Complaints.CountAsync()
            };
            return View(viewModel);
        }

        public async Task<IActionResult> ManageClasses()
        {
            var classes = await _context.Classes
                .Include(c => c.ClassTeachers).ThenInclude(ct => ct.Teacher).ThenInclude(t => t.ApplicationUser)
                .ToListAsync();
            return View(classes);
        }

        public IActionResult CreateClass()
        {
            ViewBag.TeachersList = new MultiSelectList(_context.Teachers.Include(t => t.ApplicationUser).ToList(), "Id", "ApplicationUser.FullName");
            return View(new CreateClassViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClass(CreateClassViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newClass = new Class { Name = model.Name };
                _context.Classes.Add(newClass);
                await _context.SaveChangesAsync();

                if (model.SelectedTeacherIds != null)
                {
                    foreach (var teacherId in model.SelectedTeacherIds)
                    {
                        newClass.ClassTeachers.Add(new ClassTeacher { ClassId = newClass.Id, TeacherId = teacherId });
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(ManageClasses));
            }
            ViewBag.TeachersList = new MultiSelectList(_context.Teachers.Include(t => t.ApplicationUser).ToList(), "Id", "ApplicationUser.FullName", model.SelectedTeacherIds);
            return View(model);
        }

        public async Task<IActionResult> EditClass(int? id)
        {
            if (id == null) return NotFound();

            var a_class = await _context.Classes.Include(c => c.ClassTeachers).FirstOrDefaultAsync(c => c.Id == id);
            if (a_class == null) return NotFound();

            var allTeachers = await _context.Teachers.Include(t => t.ApplicationUser).ToListAsync();
            var selectedTeacherIds = a_class.ClassTeachers.Select(ct => ct.TeacherId).ToList();

            var model = new EditClassViewModel
            {
                Id = a_class.Id,
                Name = a_class.Name,
                SelectedTeacherIds = selectedTeacherIds,
                TeachersList = new MultiSelectList(allTeachers, "Id", "ApplicationUser.FullName", selectedTeacherIds)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditClass(int id, EditClassViewModel model)
        {
            if (id != model.Id) return NotFound();
            ModelState.Remove("TeachersList");
            if (ModelState.IsValid)
            {
                var classToUpdate = await _context.Classes.Include(c => c.ClassTeachers).FirstOrDefaultAsync(c => c.Id == id);
                if (classToUpdate == null) return NotFound();

                classToUpdate.Name = model.Name;
                classToUpdate.ClassTeachers.Clear();
                if (model.SelectedTeacherIds != null)
                {
                    foreach (var teacherId in model.SelectedTeacherIds)
                    {
                        classToUpdate.ClassTeachers.Add(new ClassTeacher { ClassId = id, TeacherId = teacherId });
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageClasses));
            }

            var allTeachers = await _context.Teachers.Include(t => t.ApplicationUser).ToListAsync();
            model.TeachersList = new MultiSelectList(allTeachers, "Id", "ApplicationUser.FullName", model.SelectedTeacherIds);
            return View(model);
        }

        public async Task<IActionResult> ManageStudents(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var studentsQuery = _context.Students.Include(s => s.Class).Include(s => s.Parent).ThenInclude(p => p.ApplicationUser).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                studentsQuery = studentsQuery.Where(s => s.FullName.Contains(searchString));
            }

            var students = await studentsQuery.ToListAsync();
            return View(students);
        }

        public async Task<IActionResult> CreateStudent()
        {
            ViewBag.ClassesList = new SelectList(await _context.Classes.ToListAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent(CreateStudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var student = new Student
                {
                    FullName = model.FullName,
                    DateOfBirth = model.DateOfBirth,
                    ClassId = model.ClassId,
                    StudentType = model.StudentType,
                };
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageStudents));
            }
            ViewBag.ClassesList = new SelectList(await _context.Classes.ToListAsync(), "Id", "Name", model.ClassId);
            return View(model);
        }

        public async Task<IActionResult> EditStudent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            var model = new EditStudentViewModel
            {
                Id = student.Id,
                FullName = student.FullName,
                DateOfBirth = student.DateOfBirth,
                ClassId = student.ClassId,
                StudentType = student.StudentType,
                ParentId = student.ParentId
            };
            ViewBag.ClassesList = new SelectList(await _context.Classes.ToListAsync(), "Id", "Name", student.ClassId);
            ViewBag.ParentsList = new SelectList(await _context.Parents.Include(p => p.ApplicationUser).ToListAsync(), "Id", "ApplicationUser.FullName", student.ParentId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(int id, EditStudentViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var student = await _context.Students.FindAsync(id);
                    student.FullName = model.FullName;
                    student.DateOfBirth = model.DateOfBirth;
                    student.ClassId = model.ClassId;
                    student.StudentType = model.StudentType;
                    student.ParentId = model.ParentId;
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Students.Any(e => e.Id == model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ManageStudents));
            }
            ViewBag.ClassesList = new SelectList(await _context.Classes.ToListAsync(), "Id", "Name", model.ClassId);
            ViewBag.ParentsList = new SelectList(await _context.Parents.Include(p => p.ApplicationUser).ToListAsync(), "Id", "ApplicationUser.FullName", model.ParentId);
            return View(model);
        }

        public async Task<IActionResult> ManageComplaints()
        {
            var complaints = await _context.Complaints.Include(c => c.Student).ToListAsync();
            return View(complaints);
        }

        public async Task<IActionResult> CreateComplaint()
        {
            ViewBag.StudentsList = new SelectList(await _context.Students.ToListAsync(), "Id", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComplaint(CreateComplaintViewModel model)
        {
            if (ModelState.IsValid)
            {
                var complaint = new Complaint
                {
                    StudentId = model.StudentId,
                    Description = model.Description,
                    Date = DateTime.Now
                };
                _context.Complaints.Add(complaint);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageComplaints));
            }
            ViewBag.StudentsList = new SelectList(await _context.Students.ToListAsync(), "Id", "FullName", model.StudentId);
            return View(model);
        }
    }
}
