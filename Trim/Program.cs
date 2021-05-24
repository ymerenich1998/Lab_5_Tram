using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trim
{
    class Program
    {
        static void Main(string[] args)
        {
            const int stationsCount = 3;
            const int passengersCount = 8;
            const int capacity = 2;
            //Tram tram = new SemaporeTram(capacity, stationsCount);
            Tram tram = new MonitorTram(capacity, stationsCount);
            var tasks = new Task[passengersCount + 1];
            tasks[passengersCount] = new Task(tram.Run);
            for (int i = 0; i < passengersCount; i++)
            {
                tasks[i] = new Task(new Passenger("name" + (i + 1), tram, stationsCount).Run);
            }
            Array.ForEach(tasks, x => x.Start());
            Task.WaitAll(tasks.ToArray());
        }
    }
}
