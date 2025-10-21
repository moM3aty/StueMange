using StueMange.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace StueMange.ViewModels
{
    public class CreateStudentViewModel
    {
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

        [Required(ErrorMessage = "يجب تحديد الفصل")]
        [Display(Name = "الفصل الدراسي")]
        public int ClassId { get; set; }


    }
}

