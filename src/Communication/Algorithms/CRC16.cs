namespace Communication.Algorithms
{
    internal abstract class CRC16
    {
        protected CRC16() { }

        public static CRC16 Create()
        {
            throw new NotImplementedException();
        }

        public byte[] ComputeSum(in byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
