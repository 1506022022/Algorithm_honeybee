using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

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

        [Flags]
        public enum Direction
        {
            non = 0,
            left = 1,
            right = 2,
            up = 4,
            down = 8,
            leftUp = left | up,
            rightUp = right | up,
            leftDown = left | down,
            rightDown = right | down
        };

        public static int DirectSet(Vector2 current, Vector2 go,Hive hive) 
        {
            Direction dir = Direction.non;
            if (current != go) { 

                if (current.X > go.X) dir |= Direction.up;
                else if (current.X < go.X) dir |= Direction.down;

                if (current.Y > go.Y) dir |= Direction.left;
                else if (current.Y < go.Y) dir |= Direction.right;
                else
                {
                    // 상향 이동 시
                    if ((dir & Direction.up) != Direction.non) {
                       
                        if (hive.rooms[(int)current.X][(int)current.Y].neighbor[0] == null) dir |= Direction.right;
                        else if (hive.rooms[(int)current.X][(int)current.Y].neighbor[0].position.Y
                            == go.Y) dir |= Direction.left;
                        else dir |= Direction.right;
                            }





                    // 하향 이동 시
                    if ((dir & Direction.down)!= Direction.non){
                        if (hive.rooms[(int)current.X][(int)current.Y].neighbor[4] == null) dir |= Direction.right;
                        else if (hive.rooms[(int)current.X][(int)current.Y].neighbor[4].position.Y
                            == go.Y) dir |= Direction.left;
                        else dir |= Direction.right;
                    }
                }

                // dir 에 방향 할당 완료
                switch (dir)
                {
                    case Direction.leftUp:
                        return leftUp;
                    case Direction.rightUp:
                        return rightUp;
                    case Direction.left:
                        return left;
                    case Direction.right:
                        return right;
                    case Direction.leftDown:
                        return leftDown;
                    case Direction.rightDown:
                        return rightDown;
                    default:
                        WriteLine("잘못된 방향지정");
                        break;

                }

            }
            return -1;
        }
    }


}
