using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StueMange.Models
{
    /// <summary>
    /// يمثل المستخدم العام للتطبيق، سواء كان مدرس، ولي أمر، أو مدير
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [StringLength(150)]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; }

        [Required]
        public UserType UserType { get; set; }
    }

    public enum UserType
    {
        Admin,
        Teacher,
        Parent
    }
}

