namespace WebApp01.CustomDataProtection
{
    using System.Reflection;

    using Microsoft.AspNetCore.DataProtection;

    /// <summary>使用数据保护的Option要继承的基类,带有EncryptedAttribute标记的将会被解密</summary>
    public abstract class ProtectionOptionBase
    {
        private bool Decrypted { get; set; } = false;

        public void Decrypt(IDataProtector protector)
        {
            if (Decrypted) return;

            foreach (PropertyInfo property in GetType().GetProperties())
            {
                if (property.GetCustomAttribute<EncryptedAttribute>() != null)
                {
                    string text = property.GetValue(this).ToString();
                    property.SetValue(this, protector.Unprotect(text));

                }
            }
            Decrypted = true;
        }

        public void CopyTo(ProtectionOptionBase option)
        {
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                property.SetValue(option, property.GetValue(this));
            }
        }
    }
}
