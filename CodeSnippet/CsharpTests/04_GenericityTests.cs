using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections;
using System.Collections.Generic;

namespace CodeSnippet.CsharpTests
{
    [TestCategory("GenericityTests")]
    [TestClass()]
    public class GenericTests
    {
        public delegate void SayHi<T>(T t);

        /// <summary>
        /// 泛型优点
        /// </summary>
        [TestMethod]
        public void GenericityMerit()
        {
            // 非泛型集合ArrayList
            ArrayList arrs = new()
            {
                "wang",
                100
            };
            // 问题1:性能
            foreach (var item in arrs)
            {
                // 遍历时不可避免装箱/拆箱操作
                Console.WriteLine(item);
            }
            // 问题2:类型安全
            foreach (var item in arrs)
            {
                // 这里如果将item设置为int类型遍历集合,会抛出异常,因为集合中存在string类型属性
                Console.WriteLine(item);
            }

            // 泛型集合
            List<string> lists = new List<string>
            {
                "wang","li"
            };

            foreach (var item in lists)
            {
                // 因为泛型集合在定义时已经确定了存储元素类型,所以不存在装箱/拆箱操作,也不会出现读取时的类型安全问题
                Console.WriteLine(item);
            }
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 泛型类
        /// </summary>
        [TestMethod]
        public void GenericityClass()
        {
            MyArray<int> genericInt = new();
            genericInt.t = 123;
            Console.WriteLine(genericInt.t);

            MyArray<string> genericStr = new();
            genericStr.t = "123";
            Console.WriteLine(genericStr.t);
        }

        /// <summary>
        /// 泛型方法
        /// </summary>
        [TestMethod]
        public void GenericityMethod()
        {
            Print p = new();
            p.PrintType<int>();
            p.PrintType<double>();
        }

        /// <summary>
        /// 泛型接口
        /// </summary>
        [TestMethod]
        public void GenericityInterface()
        {
            ClassADAL<string> a = new();
            a.Add("hello");

            ClassBDAL<string> b = new();
            b.Add("hello");
        }

        /// <summary>
        /// 泛型委托
        /// </summary>
        [TestMethod]
        public void GenericityDelegate()
        {
            SayHi<int> a = new((p) => { Console.WriteLine(p); });
            a.Invoke(123);

            SayHi<string> b = new((p) => { Console.WriteLine(p); });
            b.Invoke("123");
        }

        /// <summary>
        /// 泛型协变(子类到父类的转换)
        /// </summary>
        /// <remarks>
        /// out：协变covariant，用来修饰返回值
        /// </remarks>
        [TestMethod]
        public void GenericityCovariant()
        {
            // 直接声明Animal类
            Animal animal = new();
            Console.WriteLine($"直接声明Animal类,{animal.GetType()}");

            // 直接声明Cat类
            Cat cat = new();
            Console.WriteLine($"直接声明Cat类,{cat.GetType()}");

            // 声明子类对象指向父类
            Animal animal2 = new Cat();
            Console.WriteLine($"声明子类对象指向父类,{animal2.GetType()}");

            // 声明Animal类的集合
            List<Animal> listAnimal = new();
            for (int i = 0; i < 3; i++)
            {
                listAnimal.Add(new Animal { });
                Console.WriteLine($"声明Animal类的集合,{i},{listAnimal[i].GetType()}");
            }

            // 声明Cat类的集合
            List<Cat> listCat = new();
            for (int i = 0; i < 3; i++)
            {
                listCat.Add(new Cat());
                Console.WriteLine($"声明Cat类的集合,{i},{listAnimal[i].GetType()}");
            }

#if debug
            // 一只Cat属于Animal，一群Cat也应该属于Animal。但实际上这样声明是错误的：因为List<Cat>和List<Animal>之间没有父子关系
            List<Animal> list = new List<Cat>();
#endif
            // 泛型协变，IEnumerable泛型参数类型使用了out修饰
            ICustomerListOut<Animal> customerList1 = new CustomerListOut<Animal>();
            Console.WriteLine($"{customerList1.GetType()}");

            ICustomerListOut<Animal> customerList2 = new CustomerListOut<Cat>();
            Console.WriteLine($"{customerList2.GetType()}");
        }

        /// <summary>
        /// 泛型逆变(父类到子类的转换)
        /// </summary>
        /// <remarks>
        /// int：协变contravariant，用来修饰返回值(子类到父类的转换)
        /// </remarks>
        [TestMethod]
        public void GenericityContravariant()
        {
            // 直接声明Animal类
            Animal animal = new Animal();
            Console.WriteLine($"{animal.GetType()}");

            // 直接声明Cat类
            Cat cat = new Cat();
            Console.WriteLine($"{cat.GetType()}");

            // 声明子类对象指向父类
            Animal animal2 = new Cat();
            Console.WriteLine($"{animal2.GetType()}");

            // 声明Animal类的集合
            List<Animal> listAnimal = new List<Animal>();
            foreach (var item in listAnimal)
            {
                Console.WriteLine($"{item.GetType()}");
            }

            // 声明Cat类的集合
            List<Cat> listCat = new List<Cat>();
            foreach (var item in listCat)
            {
                Console.WriteLine($"{item.GetType()}");
            }

            ICustomerListIn<Cat> customerListCat1 = new CustomerListIn<Cat>();
            Console.WriteLine($"{customerListCat1.GetType()}");

            ICustomerListIn<Cat> customerListCat2 = new CustomerListIn<Animal>();
            Console.WriteLine($"{customerListCat2.GetType()}");
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 泛型会为不同的类型都创建一个副本
        /// 所以静态构造函数会执行4次,而且每次静态属性的值都是一样的
        /// 利用泛型的这一特性，可以实现缓存
        /// 注意：只能为不同的类型缓存一次。泛型缓存比字典缓存效率高。泛型缓存不能主动释放
        /// </summary>
        [TestMethod]
        public void GenericityCache()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(GenericCache<int>.GetCache());
                Console.WriteLine(GenericCache<long>.GetCache());
                Console.WriteLine(GenericCache<DateTime>.GetCache());
                Console.WriteLine(GenericCache<string>.GetCache());
            }
            Assert.IsTrue(true);
        }
    }

