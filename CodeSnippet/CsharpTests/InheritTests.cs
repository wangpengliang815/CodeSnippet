namespace CodeSnippet.CsharpTests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    [TestCategory("Inherit")]
    [TestClass()]
    public class InheritTests
    {
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
}
