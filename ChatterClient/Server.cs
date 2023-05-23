using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatterClient
{
    class Server
    {
        public PacketReader PacketReader;

        public event Action ConnectedEvent;
        public event Action UserDisconnectedEvent;
        public event Action RecieveMessageEvent;

        TcpClient _client;

        public Server()
        {
            _client = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            if (!_client.Connected)
            {
                _client.Connect("127.0.0.1", 1524);

                PacketReader = new PacketReader(_client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    PacketBuilder connectPacket = new PacketBuilder();
                    connectPacket.WriteOpcode(Opcode.Connect);
                    connectPacket.WriteContent(username);
                    _client.Client.Send(connectPacket.GetPacketBytes());
                }

                ReadPackets();
            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {

                    switch (PacketReader.ReadOpcode())
                    {
                        case Opcode.ClientAdd:
                            ConnectedEvent?.Invoke();
                            break;

                        case Opcode.ClientRemove:
                            UserDisconnectedEvent?.Invoke();
                            break;

                        case Opcode.TextMessage:
                            RecieveMessageEvent?.Invoke();
                            break;

                        default:
                            Console.WriteLine("Error");
                            break;
                    }
                }
            });

        }

        public void SendMessageToServer(string message)
        {
            PacketBuilder messagePacket = new PacketBuilder();
            messagePacket.WriteOpcode(Opcode.TextMessage);
            messagePacket.WriteContent(message);
            _client.Client.Send(messagePacket.GetPacketBytes());
        }
    }
}
