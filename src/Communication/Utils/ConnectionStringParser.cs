using System.Text;

namespace Communication.Utils
{
    internal class ConnectionStringParser
    {
        private readonly Dictionary<string, string> _dictionary;

        public Dictionary<string, string> ConnectionDictionary => _dictionary;

        private string _connectionString = string.Empty;

        private const int c_tokenListCapacity = 8;

        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; InternalParse(); }
        }

        public ConnectionStringParser()
        {
            _dictionary = new Dictionary<string, string>();
        }

        private void InitDictionary(in IList<string> tokens)
        {
            _dictionary.Clear();
            for (int i = 0, imax = tokens.Count; i < imax; i += 2)
            {
                _dictionary[tokens[i]] = tokens[i + 1];
            }
        }

        private void InternalParse()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                return;
            }

            var state = c_apdKeyToken;
            var tokens = new List<string>(c_tokenListCapacity);
            var tokenBuilder = new StringBuilder(20);

            for (int i = 0, imax = _connectionString.Length; i < imax; i++)
            {
                var c = _connectionString[i];
                var sign = GetSignType(c);

                state = s_stm[state, sign];

                switch (state)
                {
                    case c_apdKeyToken:
                    case c_apdValueToken:
                        {
                            tokenBuilder.Append(c);
                        }
                        break;
                    case c_addKeyToken:
                        {
                            tokens.Add(tokenBuilder.ToString()
                                .Trim().ToUpper());
                            tokenBuilder.Clear();
                        }
                        break;
                    case c_addValueToken:
                        {
                            tokens.Add(tokenBuilder.ToString()
                                .Trim());
                            tokenBuilder.Clear();
                        }
                        break;
                    default:
                        ThrowFormatException();
                        break;
                }
            }

            if (tokenBuilder.Length != 0)
            {
                if (c_apdValueToken != state)
                {
                    ThrowFormatException();
                }
                tokens.Add(tokenBuilder.ToString()
                    .Trim());
                state = c_addValueToken;
            }
            if(c_addValueToken != state)
            {
                ThrowFormatException();
            }

            InitDictionary(tokens);

            static void ThrowFormatException() => throw new InvalidOperationException("连接字符串格式错误");
        }

        private static int GetSignType(char c)
        {
            if (c.Equals('='))
            {
                return c_equalSign;
            }
            else if (c.Equals(';'))
            {
                return c_semicolon;
            }
            else
            {
                return c_tokenSign;
            }
        }

        private const int c_apdKeyToken = 0;
        private const int c_apdValueToken = 1;
        private const int c_addKeyToken = 2;
        private const int c_addValueToken = 3;
        private const int c_errorState = 4;

        private const int c_equalSign = 0;
        private const int c_semicolon = 1;
        private const int c_tokenSign = 2;

        private static readonly int[,] s_stm = new int[4, 3]
        {
            {c_addKeyToken,c_errorState,c_apdKeyToken},
            {c_errorState,c_addValueToken,c_apdValueToken},
            {c_errorState,c_errorState,c_apdValueToken},
            {c_errorState,c_errorState,c_apdKeyToken}
        };
    }
}
