namespace CodeSnippet.Csharp
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class Person
    {
        /// <summary>
        /// 实例成员=>字段
        /// </summary>
        public string _name;
        private int _age;

        /// <summary>
        /// 实例成员=>属性
        /// </summary>
        public int Age
        {
            get { return _age; }
            set
            {
                if (value > 0 && value <= 100)
                {
                    _age = value;
                }
            }
        }

        /// <summary>
        /// 实例成员：只读字段
        /// </summary>
        public readonly string readonlyValue;

        /// <summary>
        /// 静态成员：字段=>属于类本身
        /// </summary>
        public static string _describe = "static field";

        /// <summary>
        /// 可空类型:指可以为空的值类型
        /// </summary>
        public int? height;

        /// <summary>
        /// 实例成员：自动实现的属性
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 实例成员：只读属性
        /// </summary>
        public DateTime Birthday { get; }

        // 方法
        public void PrintAge()
        {
            Console.WriteLine(_age);
        }

        // 如果方法实现只有一条语句,可以使用表达式体方法定义
        public void PrintName() => Console.WriteLine(_name);

        // 方法重载
        public void Print(string message) => Console.WriteLine(message);
        public void Print(string name, string messae) => Console.WriteLine($"name:{name},message:{messae}");
        // 可选参数
        public void Print(string name, string message, string address, int age = 100) => Console.WriteLine($"{name},{message},{address},{age}");
        // 个数可变的参数
        public void Print(string name, params string[] args)
        {
            Console.WriteLine($"name:{name}");
            foreach (var item in args)
            {
                Console.WriteLine(item);
            }
        }
        // out参数返回不同类型多个值
        public string Print(string name, out bool result)
        {
            result = true;
            return name;
        }

        /// <summary>
        /// 构造函数：与类同名且无返回值的方法
        /// </summary>
        public Person()
        {
            // 只读字段可以在构造函数中赋值
            readonlyValue = "value from constructor";
        }

        /// <summary>
        /// 构造函数支持重载
        /// </summary>
        /// <param name="name"></param>
        public Person(string name)
        {
            _name = name;
        }

        /// <summary>
        /// 构造函数初始化器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="age"></param>
        public Person(string name, int age) : this(name)
        {
            _age = age;
        }

        /// <summary>
        /// 静态构造函数,只执行一次
        /// </summary>
        static Person()
        {

        }
    }

    public class ClassA
    {
        public int Age { get; set; }

        public static void ChangeAge(ClassA testA)
        {
            testA.Age = 100;
        }
    }

    public struct StructA
    {
        public int Age { get; set; }

        public static void ChangeAge(StructA testA)
        {
            testA.Age = 100;
        }

        public static void ChangeAge(in StructA testA)
        {
            // 使用in参数可以保证发送到方法内的数据不会更改
            //testA.Age = 100; -- error,因为它是只读的
            Console.WriteLine(testA.Age);
        }
    }

    public struct StructB
    {
        public int Age { get; set; }

        public static void ChangeAge(ref StructB testb)
        {
            testb.Age = 100;
        }
    }

    [TestCategory("Class")]
    [TestClass()]
    public class ClassTests
    {
        /// <summary>
        /// 类的定义
        /// </summary>
        [TestMethod]
        public void Class()
        {
            Person p = new Person
            {
                _name = "wang",
                Age = 100
            };
            Console.WriteLine($"{p._name},{p.Age},{p.Birthday},{p.readonlyValue},{Person._describe}");
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 匿名类型
        /// </summary>
        ///<remarks>
        /// 继承自object且没有名称的类
        ///</remarks>
        [TestMethod]
        public void AnonymousType()
        {
            var p = new
            {
                Name = "wang",
                Age = "100"
            };
            Console.WriteLine($"{p.Name},{p.Age}");
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 方法调用
        /// </summary>
        [TestMethod]
        public void MethodCalling()
        {
            Person p = new Person();
            p.Print("test");

            p.Print("wang", "test");

            p.Print(name: "wang", messae: "test");

            p.Print("wang", "test", "bj");

            p.Print("wang", new string[] { "1", "2" });
            Assert.IsTrue(true);
        }

        [TestMethod]
        /// <summary>
        /// 参数传递
        /// </summary>
        ///<remarks>
        /// 类是引用类型,作为参数时按引用传递
        /// 结构是值类型,作为参数时按值传递
        ///</remarks>
        public void ParameterPassing()
        {
            ClassA classA = new ClassA { Age = 1 };
            ClassA.ChangeAge(classA);
            // output=100,TestA是类按引用传递
            Console.WriteLine(classA.Age);
            Assert.AreEqual(100, classA.Age);

            StructA structA = new StructA { Age = 1 };
            StructA.ChangeAge(structA);
            // output=1,StructA是结构按值传递
            Console.WriteLine(structA.Age);
            Assert.AreEqual(1, structA.Age);
        }

        [TestMethod]
        /// <summary>
        /// 参数传递ref
        /// </summary>
        ///<remarks>
        /// 类是引用类型,作为参数时按引用传递
        /// 结构是值类型,作为参数时按值传递
        ///</remarks>
        public void ParameterPassing_ref()
        {
            StructB structB = new StructB { Age = 1 };
            StructB.ChangeAge(ref structB);
            // output=100,使用ref参数可以通过引用传递值类型参数
            Console.WriteLine(structB.Age);

            Assert.AreEqual(100, structB.Age);
        }

        [TestMethod]
        /// <summary>
        /// 参数传递out
        /// </summary>
        ///<remarks>
        /// 方法返回不同类型的多个值时可以使用out参数
        ///</remarks>
        public void ParameterPassing_out()
        {
            Person p = new Person();
            p.Print("wang", out bool result);
            Assert.AreEqual(true, result);
        }

        /// <summary>
        /// 可空类型
        /// </summary>
        ///<remarks>
        /// 可空类型指可以为空的值类型
        ///</remarks>
        [TestMethod]
        public void NullableType()
        {
            Person p = new Person();
            Assert.AreEqual(null, p.height);
            p.height = 100;
            Assert.AreEqual(100, p.height);
            int test;
            // test = p.height; 将可空类型赋值给test失败的,因为无法保证p.height是有值的

            // 使用HasValue属性
#pragma warning disable IDE0059 // 不需要赋值
#pragma warning disable S1854 // Unused assignments should be removed
            test = p.height.HasValue ? p.height.Value : -1;
#pragma warning restore S1854 // Unused assignments should be removed
#pragma warning restore IDE0059 // 不需要赋值

            // 使用空合并表达式简化
#pragma warning disable IDE0059 // 不需要赋值
#pragma warning disable S1854 // Unused assignments should be removed
            test = p.height ?? -1;
#pragma warning restore S1854 // Unused assignments should be removed
#pragma warning restore IDE0059 // 不需要赋值

            // 使用Value属性
            test = p.height.Value;

            Assert.IsNotNull(test);
        }
    }
}
