namespace GeneratingCiphertext.Console
{
    using System;
    using System.Collections.Generic;

    using CommandLine;

    public class InputParameter
    {
        [Option("cpath", HelpText = "使用的证书存储路径")]
        /// <summary>
        /// 证书路径
        /// </summary>
        public string CertFilePath { get; set; }

        [Option("cpass", HelpText = "使用的证书密码")]
        /// <summary>
        /// 证书密码
        /// </summary>
        public string CertPassWord { get; set; }

        [Option("appname", HelpText = "应用程序名称")]
        /// <summary>
        /// 应用程序标识
        /// </summary>
        public string ApplicationName { get; set; }

        [Option("purpose", HelpText = "保护器名称,加密时提供,解密时也需要同样名称方可正常解密")]
        /// <summary>
        /// 保护器名称
        /// </summary>
        public string Purpose { get; set; }

        [Option("spath", HelpText = "生成的秘钥(key)文件存储路径")]
        /// <summary>
        /// 秘钥存储路径
        /// </summary>
        public string SecretKeyPath { get; set; }

        [Option("snodes", HelpText = "需要加密的节点,格式“a:1,b:2,c:3”，会根据此节点内容生成secret.xml")]
        /// <summary>
        /// 需要加密的节点,格式“a:1,b:2”
        /// </summary>
        public string SecretNodes { get; set; }

        [Option("soutputpath", HelpText = "secret.xml文件的输出路径")]
        /// <summary>
        /// secret.xml输出路径
        /// </summary>
        public string SecretOutputPath { get; set; }
    }

    [Serializable]
    public class XmlResult
    {
        /// <summary>
        /// 证书文件路径
        /// </summary>
        public string CertFilePath { get; set; }

        /// <summary>
        /// 证书密码
        /// </summary>
        public string CertPassWord { get; set; }

        /// <summary>
        /// 应用程序标识
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// 保护器名称
        /// </summary>
        public string Purpose { get; set; }

        /// <summary>
        /// 秘钥存储路径
        /// </summary>
        public string SecretKeyPath { get; set; }

        /// <summary>
        /// 加密节点
        /// </summary>
        public List<XmlNode> SecretNode { get; set; } = new List<XmlNode>();
    }

    public class XmlNode
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 原文
        /// </summary>
        public string OriginalText { get; set; }

        /// <summary>
        /// 密文
        /// </summary>
        public string Ciphertext { get; set; }
    }
}
