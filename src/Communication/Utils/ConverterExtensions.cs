using System.Runtime.CompilerServices;

namespace Communication.Utils
{
    public static class ConverterExtensions
    {
        public static T ConvertTo<T>(this byte[] src) where T : struct, IComparable, IEquatable<T>, IConvertible
        {
            return Unsafe.ReadUnaligned<T>(ref src[0]);
        }
    }
}
