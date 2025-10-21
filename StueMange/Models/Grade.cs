using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StueMange.Models
{
    /// <summary>
    /// يمثل درجة الطالب في اختبار أو واجب معين
    /// </summary>
    public class Grade : IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        [Required(ErrorMessage = "اسم الاختبار مطلوب")]
        [StringLength(100)]
        [Display(Name = "اسم الاختبار")]
        public string ExamName { get; set; }

        [Required(ErrorMessage = "المادة مطلوبة")]
        [StringLength(100)]
        [Display(Name = "المادة")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "الدرجة الكلية مطلوبة")]
        [Range(1, 1000, ErrorMessage = "الدرجة الكلية يجب أن تكون قيمة موجبة")]
        [Display(Name = "الدرجة الكلية")]
        public double MaxScore { get; set; }

        [Required(ErrorMessage = "درجة الطالب مطلوبة")]
        [Range(0, 1000)]
        [Display(Name = "درجة الطالب")]
        public double Score { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ الاختبار")]
        public DateTime Date { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Score > MaxScore)
            {
                yield return new ValidationResult(
                    "درجة الطالب لا يمكن أن تكون أكبر من الدرجة الكلية.",
                    new[] { nameof(Score) });
            }
        }
    }
}

