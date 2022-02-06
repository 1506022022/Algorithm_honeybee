#define Debug

#if Debg

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using static System.Console;
using static Algorithm_honeybee.Constant;

namespace Algorithm_honeybee
{

    public class Constant
    {
        // 상수 선언
        public const int leftUp = 0;
        public const int rightUp = 1;
        public const int left = 2;
        public const int right = 3;
        public const int leftDown = 4;
        public const int rightDown = 5;
    }

    public class Hive
    {

        [Flags]
        public enum Direction {
            non=0,
            left=1,
            right=2,
            up=4,
            down=8,
            leftUp=left|up,
            rightUp=right|up,
            leftDown=left|down,
            rightDown=right|down
        };


        public struct HiveRoom 
        {
            // 위치
            public Vector2 position;
            // 획득 가능한 에너지량
            public int energy;

            // 페로몬이 뿌려졌는지
            public bool isPheromone;

            // 이웃한 방 목록
            public Vector2?[] neighbor;
            public bool[] isVisit;
        }

        // 획득 가능한 최대 에너지
        int maximumEnergy =0;
        // 벌집을 저장할 저장소 (is 페로몬)
        HiveRoom [][] hiveRoomArray;

        // 위치
        int currentRow = 0;
        int currentColumn = 0;

        // 벌집의 크기
        public int hiveSize;
        // 날기 비용
        public int flyCost;

        // 생성자
        public Hive() {



            // 입력 받기
            ReadInput();


            // 인접 구역 서칭
            NeighborRoom();

            // 시작한 위치의 에너지 흡수, 페로몬 뿌리기
            var temp = new HiveRoom() { position = new Vector2() { X = currentRow, Y = currentColumn } };
            maximumEnergy = GetEnergy(temp);
            Pheromone(temp.position);

            //
            WriteLine("획득할 수 있는 최대 에너지 : {0}",Maximum());
        }


        // 벌집 입력 데이터 읽기
        public void ReadInput() 
        {
            using (StreamReader reader = new StreamReader(new FileStream(@"C:\Users\user\source\repos\Algorithm_honeybee\Algorithm_honeybee\Input.txt", FileMode.Open)))
            {
                string [] temp = reader.ReadLine().Split(" ");
                hiveSize = int.Parse(temp[0]);
                flyCost = int.Parse(temp[1]);

                //벌집의 길이
                int size = (hiveSize - 1) * 2 + 1;
                hiveRoomArray = new HiveRoom[size][];

                int row = 0;
                while (!reader.EndOfStream)
                {
                    string[] energys = reader.ReadLine().Split(" ");


                    HiveRoom[] Rooms = new HiveRoom[energys.Length];

                    int column = 0;
                    foreach (string energy in energys)
                    {
                        Rooms[column] = new HiveRoom() { energy = int.Parse(energy), position = new Vector2 {X=row,Y= column } };
                        column++;
                    }
                    hiveRoomArray[row] = Rooms;
                    row++;
                }

            }
        }



        // 이웃 방 정보 갱신
        public void NeighborRoom()
        {
            int myRow=0;
            int myColumn=0;

            foreach (var roomLine in hiveRoomArray)
            {
                foreach (var room in roomLine)
                {
                    SearchRoom(ref hiveRoomArray[myRow][myColumn], myRow, myColumn);
                    myColumn++;
                }
                myColumn = 0;
                myRow++;
            }


        }
        // 이웃방 정보 검색
        public void SearchRoom(ref HiveRoom room,int row, int column)
        {
            room.isVisit = new bool[6];
            room.neighbor = new Vector2?[6] ;
            int balance1 = 0;
            int balance2=0;


            // 짝수일때 = 위쪽 행+1
            // 홀수일때 = 아래 행 -1
            if (row % 2 == 0)
                balance2 = 1;
            else
                balance1 = -1;

            // 좌상
            room.neighbor[leftUp] = roomExists(row-1,column-1+ balance2);
            // 우상
            room.neighbor[rightUp] = roomExists(row - 1, column+ balance2);
            // 좌
            room.neighbor[left] = roomExists(row, column - 1);
            // 우
            room.neighbor[right] = roomExists(row, column + 1);
            // 좌하
            room.neighbor[leftDown] = roomExists(row + 1, column+ balance1);
            // 우하
            room.neighbor[rightDown] = roomExists(row + 1, column + 1+ balance1);

        }
        // 방 존재 여부 확인
        public Vector2? roomExists(int row, int column)
        {
                // 존재하지 않음 (음수 위치)
                if (row < 0 || column < 0) return null;
                // 존재하지 않음 (열 범위 오버)
                if (row >= hiveRoomArray.Length) return null;
                // 존재하지 않음 (행 범위 오버)
                if (column >= hiveRoomArray[row].Length) return null;

            //존재함
            Vector2? temp = new Vector2() { X = row, Y = column };
            return temp;
        }


