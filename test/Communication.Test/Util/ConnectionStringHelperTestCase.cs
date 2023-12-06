using Communication.Utils;

namespace Communication.Test.Util
{
    public class ConnectionStringHelperTestCase
    {
        [Fact]
        public void ConnectionStringParseTest()
        {
            var sources = new string[]
            {
                "IP = 127.0.0.1 ; Port = 8000",
                "IP=127.0.0.1;Port=8000;"
            };
            var targetIP = "127.0.0.1";
            var targetPort = 8000;

            foreach (var source in sources)
            {
                var connectionStringAccessor = ConnectionStringHelper.Parse(source);
                Assert.Equal(targetIP, connectionStringAccessor.GetString("IP"));
                Assert.Equal(targetIP, connectionStringAccessor.GetString("ip"));
                Assert.Equal(targetPort, connectionStringAccessor.GetInt("Port"));
                Assert.Equal(targetPort, connectionStringAccessor.GetInt("pORT"));
            }
        }
    }
}
