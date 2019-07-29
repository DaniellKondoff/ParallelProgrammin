using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskProgramming
{
    class StartUp
    {
        public static void Write(char c)
        {
            int i = 1000;
            while (i-- > 0)
            {
                Console.Write(c);
            }
        }

        public static void Write(object o)
        {
            int i = 1000;
            while (i-- > 0)
            {
                Console.Write(o);
            }
        }

        public static int TextLenght(object o)
        {
            Console.WriteLine($"\nTask with id {Task.CurrentId} processing object {o}...");
            return o.ToString().Length;
        }

        static void Main(string[] args)
        {
            try
            {
                HandleException();
            }
            catch(AggregateException ae)
            {
                foreach( var e in ae.InnerExceptions)
                {
                    Console.WriteLine($"Handled elsewehere: {e.GetType()}");
                }
            }
        }

        private static void HandleException()
        {
            var t = Task.Factory.StartNew(() =>
            {
                throw new InvalidOperationException("Cant do this") { Source = "t" };
            });

            var t2 = Task.Factory.StartNew(() =>
            {
                throw new AccessViolationException("Cant access this!") { Source = "t2" };
            });

            try
            {
                Task.WaitAll(t, t2);
            }
            catch (AggregateException ae)
            {
                ae.Handle(e =>
                {
                    if (e is InvalidOperationException)
                    {
                        Console.WriteLine("Invalid op!");
                        return true;
                    }
                    else return false;
                });
            }
        }

        private static void WaitingForTasks()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            var t = new Task(() =>
            {
                Console.WriteLine("I take 5 seconds");

                for (int i = 0; i < 5; i++)
                {
                    token.ThrowIfCancellationRequested();
                    Thread.Sleep(1000);
                }

                Console.WriteLine("I'm done");
            }, token);
            t.Start();

            Task t2 = Task.Factory.StartNew(() => Thread.Sleep(3000), token);

            Task.WaitAll(new[] { t, t2 }, 4000, token);

            Console.WriteLine($"Task t status is {t.Status}");
            Console.WriteLine($"Task t2 status is {t2.Status}");

            Console.ReadKey();
        }

        private static void WaitingForTimeToPass()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            var t = new Task(() =>
            {
                Console.WriteLine("Press any key to disarm; you have 5 seconds");
                bool cancelled = token.WaitHandle.WaitOne(5000);
                Console.WriteLine(cancelled ? "Boms is disarmed" : "BOOM!!!!");
            }, token);
            t.Start();

            Console.ReadKey();
            cts.Cancel();
        }

        private static void CancellingTasks()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            token.Register(() =>
            {
                Console.WriteLine("Cancellation has been requested");
            });

            var t = new Task(() =>
            {
                int i = 0;
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    Console.WriteLine(i++);
                }
            }, token);

            t.Start();
            Console.ReadKey();
            cts.Cancel();

            var planned = new CancellationTokenSource();
            var preventative = new CancellationTokenSource();
            var emergency = new CancellationTokenSource();

            var paranoid = CancellationTokenSource.CreateLinkedTokenSource(
                planned.Token, preventative.Token, emergency.Token);

            Task.Factory.StartNew(() =>
            {
                int i = 0;
                while (true)
                {
                    paranoid.Token.ThrowIfCancellationRequested();
                    Console.WriteLine(i++);
                    Thread.Sleep(1000);
                }
            }, paranoid.Token);

            Console.ReadKey();
            preventative.Cancel();
        }

        private static void StartTasks()
        {
            Task.Factory.StartNew(() => Write('1'));

            Task t = new Task(() => Write('2'));
            t.Start();

            string text1 = "testing", text2 = "this";
            var task1 = new Task<int>(TextLenght, text1);
            task1.Start();

            Task<int> task2 = Task.Factory.StartNew(TextLenght, text2);

            Console.WriteLine($"Length of {text1} is {task1.Result}");
            Console.WriteLine($"Length of {text2} is {task2.Result}");
        }
    }
}
