using Communication.Messages;
using Communication.Serialization;
using Communication.Signals;

namespace Communication.Test.Message
{
    public class LitEndianBarCode
    {
        [MessageInclude]
        public NumericSignal<int> Length { get; set; } = new();

        [MessageInclude]
        public FixedStringSignal Str { get; set; } = new(18);
    }

    public class LitEndianDemoMessage : MessageBase
    {
        [MessageInclude]
        public NumericSignal<int> Position { get; set; } = new();

        [MessageInclude]
        public FixedStringSignal CellCode { get; set; } = new(18);

        [MessageInclude]
        public LitEndianBarCode[] BarCode { get; set; } = new LitEndianBarCode[3];
    }
}
