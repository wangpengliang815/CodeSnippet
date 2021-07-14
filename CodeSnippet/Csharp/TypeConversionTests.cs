namespace codeSnippet.Csharp
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestCategory("TypeConversion")]
    [TestClass()]
    public class TypeConversionTests
    {
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
            double b = a;//自动隐式类型转换
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
            int b = (int)a; //显式将double类型转换为int
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
        public void MethodConversion_intParse()
        {
            Assert.AreEqual(2, int.Parse("2"));//正常
            // int.Parse("2.6"); //错误:输入字符串格式错误
            // int.Parse(null);  //错误:值不能为null
        }

        /// <summary>
        /// int.TryParse()
        /// </summary>
        [TestMethod]
        public void MethodConversion_intTryParse()
        {
            Assert.AreEqual(true, int.TryParse("2", out int i));//正常 i=2
            Assert.AreEqual(false, int.TryParse("2.6", out int j));//转换失败,false
            Assert.AreEqual(false, int.TryParse(null, out int k));//转换失败,false
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
            List<string> a = new List<string> { "1", "2" };
            List<string> b = new List<string> { "1", "2" };
            string c = "1";
            string d = "1";
            Assert.AreEqual(true, ReferenceEquals(null, null));
            Assert.AreEqual(false, ReferenceEquals(a, b));
            Assert.AreEqual(true, ReferenceEquals(c, d)); // 字符串驻留池：对于相同的字符串，CLR不会为其分别分配内存，而是共享同一内存。
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
            List<string> a = new List<string> { "1", "2" };
            List<string> b = new List<string> { "1", "2" };
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
            List<string> a = new List<string> { "212", "2212" };
            List<string> b = new List<string> { "14343", "2443" };
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
            List<string> c = new List<string> { "212", "2212" };
            List<string> d = new List<string> { "14343", "2443" };
            string e = "21";
            string f = "21";
            Assert.AreEqual(true, a == b);
            Assert.AreEqual(false, c == d);
            Assert.AreEqual(true, e == f);
        }
    }
}
