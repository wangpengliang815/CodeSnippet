namespace codeSnippet.Csharp
{
    using System;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestCategory("Reflect")]
    [TestClass()]
    public class ReflectTests
    {
        /// <summary>
        /// 获取所有构造函数
        /// </summary>
        [TestMethod]
        public void GetConstructors()
        {
            Type t = new ReflectClass().GetType();
            // 获取类的所有构造函数
            ConstructorInfo[] constructorInfos = t.GetConstructors();
            foreach (ConstructorInfo ci in constructorInfos)
            {
                // 获取每个构造函数的参数
                ParameterInfo[] parameterInfos = ci.GetParameters();
                foreach (ParameterInfo p in parameterInfos)
                {
                    Console.WriteLine(p.ParameterType.ToString() + "\n" + p.Name + "\n");
                }
            }
            Assert.IsTrue(constructorInfos.Length > 0);
        }

        /// <summary>
        /// 动态创建对象
        /// </summary>
        [TestMethod]
        public void DynamicCreateObject()
        {
            Type t = typeof(ReflectClass);
            Type[] pt = new Type[2];
            pt[0] = typeof(string);
            pt[1] = typeof(string);
            //根据参数类型获取构造函数
            ConstructorInfo ci = t.GetConstructor(pt);
            //构造Object数组，作为构造函数的输入参数
            object[] obj = new object[2] { "wang", "男" };
            //调用构造函数生成对象
            object @object = ci.Invoke(obj);
            //调用生成的对象的方法测试是否对象生成成功
            ((ReflectClass)@object).Show();
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 使用Activator动态创建对象
        /// </summary>
        [TestMethod]
        public void ActivatorDynamicCreateObject()
        {
            Type t = typeof(ReflectClass);
            object[] obj = new object[2] { "wang", "男" };
            //用Activator的CreateInstance静态方法，生成新对象
            object @object = Activator.CreateInstance(t, obj);
            ((ReflectClass)@object).Show();
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 获取类中的Public属性
        /// </summary>
        [TestMethod]
        public void GetProperties()
        {
            Type t = new ReflectClass().GetType();
            PropertyInfo[] propertyInfos = t.GetProperties();
            foreach (PropertyInfo p in propertyInfos)
            {
                Console.WriteLine(p.Name);
            }
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 获取类中Public方法
        /// </summary>
        [TestMethod]
        public void GetPublicMethod()
        {
            Type t = new ReflectClass().GetType();
            MethodInfo[] mi = t.GetMethods();
            foreach (MethodInfo method in mi)
            {
                Console.WriteLine(method.ReturnType + "|" + method.Name);
            }
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 获取类中Public字段
        /// </summary>
        [TestMethod]
        public void GetField()
        {
            Type t = new ReflectClass().GetType();
            FieldInfo[] fieldInfos = t.GetFields(BindingFlags.Public);
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                Console.WriteLine(fieldInfo.Name);
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Example_01()
        {
            ReflectClass rc = new ReflectClass();
            Type t = rc.GetType();
            object obj = Activator.CreateInstance(t);

            FieldInfo address = t.GetField("Address");
            address.SetValue(obj, "Beijing");

            PropertyInfo name = t.GetProperty("Name");
            name.SetValue(obj, "wang", null);

            PropertyInfo age = t.GetProperty("Age");
            age.SetValue(obj, 20, null);

            MethodInfo method = t.GetMethod("Show");
            method.Invoke(obj, null);

            Console.WriteLine("Address为：" + ((ReflectClass)obj).Address);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void GetAssembly()
        {
            // 通过程序集名称返回Assembly对象
            Assembly assembly = Assembly.Load("codeSnippet");

            // 通过Assembly获取程序集中类(参数必须是类的全名)
            Type type = assembly.GetType("codeSnippet.Csharp.ReflectClass");
            Assert.IsNotNull(type);

            // 通过Assembly获取程序集中所有的类
            Type[] types = assembly.GetTypes();
            Assert.IsNotNull(types);

            // 通过DLL文件名称返回Assembly对象
#pragma warning disable S3885 // "Assembly.Load" should be used
            Assembly assembly2 = Assembly.LoadFrom("codeSnippet.dll");
#pragma warning restore S3885 // "Assembly.Load" should be used

            // 通过Assembly获取程序集中类(参数必须是类的全名)
            Type type2 = assembly2.GetType("codeSnippet.Csharp.ReflectClass");
            Assert.IsNotNull(type2);

            // 通过Assembly获取程序集中所有的类
            Type[] types2 = assembly2.GetTypes();
            Assert.IsNotNull(types2);
        }

        /// <summary>
        /// 通过程序集的名称反射
        /// </summary>
        [TestMethod]
        public void Example_02()
        {
            Assembly assembly = Assembly.Load("codeSnippet");
            //参数必须是类的全名
            Type t = assembly.GetType("codeSnippet.Csharp.ReflectClass");
            object o = Activator.CreateInstance(t, "男");
            MethodInfo mi = t.GetMethod("Show");
            mi.Invoke(o, null);
            Assert.IsNotNull(o);
        }

        /// <summary>
        /// 通过DLL文件全名反射其中的所有类型
        /// </summary>
        [TestMethod]
        public void Example_03()
        {
#pragma warning disable S3885 // "Assembly.Load" should be used
            Assembly assembly = Assembly.LoadFrom("codeSnippet.dll");
#pragma warning restore S3885 // "Assembly.Load" should be used
            Type[] types = assembly.GetTypes();
            foreach (Type t in types)
            {
                if (t.FullName == "codeSnippet.Csharp.ReflectClass")
                {
                    object o = Activator.CreateInstance(t);
                    Assert.IsNotNull(o);
                }
            }
        }
    }

    public class ReflectClass
    {
        public string Address;

        public int Age { get; set; }

        public string Sex { get; set; }

        public string Name { get; set; }

        public ReflectClass() { }

        public ReflectClass(string name)
        {
            this.Name = name;
        }

        public ReflectClass(string name, string sex)
        {
            this.Name = name;
            this.Sex = sex;
        }

        public void Show()
        {
            Console.WriteLine("姓名：" + Name + "\n" + "年龄：" + Age + "\n" + "性别：" + Sex);
        }
    }
}
