using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        #region 컴파일러 최적화
        //volatile static bool _stop = false; // volatile -> 휘발성 이므로 최적화 x / c#에서는 다른 내용 -> 캐시를 무시하고 최신값을 가져와라

        //static void ThreadMain()
        //{
        //    Console.WriteLine("쓰레드 시작");

        //    //if (_stop == false) // 아래 코드와 동일한 내용
        //    //{
        //    //    while (true)
        //    //    {

        //    //    }
        //    //}

        //    while (_stop == false)
        //    {
        //        // 누군가가 stop 신호를 해주기를 기다린다. -> Release로 변경시 무한루프 -> 최적화 시에는 멀티스레드가 아님
        //    }

        //    Console.WriteLine("쓰레드 종료");
        //}
        #endregion

        static void Main(string[] args)
        {
            #region 캐시 이론
            // 5 * 5 배열
            // [][][][][][] [][][][][] [][][][][] [][][][][] [][][][][][]

            int[,] arr = new int[10000, 10000];

            {
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; y++)
                    for (int x = 0; x < 10000; x++)
                        arr[y, x] = 1;
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(y, x) 순서 걸린 시간 {end - now}");
            }

            {
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; y++)
                    for (int x = 0; x < 10000; x++)
                        arr[x, y] = 1;
                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(x, y) 순서 걸린 시간 {end - now}");
            }
            #endregion

            #region 컴파일러 최적화
            //    Task t = new Task(ThreadMain);
            //    t.Start();

            //    Thread.Sleep(1000); //ms 단위

            //    _stop = true;

            //    Console.WriteLine("Stop 호출");
            //    Console.WriteLine("종료 대기중");

            //    t.Wait();

            //    Console.WriteLine("종료 성공");
            #endregion
        }
    }
}
