using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;

namespace CodeSnippet.CsharpTests
{
    [TestCategory("BasicsTests")]
    [TestClass()]
    public class BasicsTests
    {
        /// <summary>
        /// 变量定义相关
        /// </summary>
        [TestMethod()]
        public void VariableDefine()
        {
            // 正常变量定义
            string name = "wang";
            int age = 25;
            DateTime time = new(1995, 8, 15);

            // 同时声明多个同类型变量
            string n1 = "wang", n2 = "li";

            Assert.AreEqual("wang", name);
            Assert.AreEqual(25, age);
            Assert.AreEqual(new DateTime(1995, 8, 15), time);
            Assert.AreEqual("wang", n1);
            Assert.AreEqual("li", n2);

            // 类型推断:运行时根据value推断出变量类型
            var name2 = "wang";
            Assert.AreEqual("wang", name2);

            // 使用new表达式:左侧类型被指定后右侧不在需要指定类型
            string name3 = new("wang2");
            Assert.AreEqual("wang2", name3);
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
            // 必须在声明常量时进行初始化且之后不能再重新赋值
            const string name = "wang";
            Assert.AreEqual("wang", name);
        }

        /// <summary>
        /// 流控制if
        /// </summary>
        [TestMethod()]
        public void FlowControl_If()
        {
            int condition = 0;
            if (condition == 0)
            {
                Assert.AreEqual(0, condition);
            }
            else if (condition == 1)
            {
                Assert.AreEqual(1, condition);
            }
            else
            {
                Assert.IsTrue(condition != 0 && condition != 1);
            }

            // 如果条件分支中只有一条语句可以省略花括号
            if (condition == 0)
                Assert.AreEqual(0, condition);
            else if (condition == 1)
                Assert.AreEqual(1, condition);
            else
                Assert.IsTrue(condition != 0 && condition != 1);
        }

        /// <summary>
        /// 流控制switch
        /// </summary>
        [TestMethod()]
        public void FlowControl_Switch()
        {
            int condition = 0;
            switch (condition)
            {
                case 0:
                    Assert.AreEqual(0, condition);
                    break;
                case 1:
                    Assert.AreEqual(1, condition);
                    break;
                default:
                    Assert.IsTrue(condition != 0 && condition != 1);
                    break;
            }
        }

        /// <summary>
        /// 循环for
        /// </summary>
        ///<remarks>
        /// 执行下一次迭代前,测试是否满足某个条件
        ///</remarks>
        [TestMethod()]
        public void Circulation_For()
        {
            int index = 0;
            for (int i = 0; i < 10; i++)
            {
                index++;
                Console.WriteLine(i);
            }
            Assert.IsTrue(index == 10);
        }

        /// <summary>
        /// 循环while
        /// </summary>
        ///<remarks>
        /// 常用于:循环开始前,不知道重复执行的次数
        ///</remarks>
        [TestMethod()]
        public void Circulation_While()
        {
            int index = 0;
            while (index < 10)
            {
                Console.WriteLine(index);
                index++;
            }
            Assert.IsTrue(index == 10);
        }

        /// <summary>
        /// 循环doWhile
        /// </summary>
        ///<remarks>
        /// while循环的后测试版本,至少实行一次
        ///</remarks>
        [TestMethod()]
        public void Circulation_DoWhile()
        {
            int index = 0;
            do
            {
                Console.WriteLine(index);
                index++;
            } while (index < 10);
            Assert.IsTrue(index == 10);
        }

        /// <summary>
        /// 循环foreach
        /// </summary>
        ///<remarks>
        /// 用于迭代集合中的每一项,但是无法改变集合中各项的值
        ///</remarks>
        [TestMethod()]
        public void Circulation_Foreach()
        {
            List<string> lists = [];
            for (int i = 0; i < 10; i++)
            {
                lists.Add(i.ToString());
            }

            int count = 0;
            foreach (var item in lists)
            {
                count++;
                Console.WriteLine(item);
            }

            Assert.IsTrue(lists.Count == count);
        }

