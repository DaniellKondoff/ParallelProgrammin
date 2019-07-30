using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataSharingAndSync
{
    class StartUp
    {
        static SpinLock sl2 = new SpinLock(true);
        static ReaderWriterLockSlim padlock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        static void Main(string[] args)
        {
            var tasks = new List<Task>();
            var ba = new BankAccount();
            var ba2 = new BankAccount();
            int x = 0;          

            Console.WriteLine($"Final balance is {ba.Balance}");
            Console.WriteLine($"Final balance is {ba2.Balance}");
        }

        private static void ReaderWriteLockSLimMethod(List<Task> tasks, int x)
        {
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    //padlock.EnterReadLock();
                    padlock.EnterUpgradeableReadLock();

                    if (i % 2 == 0)
                    {
                        padlock.EnterWriteLock();
                        x++;
                        padlock.ExitWriteLock();
                    }

                    // can now read
                    Console.WriteLine($"Entered read lock, x = {x}, pausing for 5sec");
                    Thread.Sleep(5000);

                    //padlock.ExitReadLock();
                    padlock.ExitUpgradeableReadLock();

                    Console.WriteLine($"Exited read lock, x = {x}.");
                }));
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException ae)
            {
                ae.Handle(e =>
                {
                    Console.WriteLine(e);
                    return true;
                });
            }

            Random random = new Random();

            while (true)
            {
                Console.ReadKey();
                padlock.EnterWriteLock();
                Console.WriteLine("Write lock acquired");
                int newValue = random.Next(10);
                x = newValue;
                Console.WriteLine($"Set x = {x}");
                padlock.ExitWriteLock();
                Console.WriteLine("Write lock released");
            }
        }

        private static void Mutex(List<Task> tasks, BankAccount ba, BankAccount ba2)
        {
            Mutex mutex = new Mutex();
            Mutex mutex2 = new Mutex();

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool haveLock = mutex.WaitOne();
                        try
                        {
                            ba.Deposit(100);
                        }
                        finally
                        {
                            if (haveLock) mutex.ReleaseMutex();
                        }
                    }
                }));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool haveLock = mutex2.WaitOne();
                        try
                        {
                            ba2.Deposit(100);
                        }
                        finally
                        {
                            if (haveLock) mutex2.ReleaseMutex();
                        }
                    }
                }));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int k = 0; k < 1000; k++)
                    {
                        bool haveLock = WaitHandle.WaitAll(new[] { mutex, mutex2 });
                        try
                        {
                            ba.Tramsfer(ba2, 100);
                        }
                        finally
                        {
                            if (haveLock)
                            {
                                mutex.ReleaseMutex();
                                mutex2.ReleaseMutex();
                            };
                        }
                    }
                }));
            }
        }

        private static void SpinLock(List<Task> tasks, BankAccount ba)
        {
            SpinLock sl = new SpinLock();

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool lockTaken = false;
                        try
                        {
                            sl.Enter(ref lockTaken);
                            ba.Deposit(100);
                        }
                        finally
                        {
                            if (lockTaken)
                                sl.Exit();
                        }
                    }
                }));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool lockTaken = false;
                        try
                        {
                            sl.Enter(ref lockTaken);
                            ba.Withdraw(100);
                        }
                        finally
                        {
                            if (lockTaken)
                                sl.Exit();
                        }
                    }
                }));
            }
        }

        public static void SpinLockRecursion(int x)
        {
            bool lockTaken = false;
            try
            {
                sl2.Enter(ref lockTaken);
            }
            catch (LockRecursionException e)
            {
                Console.WriteLine("Excetion: " + e);
            }
            finally
            {
                if (lockTaken)
                {
                    Console.WriteLine("Took a lock, x = " + x);
                    SpinLockRecursion(x - 1);
                    sl2.Exit();
                }
                else
                {
                    Console.WriteLine($"Failed to take a lock, x = {x}");
                }
            }
        }
    }
}
