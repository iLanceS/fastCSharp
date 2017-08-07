using System;
using fastCSharp;

namespace fastCSharp.demo.rawSocketListener
{
    class Program
    {
        /// <summary>
        /// 原始套接字监听
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Press ip and enter ");
            string ip = Console.ReadLine();
            if (ip.length() == 0) Console.WriteLine(ip = "192.168.1.100");
            try
            {
                using (fastCSharp.net.rawSocketListener rawSocket = new fastCSharp.net.rawSocketListener(getPacket, System.Net.IPAddress.Parse(ip)))
                {
                    if (rawSocket.IsDisposed)
                    {
                        if (rawSocket.LastException != null) Console.WriteLine(rawSocket.LastException.ToString());
                        Console.WriteLine("套接字监听失败，可能需要管理员权限运行。");
                    }
                    else
                    {
                        Console.WriteLine("Press quit to exit.");
                        while (Console.ReadLine() != "quit") ;
                        return;
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
            }
            Console.ReadKey();
        }
        /// <summary>
        /// 数据包处理
        /// </summary>
        /// <param name="data">数据包</param>
        private static void getPacket(subArray<byte> data)
        {
            fastCSharp.net.packet.ip ip4 = new fastCSharp.net.packet.ip(ref data);
            if (ip4.IsPacket)
            {
                switch (ip4.Protocol)
                {
                    case fastCSharp.net.packet.ip.protocol.Icmp:
                        fastCSharp.net.packet.icmp icmp = new fastCSharp.net.packet.icmp(ip4.Packet);
                        if (icmp.IsPacket)
                        {
                            Console.WriteLine(ip4.Source.toHex8() + " -> " + ip4.Destination.toHex8() + " " + ip4.Protocol.ToString() + " " + icmp.Type.ToString() + " " + icmp.Code.ToString());
                        }
                        else Console.WriteLine("Unknown");
                        break;
                    case fastCSharp.net.packet.ip.protocol.Igmp:
                        fastCSharp.net.packet.igmp igmp = new fastCSharp.net.packet.igmp(ip4.Packet);
                        if (igmp.IsPacket)
                        {
                            Console.WriteLine(ip4.Source.toHex8() + " -> " + ip4.Destination.toHex8() + " " + ip4.Protocol.ToString());
                        }
                        else Console.WriteLine("Unknown");
                        break;
                    case fastCSharp.net.packet.ip.protocol.Tcp:
                        fastCSharp.net.packet.tcp tcp = new fastCSharp.net.packet.tcp(ip4.Packet);
                        if (tcp.IsPacket)
                        {
                            Console.WriteLine(ip4.Source.toHex8() + ":" + ((ushort)tcp.SourcePort).toHex() + " -> " + ip4.Destination.toHex8() + ":" + ((ushort)tcp.DestinationPort).toHex() + " " + ip4.Protocol.ToString());
                        }
                        else Console.WriteLine("Unknown");
                        break;
                    case fastCSharp.net.packet.ip.protocol.Udp:
                        fastCSharp.net.packet.udp udp = new fastCSharp.net.packet.udp(ip4.Packet);
                        if (udp.IsPacket)
                        {
                            Console.WriteLine(ip4.Source.toHex8() + ":" + ((ushort)udp.SourcePort).toHex() + " -> " + ip4.Destination.toHex8() + ":" + ((ushort)udp.DestinationPort).toHex() + " " + ip4.Protocol.ToString());
                        }
                        else Console.WriteLine("Unknown");
                        break;
                    default:
                        Console.WriteLine(ip4.Source.toHex8() + " -> " + ip4.Destination.toHex8() + " " + ip4.Protocol.ToString());
                        break;
                }
            }
            else Console.WriteLine("Unknown");
            fastCSharp.net.rawSocketListener.FreeOnly(ref data);
        }
    }
}
