using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using static Algorithm_honeybee.Constant;
namespace Algorithm_honeybee
{
    public class Hive
    {
        // 벌집 크기
        int size;
        // 벌집 직경
        int diameter;
        // 이동 비용
       public int moveCost;
        // 벌집을 구성하는 방들
        public List<List<Room>> rooms;

        public Hive() 
        {
            ReadInput(@"C:\Users\user\source\repos\Algorithm_honeybee\Algorithm_honeybee\Input.txt");
          
            NeighborRoom();
        }

        // 벌집 입력 데이터 읽기
        public void ReadInput(string path)
        {
            using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open)))
            {
                // 방의 위치 값
                int row = 0, column = 0;

                // 첫 줄 입력 (벌집크기, 이동비용)
                string[] temp = reader.ReadLine().Split(" ");
                size = int.Parse(temp[0]);
                moveCost = int.Parse(temp[1]);

                // 벌집의 직경
                diameter = (size - 1) * 2 + 1;
                
                // 벌집 데이터 할당
                rooms = new List<List<Room>>();
                while (!reader.EndOfStream)
                {
                    // 주어진 데이터에서 각 방의 에너지 값을 읽어온다
                    string[] energys = reader.ReadLine().Split(" ");

                    List<Room> roomRow = new List<Room>();


                    
                    foreach (string energy in energys)
                    {
                        roomRow.Add(new Room()
                        {
                            energy = int.Parse(energy),
                            position = new Vector2() { X = row, Y = column },
                            isPheromone = new bool[] { false, false, false, false, false, false },
                            neighbor = new Room?[] {null,null,null,null,null,null }
                        });
                        column++;
                    }
                    column = 0;
                    row++;
                    
                    rooms.Add(roomRow);
                }

            }
        }

        //이웃 방 정보 갱신
        public void NeighborRoom()
        {


            foreach (var roomRow in rooms)
                foreach (var room in roomRow)
                    SearchRoom(room);
                

            
            // 이웃방 정보 검색
            void SearchRoom(Room room)
            {
                int balance1 = 0;
                int balance2 = 0;
                int row = (int)room.position.X;
                int column = (int)room.position.Y;

                // 짝수일때 = 위쪽 행+1
                // 홀수일때 = 아래 행 -1
                if (row % 2 == 0)
                    balance2 = 1;
                else
                    balance1 = -1;

                // 좌상
                rooms[row][column].neighbor[leftUp] = roomExists( row - 1,column - 1 + balance2 );
                // 우상
                rooms[row][column].neighbor[rightUp] = roomExists(row - 1, column + balance2);
                // 좌
                rooms[row][column].neighbor[left] = roomExists(row, column - 1);
                // 우
                rooms[row][column].neighbor[right] = roomExists(row, column + 1);
                // 좌하
                rooms[row][column].neighbor[leftDown] = roomExists(row + 1, column + balance1);
                // 우하
                rooms[row][column].neighbor[rightDown] = roomExists(row + 1, column + 1 + balance1);

            }
            // 방 존재 여부 확인
            Room? roomExists(int row, int column)
            {
                int maxRow = rooms.Count;
                // 존재하지 않음 (음수 위치)
                if (row < 0 || column < 0) return null;
                // 존재하지 않음 (열 범위 오버)
                if (row >= rooms.Count) return null;
                // 존재하지 않음 (행 범위 오버)
                if (column >= rooms[row].Count) return null;

                //존재함
                return rooms[row][column];
            }

        }

    }
}
