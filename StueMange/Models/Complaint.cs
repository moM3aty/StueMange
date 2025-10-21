using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StueMange.Models
{
    /// <summary>
    /// يمثل شكوى مسجلة بحق طالب
    /// </summary>
    public class Complaint
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
        [Display(Name = "تاريخ الشكوى")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "وصف الشكوى مطلوب")]
        [StringLength(1000, ErrorMessage = "وصف الشكوى طويل جدًا")]
        [Display(Name = "وصف الشكوى")]
        public string Description { get; set; }
    }
}

