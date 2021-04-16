namespace GeneratingCiphertext.Console
{
    using System.IO;
    using System.Security.Cryptography.X509Certificates;

    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// The following example demonstrates how to use an X509Certificate2 object to encrypt and decrypt a file.
    /// 参考：https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.x509certificate2?view=netcore-3.1
    /// </summary>
    public static class X509CertEncrypt
    {
        /// <summary>
        /// 使用X509Certificate2证书加密字符串
        /// </summary>
        /// <param name="applicationName">应用程序唯一标识(默认情况下，即使数据共享相同的物理密钥存储库，Data Protection系统也会根据其内容根路径将应用程序彼此隔离。这样可以防止应用了解彼此受保护的有效负载)</param>
        /// 多应用程序共享秘钥参考：https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-2.1#setapplicationname
        /// <param name="thumbprint">证书指纹</param>
        /// <param name="encryptStr">待加密字符串</param>
        /// <param name="purpose">数据保护器名称</param>
        /// <returns></returns>
        public static string X509Certificate2Encrypt(
            string encryptStr,
            InputParameter options)
        {
            ServiceCollection service = new ServiceCollection();

            service.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(
                    options.SecretKeyPath))
                .SetApplicationName(
                    options.ApplicationName)
                   .ProtectKeysWithCertificate(
                    new X509Certificate2(options.CertFilePath, options.CertPassWord));

            IDataProtectionProvider dataProtectionProvider = service.BuildServiceProvider()
                .GetService<IDataProtectionProvider>();

            IDataProtector protector = dataProtectionProvider.CreateProtector(options.Purpose);

            return protector.Protect(encryptStr);
        }

        /// <summary>
        /// 使用X509Certificate2证书解密字符串
        /// </summary>
        /// <param name="decryptStr">待解密字符串</param>
        /// <param name="services"></param>
        /// <returns></returns>
        public static string X509Certificate2Decrypt(
            string decryptStr,
            InputParameter options)
        {
            ServiceCollection service = new ServiceCollection();
            service.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(
                    options.SecretKeyPath))
                .SetApplicationName(
                    options.ApplicationName)
               .ProtectKeysWithCertificate(
                    new X509Certificate2(options.CertFilePath, options.CertPassWord));

            IDataProtectionProvider protector = service
                .BuildServiceProvider().GetService<IDataProtectionProvider>();

            IDataProtector dataProtector = protector.CreateProtector(options.Purpose);
            return dataProtector.Unprotect(decryptStr);
        }
    }
}
