namespace WebApp01
{
    using System;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using WebApp01.CustomDataProtection;
    using WebApp01.Options;

    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            if (Environment.IsDevelopment())
            {
                services.Configure<TestOption>(Configuration.GetSection("TestOption"));
            }
            else
            {
                // 注入数据保护需要的Option
                services.Configure<ProtectionOption>(Configuration.GetSection("ProtectionOption"));

                // 注入数据保护服务（依赖指定证书）
                IDataProtector dataProtector = services.AddDataProtectionWithX509();
                // 解密字符串
                string connStr = dataProtector.Unprotect(Configuration.GetSection("Database:ConnectString").Value);
                Console.WriteLine($"Sit:ConnStr:{connStr}");
                // 解密Option;Option上带有EncryptedAttribute标记的属性将会被解密
                services.ProtectedConfigure<TestOption>(Configuration.GetSection("TestOption"));
            }
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
