using System.Text;

namespace Common
{
    public class PacketBuilder
    {
        private readonly MemoryStream _ms;

        public PacketBuilder()
        {
            _ms = new MemoryStream();
        }

        public void WriteOpcode(Opcode opcode)
        {
            _ms.WriteByte((byte)opcode);
        }

        public void WriteContent(string content)
        {
            int contentLength = content.Length;
            _ms.Write(BitConverter.GetBytes(contentLength));
            _ms.Write(Encoding.UTF8.GetBytes(content));
        }

        public byte[] GetPacketBytes()
        {
            return _ms.ToArray();
        }
    }
}
