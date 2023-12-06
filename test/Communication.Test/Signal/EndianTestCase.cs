using Communication.Signals.Endians;

namespace Communication.Test.Signal
{
    public class EndianTestCase
    {
        [Fact]
        public void BigEndianReadTest()
        {
            var buffer = new byte[5] { 1, 2, 3, 4, 5 };
            var target = new byte[5] { 5, 4, 3, 2, 1 };
            var bigEndian = new BigEndian();
            var newBuffer = bigEndian.Read(buffer, 0, buffer.Length);
            Assert.Equal(newBuffer, target);
        }

        [Fact]
        public void BigEndianWriteTest()
        {
            var buffer = new byte[5] { 1, 2, 3, 4, 5 };
            var target = new byte[5] { 5, 4, 3, 2, 1 };
            var bigEndian = new BigEndian();
            var newBuffer = new byte[5];
            bigEndian.Write(newBuffer, buffer, 0, buffer.Length);
            Assert.Equal(target, newBuffer);
        }
    }
}