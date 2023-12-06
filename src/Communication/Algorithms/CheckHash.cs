using System.Security.Cryptography;

namespace Communication.Algorithms
{
    public static class CheckHash
    {
        public static byte[] ComputeMD5(in byte[] buffer, int offset, int count)
        {
            var md5 = MD5.Create();
            return md5.ComputeHash(buffer, offset, count);
        }

        public static byte[] ComputeSHA256(in byte[] buffer, int offset, int count)
        {
            var sha256 = SHA256.Create();
            return sha256.ComputeHash(buffer, offset, count);
        }

        public static byte[] ComputeSHA512(in byte[] buffer, int offset, int count)
        {
            var sha512 = SHA512.Create();
            return sha512.ComputeHash(buffer, offset, count);
        }
    }
}
