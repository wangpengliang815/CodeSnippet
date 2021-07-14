namespace codeSnippet.Autofac.TraditionTests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestCategory("IOC")]
    [TestClass()]
    public class TraditionTests
    {
        [TestMethod()]
        public void Tradition_SqlDal()
        {
            Order1 order = new Order1();
            order.Add();
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void Tradition_MySqlDal()
        {
            Order2 order = new Order2();
            order.Add();
            Assert.IsTrue(true);
        }
    }

    class Order1
    {
        readonly SqlDal sqlDal = new SqlDal();

        public void Add()
        {
            sqlDal.Add();
        }
    }

    class Order2
    {
        readonly MySqlDal mysqlDal = new MySqlDal();

        public void Add()
        {
            mysqlDal.Add();
        }
    }

    class SqlDal
    {
        public void Add()
        {
            Console.WriteLine("使用SqlDal插入一条数据");
        }
    }

    class MySqlDal
    {
        public void Add()
        {
            Console.WriteLine("使用MySqlDal插入一条数据");
        }
    }
}
