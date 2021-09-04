namespace CodeSnippet.Csharp
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System;
    using System.Buffers;
    using System.Collections.Generic;

    [TestCategory("Basics")]
    [TestClass()]
    public class BasicsTests
    {
        /// <summary>
        /// 变量定义
        /// </summary>
        [TestMethod()]
        public void VariableDefinition()
        {
            // 变量定义
            string name = "wang";
            int age = 25;
            DateTime time = new(1995, 8, 15);

            // 同时声明多个同类型变量
            string n1 = "wang",
                   n2 = "li";

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

            Assert.IsTrue(condition == 0);
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
                    Console.WriteLine(0);
                    break;
                case 1:
                    Console.WriteLine(1);
                    break;
                default:
                    Console.WriteLine("other value");
                    break;
            }
            Assert.IsTrue(condition == 0);
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
        /// 循环do while
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
            List<string> lists = new();
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
            Assert.IsTrue(true);
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
            //自动隐式类型转换
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
            //显式将double类型转换为int
            int b = (int)a;
            Assert.AreEqual(10, b);
        }

        /// <summary>
        /// ToString()
        /// </summary>
        [TestMethod]
        public void MethodConversion_ToString()
        {
            int a = 10;
            string s = a.ToString();
            Console.WriteLine(s);
            Assert.IsTrue(true);
        }

        /// <summary>
        /// int.Parse()
        /// </summary>
        /// <remarks>
        /// 用于将string类型参数转换为int,需要注意:string类型参数不能为null,并且也只能是各种整型,不能是浮点型
        /// </remarks>
        [TestMethod]
        public void MethodConversion_IntParse()
        {
            //正常
            Assert.AreEqual(2, int.Parse("2"));
#if debug
            //错误:输入字符串格式错误
            int.Parse("2.6");
            //错误:值不能为null
            int.Parse(null);
#endif
        }

        /// <summary>
        /// int.TryParse()
        /// </summary>
        [TestMethod]
        public void MethodConversion_IntTryParse()
        {
            //正常 i=2
            Assert.AreEqual(true, int.TryParse("2", out int i));
            //转换失败,false
            Assert.AreEqual(false, int.TryParse("2.6", out int j));
            //转换失败,false
            Assert.AreEqual(false, int.TryParse(null, out int k));

            Console.WriteLine($"{i},{j},{k}");
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
            List<string> a = new() { "1", "2" };
            List<string> b = new() { "1", "2" };
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
            List<string> a = new() { "1", "2" };
            List<string> b = new() { "1", "2" };
            string c = "1";
            string d = "1";
            Assert.AreEqual(false, Equals(a, b));
            Assert.AreEqual(true, Equals(c, d));
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
            List<string> a = new() { "212", "2212" };
            List<string> b = new() { "14343", "2443" };
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
            List<string> c = new() { "212", "2212" };
            List<string> d = new() { "14343", "2443" };
            string e = "21";
            string f = "21";
            Assert.AreEqual(true, a == b);
            Assert.AreEqual(false, c == d);
            Assert.AreEqual(true, e == f);
        }

        /// <summary>
        /// 类的定义
        /// </summary>
        [TestMethod]
        public void ClassDefinition()
        {
            Person p = new()
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
            Person p = new();
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
            ClassA classA = new() { Age = 1 };
            ClassA.ChangeAge(classA);
            // output=100,TestA是类按引用传递
            Console.WriteLine(classA.Age);
            Assert.AreEqual(100, classA.Age);

            StructA structA = new() { Age = 1 };
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
        public void ParameterPassing_Ref()
        {
            StructB structB = new() { Age = 1 };
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
        public void Operator_Ternary()
        {
            int i = 0;
            int j = i == 0 ? 10 : 20;
            Assert.AreEqual(10, j);
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
        /// is
        /// </summary>
        [TestMethod]
        public void Operator_Is()
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
        /// sizeof
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
        /// typeof
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
        /// nameof
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
        /// ??
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
        public void Operator_Empty()
        {
            Person p = new Person();
            p.Print(p);
            Assert.AreEqual(null, p.Address);
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
        /// ArrayDefinition
        /// </summary>
        [TestMethod]
        public void ArrayDefinition()
        {
            int[] arr1 = new int[3];
            Assert.AreEqual(0, arr1[0]);

            int[] arr2 = new int[3] { 1, 2, 3 };
            Assert.AreEqual(1, arr2[0]);

            int[] arr3 = { 1, 2, 3 };
            Assert.AreEqual(1, arr3[0]);

            int[] arr4;
            arr4 = new int[3] { 1, 2, 3 };
            Assert.AreEqual(1, arr4[0]);

            Person p1 = new() { Address = "wang" };
            Person p2 = new() { Address = "li" };
            Person[] persons = new Person[2] { p1, p2 };
            Assert.AreEqual("wang", persons[0].Address);
            Assert.AreEqual("li", persons[1].Address);
        }

        /// <summary>
        /// 多维数组
        /// </summary>
        [TestMethod]
        public void MultidimensionalArray()
        {
            // 下面的两种创建方式等价
            // 第一种
            int[,] intArray1 = { { 1, 1 }, { 1, 2 }, { 1, 3 } };

            //第二种
            int[,] intArray2 = new int[3, 4]{    /*  初始化化一个三行四列的数组 */
                   {0, 1, 2, 3} ,                /*  初始化索引号为 0 的行 */
                   {4, 5, 6, 7} ,                /*  初始化索引号为 1 的行 */
                   {8, 9, 10, 11}                /*  初始化索引号为 2 的行 */
                };
            //获取数组中第3行第4个元素                                
            Console.WriteLine("二维数组中的元素是通过使用下标（即数组的行索引和列索引）来访问,值为：{0}", intArray2[2, 3]);
            Console.WriteLine("属性:Length:返回数组中元素的总数,值为：{0}", intArray1.Length);
            Assert.AreEqual(11, intArray2[2, 3]);
        }

        /// <summary>
        /// 锯齿数组
        /// </summary>
        [TestMethod]
        public void JaggedArray()
        {
            int[][] intJaggedArray = new int[2][];
            intJaggedArray[0] = new int[3] { 1, 2, 3 };
            intJaggedArray[1] = new int[2] { 4, 5 };

            Assert.AreEqual(1, intJaggedArray[0][0]);
            Assert.AreEqual(3, intJaggedArray[0][2]);
            Assert.AreEqual(5, intJaggedArray[1][1]);
        }

        /// <summary>
        /// Array
        /// </summary>
        /// <remarks>
        /// 抽象类,无法使用构造函数创建
        /// </remarks>
        [TestMethod]
        public void ArrayClass()
        {
            Array arr = Array.CreateInstance(typeof(int), 3);
            for (int i = 0; i < 3; i++)
            {
                // 第一个参数是value,第二个参数是index
                arr.SetValue(i, i);
            }

            for (int i = 0; i < arr.Length; i++)
            {
                Assert.AreEqual(i, arr.GetValue(i));
            }

            // 显式将arr转换为数组
            int[] arr2 = (int[])arr;

            for (int i = 0; i < arr2.Length; i++)
            {
                Assert.AreEqual(i, arr2[i]);
            }
        }

        /// <summary>
        /// Array
        /// </summary>
        /// <remarks>
        /// 数组是引用类型,将数组变量赋予另一个数组变量,会得到两个引用同一数组的变量
        /// 复制数组,会使数组实现ICloneable接口
        /// 如果数组元素是值类型,会复制所有制
        /// 如果数组元素包含引用类型,则不复制元素则只复制引用
        /// </remarks>
        [TestMethod]
        public void ArrayClass_Clone()
        {
            int[] arr1 = new int[3] { 1, 2, 3 };
            int[] arr2 = (int[])arr1.Clone();
            Assert.AreEqual(1, arr2[0]);
        }

        /// <summary>
        /// 枚举器
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [TestMethod]
        public void Enumerator()
        {
            // TODO:枚举器的实现
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 迭代器
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [TestMethod]
        public void Iterator()
        {
            // TODO:迭代器实现
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 数组池
        /// </summary>
        /// <remarks>
        /// 如果需要多次创建/销毁数组,为了减少GC操作,可以通过ArrayPool类使用数组池
        /// </remarks>
        [TestMethod]
        public void ArrayPool()
        {
            // maxArrayLengthDefaultValue: 1024 * 1024
            // maxArraysPerBucketDefaultValue: 50
            ArrayPool<int> arrayPool = ArrayPool<int>
                .Create(maxArrayLength: 4000, maxArraysPerBucket: 10);

            // 使用预定义的共享池
            ArrayPool<int> sharePool = ArrayPool<int>.Shared;
            Console.WriteLine($"{arrayPool},{sharePool}");
            Assert.IsTrue(sharePool != null);
        }
    }

    public class Person
    {
        // 实例成员：字段
        public string _name;
        private int _age;

        // 实例成员：属性
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

        // 实例成员：只读字段
        public readonly string readonlyValue;

        // 实例成员：自动实现的属性
        public string Address { get; set; }

        // 实例成员：只读属性
        public DateTime Birthday { get; }

        // 静态成员：只读字段
        public readonly static string staticReadonlyValue;

        // 静态成员：字段
        public static string _describe = "static field";

        // 可空类型：指可以为空的值类型
        public int? height;

        // 方法
        public void PrintAge()
        {
            Console.WriteLine(_age);
        }

        // 如果方法实现只有一条语句,可以使用表达式体方法定义
        public void PrintName() => Console.WriteLine(_name);

        // 方法重载
        public void Print(string message) => Console.WriteLine(message);

        public void Print(string name
            , string messae) => Console.WriteLine($"name:{name},message:{messae}");

        public void Print(Person person)
        {
            // person为null返回null
            Address = person?.Address;
        }

        // 可选参数
        public void Print(string name
            , string message
            , string address
            , int age = 100) => Console.WriteLine($"{name},{message},{address},{age}");

        // 个数可变的参数Prams[]
        public void Print(string name
            , params string[] args)
        {
            Console.WriteLine($"name:{name}");
            foreach (var item in args)
            {
                Console.WriteLine(item);
            }
        }

        // out参数返回不同类型多个值
        public string Print(string name
            , out bool result)
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
}