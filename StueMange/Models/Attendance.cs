using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StueMange.Models
{
    /// <summary>
    /// يمثل سجل حضور للطالب في يوم معين
    /// </summary>
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "يجب تحديد الطالب")]
        [Display(Name = "الطالب")]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "التاريخ")]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "حالة الحضور")]
        public bool IsPresent { get; set; } // true for present, false for absent
    }
}

