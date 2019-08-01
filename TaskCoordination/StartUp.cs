using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskCoordination
{
    class StartUp
    {      

        static void Main(string[] args)
        {
            var semaphore = new SemaphoreSlim(2, 10);

            for (int i = 0; i < 20; ++i)
            {
                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine($"Entering task {Task.CurrentId}.");
                    semaphore.Wait(); // ReleaseCount--
                    Console.WriteLine($"Processing task {Task.CurrentId}.");
                });
            }

            while (semaphore.CurrentCount <= 2)
            {
                Console.WriteLine($"Semaphore count: {semaphore.CurrentCount}");
                Console.ReadKey();
                semaphore.Release(2); // ReleaseCount += n
            }
        }

        private static void BarrierTest()
        {
            var water = Task.Factory.StartNew(TaskCoordination.BarrierTest.Water);
            var cup = Task.Factory.StartNew(TaskCoordination.BarrierTest.Cup);

            var tea = Task.Factory.ContinueWhenAll(new[] { water, cup }, tasks =>
            {
                Console.WriteLine("Enjoy cup of tea");
            });

            tea.Wait();
        }
    }
}
