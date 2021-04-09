## ���
> ����ָ��֤�����ɼ����ļ�`secret.xml`�Լ���Ӧ`{key}.xml`�ļ�


## Options
- `options.CertFilePath`��֤��·��
- `options.CertPassWord`: ֤������
- `options.Purpose`:����������
- `options.SecretKeyPath`:`app-keys`���·��
- `options.SecretNodes`:���ܽڵ㣬��ʽ��`a:1, b:2`��
- `options.SecretOutputPath`:`secret.xml`���·��
- `options.ApplicationName`:Ӧ�ó�������

example

`--cpath="C:\\Users\\WangPengLiang\\Desktop\\DataProtection\\dev.pfx"  --cpass="wpl19950815" --purpose="RedPI.Todo" --spath="C:\\Users\\WangPengLiang\\Desktop\\DataProtection\\app-keys" --appname="RedPI.Todo" --snodes="a:1, b:2"  --soutputpath="C:\Users\WangPengLiang\Desktop\DataProtection"`

## Output
- `{key}.xml`:��Կ`key`�ļ�
- `secret.xml`:���ݼ��ܽڵ����ɵ�xml�ļ�

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
