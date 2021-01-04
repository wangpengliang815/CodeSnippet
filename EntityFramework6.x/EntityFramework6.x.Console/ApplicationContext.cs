using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace EntityFramework6.x.Console
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("Connection")
        {
    
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Address> Address { get; set; }
    }
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }
    }

    public class Address
    {
        [Key]
        public int Id { get; set; }

        public int StudentId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }
    }
}