        /// <summary>
        /// 跳转语句goto
        /// </summary>
        ///<remarks>
        /// goto语句可以直接跳转到程序中用标签指定的另一行,不推荐使用
        ///</remarks>
        [TestMethod()]
        public void Goto_Goto()
        {
            for (int i = 0; i < 10; i++)
            {
                if (i == 5)
                {
                    Console.WriteLine("goto");
                    goto login;
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
        public void Goto_Break()
        {
            for (int i = 0; i < 10; i++)
            {
                if (i == 5)
                {
                    break;
                }
                Console.WriteLine(i);
            }
        }

        /// <summary>
        /// 跳转语句continue
        /// </summary>
        ///<remarks>
        /// 跳过当前循环,开始下次循环
        ///</remarks>
        [TestMethod()]
        public void Goto_Continue()
        {
            for (int i = 0; i < 10; i++)
            {
                if (i == 5)
                {
                    continue;
                }
                Console.WriteLine(i);
            }
        }

        /// <summary>
        /// 跳转语句return
        /// </summary>
        ///<remarks>
        /// 退出类的方法,将控制权返回给方法的调用者
        /// 如果方法有返回值,return语句必须返回这个类型的值
        ///</remarks>
        [TestMethod()]
        public void Goto_Return()
        {
            for (int i = 0; i < 10; i++)
            {
                if (i == 5)
                {
                    return;
                }
                Console.WriteLine(i);
            }
        }

        /// <summary>
        /// 隐式转换,低精度=>高精度
        /// </summary>
        ///<remarks>
        /// 隐式转换是系统默认的,不需要加以声明就会自动执行隐式类型转换,在隐式转换过程,编译器无需对转换进行详细检查就能够安全的执行
        ///</remarks>
        [TestMethod]
        public void ImplicitConversion()
        {
            int a = 10;
            // 自动隐式类型转换
            double b = a;
            Assert.AreEqual(10, b);
        }

        /// <summary>
        /// 显式转换,高精度=>低精度
        /// </summary>
        /// <remarks>
        /// 将高精度值=>低精度进行数据转换时,可能会丢失数据,这时候需要使用显式转换,并且要考虑到可能出现算术溢出;显式转换需要明确指出指定要转换的类型
        /// 显式转换可能导致错误,进行这种转换时编译器会对转换进行溢出检测,如果有溢出说明转换失败,表示源类型不是一个合法的目标类型无法进行类型转换 强制类型转换会造成数据精度丢失
        /// </remarks>
        [TestMethod]
        public void ExplicitConversion()
        {
            double a = 10;
            // 显式将double类型转换为int
            int b = (int)a;
            Assert.AreEqual(10, b);
        }

        /// <summary>
        /// ToString()
        /// </summary>
        [TestMethod]
        public void Conversion_ToString()
        {
            int a = 10;
            Assert.AreEqual("10", a.ToString());
        }

        /// <summary>
        /// int.Parse()
        /// </summary>
        /// <remarks>
        /// 用于将string类型参数转换为int,需要注意:string类型参数不能为null,并且也只能是各种整型,不能是浮点型
        /// </remarks>
        [TestMethod]
        public void Conversion_IntParse()
        {
            // 正常
            Assert.AreEqual(2, int.Parse("2"));
#if debug
            // 错误:输入字符串格式错误
            int.Parse("2.6");

            // 错误:值不能为null
            int.Parse(null);
#endif
        }

        /// <summary>
        /// int.TryParse()
        /// </summary>
        [TestMethod]
        public void Conversion_IntTryParse()
        {
            // 正常 i=2
            Assert.AreEqual(true, int.TryParse("2", out int i));
            Assert.AreEqual(2, i);

            // 转换失败,false
            Assert.AreEqual(false, int.TryParse("2.6", out _));
        }

        /// <summary>
        /// ReferenceEquals()
        /// </summary>
        /// <remarks>
        /// object类的静态方法,测试两个引用是否指向类的同一个实例,两个实例是否包含内存中的相同地址
        /// null=null是成立的
        /// </remarks>
        [TestMethod]
        public void Compare_ReferenceEquals()
        {
            List<string> a = ["1", "2"];
            List<string> b = ["1", "2"];
            string c = "1";
            string d = "1";
            Assert.AreEqual(true, ReferenceEquals(null, null));
            Assert.AreEqual(false, ReferenceEquals(a, b));

            // 字符串驻留池：对于相同的字符串，CLR不会为其分别分配内存，而是共享同一内存。
            Assert.AreEqual(true, ReferenceEquals(c, d));
        }

        /// <summary>
        /// Equals()
        /// </summary>
        /// <remarks>
        /// object类提供的虚方法,允许重写,比较引用
        /// </remarks>
        [TestMethod]
        public void Compare_Equals()
        {
            List<string> a = ["1", "2"];
            List<string> b = ["1", "2"];
            string c = "1";
            string d = "1";
            Assert.AreEqual(false, a.Equals(b));
            Assert.AreEqual(true, c.Equals(d));
        }

        /// <summary>
        /// Static Equals()
        /// </summary>
        /// <remarks>
        /// Equals()的静态版本,与虚实例版本作用相同,区别在与静态版本两个参数,除了进行引用比较外,也进行相等性比较
        /// </remarks>
        [TestMethod]
        public void Compare_StaticEquals()
        {
            List<string> a = ["1", "2"];
            List<string> b = ["1", "2"];
            string c = "1";
            string d = "1";
            Assert.AreEqual(false, Equals(a, b));
            Assert.AreEqual(true, Equals(c, d));
        }

        /// <summary>
        /// ==
        /// </summary>
        [TestMethod]
        public void Compare_Equality()
        {
            int a = 10;
            int b = 10;
            List<string> c = ["1", "2"];
            List<string> d = ["1", "2"];
            string e = "21";
            string f = "21";
            Assert.AreEqual(true, a == b);
            Assert.AreEqual(false, c == d);
            Assert.AreEqual(true, e == f);
        }

        /// <summary>
        /// 前置:先加再判断
        /// </summary>
        [TestMethod]
        public void OperatorPreposition()
        {
            int i = 0;
            Assert.AreEqual(1, ++i);
        }

        /// <summary>
        /// 后置:先判断再加
        /// </summary>
        [TestMethod]
        public void OperatorPostposition()
        {
            int i = 0;
            Assert.AreEqual(0, i++);
            Console.WriteLine(i);
        }

        /// <summary>
        /// checked/unchecked
        /// </summary>
        [TestMethod]
        public void Operator_Checked()
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
        /// 运算符: is
        /// </summary>
        [TestMethod]
        public void Operator_Is()
        {
            int? a = 255;
            Assert.IsTrue(a is int);
            Assert.IsTrue(a is object);
        }

        /// <summary>
        /// 运算符: as
        /// </summary>
        /// <remarks>
        /// 用于执行引用类型的显示类型转换,如果类型兼容,转换成功
        /// 不兼容 as会返回null
        /// </remarks>
        [TestMethod]
        public void Operator_As()
        {
            object a = 255;
            object b = "wang";
            string s1 = a as string;
            string s2 = b as string;
            Assert.AreEqual(null, s1);
            Assert.AreEqual("wang", s2);
        }

        /// <summary>
        /// 运算符: sizeof
        /// </summary>
        /// <remarks>
        /// 确定栈中值类型需要的长度
        /// </remarks>
        [TestMethod]
        public void Operator_Sizeof()
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
        /// 运算符: typeof
        /// </summary>
        /// <remarks>
        /// 返回特定类型的System.Type对象
        /// </remarks>
        [TestMethod]
        public void Operator_Typeof()
        {
            Assert.AreEqual(nameof(Int32), typeof(int).Name);
        }

        /// <summary>
        /// 运算符: nameof
        /// </summary>
        /// <remarks>
        /// 接受一个符号属性或方法,返回名称
        /// </remarks>
        [TestMethod]
        public void Operator_Nameof()
        {
            Assert.AreEqual("Int32", nameof(Int32));
        }

        /// <summary>
        /// 运算符: ?:
        /// </summary>
        [TestMethod]
        public void Operator_Ternary()
        {
            int i = 0;
            int j = i == 0 ? 10 : 20;
            Assert.AreEqual(10, j);
        }

        /// <summary>
        /// 运算符: ?.
        /// </summary>
        [TestMethod]
        public void Operator_Empty()
        {
            string name = "wang";
            Assert.AreEqual("WANG", name.ToUpper());

            try
            {
                // NullReferenceException 因为name2为null
                string name2 = null;
                Assert.AreEqual(null, name2.ToUpper());
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is NullReferenceException);
            }

            // 使用?.使得程序不会运行出错但是name3结果为null
            string name3 = null;
            Assert.AreEqual(null, name3?.ToUpper());
        }

        /// <summary>
        /// 运算符: ??
        /// </summary>
        /// <remarks>
        /// 空合并运算符
        /// 如果第一个操作数是null,值等于第二个操作数
        /// 如果第一个操作数不是null,值等于第一个操作数
        /// </remarks>
        [TestMethod]
        public void Operator_EmptyMerge()
        {
            int? a = null;
            int b;
            b = a ?? 10;
            Assert.AreEqual(10, b);

            a = 3;
            b = a ?? 10;
            Assert.AreEqual(3, b);
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
            Person p1 = new() { Address = "wang" };
            Person p2 = new() { Address = "li" };
            Person p3 = p1 + p2;
            Assert.AreEqual("wangli", p3.Address);
        }

        /// <summary>
        /// 字符串插值
        /// </summary>
        [TestMethod]
        public void String_FormattableString()
        {
            string a = "hello", b = "world";
            FormattableString c = $"{a}{b}";
            for (int i = 0; i < c.ArgumentCount; i++)
            {
                Console.WriteLine($"{i},{c.GetArgument(i)}");
            }
        }

        /// <summary>
        /// 字符串Substring
        /// </summary>
        [TestMethod]
        public void String_Substring()
        {
            string str = "hello world";
            // 常见用法:截取从0开始到5结束
            Console.WriteLine(str.Substring(0, 5));

            // 使用范围运算符
            Console.WriteLine(str[0..5]);

            Console.WriteLine(str[..5]);

            Console.WriteLine(str[6..11]);

            Console.WriteLine(str[^11..^6]);
        }

        public class Person
        {
            public string Address { get; set; }

            public static Person operator +(Person a, Person b)
            {
                Person person = new()
                {
                    Address = a.Address + b.Address
                };
                return person;
            }
        }
    }
}