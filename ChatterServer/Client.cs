using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatterServer
{
    public class Client
    {
        public Guid Guid { get; set; }
        public string Username { get; set; }
        public TcpClient TcpClient { get; set; }

        private readonly PacketReader _pr;

        public Client(TcpClient tcpClient)
        {
            Guid = Guid.NewGuid();
            TcpClient = tcpClient;

            _pr = new PacketReader(TcpClient.GetStream());

            _pr.ReadOpcode();//discard init opcode
            Username = _pr.ReadContent();

            Task.Run(() =>
            {
                ProcessPackets();
            });
        }

        private void ProcessPackets()
        {
            while (true)
            {
                try
                {
                    switch (_pr.ReadOpcode())
                    {
                        case Opcode.TextMessage:
                            ChatterServer.BroadcastMessage($"[{Username}]: {_pr.ReadContent()}");
                            break;


                        default:
                            Console.WriteLine("Error");
                            break;
                    }
                }
                catch (Exception)
                {
                    ChatterServer.BroadcastDisconnection(this);
                    TcpClient.Close();

                    break;
                }
            }
        }
    }
}
