namespace ThreadTests
{
    using System;
    using System.Threading;

    static class Program
    {
        static void Main(string[] args)
        {
            Thread_Contention_Example_02();
        }

        /// <summary>
        /// 线程创建
        /// </summary>
        public static void ThreadCreate_Basic()
        {
            static void ThreadMethod()
            {
                Thread.Sleep(2000);
                Console.WriteLine("子线程结束");
            }

            Console.WriteLine("程序在启动时创建一个线程,称为主线程");
            //创建线程
            Thread t = new Thread(ThreadMethod);
            //启动线程
            t.Start();
            Console.WriteLine("主线程结束");
        }

        /// <summary>
        /// 线程创建(lambda)
        /// </summary>
        public static void ThreadCreate_Lambda()
        {
            Console.WriteLine("程序在启动时创建一个线程,称为主线程");
            //创建线程
            Thread t = new Thread(() =>
            {
                Thread.Sleep(2000);
                Console.WriteLine("子线程结束");
            });

            t.Start();
            Console.WriteLine("主线程结束");
        }

        /// <summary>
        /// 线程调用顺序
        /// </summary>
        /// <remarks>
        /// 观察输出结果说明
        /// 线程是由操作系统调度的,每次哪个线程先被执行不确定,线程的调度是无序的
        ///</remarks>
        public static void Thread_Order()
        {
            Console.WriteLine("程序在启动时创建一个线程,称为主线程");

            Thread t = new Thread(() => Console.WriteLine("A"));
            Thread t1 = new Thread(() => Console.WriteLine("B"));
            Thread t2 = new Thread(() => Console.WriteLine("C"));
            Thread t3 = new Thread(() => Console.WriteLine("D"));
            t.Start();
            t1.Start();
            t2.Start();
            t3.Start();
            Console.WriteLine("主线程结束");
        }

        /// <summary>
        /// 线程参数传递(不带参数)
        /// </summary>
        public static void ThreadParameterPassing_Null()
        {
            Thread t = new Thread(() => Console.WriteLine("ThreadMethod"));
            t.Start();
        }

        /// <summary>
        /// 线程参数传递(一个参数)
        /// </summary>
        public static void ThreadParameterPassing_One()
        {
            Thread t = new Thread((object message) => Console.WriteLine(message));
            t.Start();
        }

        /// <summary>
        /// 前台线程阻止进程的关闭
        /// </summary>
        public static void Thread_Foreground()
        {
            Thread thread = new Thread(() =>
            {
                Thread.Sleep(5000);
                Console.WriteLine("前台线程执行");
            });
            thread.Start();
            Console.WriteLine("主线程执行完毕");
        }

        /// <summary>
        /// 后台线程不阻止进程的关闭
        /// </summary>
        public static void Thread_Background()
        {
            Thread thread = new Thread(() =>
            {
                Thread.Sleep(5000);
                Console.WriteLine("前台线程执行");
            })
            { IsBackground = true };

            thread.Start();
            Console.WriteLine("主线程执行完毕");
        }

        /// <summary>
        /// 线程优先级
        /// </summary>
        /// <remarks>
        /// 设置优先级并不是会指定线程固定执行的顺序
        /// 设置线程优先级只是提高了线程被调用的概率
        /// 并不是定义CPU调用线程的顺序，具体还是要由操作系统内部来调度
        /// </remarks>
        public static void Thread_Priority()
        {
            Thread normal = new Thread(() =>
            {
                Console.WriteLine("优先级为正常线程");
            });
            normal.Start();

            Thread aboseNormal = new Thread(() =>
            {
                Console.WriteLine("优先级为高于正常线程");
            })
            { Priority = ThreadPriority.AboveNormal };
            aboseNormal.Start();

            Thread belowNormal = new Thread(() =>
            {
                Console.WriteLine("优先级为低于正常线程");
            })
            { Priority = ThreadPriority.BelowNormal };
            belowNormal.Start();

            Thread highest = new Thread(() =>
            {
                Console.WriteLine("优先级最高线程");
            })
            { Priority = ThreadPriority.Highest };
            highest.Start();

            Thread lowest = new Thread(() =>
            {
                Console.WriteLine("优先级最低线程");
            })
            { Priority = ThreadPriority.Lowest };
            lowest.Start();

            Console.WriteLine("主线程执行完毕");
        }

