using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskCoordination
{
    public static class ResetEvents
    {
        public static void ManuelResetEvent()
        {
            var evt = new ManualResetEventSlim();

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Boiling water");
                evt.Set(); // set on 1
            });

            var makeTes = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Waiting for water.....");
                evt.Wait(); // wait for 1
                Console.WriteLine("Here is your tea");
            });

            makeTes.Wait();
        }

        public static void AutoResentEvent()
        {
            var evt = new AutoResetEvent(false);

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Boiling water");
                evt.Set(); // set on true
            });

            var makeTes = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Waiting for water.....");
                evt.WaitOne(); // set on false
                Console.WriteLine("Here is your tea");
                if (evt.WaitOne(1000)) //et on false
                {
                    Console.WriteLine("Succeeded");
                }
                else
                {
                    Console.WriteLine("Timed out");
                }
            });

            makeTes.Wait();
        }
    }
}
