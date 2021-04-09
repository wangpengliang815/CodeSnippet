## 简介
> 依赖指定证书生成加密文件`secret.xml`以及对应`{key}.xml`文件


## Options
- `options.CertFilePath`：证书路径
- `options.CertPassWord`: 证书密码
- `options.Purpose`:保护器名称
- `options.SecretKeyPath`:`app-keys`输出路径
- `options.SecretNodes`:加密节点，格式“`a:1, b:2`”
- `options.SecretOutputPath`:`secret.xml`输出路径
- `options.ApplicationName`:应用程序名称

example

`--cpath="C:\\Users\\WangPengLiang\\Desktop\\DataProtection\\dev.pfx"  --cpass="wpl19950815" --purpose="RedPI.Todo" --spath="C:\\Users\\WangPengLiang\\Desktop\\DataProtection\\app-keys" --appname="RedPI.Todo" --snodes="a:1, b:2"  --soutputpath="C:\Users\WangPengLiang\Desktop\DataProtection"`

## Output
- `{key}.xml`:秘钥`key`文件
- `secret.xml`:根据加密节点生成的xml文件

example
```xml
<?xml version="1.0"?>
<XmlResult xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <CertFilePath>C:\\Users\\WangPengLiang\\Desktop\\DataProtection\\certs\\dev.pfx</CertFilePath>
  <CertPassWord>wpl19950815</CertPassWord>
  <ApplicationName>RedPI.Todo</ApplicationName>
  <Purpose>RedPI.Todo</Purpose>
  <SecretKeyPath>C:\\Users\\WangPengLiang\\Desktop\\DataProtection\\app-keys</SecretKeyPath>
  <SecretNode>
    <XmlNode>
      <NodeName>a</NodeName>
      <OriginalText>1</OriginalText>
      <Ciphertext>CfDJ8BYhjIfxN5tKhGMXUuBE_nd33YAUzB1hQUueSHqQgsytNVbEbh6wS_mlBkIJWjpuHFXztrVfCDaWGz2nTZEzcdDMcZ9PWSxRo39HgF1wReqqqV7b1AqNjyz40TG0WWasvQ</Ciphertext>
    </XmlNode>
    <XmlNode>
      <NodeName> b</NodeName>
      <OriginalText>2</OriginalText>
      <Ciphertext>CfDJ8BYhjIfxN5tKhGMXUuBE_neby-SHXqsskwFZ70CKZCQGlfLRIE3qAFhVseHlrtc5ojfGLNCJEWCd6NnM44PpjSM364urP4C9edb3CKjJ0O9zmU65KrDvAfBcR3qDEXtczA</Ciphertext>
    </XmlNode>
  </SecretNode>
</XmlResult>
```
