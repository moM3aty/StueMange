using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StueMange.ViewModels
{
    public class EditGradeViewModel : IValidatableObject
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int StudentId { get; set; }

        [Display(Name = "الطالب")]
        public string StudentName { get; set; }

        [Required(ErrorMessage = "اسم الاختبار مطلوب")]
        [StringLength(100)]
        [Display(Name = "اسم الاختبار")]
        public string ExamName { get; set; }

        [Required(ErrorMessage = "الدرجة الكلية مطلوبة")]
        [Range(1, 1000)]
        [Display(Name = "الدرجة الكلية")]
        public double MaxScore { get; set; }

        [Required(ErrorMessage = "درجة الطالب مطلوبة")]
        [Range(0, 1000)]
        [Display(Name = "درجة الطالب")]
        public double Score { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "التاريخ")]
        public DateTime Date { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Score > MaxScore)
            {
                yield return new ValidationResult("درجة الطالب لا يمكن أن تكون أكبر من الدرجة الكلية.", new[] { "Score" });
            }
        }
    }
}

