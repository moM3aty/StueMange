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
using System;
using System.Collections.Generic;

namespace StueMange.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeacherController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);
            if (teacher == null) return View("Error", new ErrorViewModel { RequestId = "لم يتم العثور على حساب المدرس" });

            var classes = await _context.ClassTeachers
                .Where(ct => ct.TeacherId == teacher.Id)
                .Select(ct => ct.Class)
                .ToListAsync();

            return View(classes);
        }

        public async Task<IActionResult> ClassDetails(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);
            if (teacher == null) return Challenge();

            var isAssigned = await _context.ClassTeachers.AnyAsync(ct => ct.ClassId == id && ct.TeacherId == teacher.Id);
            if (!isAssigned) return Forbid();

            var a_class = await _context.Classes
                .Include(c => c.Students)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (a_class == null) return NotFound();

            ViewBag.Teacher = teacher;
            return View(a_class);
        }

        public async Task<IActionResult> ManageAttendance(int? id, DateTime? date)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);
            if (teacher == null) return Challenge();

            var isAssigned = await _context.ClassTeachers.AnyAsync(ct => ct.ClassId == id && ct.TeacherId == teacher.Id);
            if (!isAssigned) return Forbid();

            var attendanceDate = date ?? DateTime.Today;

            var students = await _context.Students
                .Where(s => s.ClassId == id)
                .ToListAsync();

            var todayAttendances = await _context.Attendances
                .Where(a => a.Date.Date == attendanceDate.Date && students.Select(s => s.Id).Contains(a.StudentId))
                .ToDictionaryAsync(a => a.StudentId, a => a.IsPresent);

            var model = new AttendanceViewModel
            {
                ClassId = id.Value,
                ClassName = (await _context.Classes.FindAsync(id))?.Name,
                Date = attendanceDate,
                Students = students.Select(s => new StudentAttendanceViewModel
                {
                    StudentId = s.Id,
                    StudentName = s.FullName,
                    IsPresent = todayAttendances.ContainsKey(s.Id) ? todayAttendances[s.Id] : false
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageAttendance(AttendanceViewModel model)
        {
            var attendanceDate = model.Date.Date;
            foreach (var studentVm in model.Students)
            {
                var attendanceRecord = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.StudentId == studentVm.StudentId && a.Date.Date == attendanceDate);

                if (attendanceRecord != null)
                {
                    attendanceRecord.IsPresent = studentVm.IsPresent;
                }
                else
                {
                    _context.Attendances.Add(new Attendance
                    {
                        StudentId = studentVm.StudentId,
                        Date = attendanceDate,
                        IsPresent = studentVm.IsPresent
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("ManageAttendance", new { id = model.ClassId, date = model.Date.ToString("yyyy-MM-dd") });
        }

        public async Task<IActionResult> ManageGrades(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);
            if (teacher == null) return Challenge();

            var isAssigned = await _context.ClassTeachers.AnyAsync(ct => ct.ClassId == id && ct.TeacherId == teacher.Id);
            if (!isAssigned) return Forbid();

            var a_class = await _context.Classes.FindAsync(id);
            if (a_class == null) return NotFound();

            var grades = await _context.Grades
                .Include(g => g.Student)
                .Where(g => g.Student.ClassId == id && g.Subject == teacher.Subject)
                .OrderByDescending(g => g.Date)
                .ToListAsync();

            var model = new ManageGradesViewModel
            {
                ClassId = id.Value,
                ClassName = a_class.Name,
                Subject = teacher.Subject,
                Grades = grades
            };

            return View(model);
        }

        public async Task<IActionResult> AddGrade(int? classId)
        {
            if (classId == null) return NotFound();

            var model = new AddGradeViewModel
            {
                ClassId = classId.Value,
                Date = DateTime.Today
            };

            ViewBag.StudentsList = new SelectList(await _context.Students.Where(s => s.ClassId == classId).ToListAsync(), "Id", "FullName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGrade(AddGradeViewModel model)
        {
            ModelState.Remove("Students");
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);
                if (teacher == null) return Challenge();

                var grade = new Grade
                {
                    StudentId = model.StudentId,
                    Subject = teacher.Subject,
                    ExamName = model.ExamName,
                    Score = model.Score,
                    MaxScore = model.MaxScore,
                    Date = model.Date
                };

                _context.Grades.Add(grade);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageGrades", new { id = model.ClassId });
            }
            ViewBag.StudentsList = new SelectList(await _context.Students.Where(s => s.ClassId == model.ClassId).ToListAsync(), "Id", "FullName", model.StudentId);
            return View(model);
        }

        public async Task<IActionResult> EditGrade(int? id)
        {
            if (id == null) return NotFound();

            var grade = await _context.Grades.Include(g => g.Student).FirstOrDefaultAsync(g => g.Id == id);
            if (grade == null) return NotFound();

            var model = new EditGradeViewModel
            {
                Id = grade.Id,
                StudentId = grade.StudentId,
                StudentName = grade.Student.FullName,
                ExamName = grade.ExamName,
                Score = grade.Score,
                MaxScore = grade.MaxScore,
                Date = grade.Date
            };

            ViewBag.ClassId = grade.Student.ClassId;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGrade(EditGradeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var grade = await _context.Grades.Include(g => g.Student).FirstOrDefaultAsync(g => g.Id == model.Id);
                if (grade == null) return NotFound();

                grade.ExamName = model.ExamName;
                grade.Score = model.Score;
                grade.MaxScore = model.MaxScore;
                grade.Date = model.Date;

                _context.Update(grade);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageGrades", new { id = grade.Student.ClassId });
            }
            return View(model);
        }
    }
}

