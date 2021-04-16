namespace WebApp01.Options
{
    using WebApp01.CustomDataProtection;

    public class TestOption : ProtectionOptionBase
    {
        public string Test1 { get; set; }

        [Encrypted]
        public string Test2 { get; set; }
    }
}
