using Common;
using System.Net;
using System.Net.Sockets;

namespace ChatterServer
{
    internal class ChatterServer
    {

        private static List<Client>? _clientList;
        private static TcpListener? _tcpListener;

        static void Main(string[] args)
        {
            _clientList = new List<Client>();

            _tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 1524);
            _tcpListener.Start();

            Console.WriteLine("Waiting for connections...");

            while (true)
            {
                Client c = new Client(_tcpListener.AcceptTcpClient());
                _clientList.Add(c);
                BroadcastConnection(c.Username);
            }
        }

        private static void Broadcast(byte[] data)
        {
            foreach (Client c in _clientList)
            {
                c.TcpClient.Client.Send(data);
            }
        }

        public static void BroadcastMessage(string message)
        {
            Console.WriteLine(message);

            PacketBuilder pb = new PacketBuilder();
            pb.WriteOpcode(Opcode.TextMessage);
            pb.WriteContent(message);

            Broadcast(pb.GetPacketBytes());
        }

        public static void BroadcastConnection(string name)
        {
            Console.WriteLine($"[{name}] has connected.");
            foreach (Client c in _clientList)
            {
                foreach (Client cNames in _clientList)
                {
                    PacketBuilder pb = new PacketBuilder();
                    pb.WriteOpcode(Opcode.ClientAdd);
                    pb.WriteContent(cNames.Username);
                    pb.WriteContent(cNames.Guid.ToString());

                    c.TcpClient.Client.Send(pb.GetPacketBytes());
                }
            }
        }

        public static void BroadcastDisconnection(Client c)
        {
            Console.WriteLine($"[{c.Username}] disconnected.");
            _clientList.Remove(c);

            PacketBuilder pb = new PacketBuilder();
            pb.WriteOpcode(Opcode.ClientRemove);
            pb.WriteContent(c.Guid.ToString());

            Broadcast(pb.GetPacketBytes());
        }
    }
}