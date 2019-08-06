using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParallelLoops
{
    public class ParallelInvokeForEach
    {
        public static void Invoke()
        {
            var a = new Action(() => Console.WriteLine($"First {Task.CurrentId}"));
            var b = new Action(() => Console.WriteLine($"Second {Task.CurrentId}"));
            var c = new Action(() => Console.WriteLine($"Third {Task.CurrentId}"));

            Parallel.Invoke(a, b, c);
        }

        public static void For()
        {
            Parallel.For(1, 11, i =>
            {
                Console.WriteLine(i);
            });
        }

        public static void ForEach()
        {
            string[] words = new string[] { "first", "second", "third", "four" };

            Parallel.ForEach(words, word =>
            {
                Console.WriteLine(word);
            });
        }

        public static void ForEachWithYeld()
        {
            Parallel.ForEach(Range(1, 20, 2), Console.WriteLine);
        }

        public static IEnumerable<int> Range(int start, int end, int step)
        {
            for (int i = start; i < end; i+=step)
            {
                yield return i;
            }
        }
    }
}
