using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StueMange.Models
{
    /// <summary>
    /// يمثل بيانات الطالب
    /// </summary>
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم الكامل للطالب مطلوب")]
        [StringLength(150)]
        [Display(Name = "اسم الطالب الكامل")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "تاريخ الميلاد مطلوب")]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ الميلاد")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "نوع الطالب مطلوب")]
        [Display(Name = "نوع الطالب")]
        public StudentType StudentType { get; set; }


        // Foreign key for Class
        [Required(ErrorMessage = "يجب تحديد الفصل")]
        [Display(Name = "الفصل الدراسي")]
        public int ClassId { get; set; }

        [ForeignKey("ClassId")]
        [Display(Name = "الفصل الدراسي")]
        public virtual Class Class { get; set; }

        // Foreign key for Parent - Made nullable
        [Display(Name = "ولي الأمر")]
        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual Parent? Parent { get; set; }


        // Navigation Properties
        public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }

    public enum StudentType
    {
        StueMange1, // Boys
        StueMange2  // Girls
    }
}

