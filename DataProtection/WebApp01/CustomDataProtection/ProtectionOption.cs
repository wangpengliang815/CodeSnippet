namespace WebApp01.CustomDataProtection
{
    /// <summary>将AddDataProtection()需要用到的参数做成Option方便参数配置</summary>
    public class ProtectionOption
    {
        /// <summary>应用程序名称</summary>
        /// <value>The name of the application.</value>
        public string ApplicationName { get; set; }

        /// <summary>证书指纹</summary>
        /// <value>The thumbprint.</value>
        public string Thumbprint { get; set; }

        /// <summary>私钥存储路径</summary>
        /// <value>The secret key path.</value>
        public string SecretKeyPath { get; set; }

        /// <summary>保护器名称</summary>
        /// <value>The purpose.</value>
        public string Purpose { get; set; }
    }
}
