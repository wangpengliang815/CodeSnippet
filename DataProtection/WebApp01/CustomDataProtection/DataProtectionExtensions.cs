namespace WebApp01.CustomDataProtection
{
    using System;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;

    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public static class DataProtectionExtensions
    {
        /// <summary>注册数据保护服务,使用x509证书加密</summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        /// <exception cref="Exception">not found X509Certificate</exception>
        public static IDataProtector AddDataProtectionWithX509(this IServiceCollection services)
        {
            ServiceProvider provider = services.BuildServiceProvider();

            // 获取用户配置数据保护的Option
            ProtectionOption protectionOption = provider
                .GetRequiredService<IOptions<ProtectionOption>>().Value;

            // 获取证书
            X509Certificate2 cert = GetCertificateFromStore(protectionOption.Thumbprint);

            // 注册服务
            services.AddDataProtection()
                // 设置应用程序名称
                .SetApplicationName(protectionOption.ApplicationName)
                // 设置秘钥存储路径
                .PersistKeysToFileSystem(new DirectoryInfo(protectionOption.SecretKeyPath))
                // 设置用于加密的证书
                .UnprotectKeysWithAnyCertificate(cert);

            provider = services.BuildServiceProvider();

            IDataProtectionProvider protectionProvider = provider.GetRequiredService<IDataProtectionProvider>();

            // 创建数据保护器
            IDataProtector protector = protectionProvider.CreateProtector(protectionOption.Purpose);

            // 注册单例的数据保护器
            services.AddSingleton<IDataProtector>(serviceProvider => protector);

            return protector;
        }

        public static IServiceCollection ProtectedConfigure<TOptions>(this IServiceCollection services
            , IConfigurationSection section)
             where TOptions : ProtectionOptionBase, new()
        {
            ServiceProvider provider = services.BuildServiceProvider();
            IDataProtector protector = provider.GetRequiredService<IDataProtector>();

            services.Configure<TOptions>(option =>
            {
                var config = section.Get<TOptions>();
                config.Decrypt(protector);
                config.CopyTo(option);
            });

            return services;
        }

        /// <summary>
        /// Get the certifcate to use to encrypt the key
        /// CertSearchArea：StoreLocation.CurrentUser/StoreLocation.LocalMachine
        /// </summary>
        /// <param name="thumbprint"></param>
        /// <returns></returns>
        public static X509Certificate2 GetCertificateFromStore(string thumbprint)
        {
            X509Certificate2 signingCert =
                GetCertificateFromStore(thumbprint, StoreLocation.CurrentUser);
            if (signingCert != null)
            {
                return signingCert;
            }
            else
            {
                signingCert =
                   GetCertificateFromStore(thumbprint, StoreLocation.LocalMachine);
                if (signingCert != null)
                {
                    return signingCert;
                }
            }
            throw new X509Certificate2Exception("本机和当前用户证书存储区都未找到对应证书");
        }

        /// <summary>Gets the certificate from store.</summary>
        /// <param name="thumbprint">The thumbprint.</param>
        /// <param name="storeLocation">The store location.</param>
        /// <returns></returns>
        public static X509Certificate2 GetCertificateFromStore(string thumbprint, StoreLocation storeLocation)
        {
            // 获取本地机器证书存储
            X509Store localMachineStore = new X509Store(storeLocation);
            try
            {
                localMachineStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = localMachineStore.Certificates;
                X509Certificate2Collection localMachineCerts =
                    certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                X509Certificate2Collection signingCert =
                    localMachineCerts.Find(X509FindType.FindByThumbprint, thumbprint, false);
                if (signingCert.Count == 0)
                {
                    return null;
                }

                return signingCert[0];
            }
            finally
            {
                localMachineStore.Close();
            }
        }
    }
}
