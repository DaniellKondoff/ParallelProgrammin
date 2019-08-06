using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelLoops
{
    public class BreakingCancellationException
    {
        private static void Demo()
        {
            var cts = new CancellationTokenSource();
            var po = new ParallelOptions();
            po.CancellationToken = cts.Token;

            ParallelLoopResult result = Parallel.For(0, 20, po, (int x, ParallelLoopState state) =>
            {
                Console.WriteLine($"{x}[{Task.CurrentId}]");

                if (x == 10)
                {
                    //state.Stop(); // stop execution as soon as possible
                    //state.Break();
                    //throw new Exception();
                    cts.Cancel();
               }

                if (state.IsExceptional)
                {
                    Console.WriteLine($"EX [{Task.CurrentId}]");
                }
            });

            Console.WriteLine();
            Console.WriteLine($"Was loop completed? {result.IsCompleted}");
            if (result.LowestBreakIteration.HasValue)
                Console.WriteLine($"Lowest break iteration: {result.LowestBreakIteration}");
        }

        public static void Start()
        {
            try
            {
                Demo();
            }
            catch (OperationCanceledException) { }
            catch (AggregateException ae)
            {
                ae.Handle(e =>
                {
                    Console.WriteLine(e.Message);
                    return true;
                });
            }
        }
    }
}
