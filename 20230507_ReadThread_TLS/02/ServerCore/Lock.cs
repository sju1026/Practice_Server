using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerCore
{
    #region 재귀적락 불가
    /*
    // 재귀적 락을 허용할지 (No)
    // SpinLock 정책 (5000번 -> yield)
    class Lock
    {
        const int EMPTY_FLAG = 0x00000000;
        const int WRITE_MASK = 0x7FFF0000;
        const int READ_MASK = 0x0000FFFF;
        const int MAX_SPIN_COUNT = 5000;


        // [Unused(1)] [WriteThread(15비트)] [HeadCount(16비트)] 
        int _flag = EMPTY_FLAG;

        public void WriteLock()
        {
            // 아무도 WriteLock or ReadLock을 획득하고 있지 않을 때, 경합해서 소유권을 얻는다
            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                        return;
                    // 시도를 해서 성공하면
                    
                    //if (_flag == EMPTY_FLAG)
                    //    _flag = desired;
                    
}

Thread.Yield();
            }
        }

        public void WriteUnlock()
        {
            Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }

        public void ReadLock()
        {
            // 아무도 WriteLock을 획득하고 있지 않으면, ReadCount를 1늘린다.
            while (true)
            {
                for(int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    int expected = (_flag & READ_MASK); // A B 동시에 접속 A(0) B(0)

                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected) // A(0) => (1) B(0) => (1) -> 한사람이라도 성공한경우 다른 조건이 불가능
                        return;

                    //if ((_flag & WRITE_MASK) == 0)
                    //{
                    //    _flag = _flag + 1;
                    //    return;
                    //}
                    
                }
                Thread.Yield();
            }
        }

        public void ReadUnlock()
        {
            Interlocked.Decrement(ref _flag);
        }
    }
    */
    #endregion

    #region 재귀적 락 가능
    class Lock
    {
        // 재귀적 락 허용 (Yes) WriteLock -> WriteLock OK, WriteLock -> ReadLock Ok
        const int EMPTY_FLAG = 0x00000000;
        const int WRITE_MASK = 0x7FFF0000;
        const int READ_MASK = 0x0000FFFF;
        const int MAX_SPIN_COUNT = 5000;


        // [Unused(1)] [WriteThread(15비트)] [ReadCount(16비트)] 
        int _flag = EMPTY_FLAG;
        int _writeCount = 0;

        public void WriteLock()
        {
            // 동일 스레드가 WriteLock을 이미 획득하고 있는지 확인
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                _writeCount++;
                return;
            }

            // 아무도 WriteLock or ReadLock을 획득하고 있지 않을 때, 경합해서 소유권을 얻는다
            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        _writeCount = 1;
                        return;
                    }
                    // 시도를 해서 성공하면
                    
                    //if (_flag == EMPTY_FLAG)
                    //    _flag = desired;
                    
                }

                Thread.Yield();
            }
        }

        public void WriteUnlock()
        {
            int lockCount = --_writeCount;
            if (lockCount == 0)
                Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }

        public void ReadLock()
        {
            int lockThreadId = (_flag & WRITE_MASK) >> 16;
            if (Thread.CurrentThread.ManagedThreadId == lockThreadId)
            {
                Interlocked.Increment(ref _flag);
                return;
            }

            // 아무도 WriteLock을 획득하고 있지 않으면, ReadCount를 1늘린다.
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    int expected = (_flag & READ_MASK); // A B 동시에 접속 A(0) B(0)
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected) // A(0) => (1) B(0) => (1) -> 한사람이라도 성공한경우 다른 조건이 불가능
                        return;

                    
                    //if ((_flag & WRITE_MASK) == 0)
                    //{
                    //    _flag = _flag + 1;
                    //    return;
                    //}
                    
                }
                Thread.Yield();
            }
        }

        public void ReadUnlock()
        {
            Interlocked.Decrement(ref _flag);
        }
    }
    #endregion
}
