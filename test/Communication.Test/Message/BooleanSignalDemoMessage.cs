using Communication.Messages;
using Communication.Serialization;
using Communication.Signals;
using Communication.Utils;

namespace Communication.Test.Message
{
    public class BooleanSignalDemoMessage : MessageBase
    {
        [MessageInclude]
        public NumericSignal<short> State1 { get; set; } = new();

        [MessageInclude]
        public NumericSignal<byte> State2 { get; set; } = new();

        public bool IsMove
        {
            get => BooleanAccessor.Get(State1, 0);
            set => BooleanAccessor.Set(State1, 0, value);
        }

        public bool IsStartTest
        {
            get => BooleanAccessor.Get(State1, 1);
            set => BooleanAccessor.Set(State1, 1, value);
        }

        public bool IsStopTest
        {
            get => BooleanAccessor.Get(State2, 2);
            set => BooleanAccessor.Set(State2, 2, value);
        }

        public bool IsFinished
        {
            get => BooleanAccessor.Get(State2, 3);
            set => BooleanAccessor.Set(State2, 3, value);
        }
    }
}
