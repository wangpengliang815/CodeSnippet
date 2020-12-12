using System;
using System.Collections.Generic;
using System.Text;

namespace CodeSnippet.Csharp
{
    public class EventTests
    {
    }
    // 热水器
    public class Heater
    {
        private int temperature;

        // 烧水
        private void BoilWater()
        {
            for (int i = 0; i <= 100; i++)
            {
                temperature = i;
            }
        }
    }

    // 警报器
    public class Alarm
    {
        private void MakeAlert(int param)
        {
            Console.WriteLine("Alarm：嘀嘀嘀，水已经 {0} 度了：", param);
        }
    }

    // 显示器
    public class Display
    {
        private void ShowMsg(int param)
        {
            Console.WriteLine("Display：水已烧开，当前温度：{0}度。", param);
        }
    }
}
