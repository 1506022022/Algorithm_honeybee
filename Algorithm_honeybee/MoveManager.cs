using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
namespace Algorithm_honeybee
{
    public class MoveManager
    {
        public MoveManager() {
            hive = new Hive(); }
        public MoveManager(Hive hive) 
        {
            this.hive = Hive.Copy(hive);

        }

        Hive hive;
        Action bestMove;
        Bee bee;

        public int Simulation(Hive hive) 
        {
            int Maximum = 0;
            bool notStay = true;
            foreach (var line in hive.rooms)
                foreach (var room in line)
                {


                    // n회 움직였을 때
                    for (int i = 0; i <= hive.rooms.Sum((r)=>r.Count);i++) 
                    {

                        bee = new Bee(room.position, Hive.Copy(hive));

                        if (i == 0) continue;
                        
                        else
                        {
                            for (int j = 1; j <= i; j++)
                            {

                                
                                bee.Go(SmartMove(bee, notStay));
                                Maximum = Maximum <= bee.energy ? bee.energy : Maximum;

                            }
                        }
                    }

                }

            return Maximum;


            Vector2 SmartMove(Bee bee, bool notStay = false)
            {

                int Maximum = 0;
                Vector2 finalyGo = new Vector2() { X = (int)bee.position.X, Y = (int)bee.position.Y };
                Bee temp;
                foreach (var line in hive.rooms)
                {
                    foreach (var room in line)
                    {

                        temp = Bee.Copy(bee);


                        if (notStay && room.position == temp.position) continue;
                        


                        temp.Go(room.position);


                        if ((notStay && Maximum == 0) || (temp.energy >= Maximum))
                        {
                            
                            Maximum = temp.energy;
                            finalyGo = temp.position;
                        }

                       
                    }

                }

                return finalyGo;

            }

            //Vector2 SmartMove_V2(Bee bee, bool notStay = false) 
            //{
            //    Bee temp = Bee.Copy(bee);

            //    Room current = temp.hive.rooms[(int)temp.position.X][(int)temp.position.Y];
            //    Room?[] rooms = current.neighbor.ToArray();

            //    Vector2 Go = new Vector2();

            //    // 페이즈 1 이웃 중 에너지가 큰 쪽으로 이동
            //    if (rooms.Select((n) => n.energy).Max() > 0)
            //    {
            //        var pos = from r in rooms
            //                  orderby r.energy descending
            //                  select r.position;

            //            Go = pos.First();
            //    }
            //    // 페이즈 2 주변에서 가장 손해가 적은 쪽으로 이동

            //}
        }


    }

}
