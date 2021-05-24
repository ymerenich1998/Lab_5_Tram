using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Trim
{
    abstract class Tram
    {
        public int capacity { get;  set; }
        public int passangersCount { get;  set; }
        public int stationsCount { get; set; }
        public bool profitСircle { get; set; } // якщо коло було невигідне то трамвай повертається в депо після його заверешння 
        public abstract void SetDown(Passenger passenger);
        public abstract void SetUp(Passenger passenger);
        //public abstract void Fill(Hen hen);

        public abstract void Run();

        public int currentStationNumber = 0;

        public bool inStation(int stationNumber) {
            return currentStationNumber == stationNumber;
        }
    }

    class SemaporeTram: Tram{

        public Semaphore semaphorePassangersCount;
        private Mutex mutex = new Mutex();

        public SemaporeTram(int capacity,int stationsCount, int passangersCount =0) {
            this.capacity = capacity;
            this.stationsCount = stationsCount;
            this.passangersCount = passangersCount;
            semaphorePassangersCount = new Semaphore(capacity, capacity);

        }

        public override void Run() {
            // трамвай починає свій рух,
            do
            {
                Console.WriteLine($"Трамвай починає нове коло");
                // запуск проходження всіх станцій
                // якщо попередня станція була остання то він повртається на першу 
                for (int i = 1; i<= stationsCount; i++) {
                    profitСircle = false;
                    currentStationNumber = i;
                    // через певний проміжуток часу (час очікування) він перестає бути на станції 
                    Console.WriteLine($"Трамвай прибуває i очiкує на станцiї №{currentStationNumber}");
                    Thread.Sleep(2000);
                    mutex.WaitOne();
                    Console.WriteLine($"Трамвай вiдбуває з станцiї №{currentStationNumber}");
                    currentStationNumber = 0;
                    Console.WriteLine($"Трамвай в дорозi");
                    // через інший проміжуток (час на переміщення) він знаходиться на наступній станції
                    Thread.Sleep(2000);
                    // проїзд до наступної станції
                    mutex.ReleaseMutex();
                    //Console.WriteLine($"Трамвай прибув до наступної станції");
                }
                // перевіряється умова закінчення роботи поїзда (якщо він проїхав коло але жоден пасажир не сів)
            } while (profitСircle);
        }
        public override void SetDown(Passenger passenger) {
            while (!passenger.inTram)
            {
                Console.WriteLine($"Пасажир {passenger.Name} очiкує трамвай на станцiї №{passenger.stationNumber} щоб здiйснити {passenger.interation + 1} поїздку");
                profitСircle = true;
                mutex.WaitOne();
                // 
                if (passenger.stationNumber == currentStationNumber && !passenger.inTram && passangersCount < capacity)
                {
                    Console.WriteLine($"Пасажир {passenger.Name} сiдає на трамвай на станцiї №{currentStationNumber}");
                    passenger.inTram = true;
                    semaphorePassangersCount.WaitOne();
                    passangersCount++;
                    Console.WriteLine($"Пасажир {passenger.Name} сiв на трамвай на станцiї №{currentStationNumber}");
                }
                mutex.ReleaseMutex();
                Thread.Sleep(2000);
            }
        }

        public override void SetUp(Passenger passenger)
        {
            while (passenger.inTram)
            {
                mutex.WaitOne();
            //
            if (passenger.requiredStationNumber== currentStationNumber && passenger.inTram)
            {
                Console.WriteLine($"Пасажир {passenger.Name} виходить з трамвая на станцiї №{currentStationNumber}");
                passenger.inTram = false;
                passenger.NextStation();
                semaphorePassangersCount.Release();
                passangersCount--;
                Console.WriteLine($"Пасажир {passenger.Name} вийшов з трамвая на станцiї №{currentStationNumber}");
            }
            mutex.ReleaseMutex();
            }
        }
    }

    class MonitorTram: Tram {

        private object inWay = new object();
        private object lockPassanger = new object();
        public MonitorTram(int capacity, int stationsCount, int passangersCount = 0)
        {
            this.capacity = capacity;
            this.stationsCount = stationsCount;
            this.passangersCount = passangersCount;
        }

        public override void Run()
        {
            // трамвай починає свій рух,
            do
            {
                Console.WriteLine($"Трамвай починає нове коло");
                // запуск проходження всіх станцій
                // якщо попередня станція була остання то він повртається на першу 
                for (int i = 1; i <= stationsCount; i++)
                {
                    profitСircle = false;
                    currentStationNumber = i;
                    // через певний проміжуток часу (час очікування) він перестає бути на станції 
                    Console.WriteLine($"Трамвай прибуває i очiкує на станцiї №{currentStationNumber}");
                    Thread.Sleep(2000);
                    Monitor.Enter(inWay);
                    Console.WriteLine($"Трамвай вiдбуває з станцiї №{currentStationNumber}");
                    currentStationNumber = 0;
                    Console.WriteLine($"Трамвай в дорозi");
                    // через інший проміжуток (час на переміщення) він знаходиться на наступній станції
                    Thread.Sleep(2000);
                    // проїзд до наступної станції
                    Monitor.Exit(inWay);
                    //Console.WriteLine($"Трамвай прибув до наступної станції");
                }
                // перевіряється умова закінчення роботи поїзда (якщо він проїхав коло але жоден пасажир не сів)
            } while (profitСircle);
        }
        public override void SetDown(Passenger passenger)
        {
            while (!passenger.inTram)
            {
                Console.WriteLine($"Пасажир {passenger.Name} очiкує трамвай на станцiї №{passenger.stationNumber} щоб здiйснити {passenger.interation + 1} поїздку");
                profitСircle = true;
                Monitor.Enter(inWay);
                // 
                if (passenger.stationNumber == currentStationNumber && !passenger.inTram && passangersCount < capacity)
                {
                    Console.WriteLine($"Пасажир {passenger.Name} сiдає на трамвай на станцiї №{currentStationNumber}");
                    passenger.inTram = true;
                    lock (lockPassanger)
                    {
                        while (passangersCount >= capacity)
                        {
                            Monitor.Wait(lockPassanger);
                        }

                        passangersCount++;
                    }
 
                    Console.WriteLine($"Пасажир {passenger.Name} сiв на трамвай на станцiї №{currentStationNumber}");
                }
                Monitor.Exit(inWay);
                Thread.Sleep(2000);
            }
        }
        public override void SetUp(Passenger passenger)
        {
            while (passenger.inTram)
            {
                Monitor.Enter(inWay);
                //
                if (passenger.requiredStationNumber == currentStationNumber && passenger.inTram)
                {
                    Console.WriteLine($"Пасажир {passenger.Name} виходить з трамвая на станцiї №{currentStationNumber}");
                    passenger.inTram = false;
                    passenger.NextStation();
                    lock (lockPassanger)
                    {
                        passangersCount--;
                        Monitor.Pulse(lockPassanger);
                    }
                    Console.WriteLine($"Пасажир {passenger.Name} вийшов з трамвая на станцiї №{currentStationNumber}");
                }
                Monitor.Exit(inWay);
            }
        }

    }
}
