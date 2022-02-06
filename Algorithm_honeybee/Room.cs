using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
    }
}
