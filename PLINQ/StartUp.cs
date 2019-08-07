using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PLINQ
{
    class StartUp
    {
        static void Main(string[] args)
        {
            //var sum = Enumerable.Range(1, 1000).Sum();

            var sum = ParallelEnumerable.Range(1, 1000)
                .Aggregate(
                0,
                (partialSum, i) => partialSum += i,
                (total, subTotal) => total += subTotal,
                i => i
                );

            Console.WriteLine("Sum = " + sum);
        }

        private static void MergeOption()
        {
            var numbers = Enumerable.Range(1, 20).ToArray();

            var results = numbers.AsParallel()
                .WithMergeOptions(ParallelMergeOptions.FullyBuffered)
               .Select(x =>
               {
                   var result = Math.Log10(x);
                   Console.WriteLine($"Produced {result}");
                   return result;
               });

            foreach (var result in results)
            {
                Console.WriteLine("Consumed " + result);
            }
        }

        private static void Cancellation()
        {
            var items = ParallelEnumerable.Range(1, 20);
            var cts = new CancellationTokenSource();

            var results = items.WithCancellation(cts.Token).Select(i =>
            {
                double result = Math.Log10(i);
                //if (result > 1) throw new InvalidOperationException();

                Console.WriteLine($"i = {i}, TaskId = {Task.CurrentId}");
                return result;
            });

            try
            {
                foreach (var c in results)
                {
                    if (c > 1) cts.Cancel();
                    Console.WriteLine($"result = {c}");
                }
            }
            catch (AggregateException ae)
            {
                ae.Handle(e =>
                {
                    Console.WriteLine($"{e.GetType().Name}: {e.Message}");
                    return true;
                });
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine("Canceled");
            }
        }

        private static void AsParallel()
        {
            const int count = 50;

            var items = Enumerable.Range(1, count).ToArray();
            var results = new int[count];

            items.AsParallel().ForAll(x =>
            {
                int newValue = x * x * x;
                Console.WriteLine($"{newValue} ({Task.CurrentId})");
                results[x - 1] = newValue;
            });
            Console.WriteLine();

            var cubes = items.AsParallel().AsOrdered().Select(x => x * x * x);
        }
    }
}
