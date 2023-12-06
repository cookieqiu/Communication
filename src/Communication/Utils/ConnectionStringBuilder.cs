using System.Runtime.Serialization;
using System.Text;

namespace Communication.Utils
{
    public class ConnectionStringBuilder
    {
        private readonly StringBuilder _stringBulider;

        public ConnectionStringBuilder()
        {
            _stringBulider = new StringBuilder();
        }

        public void Set<T>(string key, T value)
        {
            _stringBulider.Append(key);
            _stringBulider.Append('=');
            _stringBulider.Append(value!.ToString());
            _stringBulider.Append(';');
        }

        public override string ToString()
        {
            return _stringBulider.ToString();
        }
    }
}
