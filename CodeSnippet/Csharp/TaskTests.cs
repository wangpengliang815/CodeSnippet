#define debug
namespace CodeSnippet.Csharp
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestCategory("Task")]
    [TestClass()]

    public class TaskTests
    {
        /// <summary>
        /// 同步方式
        /// </summary>
        [TestMethod]
        public void Task_Example_01()
        {
            Stopwatch Watch = new Stopwatch();

            int CountCharacters(int id, string address)
            {
                using var client = new WebClient
                {
#if debug
                    Proxy = new WebProxy("proxy1.bj.petrochina", 8080)
#endif
                };
                Console.WriteLine($"开始调用 id = {id}：{Watch.ElapsedMilliseconds} ms");
                var result = client.DownloadString(address);
                Console.WriteLine($"调用完成 id = {id}：{Watch.ElapsedMilliseconds} ms");

                return result.Length;
            }

            void ExtraOperation(int id)
            {
                // 通过拼接字符串进行一些相对耗时的操作
                StringBuilder stringBuilder = new StringBuilder();

                for (var i = 0; i < 6000; i++)
                {
                    stringBuilder.Append(id);
                }
                Console.WriteLine($"id = {id} 的 ExtraOperation 方法完成：{Watch.ElapsedMilliseconds} ms");
            }

            // 启动计时器
            Watch.Start();

            const string url1 = "https://www.baidu.com/";
            const string url2 = "https://cn.bing.com/";

            // 两次调用 CountCharacters 方法（下载某网站内容，并统计字符的个数）
            var result1 = CountCharacters(1, url1);
            var result2 = CountCharacters(2, url2);

            // 三次调用 ExtraOperation 方法（主要是通过拼接字符串达到耗时操作）
            for (var i = 0; i < 3; i++)
            {
                ExtraOperation(i + 1);
            }

            Console.WriteLine($"{url1} 的字符个数：{result1}");
            Console.WriteLine($"{url2} 的字符个数：{result2}");
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 异步方式
        /// </summary>
        [TestMethod]
        public void Task_Example_02()
        {
            Stopwatch Watch = new Stopwatch();

            async Task<int> CountCharactersAsync(int id, string address)
            {
                using var client = new WebClient
                {
#if debug
                    Proxy = new WebProxy("proxy1.bj.petrochina", 8080)
#endif
                };
                Console.WriteLine($"开始调用 id = {id}：{Watch.ElapsedMilliseconds} ms");
                var result = await client.DownloadStringTaskAsync(address);
                Console.WriteLine($"调用完成 id = {id}：{Watch.ElapsedMilliseconds} ms");

                return result.Length;
            }

            void ExtraOperation(int id)
            {
                // 通过拼接字符串进行一些相对耗时的操作
                StringBuilder stringBuilder = new StringBuilder();

                for (var i = 0; i < 6000; i++)
                {
                    stringBuilder.Append(id);
                }
                Console.WriteLine($"id = {id} 的 ExtraOperation 方法完成：{Watch.ElapsedMilliseconds} ms");
            }


            // 启动计时器
            Watch.Start();

            const string url1 = "https://www.baidu.com/";
            const string url2 = "https://cn.bing.com/";

            // 两次调用 CountCharacters 方法（下载某网站内容，并统计字符的个数）
            var t1 = CountCharactersAsync(1, url1);
            var t2 = CountCharactersAsync(2, url2);

            // 三次调用 ExtraOperation 方法（主要是通过拼接字符串达到耗时操作）
            for (var i = 0; i < 3; i++)
            {
                ExtraOperation(i + 1);
            }

            //控制台输出
            Console.WriteLine($"{url1} 的字符个数：{t1.Result}");
            Console.WriteLine($"{url2} 的字符个数：{t2.Result}");
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 创建Task
        /// </summary>
        [TestMethod]
        public void Task_Create_Example()
        {
            var t1 = new TaskFactory()
                .StartNew(() => Console.WriteLine("new TaskFactory().StartNew"));
            Assert.IsNotNull(t1);

            var t2 = Task.Factory
                .StartNew(() => Console.WriteLine("Task.Factory"));
            Assert.IsNotNull(t2);

            var t3 = new Task(() => Console.WriteLine("Task Constructor"));
            t3.Start();

            var t4 = Task.Run(() => Console.WriteLine("Task.Run"));

            Assert.IsNotNull(t4);
        }

        /// <summary>
        /// 同步Task
        /// </summary>
        [TestMethod]
        public void Task_RunSynchronously_Example()
        {
            var t1 = new Task(() => TaskMethod("t1"));
            t1.RunSynchronously();
            Console.WriteLine("主线程调用结束");

            static void TaskMethod(string taskName)
            {
                Console.WriteLine("Task {0} 运行在线程id为{1}的线程上。是否是线程池中线程？:{2}",
    taskName, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
            }

            Assert.IsNotNull(t1);
        }

        /// <summary>
        /// TaskCreationOptions
        /// </summary>
        [TestMethod]
        public void Task_TaskCreationOptions_Example()
        {
            var t1 = new Task(()=> TaskMethod("t1"), TaskCreationOptions.LongRunning);
            t1.Start();
            Console.WriteLine("主线程调用结束");

            static void TaskMethod(string taskName)
            {
                Console.WriteLine("Task {0} 运行在线程id为{1}的线程上。是否是线程池中线程？:{2}",
    taskName, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
            }

            Assert.IsNotNull(t1);
        }
    }
}
