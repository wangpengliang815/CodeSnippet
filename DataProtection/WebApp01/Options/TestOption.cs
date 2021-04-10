namespace WebApp01.Options
{
    using WebApp01.CustomDataProtection;

    public class TestOption: ProtectionOptionBase
    {
        [Encrypted]
        public string Test1 { get; set; }

        public string Test2 { get; set; }
    }
}
