using System.Runtime.CompilerServices;
using System.Text;

namespace Communication.Test.Message
{
    public class MessageTestCase
    {
        [Fact]
        public void MessageSerializeTestCase()
        {
            var cellCodeStr = "X4A52998";
            var barCodeStr = "9999";
            var position = 12;
            var barCodeLength = 18;
            var demoMessage = new LitEndianDemoMessage();

            demoMessage.Position.SetValue(12);
            demoMessage.CellCode.SetValue(cellCodeStr);
            int length = demoMessage.BarCode.Length;
            for (int i = 0; i < length; i++)
            {
                var barCode = new LitEndianBarCode();
                barCode.Length.SetValue(18);
                barCode.Str.SetValue(barCodeStr);
                demoMessage.BarCode[i] = barCode;
            }
            var buffer = demoMessage.Serialize();

            var target = demoMessage.CreateBuffer();
            Unsafe.WriteUnaligned(ref target[0], position);
            Unsafe.WriteUnaligned(ref target[22], barCodeLength);
            Unsafe.WriteUnaligned(ref target[44], barCodeLength);
            Unsafe.WriteUnaligned(ref target[66], barCodeLength);
            var cellCodeBuff = Encoding.ASCII.GetBytes(cellCodeStr);
            var barCodeBuff = Encoding.ASCII.GetBytes(barCodeStr);
            cellCodeBuff.CopyTo(target, 4);
            barCodeBuff.CopyTo(target, 26);
            barCodeBuff.CopyTo(target, 48);
            barCodeBuff.CopyTo(target, 70);
            Assert.True(target.SequenceEqual(buffer));
        }

        [Fact]
        public void MessageSerializeWithBigEndianTestCase()
        {
            var cellCodeStr = "X4A52998";
            var barCodeStr = "9999";
            var position = 0x0c00_0000;
            var barCodeLength = 0x1200_0000;
            var demoMessage = new BigEndianDemoMessage();

            demoMessage.Position.SetValue(12);
            demoMessage.CellCode.SetValue(cellCodeStr);
            int length = demoMessage.BarCode.Length;
            for (int i = 0; i < length; i++)
            {
                var barCode = new BigEndianBarCode();
                barCode.Length.SetValue(18);
                barCode.Str.SetValue(barCodeStr);
                demoMessage.BarCode[i] = barCode;
            }
            var buffer = demoMessage.Serialize();

            var target = demoMessage.CreateBuffer();
            Unsafe.WriteUnaligned(ref target[0], position);
            Unsafe.WriteUnaligned(ref target[22], barCodeLength);
            Unsafe.WriteUnaligned(ref target[44], barCodeLength);
            Unsafe.WriteUnaligned(ref target[66], barCodeLength);
            var cellCodeBuff = Encoding.ASCII.GetBytes(cellCodeStr);
            var barCodeBuff = Encoding.ASCII.GetBytes(barCodeStr);
            cellCodeBuff.CopyTo(target, 4);
            barCodeBuff.CopyTo(target, 26);
            barCodeBuff.CopyTo(target, 48);
            barCodeBuff.CopyTo(target, 70);
            Assert.True(target.SequenceEqual(buffer));
        }

        [Fact]
        public void MessageSerializeWithBooleanSignalMessageTest()
        {
            var target = new byte[]
            {
                0b11,
                0b0,
                0b1100
            };
            var message=new BooleanSignalDemoMessage();

            message.IsMove=true;
            message.IsFinished=true;
            message.IsStartTest=true;
            message.IsStopTest=true;

            var source = message.Serialize();

            Assert.Equal(target, source);

            target =
            [
                0,0,0b0100
            ];
            message.IsMove = false;
            message.IsFinished = false;
            message.IsStartTest = false;
            source = message.Serialize();

            Assert.Equal(target, source);
        }
    }
}
