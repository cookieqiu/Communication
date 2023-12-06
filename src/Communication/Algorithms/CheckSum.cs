namespace Communication.Algorithms
{
    internal static class CheckSum
    {
        public static byte[] ComputeCRC16(in byte[] buffer, int offset, int length)
        {
            var crc16 = CRC16.Create();
            return crc16.ComputeSum(buffer, offset, length);
        }
    }
}