        public int Maximum()
        {
            List<int> Energy = new List<int>();
            Energy.Add(0);
            Energy.Add(maximumEnergy);

            foreach (var line in hiveRoomArray)
                foreach (var room in line)
                {
                    maximumEnergy += SmartMove();
                    Energy.Add(maximumEnergy);
                }
            foreach (int a in Energy)
                WriteLine("회차별 에너지 획득량 : {0}",a);

            maximumEnergy = Energy.Max();
            return maximumEnergy;
        }
        // 최대이윤 이동 찾기
        public int SmartMove()
        {
            HiveRoom tempRoom = new HiveRoom();
            Vector2 current = new Vector2() { X = currentRow, Y = currentColumn };
            int maximum = 0;
            int tempEnergy;
            

            WriteLine("현재 위치 : <{0}, {1}>", currentRow, currentColumn);
            foreach (var line in hiveRoomArray)
                foreach (var room in line)
                {

                    // 목적지와 현재 위치가 같으면 이동하지 않는다.
                    if (current == room.position)
                    {
                        WriteLine("현재 위치로의 이동명령을 무시합니다. {0}", room.position);
                        continue;
                    }



                    // 모든 경로중 가장 에너지를 많이 획득하는 경로 탐색
                    tempEnergy = Move(room.position);
                    if(tempEnergy>=maximum)
                    {
                        maximum = tempEnergy;
                        tempRoom.position = room.position;
                    }
                }
            WriteLine("이동 경로중 최선의 경로 : {0}",tempRoom.position);

            

            // 벌의 현재 위치 이동
            currentRow = (int)tempRoom.position.X;
            currentColumn = (int)tempRoom.position.Y;

            
            
            return maximum;
        }
        // 이동 (걷기 + 날기)
        public int Move(Vector2 position) 
        {
            Write("<{0}, {1}> 에서 {2} 으로의 가상이동 ", currentRow, currentColumn, position);
            int energy = 0;

            // 이웃한 방 목록에 존재하면 Walk
            bool isNeighbor = hiveRoomArray[currentRow][currentColumn].neighbor.Contains(position);
            if (isNeighbor)
                energy = Walk(position);
            else
                energy = Fly(position);
            Pheromone(position);

            WriteLine("에너지 : {0}", energy);
            return energy;

        }
        // 걷기 기능 (에너지 반환)
        public int Walk(Vector2 position) {

            Direction direction = Direction.non;
            return hiveRoomArray[(int)position.X][(int)position.Y].energy; }

        // 날기 기능 (에너지 반환)
        public int Fly(Vector2 position) {

            // 지나 왔던 경로이면 이동비용 없음
            if (hiveRoomArray[(int)position.X][(int)position.Y].isPheromone)
                return hiveRoomArray[(int)position.X][(int)position.Y].energy;
            int count = Trace(position);
            int cost =  count* flyCost;
            WriteLine("{0} 으로의 이동 비용 : {1} , 이동 횟수 : {2}", position, cost,count);
            return hiveRoomArray[(int)position.X][(int)position.Y].energy- cost; 

           
        }

        // 경로 찾기 최단 벌집거리 반환, 오류나면 -1
        public int Trace(Vector2 target)
        {
            int count = 0;
            Direction direction;
            Vector2 current = new Vector2() { X = currentRow, Y = currentColumn };

            WriteLine("{0} 으로 이동중...", target);
            WriteLine("현재 {0}", current);
            var roomList = hiveRoomArray[(int)current.X][(int)current.Y].neighbor;

            Action<int, int> action = new Action<int, int>((one, two) => 
            {
                if (roomList[one] != null)
                {
                    current = (Vector2)roomList[one];
                    if (hiveRoomArray[(int)current.X][(int)current.Y].isVisit[one]) count=count-1>0?count-1:0;
                }
                else if (roomList[two] != null)
                {
                    current = (Vector2)roomList[two];
                    if (hiveRoomArray[(int)current.X][(int)current.Y].isVisit[two]) count = count - 1 > 0 ? count - 1 : 0;
                }
                else { Write("움직일 경로가 없습니다 !!"); }
            });

            while (target != current)
            {
                roomList = hiveRoomArray[(int)current.X][(int)current.Y].neighbor;
                direction = Direction.non;
                // 방향 지정
                if (current.X == target.X) { }
                else if (current.X > target.X) direction |= Direction.up;
                else direction |= Direction.down;

                if (current.Y == target.Y)
                {
                    if (direction == Direction.down)
                    {
                        if (roomList[leftDown] != null)
                        {
                            if (((Vector2)roomList[leftDown]).Y == current.Y)
                                direction |= Direction.left;
                            else direction |= Direction.right;
                        }
                        else direction |= Direction.right;
                    }
                    else if (direction == Direction.up) 
                    {
                        if (roomList[leftUp] != null)
                        {
                            if (((Vector2)roomList[leftUp]).Y == current.Y)
                                direction |= Direction.left;
                            else direction |= Direction.right;
                        }
                        else direction |= Direction.right;
                    }
                }

                else if (current.Y > target.Y) direction |= Direction.left;
                else direction |= Direction.right;


                WriteLine(direction);
                
                // 움직임
                switch (direction)
                {
                    case Direction.leftUp:
                        action(leftUp,left);
                        break;
                    case Direction.rightUp:
                        action(rightUp, right);
                        break;
                    case Direction.left:
                        action(left, left);
                        break;
                    case Direction.right:
                        action(right, right);
                        break;
                    case Direction.leftDown:
                        action(leftDown,left);
                        break;
                    case Direction.rightDown:
                        action(rightDown, right);
                        break;
                    default:
                        break;
                }
                count++;
            }
            return count;
        }

