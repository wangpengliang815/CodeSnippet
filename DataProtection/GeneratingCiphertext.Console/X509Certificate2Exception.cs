namespace GeneratingCiphertext.Consolesss
{
    using System;

#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class X509Certificate2Exception : Exception
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
    {
        public X509Certificate2Exception(string message) : base(message) { }

        public X509Certificate2Exception(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
