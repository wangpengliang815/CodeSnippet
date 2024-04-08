using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

/// <summary>
/// 实现继承/接口继承/抽象类
/// </summary>
///<remarks>
/// 抽象类和抽象函数
///    C#中允许把类或函数声明为abstract,抽象类不能被实例化.抽象函数也不能直接实现,必须在非抽象的派生类中重写
///    如果类包含抽象函数, 则该类也必须被声明为抽象的
///    抽象方法只在派生类中真正实现，这表明抽象方法只存放函数原型，不涉及主体代码
///    派生自抽象类的类需要实现其基类的抽象方法，才能实例化对象
///    使用override关键字可在派生类中实现抽象方法，经override声明重写的方法称为重写基类方法，其签名必须与override方法的签名相同
/// 抽象类和接口的区别
///    相同点：
///      1.都可以被继承
///      2.都不能被实例化
///      3.都包含方法声明
///      4.派生类必须实现未实现的方法
///    区别:
///      抽象基类可以定义字段/属性/方法实现.接口只能定义属性/索引器/事件/方法声明
///      抽象类是一个不完整的类, 需要通过集成进一步细化.而接口更像是一个行为规范,表明我能做什么
///      接口是可以被多重实现的, 可以有多个类实现接口, 
///      因为类的单一继承性, 抽象类只能被单一继承
///      抽象类实现继承需要使用override关键字,接口则不用 
///      如果抽象类实现接口, 可以把接口方法映射到抽象类中作为抽象方法不必实现, 而在抽象类的子类中实现接口方法 
///      抽象类表示的是:这个对象是什么
///      接口表示的是:这个对象能做什么;使用抽象类是为了代码的复用,使用接口是为了实现多态性
/// 普通类和抽象类的区别
///    都可以被继承 抽象类不能实例化,
///    普通类允许实例化 抽象方法值包含方法声明而且必须包含在抽象类中 
///    子类继承抽象类必须实现抽象类中的抽象方法除非子类也是抽象类 抽象类中可以包含抽象方法和实例方法
/// 派生类的构造函数执行顺序
///    实例化父类时，可以使用new子类，执行构造函数顺序为：执行父类构造函数=>执行子类构造函数
///    实例化子类时，只可以new子类，执行顺序同上
///    父类实例化后，只能执行父类的方法，获得父类的属性等
///    实例化子类后，可同时执行子类和父类的方法和属性，如同名方法，则执行子类的方法
///    子类构造函数可以使用base关键字指定调用的父类构造函数
///</remarks>
namespace CodeSnippet.CsharpTests
{
    [TestCategory("OopTests")]
    [TestClass()]
    public class OopTests
    {
        /// <summary>
        /// 类的定义
        /// </summary>
        [TestMethod]
        public void ClassDefine()
        {
            Person person = new()
            {
                Name = "wang",
                Age = 100
            };
            Console.WriteLine($"{person.Name},{person.Age},{person.Birthday},{person.readonlyValue},{Person._describe}");

            // 匿名类型定义
            var person2 = new
            {
                Name = "wang",
                Age = "100"
            };
            Console.WriteLine($"{person2.Name},{person2.Age}");
        }

