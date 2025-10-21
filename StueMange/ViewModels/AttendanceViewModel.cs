using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StueMange.ViewModels
{
    public class AttendanceViewModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public List<StudentAttendanceViewModel> Students { get; set; }
    }

    public class StudentAttendanceViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public bool IsPresent { get; set; }
    }
}

