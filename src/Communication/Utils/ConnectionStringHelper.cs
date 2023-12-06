namespace Communication.Utils
{
    public class ConnectionStringHelper
    {
        public static ConnectionStringAccessor Parse(string connectionString)
        {
            var parser = new ConnectionStringParser { ConnectionString = connectionString };
            return new ConnectionStringAccessor(parser.ConnectionDictionary);
        }
    }
}
