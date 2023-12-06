using Communication.Messages;
using Communication.Serialization;
using Communication.Signals;
using Communication.Signals.Endians;

namespace Communication.Test.Message
{
    public class BigEndianBarCode
    {
        [MessageInclude]
        public NumericSignal<int, BigEndian> Length { get; set; } = new();

        [MessageInclude]
        public FixedStringSignal Str { get; set; } = new(18);
    }

    public class BigEndianDemoMessage : MessageBase
    {
        [MessageInclude]
        public NumericSignal<int, BigEndian> Position { get; set; } = new();

        [MessageInclude]
        public FixedStringSignal CellCode { get; set; } = new(18);

        [MessageInclude]
        public BigEndianBarCode[] BarCode { get; set; } = new BigEndianBarCode[3];
    }
}
