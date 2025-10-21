using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StueMange.ViewModels
{
    public class RegisterParentViewModel
    {
        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "رقم هاتف غير صالح")]
        [Display(Name = "رقم الهاتف (سيكون اسم المستخدم)")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "يجب اختيار الطالب")]
        [Display(Name = "اختر ابنك/ابنتك")]
        public int StudentId { get; set; }

        public SelectList StudentsList { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [StringLength(100, ErrorMessage = "يجب أن تكون {0} على الأقل {2} وعلى الأكثر {1} حرفًا.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور")]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين.")]
        public string ConfirmPassword { get; set; }
    }
}

