#define Example_03
namespace CodeSnippet.Csharp
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
            Player player = new Player();
            player.Die();
            Assert.IsTrue(true);
        }
#endif

#if Example_02
        [TestMethod]
        public void Example_02()
        {
            Heater heater = new Heater();
            Alarm alarm = new Alarm();
            // 注册方法
            heater.BoilEvent += alarm.Alert;
            //heater.BoilEvent += (new Alarm()).MakeAlert; // 匿名对象注册方法
            // 注册静态方法
            heater.BoilEvent += Display.ShowMsg;
            // 烧水，会自动调用注册过对象的方法
            heater.BoilWater();
            Assert.IsTrue(true);
        }
#endif

#if Example_03
        [TestMethod]
        public void Example_03()
        {
            Heater heater = new Heater();
            Alarm alarm = new Alarm();
            heater.Boiled += alarm.MakeAlert;   //注册方法
            //heater.Boiled += (new Alarm()).MakeAlert;      //给匿名对象注册方法
            //heater.Boiled += new Heater.BoiledEventHandler(alarm.MakeAlert);    //也可以这么注册
            heater.Boiled += Display.ShowMsg;       //注册静态方法
            heater.BoilWater();   //烧水，会自动调用注册过对象的方法
            Assert.IsTrue(true);
        }
#endif
    }

#if Example_01
    public class Player
    {
        public void Die()
        {
            GameConfiguration configuration = new GameConfiguration();
            configuration.OnPlayerDeath();
            UI ui = new UI();
            ui.OnPlayerDeath();
        }
    }

    public class UI
    {
        public void OnPlayerDeath()
        {
            Console.Write("GameOver");
        }
    }

    public class GameConfiguration
    {
        public int DeathNumber { get; set; }

        public void OnPlayerDeath()
        {
            DeathNumber++;
            Console.WriteLine(DeathNumber);
        }
    }
#endif

#if Example_02
    /// <summary>
    /// 热水器
    /// </summary>
    public class Heater
    {
        /// <summary>
        /// 温度字段
        /// </summary>
        private int temperature;

        /// <summary>
        /// 事件委托
        /// </summary>
        /// <param name="param"></param>
        public delegate void BoilHandler(int param);

        /// <summary>
        /// 将委托封装，并对外公布订阅和取消订阅的接口
        /// </summary>
        public event BoilHandler BoilEvent;

        public void BoilWater()
        {
            for (int i = 0; i <= 100; i++)
            {
                temperature = i;
                if (temperature > 95)
                {
                    // 调用所有注册对象的方法
                    BoilEvent?.Invoke(temperature);
                }
            }
        }
    }

    /// <summary>
    /// 警报器
    /// </summary>
    public class Alarm
    {
        public void Alert(int param)
        {
            Console.WriteLine("Alarm：dddddddd，水已经 {0} 度了：", param);
        }
    }

    /// <summary>
    /// 显示器
    /// </summary>
    public static class Display
    {
        public static void ShowMsg(int param)
        {
            Console.WriteLine("Display：水已烧开，当前温度：{0}度。", param);
        }
    }
#endif

#if Example_03
    public class Heater
    {
        /// <summary>
        /// 温度
        /// </summary>
        private int temperature;

        /// <summary>
        /// 添加型号作为演示
        /// </summary>
        public string type = "01";

        /// <summary>
        /// 添加产地作为演示
        /// </summary>
        public string area = "China";

        public delegate void BoiledEventHandler(object sender, BoiledEventArgs e);

        public event BoiledEventHandler Boiled;

        /// <summary>
        /// 定义 BoiledEventArgs类，传递给 Observer 所感兴趣的信息
        /// </summary>
        public class BoiledEventArgs : EventArgs
        {
            public readonly int temperature;

            public BoiledEventArgs(int temperature)
            {
                this.temperature = temperature;
            }
        }

        /// <summary>
        /// 提供继承自Heater的类重写，以便继承类拒绝其他对象对它的监视
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBoiled(BoiledEventArgs e)
        {
            // 如果有对象注册
            Boiled?.Invoke(this, e);  // 调用所有注册对象的方法
        }

        public void BoilWater()
        {
            for (int i = 0; i <= 100; i++)
            {
                temperature = i;
                if (temperature > 95)
                {
                    // 建立BoiledEventArgs 对象。
                    BoiledEventArgs e = new BoiledEventArgs(temperature);
                    OnBoiled(e); // 调用 OnBolied 方法
                }
            }
        }
    }

    // 警报器
    public class Alarm
    {
        public void MakeAlert(object sender, Heater.BoiledEventArgs e)
        {
            Heater heater = (Heater)sender;
            Console.WriteLine("Alarm：{0} - {1}: ", heater.area, heater.type);
            Console.WriteLine("Alarm: 嘀嘀嘀，水已经 {0} 度了：", e.temperature);
            Console.WriteLine();
        }
    }

    // 显示器
    public static class Display
    {
        public static void ShowMsg(object sender, Heater.BoiledEventArgs e)
        {
            //静态方法
            Heater heater = (Heater)sender;
            Console.WriteLine("Display：{0} - {1}: ", heater.area, heater.type);
            Console.WriteLine("Display：水快烧开了，当前温度：{0}度。", e.temperature);
            Console.WriteLine();
        }
    }
#endif
}

