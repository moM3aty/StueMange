using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StueMange.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Required(ErrorMessage = "المادة الدراسية مطلوبة")]
        [StringLength(100)]
        [Display(Name = "المادة الدراسية")]
        public string Subject { get; set; }

        public ICollection<ClassTeacher> ClassTeachers { get; set; } = new List<ClassTeacher>();
    }
}

