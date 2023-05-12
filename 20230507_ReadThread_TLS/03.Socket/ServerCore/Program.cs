using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome To MMORPG Server !");
            Send(sendBuff);
            Thread.Sleep(1000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Client] {recvData}");
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
            IPAddress ipAddr =  ipHost.AddressList[0];
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
