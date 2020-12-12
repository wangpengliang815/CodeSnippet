namespace CodeSnippet.Csharp
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestCategory("Operator")]
    [TestClass()]
    public class OperatorTests
    {
        public class Person
        {
            public string Name { get; set; }

            public void Print(Person person)
            {
                // person为空时,直接返回null
                Name = person?.Name;
            }

            public static Person operator +(Person a, Person b)
            {
                Person person = new Person();
                person.Name = a.Name + b.Name;
                return person;
            }
        }

        /// <summary>
        /// 前置,先加再判断
        /// </summary>
        [TestMethod]
        public void OperatorPreposition()
        {
            int i = 0;
            Assert.AreEqual(1, ++i);
        }

        /// <summary>
        /// 后置,先判断再加
        /// </summary>
        [TestMethod]
        public void OperatorPostposition()
        {
            int i = 0;
            Assert.AreEqual(0, i++);
            Console.WriteLine(i);
        }

        /// <summary>
        /// ?:
        /// </summary>
        [TestMethod]
        public void Operator_ternary()
        {
            int i = 0;
            int j = i == 0 ? 10 : 20;
            Assert.AreEqual(10, j);
        }

        /// <summary>
        /// checked/unchecked
        /// </summary>
        [TestMethod]
        public void Operator_checked()
        {
            byte a = 255;
#if normalCode
            checked
            {
                a++;
            }
#endif
            // 禁止溢出检查,不会报错但会丢失数据
            unchecked
            {
                a++;
            }
            Assert.AreEqual(0, a);
        }

        /// <summary>
        /// is
        /// </summary>
        [TestMethod]
        public void Operator_is()
        {
            int? a = 255;
            Assert.IsTrue(a is int);
            Assert.IsTrue(a is object);
        }

        /// <summary>
        /// as
        /// </summary>
        /// <remarks>
        /// 用于执行引用类型的显示类型转换,如果类型兼容,转换成功
        /// 不兼容 as会返回null
        /// </remarks>
        [TestMethod]
        public void Operator_as()
        {
            object a = 255;
            object b = "wang";
            string s1 = a as string;
            string s2 = b as string;
            Assert.AreEqual(null, s1);
            Assert.AreEqual("wang", s2);
        }

        /// <summary>
        /// sizeof
        /// </summary>
        /// <remarks>
        /// 确定栈中值类型需要的长度
        /// </remarks>
        [TestMethod]
        public void Operator_sizeof()
        {
            Console.WriteLine(sizeof(int));

#if unsafe
            unsafe
            {
                Console.WriteLine(sizeof(string));
            }
#endif
            Assert.IsTrue(true);
        }

        /// <summary>
        /// typeof
        /// </summary>
        /// <remarks>
        /// 返回特定类型的System.Type对象
        /// </remarks>
        [TestMethod]
        public void Operator_typeof()
        {
            Assert.AreEqual(nameof(Int32), typeof(int).Name);
        }

        /// <summary>
        /// nameof
        /// </summary>
        /// <remarks>
        /// 接受一个符号属性或方法,返回名称
        /// </remarks>
        [TestMethod]
        public void Operator_nameof()
        {
            Assert.AreEqual("Int32", nameof(Int32));
        }

        /// <summary>
        /// ??
        /// </summary>
        /// <remarks>
        /// 空合并运算符
        /// 如果第一个操作数是null,值等于第二个操作数
        /// 如果第一个操作数不是null,值等于第一个操作数
        /// </remarks>
        [TestMethod]
        public void Operator_emptyMerge()
        {
            int? a = null;
            int b;
#pragma warning disable S2589 // Boolean expressions should not be gratuitous
            b = a ?? 10;
#pragma warning restore S2589 // Boolean expressions should not be gratuitous
            Assert.AreEqual(10, b);
            a = 3;
#pragma warning disable S2583 // Conditionally executed code should be reachable
            b = a ?? 10;
#pragma warning restore S2583 // Conditionally executed code should be reachable
            Assert.AreEqual(3, b);
        }

        /// <summary>
        /// ?
        /// </summary>
        [TestMethod]
        public void Operator_empty()
        {
            Person p = new Person();
            p.Print(p);
            Assert.AreEqual(null, p.Name);
        }

        /// <summary>
        /// 运算符重载
        /// </summary>
        /// <remarks>
        /// 重载运算符是具有特殊名称的函数,是通过关键字operator后跟运算符的符号来定义的。与其他函数一样,重载运算符有返回类型和参数列表
        /// 1:不是所有的运算符都支持重载,
        /// 2：比较运算符需要成对的重载,如果重载了==,则也必须重载!=,否则产生编译错误。
        /// </remarks>
        [TestMethod]
        public void Operator_Load()
        {
            Person p1 = new Person { Name = "wang" };
            Person p2 = new Person { Name = "li" };
            Person p3 = p1 + p2;
            Assert.AreEqual("wangli", p3.Name);
        }
    }
}
