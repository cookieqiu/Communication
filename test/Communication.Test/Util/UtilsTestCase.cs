using Communication.Utils;

namespace Communication.Test.Util
{
    public class UtilsTestCase
    {
        [Fact]
        public void BooleanAccessorSetTest()
        {
            var data = 0;
            BooleanAccessor.Set(ref data, 10, true);
            Assert.Equal(1 << 10, data);
        }

        [Fact]
        public void BooleanAccessorGetTest()
        {
            var data = 1 << 10;

            Assert.False(BooleanAccessor.Get(data, 9));
            Assert.True(BooleanAccessor.Get(data, 10));
            Assert.False(BooleanAccessor.Get(data, 11));
        }

        [Fact]
        public void BooleanAccessorResetTest()
        {
            var data = 1 << 10;
            BooleanAccessor.Set(ref data, 10, false);
            Assert.Equal(0, data);
        }

        [Fact]
        public void BooleanAccessorOutOfRangeAssertTest()
        {
            var data = 0;
            Assert.Throws<ArgumentOutOfRangeException>(() => BooleanAccessor.Get(data, 32));
        }
    }
}