using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cleverence
{
    using System.Threading;

    public static class Server
    {
        private static int _count;
        private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public static int GetCount()
        {
            Console.WriteLine($"[{Time()}] [Reader] Вход в GetCount()");
            _lock.EnterReadLock();
            try
            {
                Console.WriteLine($"[{Time()}] [Reader] Чтение count = {_count}");
                return _count;
            }
            finally
            {
                Console.WriteLine($"[{Time()}] [Reader] Выход из GetCount()");
                _lock.ExitReadLock();
            }
        }

        public static void AddToCount(int value)
        {
            Console.WriteLine($"[{Time()}] [Writer] Пытается войти в AddToCount()");
            _lock.EnterWriteLock();
            try
            {
                Console.WriteLine($"[{Time()}] [Writer] Пишет count + {value}");
                _count += value;
            }
            finally
            {
                Console.WriteLine($"[{Time()}] [Writer] Завершил AddToCount()");
                _lock.ExitWriteLock();
            }
        }
        private static string Time() => DateTime.Now.ToString("HH:mm:ss.fff");
    }

}