        /// <summary>
        /// 线程控制(暂停线程Sleep)
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static void ThreadControl_Sleep()
        {
            static void ThreadMethod()
            {
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("a");
                }
            }

            static void ThreadMethodSleep()
            {
                for (int i = 0; i < 10; i++)
                {
                    //暂停2s
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                    Console.WriteLine("b");
                }
            }
            Thread t = new Thread(ThreadMethodSleep);
            t.Start();
            ThreadMethod();
        }

        /// <summary>
        /// 线程控制(线程等待Join)
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static void ThreadControl_Join()
        {
            static void ThreadMethod()
            {
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("a");
                }
            }

            static void ThreadMethodSleep()
            {
                for (int i = 0; i < 10; i++)
                {
                    //暂停2s
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                    Console.WriteLine("b");
                }
            }
            Thread t = new Thread(ThreadMethodSleep);
            t.Start();
            t.Join();
            ThreadMethod();
        }

        /// <summary>
        /// 线程控制(终止线程Join)
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static void ThreadControl_Abort()
        {
            static void ThreadMethodSleep()
            {
                for (int i = 0; i < 20; i++)
                {
                    //暂停2s
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                    Console.WriteLine("b");
                }
            }

            Thread t = new Thread(ThreadMethodSleep);
            t.Start();
            Thread.Sleep(TimeSpan.FromSeconds(5));
#pragma warning disable SYSLIB0006 // 类型或成员已过时
            t.Abort();
#pragma warning restore SYSLIB0006 // 类型或成员已过时
        }

        /// <summary>
        /// 线程状态监测
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static void Thread_Status()
        {
            static void ThreadMethodSleep()
            {
                for (int i = 0; i < 3; i++)
                {
                    //暂停2s
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                    Console.WriteLine("b");
                }
            }

            Thread t = new Thread(ThreadMethodSleep);
            Console.WriteLine("创建线程,线程状态:{0}", t.ThreadState);
            t.Start();
            Console.WriteLine("线程调用Start()方法,线程状态:{0}", t.ThreadState);
            Thread.Sleep(TimeSpan.FromSeconds(5));
            Console.WriteLine("线程休眠5s,线程状态:{0}", t.ThreadState);
            t.Join();
            Console.WriteLine("等待线程结束,线程状态:{0}", t.ThreadState);
        }

        /// <summary>
        /// 线程异常处理
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static void Thread_Exception()
        {
            // 没有对异常进行处理
            static void ThreadMethodA()
            {
                throw new AggregateException("AError");
            }

            // 对异常进行处理
            static void ThreadMethodB()
            {
                try
                {
                    throw new AggregateException("BError");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:{0}", ex.Message);
                }
            }

            Thread t = new Thread(ThreadMethodB);
            t.Start();
            t.Join();
            try
            {
                Thread t1 = new Thread(ThreadMethodA);
                t1.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:{0}", e.Message);
            }
        }

        /// <summary>
        /// 线程池(ThreadPool)
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static void Thread_ThreadPool()
        {
            static void AddThread(object e)
            {
                Console.WriteLine("当前线程ID:{0}", Thread.CurrentThread.ManagedThreadId);
            }

            ThreadPool.GetMaxThreads(out int workThread, out int ioThread);
            Console.WriteLine("工作线程数:{0},io线程数{1}", workThread, ioThread);

            for (int i = 0; i < 5; i++)
            {
                ThreadPool.QueueUserWorkItem(AddThread);
                workThread = ioThread = 0;
            }
            Console.WriteLine($"{workThread},{ioThread}");
        }

        /// <summary>
        /// 多线程争用条件
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static void Thread_Contention_Example_01()
        {
            StateObject m = new StateObject();
            Thread t1 = new Thread(ChangeState);
            t1.Start(m);
            Console.ReadKey();

            static void ChangeState(object o)
            {
                StateObject m = o as StateObject;
                while (true)
                {
                    m.ChangeState();
                }
            }
        }

        /// <summary>
        /// 多线程争用条件
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static void Thread_Contention_Example_02()
        {
            StateObject m = new StateObject();
            Thread t1 = new Thread(ChangeState);
            Thread t2 = new Thread(ChangeState);
            t1.Start(m);
            t2.Start(m);
            Console.ReadKey();

            static void ChangeState(object o)
            {
                StateObject m = o as StateObject;
                while (true)
                {
                    m.ChangeState();
                }
            }
        }

        /// <summary>
        /// 多线程死锁
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static void Thread_Deadly_Example_01()
        {
            Thread t1 = new Thread(Deadlock.DeadlockA);
            Thread t2 = new Thread(Deadlock.DeadlockB);
            t1.Start("t1");
            t2.Start("t2");
        }

        /// <summary>
        /// 线程同步(Lock)
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static void ThreadSync_Lock_Example_01()
        {
            StateObject m = new StateObject();
            Thread t1 = new Thread(ChangeState);
            t1.Start(m);
            Console.ReadKey();

            static void ChangeState(object o)
            {
                StateObject m = o as StateObject;
                while (true)
                {
                    //给变量m加锁
                    lock (m)
                    {
                        m.ChangeState();
                    }
                }
            }
        }

        /// <summary>
        /// 线程同步(Lock)
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static void ThreadSync_Lock_Example_02()
        {
            StateObject m = new StateObject();
            Thread t1 = new Thread(ChangeState);
            t1.Start(m);
            Console.ReadKey();

            static void ChangeState(object o)
            {
                object syncRoot = new object();

                StateObject m = o as StateObject;
                while (true)
                {
                    lock (syncRoot)
                    {
                        m.ChangeState();
                    }
                }
            }
        }

        /// <summary>
        /// 线程同步(Monitors)
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static void ThreadSync_Monitors_Example_01()
        {
            StateObject m = new StateObject();
            Thread t1 = new Thread(ChangeState);
            t1.Start(m);
            Console.ReadKey();

            static void ChangeState(object o)
            {
                object syncRoot = new object();

                StateObject m = o as StateObject;
                while (true)
                {
                    Monitor.Enter(syncRoot);
                    try
                    {
                        m.ChangeState();
                    }
                    finally
                    {
                        Monitor.Exit(syncRoot);
                    }
                }
            }
        }

        /// <summary>
        /// 线程同步(Mutex)
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static void ThreadSync_Mutex_Example_01()
        {
            for (int i = 0; i < 3; i++)
            {
                //在不同的线程中调用受互斥锁保护的方法
                Thread test = new Thread(MutexMethod);
                test.Start();
            }
            Console.Read();

            static void MutexMethod()
            {
                //实例化一个互斥锁
                Mutex mutex = new Mutex();

                Console.WriteLine("{0} 请求获取互斥锁", Thread.CurrentThread.ManagedThreadId);
                mutex.WaitOne();
                Console.WriteLine("{0} 已获取到互斥锁", Thread.CurrentThread.ManagedThreadId);
                Console.WriteLine("{0} 准备释放互斥锁", Thread.CurrentThread.ManagedThreadId);
                // 释放互斥锁
                mutex.ReleaseMutex();
                Console.WriteLine("{0} 已经释放互斥锁", Thread.CurrentThread.ManagedThreadId);
            }
        }
    }

    class StateObject
    {
        private int state = 5;

        public void ChangeState()
        {
            state++;
            if (state == 5)
            {
                Console.WriteLine("value=5");
            }
            state = 5;
        }
    }

    static class Deadlock
    {
        static readonly StateObject o1 = new StateObject();
        static readonly StateObject o2 = new StateObject();

        public static void DeadlockA(object o)
        {
            lock (o1)
            {
                Console.WriteLine("我是线程{0},我锁定了对象o1", o);
                lock (o2)
                {
                    Console.WriteLine("我是线程{0},我锁定了对象o2", o);
                }
            }
        }

        public static void DeadlockB(object o)
        {
            lock (o2)
            {
                Console.WriteLine("我是线程{0},我锁定了对象o2", o);
                lock (o1)
                {
                    Console.WriteLine("我是线程{0},我锁定了对象o1", o);
                }
            }
        }
    }
}

