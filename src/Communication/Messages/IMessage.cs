namespace Communication.Messages
{
    public interface IMessage
    {
        byte[] Serialize();

        void Deserialize(in byte[] buffer);
    }
}
