using System.ComponentModel.DataAnnotations;

namespace StueMange.Models
{
    public class ClassTeacher
    {
        public int ClassId { get; set; }
        [Display(Name = "الفصل")]

        public Class Class { get; set; }

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
    }
}
