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

        public Bee(Hive hive)
        {
            position = new Vector2() { X = 0, Y = 0 };
            this.hive = hive;
            GetEnergy();
        }
        public void Move(Vector2 go)
        {
            // 현재 위치
            int row = (int)position.X;
            int column = (int)position.Y;

            // 이동
            foreach (var room in hive.rooms[row][column].neighbor)
                if (room != null && room.position == go) {
                    Pheromone(); return; 
                }
                WriteLine("이동할 수 없는 경로입니다.");

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
        public void Fly(Vector2 go) 
        {
            while (position != go)
            {
                // 이동 방향 지정
                var dir = Constant.DirectSet(position, go, hive);

                // 처음 지나갈 때는 이동비용 지불하고
                if (!hive.rooms[(int)position.X][(int)position.Y].isPheromone[dir])
                    energy -= hive.moveCost;

                // 이동
                Move(hive.rooms[(int)position.X][(int)position.Y].neighbor[dir].position);
                
            }
            GetEnergy();
            
        }
        void Walk(Vector2 go) 
        {
            // 이동 후 에너지 흡수
            Move(go);
            GetEnergy();
        }
        void GetEnergy()
        {
            energy += hive.rooms[(int)position.X][(int)position.Y].energy;
            hive.rooms[(int)position.X][(int)position.Y].energy = 0;
        }

    }
}