    /// <summary>
    /// 泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MyArray<T>
    {
        public T t;
    }

    /// <summary>
    /// 泛型方法
    /// </summary>
    public class Print
    {
        public void PrintType<T>() where T : new()
        {
            T t = new();
            Console.WriteLine(t.GetType());
        }
    }

    /// <summary>
    /// 泛型接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDAL<T>
    {
        void Add(T t);
    }

    public class ClassADAL<T> : IDAL<T>
    {
        public void Add(T t)
        {
            Console.WriteLine(t);
        }
    }

    public class ClassBDAL<T> : IDAL<T>
    {
        public void Add(T t)
        {
            Console.WriteLine(t);
        }
    }

    public class Animal
    {
        public string Name { get; set; }

        public string Age { get; set; }
    }

    public class Cat : Animal
    {
        public string Color { get; set; }
    }

    /// <summary>
    /// out 协变 只能是返回结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICustomerListOut<out T>
    {
        T Get();
    }

    public class CustomerListOut<T> : ICustomerListOut<T>
    {
        public T Get()
        {
            return default;
        }
    }

    /// <summary>
    /// 逆变 只能是方法参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICustomerListIn<in T>
    {
        void Show(T t);
    }

    public class CustomerListIn<T> : ICustomerListIn<T>
    {
        public void Show(T t)
        {
            Console.WriteLine($"{t.GetType()}");
        }
    }

    /// <summary>
    /// 类中的静态类型无论实例化多少次，在内存中只会有一个
    /// 静态构造函数只会执行一次
    /// 在泛型类中，T类型不同
    /// 每个不同的T类型，都会产生一个不同的副本，所以会产生不同的静态属性、不同的静态构造函数
    /// </summary>
    /// <typeparam name="T"></typeparam>
#pragma warning disable S1118 // Utility classes should not have public constructors
    public class GenericCache<T>
#pragma warning restore S1118 // Utility classes should not have public constructors
    {
#pragma warning disable S3963 // "static" fields should be initialized inline
        static GenericCache()
        {
            _CachedValue = string.Format("{0}_{1}",
                typeof(T).FullName, DateTime.Now.ToString("yyyyMMddHHmmss.fff"));
        }
#pragma warning restore S3963 // "static" fields should be initialized inline

#pragma warning disable S2743 // Static fields should not be used in generic types
        private static readonly string _CachedValue = "";
#pragma warning restore S2743 // Static fields should not be used in generic types

        public static string GetCache()
        {
            return _CachedValue;
        }
    }
}
