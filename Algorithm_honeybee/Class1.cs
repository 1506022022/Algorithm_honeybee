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
            WriteLine("프로그램 시작");
            Hive hive = new Hive();
            MoveManager moveManager = new MoveManager();

            WriteLine(moveManager.Simulation(hive));



        }

    }
}
#endif