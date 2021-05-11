namespace CodeSnippet.Csharp
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestCategory("Basics")]
    [TestClass()]
    public class BasicsTests
    {
        /// <summary>
        /// 变量定义
        /// </summary>
        [TestMethod()]
        public void Variable()
        {
            // 变量定义
            string name = "wang";
            int age = 25;
            DateTime time = new DateTime(1995, 8, 15);

            // 同时声明多个同类型变量
            string n1 = "wang", n2 = "li";

            Assert.AreEqual("wang", name);
            Assert.AreEqual(25, age);
            Assert.AreEqual(new DateTime(1995, 8, 15), time);
            Assert.AreEqual("wang", n1);
            Assert.AreEqual("li", n2);
        }

        /// <summary>
        /// 类型推断
        /// </summary>
        ///<remarks>
        /// 运行时根据value推断出变量类型
        ///</remarks>
        [TestMethod()]
        public void VariableTypeInference()
        {
            var name = "wang";
            Assert.AreEqual("wang", name);
        }

        /// <summary>
        /// 常量
        /// </summary>
        ///<remarks>
        /// 值在使用过程中不会发生改变的值称为常量,使用const关键字定义
        ///</remarks>
        [TestMethod()]
        public void ConstVariable()
        {
            // 必须在声明常量时进行初始化,且之后不能再重新赋值
            const string name = "wang";
            Assert.AreEqual("wang", name);
        }

        /// <summary>
        /// 流控制if
        /// </summary>
        [TestMethod()]
        public void FlowControl_if()
        {
            int condition = 0;
            if (condition == 0)
            {
                Console.WriteLine(0);
            }
            else if (condition == 1)
            {
                Console.WriteLine(1);
            }
            else
            {
                Console.WriteLine("other value");
            }

            // 如果条件分支中只有一条语句可以省略花括号
            if (condition == 0)
                Console.WriteLine(0);
            else if (condition == 1)
                Console.WriteLine(1);
            else
                Console.WriteLine("other value");

            Assert.IsTrue(true);
        }

        /// <summary>
        /// 流控制switch
        /// </summary>
        [TestMethod()]
        public void FlowControl_switch()
        {
            int condition = 0;
            switch (condition)
            {
                case 0:
                    Console.WriteLine(0);
                    break;
                case 1:
                    Console.WriteLine(1);
                    break;
                default:
                    Console.WriteLine("other value");
                    break;
            }
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 循环for
        /// </summary>
        ///<remarks>
        /// 执行下一次迭代前,测试是否满足某个条件
        ///</remarks>
        [TestMethod()]
        public void Circulation_for()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
            }
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 循环while
        /// </summary>
        ///<remarks>
        /// 常用于:循环开始前,不知道重复执行的次数
        ///</remarks>
        [TestMethod()]
        public void Circulation_while()
        {
            int i = 0;
            while (i < 10)
            {
                Console.WriteLine(i);
                i++;
            }
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 循环do while
        /// </summary>
        ///<remarks>
        /// while循环的后测试版本,至少实行一次
        ///</remarks>
        [TestMethod()]
        public void Circulation_dowhile()
        {
            int i = 0;
            do
            {
                Console.WriteLine(i);
                i++;
            } while (i < 10);
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 循环foreach
        /// </summary>
        ///<remarks>
        /// 用于迭代集合中的每一项,但是无法改变集合中各项的值
        ///</remarks>
        [TestMethod()]
        public void Circulation_foreach()
        {
            List<string> lists = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                lists.Add(i.ToString());
            }

            foreach (var item in lists)
            {
                Console.WriteLine(item);
            }

            Assert.IsTrue(true);
        }

        /// <summary>
        /// 跳转语句goto
        /// </summary>
        ///<remarks>
        /// goto语句可以直接跳转到程序中用标签指定的另一行,不推荐使用
        ///</remarks>
        [TestMethod()]
        public void Goto_goto()
        {
            for (int i = 0; i < 10; i++)
            {
                if (i == 5)
                {
                    Console.WriteLine("goto");
#pragma warning disable S907 // "goto" statement should not be used,-sample code
                    goto login;
#pragma warning restore S907 // "goto" statement should not be used,-sample code
                }
                Console.WriteLine(i);
            }
            login:
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 跳转语句break
        /// </summary>
        ///<remarks>
        /// 用于结束循环
        ///</remarks>
        [TestMethod()]
        public void Goto_break()
        {
            for (int i = 0; i < 10; i++)
            {
                if (i == 5)
                {
                    break;
                }
                Console.WriteLine(i);
            }

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (j == 2)
                    {
                        // break如果在嵌套循环内部,只退出当前循环
                        break;
                    }
                    Console.WriteLine($"j={j}");
                }
                Console.WriteLine($"i={i}");
            }
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 跳转语句continue
        /// </summary>
        ///<remarks>
        /// 跳过当前循环,开始下次循环
        ///</remarks>
        [TestMethod()]
        public void Goto_continue()
        {
            for (int i = 0; i < 10; i++)
            {
                if (i == 5)
                {
                    continue;
                }
                Console.WriteLine(i);
            }
            Assert.IsTrue(true);
        }
        
        /// <summary>
        /// 跳转语句return
        /// </summary>
        ///<remarks>
        /// 退出类的方法,将控制权返回给方法的调用者
        /// 如果方法有返回值,return语句必须返回这个类型的值
        ///</remarks>
        [TestMethod()]
        public void Goto_return()
        {
            for (int i = 0; i < 10; i++)
            {
                if (i == 5)
                {
                    return;
                }
                Console.WriteLine(i);
            }
            Assert.IsTrue(true);
        }
    }
}
