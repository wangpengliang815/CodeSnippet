namespace CodeSnippet.Autofac.IocTests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestCategory("IOC")]
    [TestClass()]
    public class IocTests
    {
        /// <summary>构造函数注入</summary>
        [TestMethod()]
        public void IOC_Example_CtorInject()
        {
            Order order = new Order(new SqlDal());
            order.Add();
            Assert.IsTrue(true);
        }

        /// <summary>属性注入</summary>
        [TestMethod()]
        public void IOC_Example_PropertyInject()
        {
            Order2 order = new Order2
            {
                Dal = new SqlDal()
            };
            order.Add();
            Assert.IsTrue(true);
        }
    }

    class Order
    {
        private readonly IDal dal;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="dal"></param>
        public Order(IDal dal)
        {
            this.dal = dal;
        }

        public void Add()
        {
            dal.Add();
        }
    }

    class Order2
    {
        /// <summary>
        /// 属性注入
        /// </summary>
        public IDal Dal { get; set; }

        public void Add()
        {
            Dal.Add();
        }
    }

    interface IDal
    {
        void Add();
    }

    class SqlDal : IDal
    {
        public void Add()
        {
            Console.WriteLine("使用SqlDal插入一条数据");
        }
    }

    class MySqlDal : IDal
    {
        public void Add()
        {
            Console.WriteLine("使用MySqlDal插入一条数据");
        }
    }
}
