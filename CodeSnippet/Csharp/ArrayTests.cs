namespace CodeSnippet.Csharp
{
    using System;
    using System.Buffers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestCategory("Array")]
    [TestClass()]
    public class ArrayTests
    {
        public class Person
        {
            public string Name { get; set; }
        }

        /// <summary>
        /// ArrayDefinition
        /// </summary>
        [TestMethod]
        public void ArrayDefinition()
        {
            int[] arr1 = new int[3];
            int[] arr2 = new int[3] { 1, 2, 3 };
            int[] arr3 = { 1, 2, 3 };
            int[] arr4;
            arr4 = new int[3] { 1, 2, 3 };
            Assert.AreEqual(0, arr1[0]);
            Assert.AreEqual(1, arr2[0]);
            Assert.AreEqual(1, arr3[0]);
            Assert.AreEqual(1, arr4[0]);

            Person p1 = new Person { Name = "wang" };
            Person p2 = new Person { Name = "li" };
            Person[] persons = new Person[2] { p1, p2 };
            Assert.AreEqual("wang", persons[0].Name);
            Assert.AreEqual("li", persons[1].Name);
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

            // 显式将arr装换为数组
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
        [TestCategory("Unrealized")]
        [TestMethod]
        public void Enumerator()
        {
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 迭代器
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [TestCategory("Unrealized")]
        [TestMethod]
        public void Iterator()
        {
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 数组池
        /// </summary>
        /// <remarks>
        /// 如果需要多次创建/销毁数组,为了减少GC操作,可以通过ArrayPool类使用数组池
        /// </remarks>
        [TestMethod]
        [TestCategory("Unrealized")]
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
}