using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StueMange.Models
{
    /// <summary>
    /// يمثل بيانات ولي الأمر
    /// </summary>
    public class Parent
    {
        [Key]
        public int Id { get; set; }

        // Foreign key to link with the user account
        [Required]
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        // Navigation Properties
        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    }
}

