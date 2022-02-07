using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
namespace Algorithm_honeybee
{
    public class Room
    {
        // 방의 위치
        public Vector2 position;
        // 이웃한 방들
        public Room?[] neighbor;
        // 방에서 획득 가능한 에너지량
        public int energy;

        // 지나간 길에는 페로몬이 뿌려진다
        public bool[] isPheromone;
        // 디버깅용 내용 확인 메서드
        public void WriteRoom()
        {
            WriteLine("방 위치 : {0}\t에너지량 : {1}",position,energy);
            foreach (var n in neighbor)
                if(n!=null)
                    WriteLine("이웃한 방들 : {0}\t", n.position);
            WriteLine();
        }
        public static Room Copy(Room original)
        {
            if (original == null) return null;

            Room copy = new Room();
            copy.position = original.position;
            copy.energy = original.energy;
            copy.isPheromone = original.isPheromone.ToArray();
            copy.neighbor = original.neighbor.ToArray();

            return copy;
        }

    }
}
