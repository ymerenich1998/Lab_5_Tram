using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trim
{
    class Passenger
    {
        static Random random = new Random();
        public string Name { get; set; } // ім'я пасажиру
        private Tram tram; // трамвай
        public int stationNumber { get; set; }// поточна станція
        public int requiredStationNumber { get; set; } // станція на які планує вийти
        private int stationCount; // Загальна кількість станцій
        private int tripsNumber; // кількість поїздок пасажира
        public bool inTram = false;
        public int interation = 0;

        public Passenger(string name, Tram tram,int stationCount)
        {
            Name = name;
            this.tram = tram;
            this.stationCount = stationCount;
            requiredStationNumber = random.Next(0, stationCount)+1;
            NextStation();
            //tripsNumber = random.Next(1, 5);
            tripsNumber = 2;
        }

        public void NextStation() {
            stationNumber = requiredStationNumber;
            do
            {
                requiredStationNumber = random.Next(0, stationCount)+1;
            } while (requiredStationNumber == stationNumber);
        }


        public void Run()
        {
            for (int i = 0; i < tripsNumber; i++)
            {
                tram.SetDown(this);
                tram.SetUp(this);
                interation++;
                Thread.Sleep(3000);
            }
        }
    }
}
