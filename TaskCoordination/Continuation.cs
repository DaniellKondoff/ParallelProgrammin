using System;
using System.Threading.Tasks;

namespace TaskCoordination
{
    public static class Continuation
    {

        public static void BasicContinuation()
        {
            var task = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Boilig the water");
            });

            var task2 = task.ContinueWith(t =>
            {
                Console.WriteLine($"Completed task {t.Id}, pour water into cup.");
            });

            task2.Wait();
        }

        public static void ContinuationAll()
        {
            var task = Task.Factory.StartNew(() => "Task 1");
            var task2 = Task.Factory.StartNew(() => "Task 2");

            Task.Factory.ContinueWhenAll(new[] { task, task2 }, tasks =>
            {
                Console.WriteLine("Task completed");
                foreach (var t in tasks)
                {
                    Console.WriteLine(" - " + t.Result);
                }
                Console.WriteLine("All tasks done");
            }).Wait(); ;
        }
    }
}
