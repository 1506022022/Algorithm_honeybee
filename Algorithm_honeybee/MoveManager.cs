using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static Algorithm_honeybee.Group;
using static System.Console;
namespace Algorithm_honeybee
{

    public class Moving
    {

        public Vector2 go;
        public Action<Bee, Vector2> action;

        public Moving(Action<Bee, Vector2> action, Vector2 go)
        {
            this.action = action;
            this.go = go;
        }
        public void Invoke(Bee bee)
        {
            action.Invoke(bee, go);
        }

        public static void Walk(Bee bee, Vector2 go) { bee.Walk(go); }
        public static void Fly(Bee bee, Vector2 go) { bee.Fly(go); }
    }
    public class Routine
    {

        public List<Moving> route;
        int totalCost;
        int score;
        public Routine(List<Moving> route, Bee bee)
        {
            this.route = route.ToList();

            foreach (var move in route)
            {
                ReadLine();
                bee.action = move;
                bee.action.Invoke(bee);
            }

        }
    }

    public class Group
    {
        public struct RoomInfo
        {
            public Vector2 position;
            public int energy;

        }

        public List<List<RoomInfo>> group = new List<List<RoomInfo>>();
        public Group(Hive hive)
        {
            foreach (var line in hive.rooms)
                foreach (var room in line)
                {
                    Grouping(room);
                }
            SortGroup(group);

            NeighborCounting();



            void NeighborCounting()
            {
                int groupID = 0;
                bool isGroupMember;
                int row, column;
                Room?[] neighbors;


                foreach (var g in hive.rooms)
                    foreach (var room in g)
                    {
                        // 그룹 멤버인가?
                        isGroupMember = group.Any((x) => x.Any((s) => s.position == room.position));

                        // 그룹 멤버가 아니라면
                        if (!isGroupMember)
                        {
                            List<int> groupCount = new List<int>();
                            row = (int)room.position.X;
                            column = (int)room.position.Y;
                            Room current = hive.rooms[row][column];
                            neighbors = current.neighbor;

                            // 인접하는 방들을 서칭해서
                            // 인접한 그룹들의 갯수를 파악해서
                            // 인접그룹갯수에 저장하라

                            foreach (var n in neighbors)
                                for (int i = 0; i < group.Count; i++)
                                    if (group[i].Any((x) => n != null && x.position == n.position))
                                    {
                                        groupCount.Add(i);
                                        break;
                                    }


                            current.neighborGroupCount = groupCount.GroupBy(c => c).Count();

                        }
                    }

            }
        }
        void Grouping(Room room)
        {

            RoomInfo temp = new RoomInfo() { position = room.position, energy = room.energy };
            var list = new List<RoomInfo>();

            // 에너지가 음수이면 넘어가라
            if (room.energy < 0) return;
            // 리스트가 비어 있으면 생성해라
            if (group.Count == 0)
            {
                // 리스트에 이웃조차 없으면 새로운 그룹을 생성 후 추가해라

                list.Add(temp);
                group.Add(list);
            }


            // 리스트에 room 이 이미 있으면 넘어가라
            if (group.Any((x) => x.Contains(temp))) return;
            else
            {


                // 리스트에 room이 없지만 room 의 이웃이 있으면, 이웃이 있는 그룹에 room 을 추가해라
                for (int row = 0; row < group.Count; row++)
                {
                    for (int column = 0; column < group[row].Count; column++)
                    {

                        foreach (var n in room.neighbor)
                        {

                            if (n != null && n.position == group[row][column].position)
                            {


                                group[row].Add(temp);
                                return;
                            }
                        }


                    }
                }
            }
            // 리스트에 이웃조차 없으면 새로운 그룹을 생성 후 추가해라
            list.Add(temp);
            group.Add(list);

        }

        public int GetGroupEnergy(int groupID)
        {
            // groupID 가 그룹 범위를 벗어나면 -1 리턴
            if (groupID >= group.Count || groupID < 0) return -1;

            // 그룹의 에너지총합 리턴
            return group[groupID].Sum((x) => x.energy);
        }

        public void SortGroup(List<List<RoomInfo>> original)
        {
            group = original.OrderByDescending((x) => x.Sum((r) => r.energy)).ToList();
        }

        public void WriteGroup()
        {
            foreach (var g in group)
            {
                WriteLine("\n그룹 시작 ################");
                foreach (var i in g)
                    WriteLine("위치 : {0}\t에너지 : {1} 방의 점수 : {2}", i.position, i.energy);
            }
        }


    }


    public class Simulation

    {
        Group groupInfo;
        Bee bee;
        List<List<RoomInfo>> groups;
        Hive hive;
        List<RoomInfo> currentGroup;
        List<RoomInfo> eatedGroup;

        struct RtoR
        {
            public Vector2 current;
            public Vector2 go;
            public int distance;
        }
        public Simulation()
        {
            eatedGroup = new List<RoomInfo>();
            int max = Start();

            bee.hive.WriteHive();
            WriteLine("벌이 획득한 에너지 : {0}", bee.energy);
            WriteLine("벌이 최대로 획득한 에너지 {0}", max);
        }

