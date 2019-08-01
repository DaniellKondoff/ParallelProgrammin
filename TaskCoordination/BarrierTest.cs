using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TaskCoordination
{
    public class BarrierTest
    {
        static Barrier barrier = new Barrier(2, b =>
        {
            Console.WriteLine($"Phase {b.CurrentPhaseNumber} is finished");
        });

        public static void Water()
        {
            Console.WriteLine("Putting the kettleon");
            Thread.Sleep(2000);
            barrier.SignalAndWait(); //2
            Console.WriteLine("Pouring water into cup"); //0
            barrier.SignalAndWait(); // 1
            Console.WriteLine("Putting the kettle away");
        }

        public static void Cup()
        {
            Console.WriteLine("Finding the nicest cup of thea (fast)");
            barrier.SignalAndWait(); //1
            Console.WriteLine("Adding tea"); //0
            barrier.SignalAndWait(); //2
            Console.WriteLine("Adding sugar");
        }
    }
}
