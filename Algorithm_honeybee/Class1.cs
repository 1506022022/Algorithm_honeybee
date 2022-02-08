#define test

#if test

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
namespace Algorithm_honeybee
{

    class Class1
    {
        struct RoomInfo
        {
            public Vector2 position;
            public int energy;
        }

        public static void Main()
        {
            //WriteLine("프로그램 시작");
            //Hive hive = new Hive();
            //MoveManager moveManager = new MoveManager();

            //WriteLine(moveManager.Simulation(hive));


            Vector2 vector = new Vector2(1, 2);
            int energy = 10;
            RoomInfo info = new RoomInfo() { position = vector, energy = energy };
            RoomInfo temp = new RoomInfo() { position = vector, energy = energy };

            Simulation s = new Simulation();





            }




        }

        
    }

#endif