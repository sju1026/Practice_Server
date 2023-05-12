using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    #region Locks
    //class Lock 
    //{
    //    // bool <- 커널
    //    // AutoResetEvent _available = new AutoResetEvent(true); // 문을 닫는 행위는 자동으로 : ex)톨게이트 <-> ManualResetEvent : 수동으로 잠구는 것 -> 속도가 느리다
    //    ManualResetEvent _available = new ManualResetEvent(false); // 정상적인 실행 x -> 원자성문제 : 스레드를 재가동하기 위해 사용하는 것에는 유용

    //    public void Acquire()
    //    {
    //        _available.WaitOne(); // 입장을 시도
    //        _available.Reset(); //  bool => false / 문을 닫는 기능
    //    }

    //    public void Relese()
    //    {
    //        _available.Set(); // flag = true
    //    }
    //}
    #endregion
    // 1. 근성 / 양보 / 갑질 => 3가지 방법 / 여러가지 방법을 혼합하여 사용
    class Program
    {
        #region Lock
        //static int _num = 0;
        
        //// bool => AutoResetEvent 주로 사용

        //// int => counting / ThreadId => 추가비용
        //static Mutex _lock = new Mutex(); // 커널단위까지 가기 때문에 시간이 오래 걸림

        //static void Thread_1()
        //{
        //    for (int i = 0; i < 100000; i++)
        //    {
        //        _lock.WaitOne();
        //        _lock.WaitOne();

        //        _num++;
        //        _lock.ReleaseMutex();
        //        _lock.ReleaseMutex();
        //    }
        //}

        //static void Thread_2()
        //{
        //    for (int i = 0; i < 100000; i++)
        //    {
        //        _lock.WaitOne();
        //        _num--;
        //        _lock.ReleaseMutex();
        //    }
        //}
        #endregion

        //Monitor => 상호배제 / 한번에 한 사람만 사용가능
        static object _lock = new object();
        static SpinLock _lock2 = new SpinLock(); // 근성 양보 혼합
        // static Mutex _lock3 = new Mutex(); // 같은 프로그램이 아니더라도 동기화 가능 / 시간이 오래 걸림

        // RWLock ReaderWriterLock
        static ReaderWriterLockSlim _lock3 = new ReaderWriterLockSlim(); // Slim 최신버전
        // 직접 만든다.

        // [ ] [ ] [ ] / [ ] [ ] 
        class Reward
        {

        }

        // RWLock ReaderWriterLock
        // 낮은 확률로 생기는 이벤트 때문에 만들어야 됨
        static Reward GetRewardById(int id)
        {
            _lock3.EnterReadLock();

            _lock3.ExitReadLock();

            //lock (_lock)
            //{

            //}
            return null;
        }

        // 0.000001%
        static void AddReward(Reward reward)
        {
            _lock3.EnterWriteLock();

            _lock3.ExitWriteLock();
            lock (_lock) 
            {

            }
        }

        static void Main(string[] args)
        {
            #region Lock
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(_num);
            #endregion

            lock (_lock)
            {

            }
            #region
            /*
            bool lockTaken = false;
            try
            {
                _lock2.Enter(ref lockTaken);
            }
            finally
            {
                if (lockTaken)
                 _lock2.Exit();
            }
            */
            #endregion
        }
    }
}
