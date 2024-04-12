using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CodeSnippet.CsharpTests
{
    [TestCategory("DelegateTests")]
    [TestClass()]
    public class DelegateTests
    {
        // 01：声明委托
        delegate int MyDelegate(int x, int y);

        // 02:定义委托对应的方法
        int AddMethod(int x, int y)
        {
            return x + y;
        }

        /// <summary>
        /// 简单委托定义
        /// </summary>
        /// <remarks>
        /// step01：使用delegate关键字定义委托
        /// step02：声明委托对应的方法
        /// step03：实例化委托将方法作为参数传入
        /// </remarks>
        [TestMethod]
        public void DelegateDefine()
        {
            // 03：实例化委托将方法作为参数传入
            MyDelegate add = new(AddMethod);
            Console.WriteLine(add.Invoke(1, 2));
        }

        /// <summary>
        /// 匿名方法定义委托
        /// </summary>
        /// <remarks>
        /// step01：使用delegate关键字定义委托
        /// step02：使用匿名方法的写法把一个方法赋值给委托
        /// 这时省略了定义方法这一步,将三步简化成了两步
        /// </remarks>
        [TestMethod]
        public void DelegateDefine_AnonymousMethods()
        {
            // 02：使用匿名方法的写法把一个方法赋值给委托
            MyDelegate add = delegate (int x, int y)
            {
                return x + y;
            };
            Console.WriteLine(add.Invoke(1, 2));
        }

        /// <summary>
        /// Lambda表达式定义委托
        /// </summary>
        /// <remarks>
        /// 运算符"=>"左边列出了需要的参数,右边定义了赋予lambda变量的方法实现代码
        /// 方法1：简单的把delegate去掉，在() 与 {}之间加上 =>
        /// 方法2：在方法1的基础上把参数类型都干掉了
        /// 方法3：要干就干彻底点,把{}去掉
        /// </remarks>
        [TestMethod]
        public void DelegateDefine_Lambda()
        {
            //方法一：
            MyDelegate add1 = (int x, int y) => { return x + y; };
            Console.WriteLine(add1(1, 2));

            //方法二：
            MyDelegate add2 = (x, y) => { return x + y; };
            Console.WriteLine(add2(1, 2));

            //方法三：
            MyDelegate add3 = (x, y) => x + y;
            Console.WriteLine(add3(1, 2));
        }

        /// <summary>
        /// 泛型委托Action
        /// </summary>
        /// <remarks>
        /// 用于没有返回值的方法（参数根据自己情况进行传递）
        /// </remarks>
        [TestMethod]
        public void Delegate_Action()
        {
            // Action：无参数
            Action action1 = () => { Console.WriteLine("啦啦啦啦"); };
            action1();

            // Action：一个参数
            Action<string> action2 = p => { Console.WriteLine("啦啦啦啦,name:{0}", p); };
            action2("wang");

            // Action：多个参数
            Action<string, int> action3 = (name, age) => { Console.WriteLine("啦啦啦啦,name:{0},age:{1}", name, age); };
            action3("wang", 25);
        }

        /// <summary>
        /// 泛型委托Func
        /// </summary>
        /// <remarks>
        /// 用于有返回值的方法（参数根据自己情况进行传递）
        /// </remarks>
        [TestMethod]
        public void Delegate_Func()
        {
            //方法一：
            Func<int, int, int> add1 = (int x, int y) => { return x + y; };
            int r1 = add1(1, 2);

            //方法二：
            Func<int, int, int> add2 = (x, y) => { return x + y; };
            int r2 = add2(1, 2);

            //方法三：
            Func<int, int, int> add3 = (x, y) => x + y;
            int r3 = add3(1, 2);

            Console.WriteLine($"{r1},{r2},{r3}");
        }

        /// <summary>
        /// 表达式树
        /// </summary>
        /// <remarks>
        /// 表达式树是存取Lambda表达式的一种数据结构。要用Lambda表达式的时候,直接从表达式中获取出来 Compile() 就可以直接用了
        /// </remarks>
        [TestMethod]
        public void Delegate_Expression()
        {
            Expression<Func<int, int, int>> exp = (x, y) => x + y;
            Func<int, int, int> fun = exp.Compile();
            int result = fun(1, 2);
            Console.WriteLine(result);
        }

        /// <summary>
        /// Invoke
        /// </summary>
        [TestMethod]
        public void Delegate_Invoke()
        {
            MyDelegate add = delegate (int x, int y)
            {
                return x + y;
            };
            add.Invoke(1, 2);
            //等价于
            add.Invoke(1, 2);
        }

        /// <summary>
        /// 委托数组
        /// </summary>
        [TestMethod]
        public void Delegate_Array()
        {
            // 定义委托数组
            Func<double, double>[] delegates = [
                 Math.MultipleTwo,
                 Math.Square
            ];

            // 使用委托数组
            for (int i = 0; i < delegates.Length; i++)
            {
                Console.WriteLine(delegates[i](3.7));
                Console.WriteLine(delegates[i](3));
            }
        }

        /// <summary>
        /// 委托数组使用内置Func
        /// </summary>
        [TestMethod]
        public void Delegate_Array2()
        {
            // 定义委托数组这里不再使用CalcDelegate委托
            Func<double, double>[] delegates = [
                 Math.MultipleTwo,
                 Math.Square
            ];

            // 使用委托数组
            for (int i = 0; i < delegates.Length; i++)
            {
                Console.WriteLine(delegates[i](3.7));
                Console.WriteLine(delegates[i](3));
            }
        }

        /// <summary>
        /// 多播委托
        /// 多播委托可以按顺序调用多个方法，为此委托的签名必须返回void，否则就只能得到委托最后调用的最后一个方法的结果。
        /// </summary>
        [TestMethod]
        public void Delegate_Multicast()
        {
            // 定义委托数组这里不再使用CalcDelegate委托
            Func<double, double> func = Math.MultipleTwo;
            func += Math.Square;
            // 这里只返回了3.0阶乘的值
            Console.WriteLine(func(3));

            // 使用+=和-=在委托中增减方法调用
            Action action = () => { Console.WriteLine("hello"); };
            action += () => { Console.WriteLine("world"); };
            action();
        }

        /// <summary>
        /// 多播委托异常处理
        /// 使用多播委托，意味着多播委托里包含一个逐个调用的委托集合，如果集合其中一个方法抛出异常.整个迭代就会停止
        /// </summary>
        [TestMethod]
        public void Delegate_MulticastException()
        {
#if debug
            Action action = () =>
            {
                Console.WriteLine("hello");
                throw new Exception();
            };
            action += () => { Console.WriteLine("world"); };
            action();
            // 委托只调用了第一个方法，因为第一个方法抛出了异常，委托的迭代停止
#endif
        }

        /// <summary>
        /// 多播委托获取方法列表
        /// 使用 Delegate的GetInvocationList() 方法自己迭代方法列表。
        /// </summary>
        [TestMethod]
        public void Delegate_GetInvocationList()
        {
            Action action = () => { Console.WriteLine("hello"); throw new Exception(); };
            action += () => { Console.WriteLine("world"); };
            var delegates = action.GetInvocationList();

            // 如果不处理异常程序在抛出异常后停止
            //foreach (Action item in delegates)
            //{
            //    item();
            //}

            // 这里必须显式指定item类型为Action委托
            foreach (Action item in delegates)
            {
                // 修改后，程序在捕获异常后，会迭代下一个方法
                try
                {
                    item();
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                }
            }
        }

        /// <summary>
        /// 委托闭包陷阱
        /// 所谓的闭包对象，指的是如果匿名方法（Lambda表达式）引用了某个局部变量，编译器就会自动将该引用提升到该闭包对象中。
        /// 即将for循环中的变量 i 修改成了引用闭包对象的公共变量i。这样一来，即使代码执行后离开了原局部变量 i 的作用域(如for循环)，包含该闭包对象的作用域也还存在。
        /// 
        /// </summary>
        [TestMethod]
        public void Delegate_Closure()
        {
            //List<Action> list = new();
            //for (int i = 0; i < 5; i++)
            //{
            //    Action t = () => Console.WriteLine(i.ToString());
            //    list.Add(t);
            //}
            //foreach (Action t in list)
            //{
            //    // 此时其实内部的i用的是for循环的i所以会循环输出5
            //    t();
            //}

            List<Action> list2 = new List<Action>();
            for (int i = 0; i < 5; i++)
            {
                int temp = i;
                Action t = () => Console.WriteLine(temp.ToString());
                list2.Add(t);
            }
            foreach (Action t in list2)
            {
                t();
            }
        }
    }

    class Math
    {
        public static double MultipleTwo(double value)
        {
            return value * 2;
        }

        public static double Square(double value)
        {
            return value * value;
        }
    }
}
