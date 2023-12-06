using Communication.Serialization;
using Communication.Signals;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace Communication.Messages
{
    public abstract class MessageBase : IMessage
    {
        public virtual byte[] CreateBuffer()
        {
            if (!_bufferSize.TryGetValue(this.GetType(), out var size))
            {
                size = CalculateBufferSize();
                _bufferSize.TryAdd(this.GetType(), size);
            }
            return new byte[size];
        }

        protected static readonly ConcurrentDictionary<Type, int> _bufferSize = new();

        protected int CalculateBufferSize()
        {
            return SignalSizeSum(this);
        }

        public byte[] Serialize()
        {
            BeforeSerialize();

            var buffer = CreateBuffer();
            var cursor = SignalSerialize(this, buffer, 0);

            Debug.Assert(cursor == buffer.Length
                , "Frame length is not correct.");

            AfterSerialize();

            return buffer;
        }

        public void Deserialize(in byte[] buffer)
        {
            BeforeDeserialize();

            var cursor = SignalDeserialize(this, buffer, 0);

            Debug.Assert(cursor == buffer.Length
                , "Frame length is not correct.");

            AfterDeserialize();
        }

        protected virtual void BeforeSerialize() { }

        protected virtual void AfterSerialize() { }

        protected virtual void BeforeDeserialize() { }

        protected virtual void AfterDeserialize() { }

        private static int SignalSizeSum(object @this)
        {
            var thisType = @this.GetType();
            if (thisType.IsValueType)
            {
                throw new InvalidDataException($"{thisType.FullName} is not a signal");
            }

            var size = 0;
            foreach (var property in thisType.GetProperties())
            {
                var attribute = property.GetCustomAttribute<MessageIncludeAttribute>();
                if (attribute != null)
                {
                    var obj = property.GetValue(@this) ?? throw new NullReferenceException();
                    if (obj is ISignal signal)
                    {
                        size += signal.Size;
                    }
                    else if (obj is IEnumerable elements)
                    {
                        foreach (var element in elements)
                        {
                            size += SignalSizeSum(element);
                        }
                    }
                    else
                    {
                        size += SignalSizeSum(obj);
                    }
                }
            }
            return size;
        }

        private static int SignalSerialize(object @this, in byte[] buffer, int offset)
        {
            var thisType = @this?.GetType() ?? throw new ArgumentNullException(nameof(@this));
            if (thisType.IsValueType)
            {
                throw new InvalidDataException($"{thisType.FullName} is not a signal");
            }

            foreach (var property in thisType.GetProperties())
            {
                var attribute = property.GetCustomAttribute<MessageIncludeAttribute>();
                if (attribute != null)
                {
                    var obj = property.GetValue(@this) ?? throw new NullReferenceException();
                    if (obj is ISignal signal)
                    {
                        offset += signal.Write(buffer, offset);
                    }
                    else if (obj is IEnumerable elements)
                    {
                        foreach (var element in elements)
                        {
                            offset = SignalSerialize(element, buffer, offset);
                        }
                    }
                    else
                    {
                        offset = SignalSerialize(obj, buffer, offset);
                    }
                }
            }

            return offset;
        }

        private static int SignalDeserialize(object @this, in byte[] buffer, int offset)
        {
            var thisType = @this.GetType();
            if (thisType.IsValueType)
            {
                throw new InvalidDataException($"{thisType.FullName} is not a signal");
            }

            foreach (var property in thisType.GetProperties())
            {
                var attribute = property.GetCustomAttribute<MessageIncludeAttribute>();
                if (attribute != null)
                {
                    var obj = property.GetValue(@this) ?? throw new NullReferenceException();
                    if (obj is ISignal signal)
                    {
                        offset += signal.Read(buffer, offset);
                    }
                    else if (obj is IEnumerable elements)
                    {
                        foreach (var element in elements)
                        {
                            offset = SignalDeserialize(element, buffer, offset);
                        }
                    }
                    else
                    {
                        offset = SignalDeserialize(obj, buffer, offset);
                    }
                }
            }
            return offset;
        }
    }
}
