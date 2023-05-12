using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    // Lock 구현 : 예 ) 존버, 랜덤, 갑질
    // SpinLock 구현 => 동시에 접근 = 공동사용 = 동작분할에 의한 오류 = 원자성 문제 발생
    class SpinLock // lock을 확보할 때 까지 무한루프 => 중요
    {
        volatile int _locked = 0;
        public void Acquire()
        {
            while (true) {
                //int original = Interlocked.Exchange(ref _locked, 1); // 하나의 스택 즉 스레드에서만 사용되는 _locked이기 때문에 사용 가능
                //if (original == 0)
                //    break;

                // CAS Compare-And-Snap
                int expected = 0;
                int desired = 1;
                if (Interlocked.CompareExchange(ref _locked, desired, expected) == expected) // location값과 comparand값과 비교 후 같다면 value를 할당
                    break;

                // 쉬다 온다 => 무한정 무한루프는 회피할 수 있다.  Context Switching
                //Thread.Sleep(1); // 무조건 휴식 => 무조건 1ms 정도 쉬고 싶다. => OS의 Scheduler가 처리
                //Thread.Sleep(0); // 조건부 양보 => 자신보다 우선순위가 낮은 애들한테는 양보 불가 -> 우선순위가 자신보다 같거나 높은 쓰레드가 없으면 다시 본인
                Thread.Yield(); // 관대한 양보 => 관대하게 양보할테니 지금 실행이 가능한 스레드가 있으면 실행 => 실행 가능한 애가 없으면 남은시간 소진
            }
        }

        public void Relese()
        {
            _locked = 0; // Acquire가 돌아가는 동안 점유하고 있다는 의미이기 때문에 탈출할 때만 사용
        }
    }
    
    class Program
    {
        static int _num = 0;
        static SpinLock _lock = new SpinLock();

        static void Thread_1()
        {
            for (int i = 0; i< 10000000; i++)
            {
                _lock.Acquire();
                _num++;
                _lock.Relese();
            }
        }

        static void Thread_2()
        {
            for (int i = 0; i < 10000000; i++)
            {
                _lock.Acquire();
                _num--;
                _lock.Relese();
            }
        }

        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(_num);
        }
    }
}
