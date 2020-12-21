namespace CodeSnippet.EntityFramework6.x
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestCategory("EntityFrameworkOperation")]
    [TestClass()]
    public class OperationTests
    {
        static readonly string conn =
             @"Server=.;Initial Catalog=CodeSnippet.DataBase;User ID=sa;Password=wpl19950815;Connection Timeout=30;Persist Security Info=False;Max Pool Size=500;";

        [TestInitialize]
        public void Init()
        {
            // Method intentionally left empty.
        }

        [TestCleanup]
        public void Dispose()
        {
            // Method intentionally left empty.
        }

        [TestMethod]
        public void Operation_Add_Example()
        {
            using ApplicationContext context = new ApplicationContext(conn);
            string name = Faker.Name.First();
            context.Students.Add(new Student
            {
                Name = name
            });
            context.SaveChanges();
            Assert.IsNotNull(context.Students.AsNoTracking().FirstOrDefault(p => p.Name == name));
        }

        [TestMethod]
        public void Operation_AddRange_Example()
        {
            using ApplicationContext context = new ApplicationContext(conn);
            int count = 10;
            List<Student> students = new List<Student>();
            for (int i = 0; i < count; i++)
            {
                students.Add(new Student
                {
                    Name = Faker.Name.First()
                });
            }
            context.Students.AddRange(students);
            context.SaveChanges();
            Assert.AreEqual(count, context.Students.AsNoTracking().ToList().Count);
        }

        [TestMethod]
        public void Operation_Update_Example()
        {
            using ApplicationContext context = new ApplicationContext(conn);
            string name = Faker.Name.First();
            context.Students.Add(new Student
            {
                Name = name
            });
            context.SaveChanges();
            var model = context.Students.FirstOrDefault();
            Console.WriteLine(context.Entry(model).State);
            if (model is not null)
            {
                model.Name += "_update";
                Console.WriteLine(context.Entry(model).State);
            }
            context.SaveChanges();
            Console.WriteLine(context.Entry(model).State);

            Assert.IsTrue(model.Name != name);
        }

        [TestMethod]
        public void Operation_Delete_Example_01()
        {
            using ApplicationContext context = new ApplicationContext(conn);
            string name = Faker.Name.First();
            context.Students.Add(new Student
            {
                Name = name
            });
            context.SaveChanges();

            var model = context.Students.FirstOrDefault(p => p.Name == name);
            // Remove需要先查询后删除
            context.Students.Remove(model);
            context.SaveChanges();
            Assert.IsTrue(context.Students.AsNoTracking().FirstOrDefault(p => p.Id == model.Id) == null);
        }

        /// <summary>
        /// 根据主键查询
        /// </summary>
        [TestMethod]
        public void Operation_Query_Find()
        {
            using ApplicationContext context = new ApplicationContext(conn);
            string name = Faker.Name.First();
            context.Students.Add(new Student
            {
                Name = name
            });
            context.SaveChanges();

            var model = context.Students.Find(1);
            Assert.IsTrue(model != null);
        }

        [TestMethod]
        public void Operation_Query_Load()
        {
            using ApplicationContext context = new ApplicationContext(conn);
            string name = Faker.Name.First();
            context.Students.Add(new Student
            {
                Name = name
            });
            context.SaveChanges();
            context.Students.Load();
            var models = context.Students.Local.ToBindingList();
            Assert.IsTrue(models != null);
        }

        public class ApplicationContext : DbContext
        {
            public ApplicationContext(string nameOrConnectionString) : base(nameOrConnectionString)
            {

            }

            public DbSet<Student> Students { get; set; }

            public DbSet<StudentAddress> StudentAddresses { get; set; }
        }

        public class Student
        {
            public Student() { }

            [Key]
            public int Id { get; set; }

            [Required, MaxLength(100)]
            public string Name { get; set; }
        }

        public class StudentAddress
        {
            [Key]
            public int Id { get; set; }

            [Required, MaxLength(100)]
            public string Address { get; set; }
        }
    }
}
