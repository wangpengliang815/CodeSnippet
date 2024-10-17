namespace CodeSnippet.CsharpTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Newtonsoft.Json;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestCategory("LinqTests")]
    [TestClass()]
    public class LinqTests
    {
        public class User
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        public class Orders
        {
            public int UserId { get; set; }


            public int Id { get; set; }

            public string OrderNumber { get; set; }

            public List<string> Flag { get; set; } = [];
        }

        public List<User> users =
        [
            new User { Id = 1,Name="wang"},
            new User { Id = 2,Name="li"},
            new User { Id = 3,Name="zhang"},
        ];

        public List<Orders> orders =
        [
            new Orders {UserId=2, Id = 1,OrderNumber="111",Flag=["非加急","普通"]},
            new Orders {UserId=3, Id = 2,OrderNumber="222",Flag=["非加急","普通"]},
            new Orders {UserId=1, Id = 3,OrderNumber="333",Flag=["加急","Vip"] },
        ];

        /// <summary>
        /// 最简单的Linq语法查询
        /// </summary>
        [TestMethod]
        public void Linq_Example_Normal()
        {
            // 最简单查询
            var query = from order in orders
                        where order.UserId == 1
                        select order;

            Console.WriteLine(JsonConvert.SerializeObject(query));

            // 使用Linq扩展方法
            var query2 = orders.Where(p => p.UserId == 1).FirstOrDefault();
            Console.WriteLine(JsonConvert.SerializeObject(query2));
        }

        /// <summary>
        /// 最简单的Linq语法查询
        /// </summary>
        [TestMethod]
        public void Linq_Example_Where()
        {
            // 简单查询
            var query = from order in orders
                        where order.UserId == 1
                        select order;

            // 多条件查询
            var query2 = from order in orders
                         where (order.UserId > 1 && order.Id < 2)
                         select order;

            // Linq扩展方法
            var query3 = orders.Where(p => p.OrderNumber.StartsWith('1') && p.UserId > 2).ToList();


            // 根据类型筛选
            var query4 = orders.OfType<Orders>();

            // 指定返回字段
            var query5 = from order in orders
                         where order.UserId == 1
                         select new
                         {
                             order.Id,
                             order.UserId,
                         };

            // 复合from
            var query6 = from order in orders
                         from flag in order.Flag
                         where flag == "加急"
                         select new
                         {
                             order.Id,
                             order.UserId,
                         };

            // 复合from
            var query7 = orders.SelectMany(r => r.Flag, (p, d) =>
                new
                {
                    p.Id,
                    p.UserId
                }
            );

            foreach (var item in query6)
            {
                Console.WriteLine(item);
            }
        }
    }
}
