using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    // ReaderWriteLock 구현 연습

    class Program
    {
        //static volatile int count = 0;
        //static Lock _lock = new Lock();

        // TLS
        static ThreadLocal<string> ThreadName = new ThreadLocal<string>(() => { return $"My Name Is {Thread.CurrentThread.ManagedThreadId}"; }); // valueFactory추가 가능

        //static string ThreadName;

        static void WHoAmI()
        {
            bool repeat = ThreadName.IsValueCreated;
            if (repeat)
                Console.WriteLine(ThreadName.Value + "(repeat)");
            else
                Console.WriteLine(ThreadName.Value);
            // ThreadName.Value = $"My Name Is {Thread.CurrentThread.ManagedThreadId}";

            //Thread.Sleep(1000);

            //Console.WriteLine(ThreadName.Value);
        }

        static void Main(string[] args)
        {
            #region Lock
            //Task t1 = new Task(delegate () 
            //{ 
            //    for (int i = 0; i < 100000; i++)
            //    {
            //        _lock.WriteLock();

            //        count++;
            //        _lock.WriteUnlock();

            //    }
            //});

            //Task t2 = new Task(delegate ()
            //{
            //    for (int i = 0; i < 100000; i++)
            //    {
            //        _lock.WriteLock();
            //        count--;
            //        _lock.WriteUnlock();

            //    }
            //});

            //t1.Start();
            //t2.Start();

            //Task.WaitAll(t1, t2);

            //Console.WriteLine(count);
            #endregion

            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(3, 3);
            Parallel.Invoke(WHoAmI, WHoAmI, WHoAmI, WHoAmI, WHoAmI, WHoAmI);

            ThreadName.Dispose();
        }
    }
}

// Thread Local Storage = TLS
// 모든 부분에 락을 할 경우 한 곳에 몰리는 경우 문제가 생긴다.
// Heap영역과 Static 영역은 공유하나 Stack 영역은 각자 스레드 별 사용 => 공유하는 영역도 각자 사용하는 영역으로 분리를 해주자 => TLS
// 많은 일들이 큐에 들어있을 때 일정 갯수만큼 가져와 처리하는 것 => 락을 걸지 않더라도 분할 처리 가능
// [ JobQueue ] => TLS공간에 가져와서 처리 => 락 한번안에 최대한 많은 일거리를 처리 가능 / 스레드의 이름, id 즉 고유의 정보를 처리할 때도 사용
