using System.Net.Sockets;
using System.Text;

namespace Common
{
    public class PacketReader : BinaryReader
    {
        private readonly NetworkStream _ns;

        public PacketReader(NetworkStream ns) : base(ns) 
        {
            _ns = ns; 
        }

        public Opcode ReadOpcode()
        {
            return (Opcode)ReadByte();
        }

        public string ReadContent()
        {
            int msgLength = ReadInt32();

            byte[] buffer = new byte[msgLength];
            _ns.Read(buffer, 0, msgLength);

            return Encoding.UTF8.GetString(buffer);
        }
    }
}
