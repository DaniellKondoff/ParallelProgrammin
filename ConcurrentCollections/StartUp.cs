using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentCollections
{
    class StartUp
    {
        private static ConcurrentDictionary<string, string> capitals = new ConcurrentDictionary<string, string>();
        private static Random random = new Random();
        static CancellationTokenSource cts = new CancellationTokenSource();
        static BlockingCollection<int> messages = new BlockingCollection<int>(new ConcurrentBag<int>(), 10);

        public static void AddParis()
        {
            bool success = capitals.TryAdd("France", "Paris");
            string who = Task.CurrentId.HasValue ? ("Task " + Task.CurrentId) : "MainThread";
            Console.WriteLine($"{who} {(success ? "added" : "did not aded")} the element");
        }

        static void Main(string[] args)
        {
            //ConcurrentDictionary();
            //ConcurrentQueue();
            //ConcurrentStack();
            //ConcurretnBag();

            Task.Factory.StartNew(ProduceAndConsume, cts.Token);

            Console.ReadKey();
            cts.Cancel();
        }

        private static void ConcurretnBag()
        {
            var bag = new ConcurrentBag<int>();
            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                var i1 = i;
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    bag.Add(i1);
                    Console.WriteLine($"{Task.CurrentId} has added {i1}");
                    int result;
                    if (bag.TryPeek(out result))
                    {
                        Console.WriteLine($"{Task.CurrentId} has peeked the value {result}");
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // take whatever's last
            int last;
            if (bag.TryTake(out last))
            {
                Console.WriteLine($"Last element is {last}");
            }
        }

        private static void ConcurrentStack()
        {
            var stack = new ConcurrentStack<int>();
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);

            int result;
            if (stack.TryPeek(out result))
                Console.WriteLine($"{result} is on top");

            if (stack.TryPop(out result))
                Console.WriteLine($"Popped {result}");

            var items = new int[5];
            if (stack.TryPopRange(items, 0, 5) > 0) // actually pops only 3 items
            {
                var text = string.Join(", ", items.Select(i => i.ToString()));
                Console.WriteLine($"Popped these items: {text}");
            }
        }

        private static void ConcurrentQueue()
        {
            var q = new ConcurrentQueue<int>();
            q.Enqueue(1);
            q.Enqueue(2);

            int result;
            if (q.TryDequeue(out result))
            {
                Console.WriteLine($"Remove element is {result}");
            }

            if (q.TryPeek(out result))
            {
                Console.WriteLine($"Front element is {result}");
            }
        }

        private static void ConcurrentDictionary()
        {
            Task.Factory.StartNew(AddParis).Wait();
            AddParis();

            capitals["Russia"] = "Leningrad";
            capitals.AddOrUpdate("Russia", "Moscow", (k, old) => old + "--> Moscow");
            Console.WriteLine(capitals["Russia"]);

            capitals["Bulgaria"] = "Veliko Turnovo";
            var capitalOfBulgaria = capitals.GetOrAdd("Bulgaria", "Sofia");
            Console.WriteLine("Capial of Bulgaria is: " + capitalOfBulgaria);

            const string toRemove = "Russia";
            string removed;
            var didRemove = capitals.TryRemove(toRemove, out removed);
            if (didRemove)
                Console.WriteLine($"We just Removed {removed}");
            else
                Console.WriteLine($"Failed to remove the capital of {toRemove}");
        }

        public static void ProduceAndConsume()
        {
            var producer = Task.Factory.StartNew(RunProducer);
            var consumer = Task.Factory.StartNew(RunConsumer);

            try
            {
                Task.WaitAll(new[] { producer, consumer }, cts.Token);
            }
            catch (AggregateException ae)
            {
                ae.Handle(e => true);
            }
        }

        private static void RunConsumer()
        {
            foreach (var item in messages.GetConsumingEnumerable())
            {
                cts.Token.ThrowIfCancellationRequested();
                Console.WriteLine($"-{item}");
                Thread.Sleep(random.Next(1000));
            }
        }

        private static void RunProducer()
        {

            while (true)
            {
                cts.Token.ThrowIfCancellationRequested();
                int i = random.Next(100);
                messages.Add(i);
                Console.WriteLine($"+{i}\t");
                Thread.Sleep(random.Next(1000));
            }
        }
    }
}
