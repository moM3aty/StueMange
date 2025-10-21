using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StueMange.ViewModels
{
    public class CreateClassViewModel
    {
        [Required(ErrorMessage = "اسم الفصل مطلوب")]
        [Display(Name = "اسم الفصل")]
        public string Name { get; set; }

        [Required(ErrorMessage = "يجب اختيار مدرس واحد على الأقل")]
        [Display(Name = "المدرسون")]
        public List<int> SelectedTeacherIds { get; set; } = new List<int>();
    }
}
