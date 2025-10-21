using System;
using System.ComponentModel.DataAnnotations;

namespace StueMange.ViewModels
{
    public class CreateComplaintViewModel
    {
        [Required(ErrorMessage = "يجب تحديد الطالب")]
        [Display(Name = "الطالب")]
        public int StudentId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ الشكوى")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "وصف الشكوى مطلوب")]
        [StringLength(1000)]
        [Display(Name = "وصف الشكوى")]
        public string Description { get; set; }
    }
}

