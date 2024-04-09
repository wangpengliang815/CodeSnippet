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
        delegate void Print(string message);

        // 02:定义委托对应的方法
        void PrintMethod(string message)
        {
            Console.WriteLine(message);
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
        public void DelegateDefinition()
        {
            // 03：实例化委托将方法作为参数传入
            Print print = new Print(PrintMethod);
            print.Invoke(nameof(DelegateDefinition));
            Assert.IsTrue(true);
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
        public void DelegateDefinition_AnonymousMethods()
        {
            // 02：使用匿名方法的写法把一个方法赋值给委托
            Print print = delegate (string message)
            {
                Console.WriteLine(message);
            };
            print.Invoke(nameof(DelegateDefinition_AnonymousMethods));
            Assert.IsTrue(true);
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
        public void DelegateDefinition_Lambda()
        {
            //方法一：
            Print print1 = (string message) => { Console.WriteLine(message); };
            print1(nameof(print1));

            //方法二：
            Print print2 = (message) => { Console.WriteLine(message); };
            print2(nameof(print2));

            //方法三：
            Print print3 = (message) => Console.WriteLine(message);
            print3(nameof(print3));
            Assert.IsTrue(true);
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
            Action<string, int> action3 = (name, age) => { Console.WriteLine("啦啦啦啦,    name:{0},age:{1}", name, age); };
            action3("wang", 25);

            Assert.IsTrue(true);
        }

        /// <summary>
        /// 泛型委托Func
        /// </summary>
        /// <remarks>
        /// 用于没有返回值的方法（参数根据自己情况进行传递）
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
            Assert.IsTrue(true);
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
            Assert.AreEqual(3, result);
        }

        /// <summary>
        /// Invoke
        /// </summary>
        [TestMethod]
        public void Delegate_Invoke()
        {
            Print print = delegate (string message)
            {
                Console.WriteLine(message);
            };
            print.Invoke(nameof(Delegate_Invoke));
            //等价于
            print(nameof(Delegate_Invoke));
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 委托闭包陷阱
        /// </summary>
        [TestMethod]
        public void Delegate_Closure()
        {
            //List<Action> list = new List<Action>();
            //for (int i = 0; i < 5; i++)
            //{
            //    Action t = () => Console.WriteLine(i.ToString());
            //    list.Add(t);
            //}
            //foreach (Action t in list)
            //{
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
            Assert.IsTrue(true);
        }
    }
}
