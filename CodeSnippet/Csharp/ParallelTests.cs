namespace codeSnippet.Csharp
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestCategory("Parallel")]
    [TestClass()]
    public class ParallelTests
    {
        [TestMethod]
        public void Parallel_Example_01()
        {
            var watch = Stopwatch.StartNew();
            watch.Start();
            Run1();
            Run2();
            Run3();
            watch.Stop();
            Console.WriteLine("串行开发,总耗时{0}", watch.ElapsedMilliseconds);

            watch.Restart();

            Parallel.Invoke(Run1, Run2, Run3);
            watch.Stop();
            Console.WriteLine("并行开发,总耗时{0}", watch.ElapsedMilliseconds);

            static void Run1()
            {
                Console.WriteLine("Run1,我需要1s");
                Thread.Sleep(1000);
            }
            static void Run2()
            {
                Console.WriteLine("Run2,我需要2s");
                Thread.Sleep(2000);
            }
            static void Run3()
            {
                Console.WriteLine("Run3,我需要3s");
                Thread.Sleep(3000);
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Parallel_Example_02()
        {
            Console.WriteLine("主线程启动,线程ID:{0}", Thread.CurrentThread.ManagedThreadId);

            Parallel.Invoke(
                () => Run1("task1"),
                () => Run2("task2"),
                () => Run3("task3"));

            Console.WriteLine("主线程结束,线程ID:{0}", Thread.CurrentThread.ManagedThreadId);

            static void Run1(string taskName)
            {
                Console.WriteLine("任务名：{0}线程ID:{1}", taskName, Thread.CurrentThread.ManagedThreadId);
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine("a");
                }
            }
            static void Run2(string taskName)
            {
                Console.WriteLine("任务名：{0}线程ID:{1}", taskName, Thread.CurrentThread.ManagedThreadId);
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine("b");
                }
            }
            static void Run3(string taskName)
            {
                Console.WriteLine("任务名：{0}线程ID:{1}", taskName, Thread.CurrentThread.ManagedThreadId);
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine("c");
                }
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Parallel_Example_For()
        {
            for (int i = 0; i < 3; i++)
            {
                ConcurrentBag<int> bag = new ConcurrentBag<int>();
                var watch = Stopwatch.StartNew();
                watch.Start();

                for (int j = 0; j < 20000000; j++)
                {
                    bag.Add(i);
                }
                watch.Stop();
                Console.WriteLine("串行添加,总数20000000,耗时{0}", watch.ElapsedMilliseconds);
                watch.Restart();
                Parallel.For(0, 20000000, j =>
                {
                    bag.Add(j);
                });
                watch.Stop();
                Console.WriteLine("并行添加,总数20000000,耗时{0}", watch.ElapsedMilliseconds);
                Console.WriteLine("***********************************");
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Parallel_Example_ForEach()
        {
            ConcurrentBag<int> bag = new ConcurrentBag<int>();
            Parallel.For(0, 10, j =>
            {
                bag.Add(j);
            });
            Console.WriteLine("集合总数:{0}", bag.Count);
            Parallel.ForEach(bag, item =>
            {
                Console.WriteLine(item);
            });
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Parallel_Example_For_Break()
        {
            int maxCount = 1000;
            ConcurrentBag<int> bag = new ConcurrentBag<int>();
            var watch = Stopwatch.StartNew();
            watch.Start();
            Parallel.For(0, 2000, (j, state) =>
            {
                if (bag.Count == maxCount)
                {
                    state.Break();
                    //return是必须的,否则依旧会继续执行
                    return;
                }
                bag.Add(j);
            });
            watch.Stop();
            Console.WriteLine("集合元素个数{0}", bag.Count);
            Assert.AreEqual(maxCount, bag.Count);
        }

        [TestMethod]
        public void Parallel_Example_For_Stop()
        {
            int maxCount = 1000;
            ConcurrentBag<int> bag = new ConcurrentBag<int>();

            bag = new ConcurrentBag<int>();
            Parallel.For(0, 2000, (i, state) =>
            {
                if (bag.Count == maxCount)
                {
                    state.Stop();
                    return;
                }
                bag.Add(i);
            });
            Console.WriteLine("集合元素个数{0}", bag.Count);
            Assert.AreEqual(maxCount, bag.Count);
        }

        [TestMethod]
        public void Parallel_Example_Exception()
        {
            Console.WriteLine("主线程启动,线程ID:{0}", Thread.CurrentThread.ManagedThreadId);
            try
            {
                Parallel.Invoke(() => Run1("task1"), () => Run2("task2"), () => Run3("task3"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("主线程结束,线程ID:{0}", Thread.CurrentThread.ManagedThreadId);


            static void Run1(string taskName)
            {
                Console.WriteLine("任务名：{0}线程ID:{1}",
                    taskName, Thread.CurrentThread.ManagedThreadId);
                throw new Exception("Run1出现异常");
            }

            static void Run2(string taskName)
            {
                Console.WriteLine("任务名：{0}线程ID:{1}",
                    taskName, Thread.CurrentThread.ManagedThreadId);
            }

            static void Run3(string taskName)
            {
                Console.WriteLine("任务名：{0}线程ID:{1}",
                    taskName, Thread.CurrentThread.ManagedThreadId);
                throw new Exception("Run3出现异常");
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Parallel_Example_AggregateException()
        {
            Console.WriteLine("主线程启动,线程ID:{0}", Thread.CurrentThread.ManagedThreadId);
            try
            {
                Parallel.Invoke(() => Run1("task1"), () => Run2("task2"), () => Run3("task3"));
            }
            catch (AggregateException ex)
            {
                // AggregateException捕获并行产生的一组异常集合
                foreach (var item in ex.InnerExceptions)
                {
                    Console.WriteLine(item);
                }
            }
            static void Run1(string taskName)
            {
                Console.WriteLine("任务名：{0}线程ID:{1}",
                    taskName, Thread.CurrentThread.ManagedThreadId);
                throw new Exception("Run1出现异常");
            }

            static void Run2(string taskName)
            {
                Console.WriteLine("任务名：{0}线程ID:{1}",
                    taskName, Thread.CurrentThread.ManagedThreadId);
            }

            static void Run3(string taskName)
            {
                Console.WriteLine("任务名：{0}线程ID:{1}",
                    taskName, Thread.CurrentThread.ManagedThreadId);
                throw new Exception("Run3出现异常");
            }

            Console.WriteLine("主线程结束,线程ID:{0}", Thread.CurrentThread.ManagedThreadId);
            Assert.IsTrue(true);
        }

#if debug
        [TestMethod]
        public void Parallel_Example_Performance()
        {
            Stopwatch stopWatch = new Stopwatch();

            var obj = new object();
            long num = 0;

            stopWatch.Start();
            for (int i = 0; i < 10000; i++)
            {
                for (int j = 0; j < 60000; j++)
                {
                    num++;
                }
            }
            stopWatch.Stop();
            Console.WriteLine("for run " + stopWatch.ElapsedMilliseconds + " ms.");

            stopWatch.Reset();
            stopWatch.Start();
            Parallel.For(0, 10000, item =>
            {
                for (int j = 0; j < 60000; j++)
                {
                    lock (obj)
                    {
                        num++;
                    }
                }
            });
            stopWatch.Stop();
            Console.WriteLine("ParallelFor run " + stopWatch.ElapsedMilliseconds + " ms.");
            Assert.IsTrue(true);
        }
#endif
    }
}
