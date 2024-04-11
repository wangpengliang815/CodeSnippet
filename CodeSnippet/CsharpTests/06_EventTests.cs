#define Example_02
namespace CodeSnippet.CsharpTests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestCategory("Event")]
    [TestClass()]
    public class EventTests
    {

#if Example_01
        [TestMethod]
        public void Example_01()
        {
            Pub pub = new();
            ServiceA serviceA = new();
            ServiceB serviceB = new();

            // 事件订阅
            pub.OpenEvent += serviceA.ServiceAOpen;
            pub.OpenEvent += serviceB.ServiceBOpen;

            // 调用函数触发事件
            pub.Open();
        }
#endif

#if Example_02
        [TestMethod]
        public void Example_02()
        {
            Pub pub = new();
            ServiceA serviceA = new();
            ServiceB serviceB = new();

            // 事件订阅
            pub.Open += serviceA.ServiceAOpen;
            pub.Open += serviceB.ServiceBOpen;

            // 调用函数触发事件
            pub.OpenConn();
        }
#endif
    }

#if Example_01
    /// <summary>
    /// 发布者
    /// </summary>
    public class Pub
    {
        // 定义事件所需委托
        public delegate void OpenEventHandler();

        // 使用委托类型定义事件
        public event OpenEventHandler OpenEvent;

        /// <summary>
        /// 定义事件触发的函数
        /// </summary>
        public void Open()
        {
            // 这个简单的修改可确保在检查空值和发送通知之间，如果一个不同的线程移除了所有OpenEvent订阅者，将不会引发NullReferenceException异常
            OpenEventHandler openEventHandler = OpenEvent;

            Console.WriteLine("总服务上线...");
            // 为确保有事件可用需要使用?.
            OpenEvent?.Invoke();
        }
    }

    /// <summary>
    /// 事件订阅者A
    /// </summary>
    public class ServiceA
    {
        public void ServiceAOpen()
        {
            Console.WriteLine("服务A已连接");
        }
    }

    /// <summary>
    /// 事件订阅者B
    /// </summary>
    public class ServiceB
    {
        public void ServiceBOpen()
        {
            Console.WriteLine("服务B已连接");
        }
    }
#endif

#if Example_02
    public class Pub
    {
        /// <summary>
        /// 定义OpenEventArgs，传递给Observer所感兴趣的信息
        /// </summary>
        public class OpenEventArgs
        {
            public OpenEventArgs()
            {

            }
        }

        public delegate void OpenEventHandler(object sender, OpenEventArgs e);

        public event OpenEventHandler Open;

        /// <summary>
        /// 提供继承自Pub的类重写，以便继承类拒绝其他对象对它的监视
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnOpen(OpenEventArgs e)
        {
            // 如果有对象注册
            Open?.Invoke(this, e);  //调用所有注册对象的方法
        }

        public void OpenConn()
        {
            Console.WriteLine("总服务上线...");
            OpenEventArgs e = new();
            OnOpen(e);
        }
    }

    public class ServiceA
    {
        public void ServiceAOpen(object sender, Pub.OpenEventArgs e)
        {
            Console.WriteLine("服务B已连接");
        }
    }

    public class ServiceB
    {
        public void ServiceBOpen(object sender, Pub.OpenEventArgs e)
        {
            Console.WriteLine("服务B已连接");
        }
    }

#endif
}

