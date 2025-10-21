using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StueMange.ViewModels
{
    public class EditClassViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الفصل مطلوب")]
        [StringLength(100)]
        [Display(Name = "اسم الفصل")]
        public string Name { get; set; }

        [Display(Name = "المدرسون")]
        public List<int> SelectedTeacherIds { get; set; } = new List<int>();

        public MultiSelectList TeachersList { get; set; }
    }
}

