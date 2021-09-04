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
        public void Task_Create_ExampleAsync()
        {
            //var t1 = new TaskFactory()
            //    .StartNew(() => Console.WriteLine("new TaskFactory().StartNew"));
            //Assert.IsNotNull(t1);

            //var t2 = Task.Factory
            //    .StartNew(() => Console.WriteLine("Task.Factory"));
            //Assert.IsNotNull(t2);

            //var t3 = new Task(() => Console.WriteLine("Task Constructor"));
            //t3.Start();

            //var t4 = Task.Run(() => Console.WriteLine("Task.Run"));

            //Assert.IsNotNull(t4);


            //Task.Run(Print);

            Task.Run(async () =>
            {
                await PrintAsync();
            });

            //await PrintAsync();

            Console.WriteLine("ok");
        }

        void Print()
        {
            Console.WriteLine($"Hello, 线程Id:{ Thread.CurrentThread.ManagedThreadId}");
        }

        async Task PrintAsync()
        {
            await Task.Run(() =>
            {
                Console.WriteLine($"Hello, 线程Id:{ Thread.CurrentThread.ManagedThreadId}");
            });
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
            var t1 = new Task(() => TaskMethod("t1"), TaskCreationOptions.LongRunning);
            t1.Start();
            Console.WriteLine("主线程调用结束");

            static void TaskMethod(string taskName)
            {
                Console.WriteLine("Task {0} 运行在线程id为{1}的线程上。是否是线程池中线程？:{2}",
    taskName, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
            }

            Assert.IsNotNull(t1);
        }

        /// <summary>
        /// 不关心结果返回Void
        /// </summary>
        [TestMethod]
        public void Task_Return_Void()
        {
            Console.WriteLine($"不关心结果,返回Void,--Start,线程Id:{Thread.CurrentThread.ManagedThreadId}");
            Print();
            Console.WriteLine($"不关心结果,返回Void,--End,线程Id:{Thread.CurrentThread.ManagedThreadId}");
            Assert.IsTrue(true);

            static async void Print()
            {
                await Task.Run(() =>
                {
                    Console.WriteLine($"Hello, 线程Id:{ Thread.CurrentThread.ManagedThreadId}");
                });
            }
        }

        /// <summary>
        /// 关心是否完成，返回 Task 类型
        /// </summary>
        [TestMethod]
        public void Task_Return_Task()
        {
            Console.WriteLine($"看电视中...,线程Id:{Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("突然停电，看下是不是跳闸了");
            var task = OpenMainsSwitch();
            Console.WriteLine($"没电了先玩会儿手机吧，线程Id为：{Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(100);
            // 等着电源开关被打开
            task.Wait();
            Console.WriteLine($"又有电了,继续看电视...,线程Id:{Thread.CurrentThread.ManagedThreadId}");
            Assert.IsTrue(true);


            static async Task OpenMainsSwitch()
            {
                Console.WriteLine($"准备打开电源开关，线程Id：{ Thread.CurrentThread.ManagedThreadId}");
                await Task.Run(() =>
                {
                    Console.WriteLine($"打开电源开关, 线程Id:{ Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(2000);
                });
                Console.WriteLine($"电源开关打开了，线程Id：{ Thread.CurrentThread.ManagedThreadId}");
            }
        }

        /// <summary>
        /// 不止关心是否执行完成，还要获取执行结果。返回 Task<TResult> 类型
        /// </summary>
        [TestMethod]
        public async Task Task_Return_TaskTAsync()
        {
            string message = $"Today is {DateTime.Today:D}\n" + "Today's hours of leisure: " + $"{await GetLeisureHoursAsync()}";
            Console.WriteLine(message);
            Assert.IsTrue(true);

            static async Task<int> GetLeisureHoursAsync()
            {
                DayOfWeek today = await Task.FromResult(DateTime.Now.DayOfWeek);

                int leisureHours =
                    today is DayOfWeek.Saturday || today is DayOfWeek.Sunday
                    ? 16 : 5;

                return leisureHours;
            }
        }

        /// <summary>
        /// 连续Task
        /// </summary>
        [TestMethod]
        public void Task_ContinueWith_Example()
        {
            Task<string> t1 = new Task<string>(
                () => TaskMethod1("t1"));
            Console.WriteLine("Task1-创建,状态为:{0}", t1.Status);
            t1.Start();
            Console.WriteLine("Task1-启动,状态为:{0}", t1.Status);
            Console.WriteLine(t1.Result);
            Console.WriteLine("Task1-完成,状态为:{0}", t1.Status);
            Task t2 = t1.ContinueWith(TaskMethod2);
            Console.WriteLine("Task2,状态为:{0}", t2.Status);

            static string TaskMethod1(string taskName)
            {
                var result = string.Format($"Task:{taskName} 运行在线程id:{ Thread.CurrentThread.ManagedThreadId}的线程上," +
                    $"是否是线程池中线程？:{Thread.CurrentThread.IsThreadPoolThread}");
                return result;
            }

            static void TaskMethod2(Task t)
            {
                Console.WriteLine($"TaskID:{ t.Id} 运行在线程id:{Thread.CurrentThread.ManagedThreadId}的线程上。是否是线程池中线程？:{Thread.CurrentThread.IsThreadPoolThread}");

            }
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 嵌套Task(非关联)
        /// </summary>
        [TestMethod]
        public void Task_NoRelevance_Example()
        {
            var pTask = Task.Factory.StartNew(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Childen task finished!");
                });
                Console.WriteLine("Parent task finished!");
            });
            pTask.Wait();
            Console.WriteLine("Flag");
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 嵌套Task(关联)
        /// </summary>
        [TestMethod]
        public void Task_Relevance_Example()
        {
            var pTask = Task.Factory.StartNew(() =>
            {
                Task.Factory.StartNew(() =>
                 {
                     Console.WriteLine("Childen task finished!");
                 }, TaskCreationOptions.AttachedToParent);
                Console.WriteLine("Parent task finished!");
            });
            pTask.Wait();
            Console.WriteLine("Flag");
            Assert.IsTrue(true);
        }

        /// <summary>
        /// Task取消单个Task
        /// </summary>
        [TestMethod]
        public void Task_CancellationToken_SingleTask_Example()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Task t1 = Task.Run(() =>
            {
                for (int i = 0; i < 30; i++)
                {
                    if (cts.Token.IsCancellationRequested)
                    {
                        cts.Token.ThrowIfCancellationRequested();
                    }
                    else
                    {
                        Thread.Sleep(500);
                        Console.WriteLine("任务t1,共执行30次,当前第{0}次", i + 1);
                    }
                }
            }, cts.Token);
            Thread.Sleep(2000);
            // 传达取消请求
            cts.Cancel();
            Console.WriteLine($"已停止,Status{t1.Status}");
            Assert.IsTrue(true);
        }

        /// <summary>
        /// Task取消多个Task
        /// </summary>
        [TestMethod]
        public void Task_CancellationToken_MultiTask_Example()
        {
            CancellationTokenSource cts1 = new CancellationTokenSource();
            CancellationTokenSource cts2 = new CancellationTokenSource();

            // 任何Task处于取消状态时其余也将取消
            CancellationTokenSource ctsCombine =
                 CancellationTokenSource.CreateLinkedTokenSource(cts1.Token, cts2.Token);
            Task t1 = Task.Run(() =>
            {
                for (int i = 0; i < 30; i++)
                {
                    if (!ctsCombine.IsCancellationRequested)
                    {
                        Thread.Sleep(500);
                        Console.WriteLine("任务t1,共执行30次,当前第{0}次", i + 1);
                    }
                }
            }, ctsCombine.Token);

            Task t2 = Task.Run(() =>
            {
                for (int i = 0; i < 30; i++)
                {
                    if (!ctsCombine.IsCancellationRequested)
                    {
                        Thread.Sleep(500);
                        Console.WriteLine("任务t2,共执行30次,当前第{0}次", i + 1);
                    }
                }
            }, ctsCombine.Token);
            Thread.Sleep(2000);
            cts1.Cancel();
            Console.WriteLine($"t1:Status_{t1.Status},t2:Status_{t2.Status}");
            Assert.IsTrue(true);
        }

        /// <summary>
        /// Task定时取消
        /// </summary>
        [TestMethod]
        public void Task_CancellationToken_Timing_Example()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(8000);
            Task t1 = Task.Run(() =>
            {
                for (int i = 0; i < 30; i++)
                {
                    if (cts.Token.IsCancellationRequested)
                    {
                        cts.Token.ThrowIfCancellationRequested();
                    }
                    else
                    {
                        Thread.Sleep(500);
                        Console.WriteLine("任务t1,共执行30次,当前第{0}次", i);
                    }
                }
            }, cts.Token);
            try
            {
                t1.Wait();
            }
            catch (AggregateException e)
            {
                foreach (var item in e.InnerExceptions)
                {
                    Console.WriteLine(item);
                }
            }
            Assert.IsTrue(true);
        }

        /// <summary>
        /// CancellationToken回调
        /// </summary>
        [TestMethod]
        public void Task_CancellationToken_Register_Example()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            var token = cts.Token;
            cts.CancelAfter(8000);
            token.Register(Callback);
            Task t1 = Task.Run(() =>
            {
                for (int i = 0; i < 30; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    else
                    {
                        Thread.Sleep(500);
                        Console.WriteLine("任务t1,共执行30次,当前第{0}次", i);
                    }
                }
            }, token);

            try
            {
                t1.Wait();
            }
            catch (AggregateException e)
            {
                foreach (var item in e.InnerExceptions)
                {
                    Console.WriteLine(item);
                }
            }
            static void Callback()
            {
                Console.WriteLine("Register登记的任务取消回调函数");
            }
            Assert.IsTrue(true);
        }
    }
}
