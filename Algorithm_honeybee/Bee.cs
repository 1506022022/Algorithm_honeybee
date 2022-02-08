using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Algorithm_honeybee
{

    public class Bee
    {
        public int energy;
        public Vector2 position;
        public Hive hive;
        public Moving action;

        public Bee(Vector2 position,Hive hive)
        {
            this.position = position;
            this.hive = hive;

            Pheromone();
            GetEnergy();

        }
       

        void Pheromone()
        {
            Room current = hive.rooms[(int)position.X][(int)position.Y];
            current.isPheromone = true;
        }
        void MoveCost() 
        {
            Room current = hive.rooms[(int)position.X][(int)position.Y];
            if (current.isPheromone == false) energy -= hive.moveCost;

        }
        public void Fly(Vector2 go) 
        {
            if (go == position) return;

            Room current = hive.rooms[(int)position.X][(int)position.Y];



            bool isPossible = current.neighbor.Any((n) => n != null && n.position == go);
            WriteLine("{0} 에서 {1} 으로 날아서 이동!", position, go);
            position = go;
            MoveCost();

            hive.rooms[(int)position.X][(int)position.Y].energy = 0;
            Pheromone();


        }
        public void Walk(Vector2 go) 
        {
            if (go == position) return;
            Room current = hive.rooms[(int)position.X][(int)position.Y];
            bool isPossible = current.neighbor.Any((n) => n != null && n.position == go);

            if (isPossible)
            {
                WriteLine("{0} 에서 {1} 으로 걸어서 이동!", position, go);
                position = go;
                Pheromone();
                GetEnergy();


            }
            else WriteLine("{0} {1} 이동불가능",position,go);

        }
        public void Move(Vector2 go)
        {
            if (go == position) return;

            Room current = hive.rooms[(int)position.X][(int)position.Y];
            Room Go = hive.rooms[(int)go.X][(int)go.Y];

            
            bool isPossible = current.neighbor.Any((n) => n != null && n.position == go);


            
            if (Go.energy<(-hive.moveCost) && (!Go.isPheromone) || !isPossible)
            { 
                Fly(go);

            }else
            {
                Walk(go);
            }
        }

        public void Go(Vector2 go) 
        {
            position = go;        
        }
        

        void GetEnergy()
        {
            Room current = hive.rooms[(int)position.X][(int)position.Y];
            energy += current.energy;
            current.energy = 0;
        }

        public static Bee Copy(Bee original)
        {
            Bee copy = new Bee(original.position, Hive.Copy(original.hive));

            copy.hive = Hive.Copy(original.hive);
            copy.energy = original.energy;
            copy.position = original.position;

            return copy;
        }

    }
}
