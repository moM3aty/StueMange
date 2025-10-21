using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StueMange.Models
{
    public class Class
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الفصل مطلوب")]
        [StringLength(100)]
        [Display(Name = "اسم الفصل")]
        public string Name { get; set; }

        public ICollection<Student> Students { get; set; }

        public ICollection<ClassTeacher> ClassTeachers { get; set; } = new List<ClassTeacher>();
    }
}

