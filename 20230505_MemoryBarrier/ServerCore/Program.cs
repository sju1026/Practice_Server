using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    #region Import Memory Barrier
    // 메모리 베리어 => 선을 그어서 강제로 순서대로 실행되게 하는 방법
    // A) 코드 재배치 억제
    // B) 가시성 => volatile로도 사용 가능 / 동기화 기능 Lock, Atomic기능

    // 1) Full Memory Barrier (ASM MFENCE, C# Thread.MemoryBarrier) : Store / Load 둘다 막는다
    // 2) Store Memory Barrier (ASM SFENCE) : Store만 막는다
    // 3) Load Memory Barrier (ASM LFENCE) : Load만 막는다
    #endregion

    #region Import InterLocked
    // 경합조건
    // 100000번으로 돌릴경우 0이 아닌 다른 숫자 volatile => 의미 없음 = 가시성 문제 x => Interlocked : 원자적으로 산수를 한다
    // Interlocked : All or Nothing, 순서 보장이 된다. 임계영역
    // atomic = 원자성

    // 골드 -= 100;
    // 서버다운 => 검이 없음 Item거래시 복사될 경우도 있음
    // 인벤 += 검
    #endregion

    class Program
    {
        #region Field MemoryBarrier
        //static int x = 0;
        //static int y = 0;
        //static int r1 = 0;
        //static int r2 = 0;

        //// Thread에서 서로의 값이 연관성없기 때문에 순서를 변경하여 0이 출력되게 된 것이다. => 하드웨어의 최적화 / 싱글 스레드에서는 상관 없지만 멀티에서는 문제
        //static void Thread_1()
        //{
        //    y = 1; //Store y

        //    Thread.MemoryBarrier();

        //    r1 = x; // Load x
        //}

        //static void Thread_2()
        //{
        //    x = 1; // Store x

        //    Thread.MemoryBarrier(); // 메모리 베리어로 인한 무한루프로 돌아가게 된다

        //    r2 = y; // Load y
        //}
        #endregion

        #region Barrier Ex
        //int _answer;
        //bool _complete;

        //void A()
        //{
        //    _answer = 123;
        //    Thread.MemoryBarrier(); // Barrier 1 Write 가시성 보장용
        //    _complete = true;
        //    Thread.MemoryBarrier(); // Barrier 2 Write
        //}

        //void B()
        //{
        //    Thread.MemoryBarrier(); // Barrier 3 Read
        //    if (_complete)
        //    {
        //        Thread.MemoryBarrier(); // Barrier 4 Read 최신 정보를 가져오기 위해
        //        Console.WriteLine(_answer);
        //    }
        //}
        #endregion

        #region DeadLock
        
        class FastLock // 미리 매핑을 한 후 사용, id를 사용하여 추적
        {
            public int id;
        }
        // 발생원인 호출하는것을 반대로 잡고 있기 때문, 정확하게 동시에 발생하야 deadlock 발생
        // 일반적으로는 crash를 낸 후 고치는 것이 대부분
        // 완전하게 막을수는 없으며 발생한 후 고치는 것이 더 쉽다
        class SessionManager
        {
           static object _lock = new object();

            public static void TestSession()
            {
                lock (_lock)
                {

                }
            }

            public static void Test()
            {
                lock (_lock)
                {
                    UserManager.TestUser();
                }
            }
        }

        class UserManager
        {
           static object _lock = new object();

            public static void Test()
            {

                lock (_lock)
                {
                    SessionManager.TestSession();
                }
            }

            public static void TestUser()
            {
                lock (_lock)
                {

                }
            }
        }
        #endregion

        #region Interlocked
        static int number = 0;
        static object _obj = new object();


        static void Thread_1()
        {

            for (int i = 0; i < 10000; i++)
            {
                SessionManager.Test();

                //lock (_obj)
                //{
                //    number++;
                //}
                #region Lock try/finally
                //try
                //{
                //    Monitor.Enter(_obj); // 출입을 막는 행위
                //    number++;

                //    return;
                //}
                //finally
                //{
                //    Monitor.Exit(_obj);
                //}
                #endregion

                #region Lock Normal
                // 상호배제 Mutual Exclusive => Single Thread와 유사하게 동작 가능 / 관리하기 어려워 진다
                // C++ : CriticalSection
                //Monitor.Enter(_obj); // 출입을 막는 행위
                //{
                //    number++;
                //    // return; // 무한루프 thread_2 무한대기 : DeadLock
                //    if (number == 10000)
                //    {
                //        Monitor.Exit(_obj); // exception시 문제
                //        return;
                //    }
                //}
                //// int afterValue = Interlocked.Increment(ref number); // 가시성 문제는 알아서 적용 / 단점 : 성능면에서 손해 <ref => 레퍼런스 즉 주소값으로 값을 넣는다 => 값을 접근한 순간 다른 곳에서 값을 접근하는 것 방지>
                //// 정수만 사용 가능하다는 단점

                //Monitor.Exit(_obj); // 사용 종료
                #endregion
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 10000; i++)
            {
                UserManager.Test();
                //lock (_obj)
                //{
                //    number--;
                //}
                //Monitor.Enter(_obj); // 앞에 사용이 끝날때까지 대기

                //number--;
                ////Interlocked.Decrement(ref number);
                //Monitor.Exit(_obj);
            }
        }
        #endregion

        static void Main(string[] args)
        {
            #region Main Memory Barrier
            //int count = 0;
            //while (true)
            //{
            //    count++;
            //    x = y = r1 = r2 = 0;

            //    Task t1 = new Task(Thread_1);
            //    Task t2 = new Task(Thread_2);

            //    t1.Start();
            //    t2.Start();

            //    Task.WaitAll(t1, t2);

            //    if (r1 == 0 && r2 == 0)
            //        break;
            //}

            //Console.WriteLine($"{count}번만에 빠져나옴!");
            #endregion

            #region Main Interlocked
            //// 쪼개질 수 없는 최소 단위 아래와 같이 사용할 경우 다른곳에서 접근시 문제 발생
            //int temp = number;
            //temp += 1;
            //number = temp;

            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();

            Thread.Sleep(100);

            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
            #endregion
        }
    }
}

//#region
//#endregion