using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StueMange.Models;

namespace StueMange.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<ClassTeacher> ClassTeachers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Student>()
               .HasOne(s => s.Class)
               .WithMany(c => c.Students)
               .HasForeignKey(s => s.ClassId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Parent)
                .WithMany(p => p.Students)
                .HasForeignKey(s => s.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClassTeacher>()
                .HasKey(ct => new { ct.ClassId, ct.TeacherId });

            modelBuilder.Entity<ClassTeacher>()
                .HasOne(ct => ct.Class)
                .WithMany(c => c.ClassTeachers)
                .HasForeignKey(ct => ct.ClassId);

            modelBuilder.Entity<ClassTeacher>()
                .HasOne(ct => ct.Teacher)
                .WithMany(t => t.ClassTeachers)
                .HasForeignKey(ct => ct.TeacherId);
        }
    }
}
