namespace Communication.Utils
{
    public class ConnectionStringAccessor
    {
        private readonly Dictionary<string, string> _dictionary;

        public ConnectionStringAccessor(Dictionary<string, string> connectionDictionary)
        {
            _dictionary = connectionDictionary;
        }

        private string GetValue(string key)
        {
            return _dictionary[key.ToUpper()];
        }

        public int GetInt(string key) => int.Parse(GetValue(key));

        public decimal GetDecimal(string key) => decimal.Parse(GetValue(key));

        public short GetShort(string key) => short.Parse(GetValue(key));

        public bool GetBoolean(string key) => bool.Parse(GetValue(key));

        public string GetString(string key) => GetValue(key);
    }
}