        /// <summary>
        /// 方法调用
        /// </summary>
        [TestMethod]
        public void MethodCalling()
        {
            Person p = new();
            p.Print("test");

            p.Print("wang", "test");

            p.Print(name: "wang", messae: "test");

            p.Print("wang", "test", "bj");

            p.Print("wang", new string[] { "1", "2" });
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
            static void SetIntValue(int i)
            {
                i += 100;
            }


            static void SetStringValue(string oldStr)
            {
                oldStr = "world";
            }

            int i = 1;
            SetIntValue(i);
            // i=1,i是值类型按值传递，此处传递给方法的其实是变量i的副本
            Console.WriteLine(i);

            string str = "hello";
            SetStringValue(str);
            // 此处正常理解应该返回hello,因为j是引用类型。
            // 但其实因为string的“不变性”，所以在被调用方法中执行 oldStr = "world"时，此时并不会直接修改oldStr中的"hello"值为"world"，因为string类型是不变的，不可修改的
            // 此时内存会重新分配一块内存，然后把这块内存中的值修改为 “world”，然后把内存中地址赋值给oldStr变量，所以此时str仍然指向 "hello"字符，而oldStr却改变了指向，它最后指向了"world"字符串
            Console.WriteLine(str);

            ClassA classA = new() { Age = 1 };
            classA.ChangeAge(classA);
            // output=100,TestA是类按引用传递
            Console.WriteLine(classA.Age);

            StructA structA = new() { Age = 1 };
            StructA.ChangeAge(structA);
            // output=1,StructA是结构按值传递
            Console.WriteLine(structA.Age);

            StructA.ChangeAgeRef(ref structA);
            // output=100,使用ref参数可以通过引用传递值类型参数
            Console.WriteLine(structA.Age);
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

        /// <summary>
        /// 可空类型
        /// </summary>
        ///<remarks>
        /// 可空类型指可以为空的值类型
        ///</remarks>
        [TestMethod]
        public void NullableType()
        {
            Person p = new();
            Assert.AreEqual(null, p.height);
            p.height = 100;
            Assert.AreEqual(100, p.height);
            int test;
            // test = p.height; 将可空类型赋值给test失败的,因为无法保证p.height是有值的

            // 使用HasValue属性
            test = p.height.HasValue ? p.height.Value : -1;

            // 使用空合并表达式简化
            test = p.height ?? -1;

            // 使用Value属性
            test = p.height.Value;

            Assert.IsNotNull(test);
        }

        /// <summary>
        /// 实现继承
        /// </summary>
        [TestMethod]
        public void ImplementationInheritance()
        {
            ChildrenA childrenA = new();
            childrenA.Print();
            childrenA.Print("test");
            childrenA.CallBasePrint("test");
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 抽象类和抽象方法
        /// </summary>
        [TestMethod]
        public void Abstract()
        {
            AbstractChildrenA abstractChildrenA = new();
            abstractChildrenA.Print();
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 接口继承
        /// </summary>
        [TestMethod]
        public void InterfaceInheritance()
        {
            InterfaceChildrenA interfaceChildrenA = new();
            interfaceChildrenA.Print();
            interfaceChildrenA.Print("test");
            Assert.IsTrue(true);
        }
    }

    /// <summary>
    /// 基类
    /// </summary>
    public class BaseClass
    {
        public string BaseA { get; set; }

        public string BaseB { get; set; }

        /// <summary>
        /// 基类方法
        /// </summary>
        public void Print()
        {
            Console.WriteLine($"from {nameof(BaseClass)}");
        }

        /// <summary>
        /// 使用virtual声明的方法,可以在任何派生类中重写
        /// </summary>
        public virtual void Print(string message)
        {
            Console.WriteLine($"from {nameof(BaseClass)},{message}");
        }
    }

    /// <summary>
    /// 抽象基类,不允许实例化
    /// 如果一个类中包含抽象方法,则类也必须是抽象的
    /// 抽象方法不需要定义方法体
    /// </summary>
    public abstract class AbstractBaseClass
    {
        public string BaseA { get; set; }

        public string BaseB { get; set; }

        public abstract void Print();
    }

    /// <summary>
    /// 继承自类BaseClass
    /// </summary>
    public class ChildrenA : BaseClass
    {
        /// <summary>
        /// 如果在基类中也定义了签名相同的方法,
        /// 但没有分别声明为virtual和override,子类就会隐藏基类方法
        /// 推荐使用new关键字显式隐藏基类方法
        /// </summary>
        new public void Print()
        {
            Console.WriteLine($"from {nameof(ChildrenA)}");
        }

        /// <summary>
        /// 使用override重写基类的虚方法
        /// </summary>
        public override void Print(string message)
        {
            Console.WriteLine($"from {nameof(ChildrenA)},{message}");
        }

        /// <summary>
        /// 使用base调用基类方法
        /// </summary>
        public void CallBasePrint(string message)
        {
            base.Print(message);
        }
    }

    /// <summary>
    /// 继承自抽象基类AbstractBaseClass
    /// </summary>
    public class AbstractChildrenA : AbstractBaseClass
    {
        /// <summary>
        /// 如果在基类中也定义了签名相同的方法,
        /// 但没有分别声明为virtual和override,子类就会隐藏基类方法
        /// 推荐使用new关键字显式隐藏基类方法
        /// </summary>
        public override void Print()
        {
            Console.WriteLine($"from {nameof(AbstractChildrenA)}");
        }
    }

    /// <summary>
    /// 接口定义
    /// </summary>
    public interface IPrint
    {
        void Print();

        void Print(string message);
    }

    /// <summary>
    /// 实现接口
    /// </summary>
    public class InterfaceChildrenA : IPrint
    {
        public void Print()
        {
            Console.WriteLine($"{nameof(InterfaceChildrenA)}");
        }

        public void Print(string message)
        {
            Console.WriteLine($"{nameof(InterfaceChildrenA)},{message}");
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

    public class ClassA
    {
        public int Age { get; set; }

        public void ChangeAge(ClassA obj)
        {
            obj.Age = 100;
        }
    }

    public struct StructA
    {
        public int Age { get; set; }

        public static void ChangeAge(StructA obj)
        {
            obj.Age = 100;
        }

        public static void ChangeAgeIn(in StructA obj)
        {
            // 使用in参数可以保证发送到方法内的数据不会更改
            //testA.Age = 100; -- error,因为它是只读的
            Console.WriteLine(obj.Age);
        }

        public static void ChangeAgeRef(ref StructA obj)
        {
            obj.Age = 100;
        }
    }
}
