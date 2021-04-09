namespace GeneratingCiphertext.Console
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    using CommandLine;

    /// <summary>依赖指定证书生成加密文件secret.xml以及对应{key}.xml文件</summary>
    static class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<InputParameter>(args)
                .WithParsed(options =>
                {
                    try
                    {
                        Console.WriteLine($"{nameof(options.CertFilePath)}:{options.CertFilePath}");
                        Console.WriteLine($"{nameof(options.CertPassWord)}:{options.CertPassWord}");
                        Console.WriteLine($"{nameof(options.Purpose)}:{options.Purpose}");
                        Console.WriteLine($"{nameof(options.SecretKeyPath)}:{options.SecretKeyPath}");
                        Console.WriteLine($"{nameof(options.SecretNodes)}:{options.SecretNodes}");
                        Console.WriteLine($"{nameof(options.SecretOutputPath)}:{options.SecretOutputPath}");
                        Console.WriteLine($"{nameof(options.ApplicationName)}:{options.ApplicationName}");

                        try
                        {
                            XmlResult xmlResult = new XmlResult()
                            {
                                CertFilePath = options.CertFilePath,
                                CertPassWord = options.CertPassWord,
                                Purpose = options.Purpose,
                                SecretKeyPath = options.SecretKeyPath,
                                ApplicationName = options.ApplicationName
                            };

                            string[] nodes = options.SecretNodes.Split(',');
                            if (nodes.Length > 0)
                            {
                                foreach (string item in nodes)
                                {
                                    string nodeName = item.Split(':')[0];
                                    string nodeValue = item.Split(':')[1];
                                    xmlResult.SecretNode.Add(new XmlNode
                                    {
                                        NodeName = nodeName,
                                        OriginalText = nodeValue,
                                        Ciphertext = X509CertEncrypt.X509Certificate2Encrypt(nodeValue, options)
                                    });
                                }
                            }

                            FileStream fs = new FileStream($"{options.SecretOutputPath}\\secret.xml"
                                , FileMode.Create);
                            XmlSerializer xs = new XmlSerializer(typeof(XmlResult));
                            xs.Serialize(fs, xmlResult);
                            fs.Close();
                            Console.WriteLine($"==========================================>");
                            Console.WriteLine($"Success");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{ ex.GetBaseException().Message }");
                    }
                })
                .WithNotParsed((errs) =>
                {
                    Console.WriteLine($"{ errs.FirstOrDefault().Tag}");
                });
        }
    }
}
