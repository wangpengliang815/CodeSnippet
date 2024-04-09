using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace CodeSnippet.CsharpTests
{
    [TestCategory("OopTests")]
    [TestClass()]
    public class OOP_01Tests
    {
        /// <summary>
        /// 类的定义
        /// </summary>
        [TestMethod]
        public void ClassDefine()
        {
            var person = new Person()
            {
                Name = "wang",
                Age = 100
            };
            Console.WriteLine($"{person.Name},{person.Age},{person.Birthday},{person.readonlyValue},{Person._describe}");
        }

        /// <summary>
        /// 方法调用
        /// </summary>
        [TestMethod]
        public void MethodCalling()
        {
            var person = new Person();

            person.Print("test");

            person.Print("wang", "test");

            person.Print(name: "wang", messae: "test");

            person.Print("wang", "test", "beijing");

            person.Print("wang", new string[] { "1", "2" });
        }

        /// <summary>
        /// 本地函数
        /// </summary>
        [TestMethod]
        public void LocalMethod()
        {
            static int Add(int x, int y) => x + y;

            int result = Add(1, 2);
            Console.WriteLine(result);
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
            var person = new Person()
            {
                height = 100
            };
            Console.WriteLine(person.height);

            int test;
            //将可空类型直接赋值给test是失败的(test = person.height),因为无法保证p.height是有值的

            // 使用HasValue属性
            test = person.height.HasValue ? person.height.Value : -1;

            // 使用空合并表达式简化
            test = person.height ?? -1;

            // 使用Value属性
            test = person.height.Value;

            Console.WriteLine(test);
        }

        [TestMethod]
        /// <summary>
        /// 值类型对象按值传递
        /// 值类型对象传递给方法时，传递的是值类型对象的副本而不是值类型对象本身
        /// </summary>
        public void ParamByVal()
        {
            static void SetIntValue(int i)
            {
                i += 100;
            }

            int i = 1;
            SetIntValue(i);

            // i=1,i是值类型按值传递，此处传递给方法的其实是变量i的副本而不是对象本身
            Assert.AreEqual(1, i);
        }

        [TestMethod]
        /// <summary>
        /// 引用类型对象参数传递按引用传递
        /// 对于引用类型对象，其实也是按值传递的，但是不像值类型传递的是一个副本，引用类型传递的是引用地址
        /// 在方法中使用这个地址去修改对象的成员，自然就会影响到原来的对象
        /// 注意：如果值类型对象中含有引用类型的成员，那么当值类型对象在传递给方法时，副本中克隆的是引用类型成员的地址，而不是引用类型对象的副本，所以在方法中修改此引用类型对象成员中的成员等也会影响到原来的引用类型对象。
        /// </summary>
        public void ParamByReference()
        {
            static void SetIntArrValue(List<int> arr)
            {
                arr.Add(100);
            }

            var arr = new List<int>() { 1, 2, 3, 4, 5 };

            Console.WriteLine(JsonConvert.SerializeObject(arr));
            SetIntArrValue(arr);
            // [1,2,3,4,5,100]
            Console.WriteLine(JsonConvert.SerializeObject(arr));
        }

        [TestMethod]
        /// <summary>
        /// 尽管string属于引用类型，它在参数传递时表现出了按值传递的特色
        /// </summary>
        public void ParamByString()
        {
            static void SetStringValue(string oldStr)
            {
                oldStr = "world";
            }

            string str = "hello";
            SetStringValue(str);
            // 此处正常理解应该返回hello,因为j是引用类型。
            // 但其实因为string的“不变性”，所以在被调用方法中执行 oldStr = "world"时，此时并不会直接修改oldStr中的"hello"值为"world"，因为string类型是不变的，不可修改的
            // 此时内存会重新分配一块内存，然后把这块内存中的值修改为 “world”，然后把内存中地址赋值给oldStr变量，所以此时str仍然指向 "hello"字符，而oldStr却改变了指向，它最后指向了"world"字符串
            Console.WriteLine(str);
        }

        [TestMethod]
        /// <summary>
        /// 参数传递out
        /// </summary>
        ///<remarks>
        /// 方法返回不同类型的多个值时可以使用out参数
        ///</remarks>
        public void ParameterPassing_Out()
        {
            Person p = new();
            p.Print("wang", out bool result);
            Assert.AreEqual(true, result);
        }
    }

    /// <summary>
    /// 类的定义
    /// </summary>
    public class Person
    {
        // 实例成员：字段
        private string _name;
        private int _age;

        /// <summary>
        /// 实例成员:属性
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// 实例成员:属性(控制赋值)
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
        /// 实例成员:自动实现的属性
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 实例成员:只读属性
        /// </summary>
        public DateTime Birthday { get; }

        /// <summary>
        /// 实例成员:只读字段
        /// </summary>
        public readonly string readonlyValue;

        /// <summary>
        /// 实例成员:可空类型(可以为空的值类型)
        /// </summary>
        public int? height;

        // 静态成员:只读字段
        public readonly static string staticReadonlyValue;

        // 静态成员:字段
        public static string _describe = "static field";

        // 方法
        public void PrintAge()
        {
            Console.WriteLine(_age);
        }

        // 如果方法实现只有一条语句,可以使用表达式体方法定义，如下所示：
        //public void PrintAge() => Console.WriteLine(_age);

        // 方法重载
        public void Print(string message) => Console.WriteLine(message);

        public void Print(string name, string messae) => Console.WriteLine($"name:{name},message:{messae}");

        // 可选参数
        public void Print(string name, string message, string address, int age = 100) => Console.WriteLine($"{name},{message},{address},{age}");

        // 个数可变的参数Prams[]
        public void Print(string name, params string[] args)
        {
            foreach (var item in args)
            {
                Console.WriteLine($"{name}:{item}");
            }
        }

        // out参数返回不同类型多个值
        public string Print(string name, out bool result)
        {
            result = true;
            return name;
        }

        // 构造函数：与类同名且无返回值的方法
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
            staticReadonlyValue = "value from staticConstructor";
        }

        /// <summary>
        /// 运算符重载
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Person operator +(Person a, Person b)
        {
            Person person = new()
            {
                Address = a.Address + b.Address
            };
            return person;
        }
    }

    /// <summary>
    /// 结构定义
    /// </summary>
    public struct PersonStruct
    {
        public int Age { get; set; }
    }

    /// <summary>
    /// 枚举定义
    /// </summary>
    public enum Colors
    {
        Red = 1,
        Green = 2,
        Blue = 3
    }
}
