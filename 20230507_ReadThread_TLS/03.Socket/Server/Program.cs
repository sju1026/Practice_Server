using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    class Packet
    {
        public ushort size;
        public ushort packetId;
    }
    class GameSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            // Packet packet = new Packet() { size = 100, packetId = 10 };

            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            //byte[] buffer = BitConverter.GetBytes(packet.size);
            //byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
            //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);


            // Session 내부에 Send를 생성할 경우 모든 유저에 복사를 해야하므로 무수히 많은 수의 Send가 일어난다 => 외부에서 한번만 만들어 루프를 이용해 사용

            // 다양한 유저한테 보내기 때문에 Clear는 불가능 => 일회용
            // Send(sendBuff);
            Thread.Sleep(5000);
            Disconnect();
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
            Console.WriteLine($"RecvPakcetid : {id}, Size {size}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
    class Program
    {
        static Listener _listener = new Listener();
        //static void OnAcceptHandler(Socket clientSocket)
        //{
        //    try
        //    {
        //        GameSession session = new GameSession();
        //        session.Start(clientSocket);

        //        byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome To MMORPG Server !");
        //        session.Send(sendBuff);

        //        Thread.Sleep(100);

        //        session.Disconnect();
        //        session.Disconnect();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}
        static void Main(string[] args)
        {
            // DNS Domain Name System
            // 172.1.2.3 => 문제 : 서버를 이전할 경우 ip주소가 변경될 수 있음=> 자동처리 x
            // 도메인을 등록할 경우 => 해당 IP탐색 가능
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.init(endPoint, () => { return new GameSession(); });
            Console.WriteLine("Listening...");

            while (true)
            {
                ;
            }

        }
    }
}
