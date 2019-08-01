using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskCoordination
{
    public class ChildTasks
    {
        public static void  RundChildTask()
        {
            var parent = new Task(() =>
            {
                Console.WriteLine("Parent task starting");
                var child = new Task(() =>
                {
                    Console.WriteLine("Child task starting");
                    Thread.Sleep(3000);
                    Console.WriteLine("Child task finishing");
                }, TaskCreationOptions.AttachedToParent);

                var completionHandler = child.ContinueWith(t =>
                {
                    Console.WriteLine($"Hooray, task {t.Id}' state is {t.Status}");
                },
                TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnRanToCompletion);

                var failedHandler = child.ContinueWith(t =>
                {
                    Console.WriteLine($"Ooops, task {t.Id}' state is {t.Status}");
                }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnFaulted);

                Console.WriteLine("Parent task finishing");

                child.Start();
            });

            parent.Start();

            try
            {
                parent.Wait();
            }
            catch (AggregateException ae)
            {
                ae.Handle(a => true);
            }
        }
    }
}
