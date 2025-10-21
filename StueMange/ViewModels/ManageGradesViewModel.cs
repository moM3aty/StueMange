using StueMange.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StueMange.ViewModels
{

        public class ManageGradesViewModel
        {
            public int ClassId { get; set; }
            public string ClassName { get; set; }
            public string Subject { get; set; }
            public List<Grade> Grades { get; set; } = new List<Grade>();
        }
    

    public class StudentGradeViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }

        [Range(0, 100, ErrorMessage = "الدرجة يجب أن تكون بين 0 و 100")]
        public double? Score { get; set; }
    }
}

