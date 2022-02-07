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

        public Bee(Vector2 position,Hive hive)
        {
            this.position = position;
            this.hive = hive;
            
            GetEnergy();
        }
        bool Move(Vector2 go)
        {
            // 현재 위치
            int row = (int)position.X;
            int column = (int)position.Y;

            // 이동
            foreach (var room in hive.rooms[row][column].neighbor)
                if (room != null && room.position == go) {
                    Pheromone(); return true; 
                }

            return false;
            // 방문 처리
            void Pheromone()
            {
                // 위치 확인 후 이동경로 체크
                for (int i = 0; i < 6; i++)
                    if (hive.rooms[row][column].neighbor[i] != null&&
                        hive.rooms[row][column].neighbor[i].position == go)
                    {

                        hive.rooms[row][column].isPheromone[i] = true;
                        hive.rooms[(int)go.X][(int)go.Y].isPheromone[5 - i] = true;
                        position = go;
                    }

            }
        }
        void Fly(Vector2 go) 
        {

            while (position != go)
            {
                // 이동 방향 지정
                var dir = Constant.DirectSet(position, go, hive);

                // 처음 지나갈 때는 이동비용 지불하고
                if (!hive.rooms[(int)position.X][(int)position.Y].isPheromone[dir])
                    energy -= hive.moveCost;

                // 이동
                try{
                    if (hive.rooms[(int)position.X][(int)position.Y].neighbor[dir] == null) throw new Exception();
                    Move(hive.rooms[(int)position.X][(int)position.Y].neighbor[dir].position);
                }
                catch(Exception e)  
                {
                    WriteLine(hive.rooms[(int)position.X][(int)position.Y].position);
                    WriteLine(dir);
                    WriteLine("예외 발생");
                    hive.WriteHive();
                    ReadLine();
                }
                finally { }
                
            }
            GetEnergy();
            
        }
        bool Walk(Vector2 go) 
        {
            // 성공 여부
            bool isSuccess = false;



            // 이동 후 에너지 흡수
            isSuccess = Move(go);

            if (isSuccess)
            {
                GetEnergy();

            }

            
            return isSuccess;
        }
        public void Go(Vector2 go)
        {

                if (position != go)
                    if (hive.rooms[(int)position.X][(int)position.Y].neighbor.Any((n) => n != null && n.position == go))
                        Walk(go); 
                    else
                        Fly(go);

        }
        void GetEnergy()
        {
            
            energy += hive.rooms[(int)position.X][(int)position.Y].energy;
            hive.rooms[(int)position.X][(int)position.Y].energy = 0;
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
