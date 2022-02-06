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
    public struct A { }
    class Class1
    {

        public static void Main()
        {
            Hive hive = new Hive();


            Bee bee = new Bee(hive);

            bee.Fly(new Vector2() { X=0,Y=1});



            Print();

            void Print()
            {
                foreach (var room in hive.rooms)
                    foreach (var r in room)
                    {
                        WriteLine("위치 : {0} \t이웃 : {1} \t에너지 : {2} \t페로몬 : ", r.position, "no", r.energy);
                        foreach (var temp in r.isPheromone)
                            Write("{0} ", temp);
                        WriteLine();
                        WriteLine("좌상 : {0}\t우상 : {1}\t좌 : {2}\t우 : {3}\t좌하 : {4}\t우하 : {5}", r.neighbor[0], r.neighbor[1], r.neighbor[2], r.neighbor[3], r.neighbor[4], r.neighbor[5]);
                        WriteLine();

                        WriteLine("모은 에너지 : {0}", bee.energy);
                    }
            }
        }

    }
}
#endif