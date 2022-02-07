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


                int row = (int)room.position.X;
                int column = (int)room.position.Y;
                int midle = ((diameter/2) +(diameter%2)) -1;
                bool isUp = row < midle;



                // 벌집의 중간부분일때
                if (row == midle)
                {
                    // 좌상
                    rooms[row][column].neighbor[leftUp] = roomExists(row - 1, column - 1);
                    // 우상
                    rooms[row][column].neighbor[rightUp] = roomExists(row - 1, column);
                    // 좌
                    rooms[row][column].neighbor[left] = roomExists(row, column - 1);
                    // 우
                    rooms[row][column].neighbor[right] = roomExists(row, column + 1);
                    // 좌하
                    rooms[row][column].neighbor[leftDown] = roomExists(row + 1, column - 1);
                    // 우하
                    rooms[row][column].neighbor[rightDown] = roomExists(row + 1, column);
                }

                // 벌집의 상부일때
                else if (isUp)
                {
                    // 좌상
                    rooms[row][column].neighbor[leftUp] = roomExists(row - 1, column - 1);
                    // 우상
                    rooms[row][column].neighbor[rightUp] = roomExists(row - 1, column);
                    // 좌
                    rooms[row][column].neighbor[left] = roomExists(row, column - 1);
                    // 우
                    rooms[row][column].neighbor[right] = roomExists(row, column + 1);
                    // 좌하
                    rooms[row][column].neighbor[leftDown] = roomExists(row + 1, column - 1);
                    // 우하
                    rooms[row][column].neighbor[rightDown] = roomExists(row + 1, column);
                }

                // 하부일때
                else
                {
                    // 좌상
                    rooms[row][column].neighbor[leftUp] = roomExists(row - 1, column);
                    // 우상
                    rooms[row][column].neighbor[rightUp] = roomExists(row - 1, column + 1);
                    // 좌
                    rooms[row][column].neighbor[left] = roomExists(row, column - 1);
                    // 우
                    rooms[row][column].neighbor[right] = roomExists(row, column + 1);
                    // 좌하
                    rooms[row][column].neighbor[leftDown] = roomExists(row + 1, column - 1);
                    // 우하
                    rooms[row][column].neighbor[rightDown] = roomExists(row + 1, column);
                }



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

        public static Hive Copy(Hive original)

        {
            Hive copy = new Hive();
            copy.size = original.size;
            copy.diameter = original.diameter;
            copy.moveCost = original.moveCost;

            // 방 목록 복사
            for (int row = 0; row < original.rooms.Count; row++)
                for (int column = 0; column < original.rooms[row].Count; column++)
                    copy.rooms[row][column] = Room.Copy(original.rooms[row][column]);

            return copy;
                
        }

        public void WriteHive(string name = "벌집")
        {
            WriteLine("[{0} 의 정보를 출력합니다.]\n",name);
            WriteLine("크기 : {0} \t직경 : {1} \t이동비용 : {2}", size, diameter, moveCost);
            WriteLine();
            WriteLine("[벌집의 방들을 출력합니다.]\n");

            foreach (var line in rooms)
                foreach (var room in line)
                    room.WriteRoom();
        }

    }
}