        private int Start()
        {
            int max = 0;
            // 벌집을 생성하고
            CreateHive();
            // 그룹을 만들고
            CreateGroup();
            // 그룹력이 가장 큰 그룹을 가져와서
            // 그 그룹의 에너지가 가장 큰 곳에
            // 벌을 생성
            CreateBee();
            // 벌은 그룹을 옮겨다니면서 에너지 획득
            EatGroup();
            max = bee.energy;

            // 모든 그룹을 순회할때까지 반복
            while (groups.Count != 0)
            {
                // 그룹의 모든 에너지를 흡수했으면 다른 그룹으로 이동
                MoveNextGroup();
                EatGroup();
                max = bee.energy >= max ? bee.energy : max;
            }



            return max;
        }

        private void MoveNextGroup()
        {
            List<RtoR> list = new List<RtoR>();
            // 지나온 방들에서 그룹으로 이동할때의
            // 리스트를 가져와라
            foreach (var go in groups)
                foreach (var here in eatedGroup)
                    list.AddRange(ToGroup(here, go));


            // 최단거리만 추출

            var listLinq = from rtor in list
                           where rtor.distance == list.Min(x => x.distance)
                           select rtor;
            list = listLinq.ToList();

            // 경로 생성

            var routines = CreateRoute(list);

            //점수가 높은 순서로 정렬
            var linq = from route in routines
                       select new
                       {
                           score = GetScore(route),
                           route
                       };
            linq = linq.OrderByDescending(x => x.score);


            //일단 벌의 위치를 루트의 시작 위치로 이동


            bee.Go(linq.First().route.First());



            // 우선순위가 가장 높은 길로 이동
            foreach (var route in linq.First().route)
            {

                bee.Move(route);
            }

            // 현재 그룹 지정
            foreach (var g in groupInfo.group)
                if (g.Any(x => x.position == bee.position))
                {
                    currentGroup = g;
                    groups.Remove(g);
                }




            int GetScore(List<Vector2> route)
            {
                int score = 0;
                foreach (var r in route)
                    score += hive.rooms[(int)r.X][(int)r.Y].neighborGroupCount;

                return score;
            }




        }

        private List<List<Vector2>> CreateRoute(List<RtoR> list)
        {
            List<List<Vector2>> routines = new List<List<Vector2>>();

            foreach (var rtor in list)
            {
                // 지나간 경로를 저장할 리스트
                List<Vector2> routine = new List<Vector2>();

                // 현재 위치정보 가져와서
                var room = hive.rooms[(int)rtor.current.X][(int)rtor.current.Y];

                // 지금 방의 이웃들을 검색해서
                // 이동했을때 목적지에 가까워 지는 곳으로 이동하고
                // 최종 경로들을 routines 에 저장
                Enumerate(room, new RtoR() { go = rtor.go, distance = rtor.distance - 1 });

            }

            return routines;

            void Enumerate(Room room, RtoR rtor, List<Vector2> routes = null)
            {
                // 처음 시작했을 때는 리스트를 넘기지 않기 때문에
                // 초기화
                if (routes == null)
                    routes = new List<Vector2>();

                // 자기 자신의 위치를 입력
                routes.Add(room.position);

                // 목적지에 도달했다면
                // 지나온 경로를 반환하고 종료
                if (room.position == rtor.go)
                {
                    routines.Add(routes);
                    return;
                }

                // 이동했을때 목적지에 가까워지는
                var linq = room.neighbor.Where(
                            n => n != null && GetMinDistance(n.position, rtor.go) <= rtor.distance);

                // 모든 경로로 이동한다.
                foreach (var rea in linq)
                    Enumerate(rea, new RtoR() { go = rtor.go, distance = rtor.distance - 1 }, routes.ToList());

            }
        }


        private List<RtoR> ToGroup(RoomInfo here, List<RoomInfo> go)
        {
            List<RtoR> list = new List<RtoR>();

            foreach (var g in go)
                list.Add(new RtoR()
                {
                    current = here.position,
                    go = g.position,
                    distance = GetMinDistance(here.position, g.position)
                });

            return list;
        }

        private void EatGroup()
        {


            foreach (var room in currentGroup)
            {
                bee.Move(room.position);
            }
            foreach (var c in currentGroup)
                eatedGroup.Add(c);
        }

        private void CreateHive()
        {
            // 벌집을 생성
            hive = new Hive();
        }

        private void CreateBee()
        {
            // 위치하고 있는 그룹을 저장하고
            currentGroup = groups.First();
            groups.RemoveAt(0);
            // 그룹 내에서 가장 에너지가 큰 방에 벌을 생성
            var pos = currentGroup.OrderBy((x) => x.energy).First().position;
            bee = new Bee(pos, hive);
        }


        private void CreateGroup()
        {

            hive = new Hive();
            // 그룹을 생성하고
            groupInfo = new Group(hive);

            // 리스트에 저장
            groups = new List<List<RoomInfo>>();

            foreach (var g in groupInfo.group)
                groups.Add(g);

        }

        //최단거리
        public int GetMinDistance(Vector2 current, Vector2 go)
        {
            int X = Abs((int)(current.X - go.X));
            int Y = Abs((int)(current.Y - go.Y));
            return X >= Y ? X : Y;


        }
        int Abs(int i) => i >= 0 ? i : -i;

    }
}
