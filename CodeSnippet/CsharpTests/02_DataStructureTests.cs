using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Buffers;
using System.Collections;

/// <summary>
/// 数据结构
/// </summary>
namespace CodeSnippet.CsharpTests.DataStructureTests
{
    [TestCategory("DataStructureTests")]
    [TestClass()]
    public class DataStructureTests
    {
        /// <summary>
        /// 数组定义
        /// </summary>
        [TestMethod]
        public void ArrayDefinition()
        {
            // 数组可以先声明后赋值,也可以声明同时赋值,下面的方式是等价的,数组中必须存储同一类型数据,这在数组被定义时就已经确定
            // 第一种
            int[] intArr = new int[5];
            intArr[0] = 1;
            intArr[1] = 2;
            intArr[2] = 3;
            intArr[3] = 4;
            intArr[4] = 5;

            // 第二种
            int[] intArr2 = new int[5] { 1, 2, 3, 4, 5 };

            // 第三种
            int[] intArr3 = { 1, 2, 3, 4, 5 };

            // 注意这里是维度不是索引
            Console.WriteLine("GetLength(int维度):返回指定维度中的元素数,值为：{0}", intArr.GetLength(0));
            Console.WriteLine("GetLowerBound(int维度):返回指定维度的最低索引,值为：{0}", intArr.GetLowerBound(0));
            Console.WriteLine("GetUpperBound(int维度):返回指定维度的最高索引,值为：{0}", intArr.GetUpperBound(0));
            Console.WriteLine("GetValue(int index):	返回指定索引处的值,值为：{0}", intArr.GetValue(2));
            Console.WriteLine("属性:Length:返回数组中元素的总数,值为：{0}", intArr.Length);

            // 使用索引来访问数组元素
            Console.WriteLine("使用索引访问数组元素,索引为2的值为：{0}", intArr[2]);


            // 试图访问数组中不存在的索引元素,会发生数组越界
            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                Console.WriteLine("使用索引访问数组元素,索引为10的值为：{0}", intArr[10]);
            });


            for (int i = 0; i < intArr.Length; i++)
            {
                Console.WriteLine(intArr[i]);
            }

            foreach (var item in intArr)
            {
                Console.WriteLine(item);
            }

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
            int[,] intArray2 = new int[3, 4]
            {    /*  初始化化一个三行四列的数组 */
                   {0, 1, 2, 3} ,                /*  初始化索引号为 0 的行 */
                   {4, 5, 6, 7} ,                /*  初始化索引号为 1 的行 */
                   {8, 9, 10, 11}                /*  初始化索引号为 2 的行 */
            };
            // 获取数组中第3行第4个元素                                
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
            Console.WriteLine("IsFixedSize:数组是否带有固定大小,值为：{0}", arr.IsFixedSize);
            Console.WriteLine("IsReadOnly :数组是否只读,值为：{0}", arr.IsReadOnly);
            Console.WriteLine("Length     :32位整数,数组元素总数,值为：{0}", arr.Length);
            Console.WriteLine("LongLength :64位整数,数组元素总数,值为：{0}", arr.LongLength);
            Console.WriteLine("Rank       :数组的维度,值为：{0}", arr.Rank);

            Array.Clear(arr, 0, 3);
            for (int i = 0; i < arr.Length; i++)
            {
                Assert.AreEqual(0, arr.GetValue(i));
            }

            // 显式将arr转换为数组
            int[] arr2 = (int[])arr;

            for (int i = 0; i < arr2.Length; i++)
            {
                Assert.AreEqual(0, arr2[i]);
            }
        }

        /// <summary>
        /// ArrayList
        /// </summary>
        [TestMethod]
        public void ArrayList()
        {
            ArrayList arrayList = new();
            arrayList.Add("wang");
            arrayList.Add(1);
            // ArrayList允许插入null
            arrayList.Add(null);

            foreach (var item in arrayList)
            {
                Console.WriteLine(item);
            }
            Assert.AreEqual("wang", arrayList[0]);
        }

        /// <summary>
        /// SortedList
        /// </summary>
        [TestMethod]
        public void SortedList()
        {
            SortedList sortedList = new();
            sortedList.Add(2, "wang");
            sortedList.Add(5, "li");
            sortedList.Add(3, 5);

            // SortedList键可以是任何数据类型，但不能在同一SortedList中添加不同数据类型的键。
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                sortedList.Add("wang", 32);
            });


            for (int i = 0; i < sortedList.Count; i++)
            {
                Console.WriteLine("key:{0}，value:{1}", sortedList.GetKey(i), sortedList.GetByIndex(i));
            }

            foreach (DictionaryEntry item in sortedList)
            {
                Console.WriteLine("key:{0}，value:{1}", item.Key, item.Value);
            }
        }

        /// <summary>
        /// Stack(后入先出)
        /// </summary>
        [TestMethod]
        public void Stack()
        {
            Stack stack = new();
            stack.Push("1");
            stack.Push(1);
            stack.Push(false);

            foreach (var item in stack)
            {
                Console.WriteLine(item);
            }
        }

        /// <summary>
        /// Queue(先入先出)
        /// </summary>
        [TestMethod]
        public void Queue()
        {
            Queue queue = new();
            queue.Enqueue("1");
            queue.Enqueue(1);
            queue.Enqueue(false);

            foreach (var item in queue)
            {
                Console.WriteLine(item);
            }
        }

        /// <summary>
        /// Hashtables:通过计算每个密钥的哈希码来优化查找，并在内部将其存储在不同的存储桶中，然后在访问值时匹配指定密钥的哈希码
        /// </summary>
        [TestMethod]
        public void Hashtable()
        {
            Hashtable hashtable = new()
            {
                { 1, "wang" },
                { 3, false },
                { 2, "li" }
            };

            foreach (DictionaryEntry item in hashtable)
            {
                Console.WriteLine("key:{0}, value:{1}", item.Key, item.Value);
            }
        }

        [TestMethod]
        public void BitArray()
        {
            BitArray bitArray = new(5);
            for (int i = 0; i < bitArray.Count; i++)
            {
                bitArray.Set(i, i % 2 == 0);
            }
            foreach (var item in bitArray)
            {
                Console.WriteLine(item);
            }
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
        /// 索引器
        /// </summary>
        [TestMethod]
        public void Indexer()
        {
            // TODO:索引器实现
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
        public string Address { get; set; }
    }
}
