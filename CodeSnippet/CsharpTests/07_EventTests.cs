namespace CodeSnippet.CsharpTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System;

    [TestCategory("Event")]
    [TestClass()]
    public class EventTests
    {
        [TestMethod]
        public void Example()
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


        public class Pub
        {
            /// <summary>
            /// 自定义事件参数以EventArgs结尾
            /// </summary>
            public class OpenEventArgs
            {
                public OpenEventArgs()
                {

                }
            }

            // 标准事件模式委托名称以EventHandler结尾
            // 委托的原型定义：有一个 void 返回值，并接受两个输入参数：一个 Object 类型，一个 EventArgs 类型(或继承自EventArgs)
            public delegate void OpenEventHandler(object sender, OpenEventArgs e);

            // 事件的命名为委托去掉 EventHandler 之后剩余的部分
            public event OpenEventHandler Open;

            public void OpenConn()
            {
                Console.WriteLine("总服务上线...");
                OpenEventArgs e = new();
                Open?.Invoke(this, e);
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
    }
}