        public int Trace(Vector2 target , bool isReal)
        {
            int count = 0;
            Direction direction;
            Vector2 current = new Vector2() { X = currentRow, Y = currentColumn };

            WriteLine("{0} 으로 이동중...", target);
            WriteLine("현재 {0}", current);
            var roomList = hiveRoomArray[(int)current.X][(int)current.Y].neighbor;

            Action<int, int> action = new Action<int, int>((one, two) =>
            {
                if (roomList[one] != null)
                {
                    if (hiveRoomArray[(int)current.X][(int)current.Y].isVisit[one]) {
                        count = count - 1 > 0 ? count - 1 : 0;
                        current = (Vector2)roomList[one];
                    }
                    else 
                    {
                        hiveRoomArray[(int)current.X][(int)current.Y].isVisit[one] = true;
                        current = (Vector2)roomList[one];
                        int reverse = 0;
                        if (one == leftUp) reverse = rightDown;
                        if (one == rightUp) reverse = leftDown;
                        if (one == left) reverse = right;
                        if (one == right) reverse = left;
                        if (one == leftDown) reverse = rightUp;
                        if (one == rightDown) reverse = leftUp;
                        hiveRoomArray[(int)current.X][(int)current.Y].isVisit[reverse] = true;
                    }
                    
                }
                else if (roomList[two] != null)
                {
                    if (hiveRoomArray[(int)current.X][(int)current.Y].isVisit[two])
                    {
                        count = count - 1 > 0 ? count - 1 : 0;
                        current = (Vector2)roomList[two];
                    }
                    else
                    {
                        hiveRoomArray[(int)current.X][(int)current.Y].isVisit[two] = true;
                        current = (Vector2)roomList[two];
                        int reverse = 0;
                        if (two == leftUp) reverse = rightDown;
                        if (two == rightUp) reverse = leftDown;
                        if (two == left) reverse = right;
                        if (two == right) reverse = left;
                        if (two == leftDown) reverse = rightUp;
                        if (two == rightDown) reverse = leftUp;
                        hiveRoomArray[(int)current.X][(int)current.Y].isVisit[reverse] = true;
                    }
                }
                else { Write("움직일 경로가 없습니다 !!"); }
            });

            while (target != current)
            {
                roomList = hiveRoomArray[(int)current.X][(int)current.Y].neighbor;
                direction = Direction.non;
                // 방향 지정
                if (current.X == target.X) { }
                else if (current.X > target.X) direction |= Direction.up;
                else direction |= Direction.down;

                if (current.Y == target.Y)
                {
                    if (direction == Direction.down)
                    {
                        if (roomList[leftDown] != null)
                        {
                            if (((Vector2)roomList[leftDown]).Y == current.Y)
                                direction |= Direction.left;
                            else direction |= Direction.right;
                        }
                        else direction |= Direction.right;
                    }
                    else if (direction == Direction.up)
                    {
                        if (roomList[leftUp] != null)
                        {
                            if (((Vector2)roomList[leftUp]).Y == current.Y)
                                direction |= Direction.left;
                            else direction |= Direction.right;
                        }
                        else direction |= Direction.right;
                    }
                }

                else if (current.Y > target.Y) direction |= Direction.left;
                else direction |= Direction.right;


                WriteLine(direction);

                // 움직임
                switch (direction)
                {
                    case Direction.leftUp:
                        action(leftUp, left);
                        break;
                    case Direction.rightUp:
                        action(rightUp, right);
                        break;
                    case Direction.left:
                        action(left, left);
                        break;
                    case Direction.right:
                        action(right, right);
                        break;
                    case Direction.leftDown:
                        action(leftDown, left);
                        break;
                    case Direction.rightDown:
                        action(rightDown, right);
                        break;
                    default:
                        break;
                }
                count++;
            }
            return count;
        }


        // 에너지 흡수
        public int GetEnergy(HiveRoom room)
        {
            int temp = hiveRoomArray[(int)room.position.X][(int)room.position.Y].energy;
            hiveRoomArray[(int)room.position.X][(int)room.position.Y].energy = 0;

            return temp;
        }
        // 페로몬
        public void Pheromone(Vector2 position) 
        {

            hiveRoomArray[(int)position.X][(int)position.Y].isPheromone = true;


        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Hive hive = new Hive();

        }
    }
}

#endif
//https://www.acmicpc.net/problem/19573