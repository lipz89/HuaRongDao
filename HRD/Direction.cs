using System;
using System.Collections.Generic;
using System.Linq;

namespace HRD
{
    public enum Direction
    {
        Up,
        Left,
        Down,
        Right
    }

    public static class DirectionExtensions
    {
        public static readonly Direction[] Directions;
        private static readonly Dictionary<Direction, string> dirs = new Dictionary<Direction, string>()
        {
            {Direction.Down, "下" },
            {Direction.Left, "左" },
            {Direction.Right, "右" },
            {Direction.Up, "上" },
        };
        static DirectionExtensions()
        {
            Directions = (Direction[])Enum.GetValues(typeof(Direction));
        }

        public static bool IsReverse(this Direction direction, Direction newDirection)
        {
            return Math.Abs(direction - newDirection) == 2;
        }
        public static string String(this Direction direction)
        {
            return dirs[direction];
        }
        public static string String(this IEnumerable<Direction> directions)
        {
            var s = directions.Select(x => x.String()).ToList();
            if (s.Any())
            {
                return string.Join("", s);
                //var str = "";
                //var last = "";
                //var count = 1;
                //for (int i = 0; i < s.Count; i++)
                //{
                //    var th = s[i];
                //    if (last == th)
                //    {
                //        count++;
                //    }
                //    else
                //    {
                //        if (count > 1)
                //            str += count;
                //        str += th;
                //        last = th;
                //        count = 1;
                //    }
                //}

                //if (count > 1)
                //    str += count;

                //return str;
            }

            return null;
        }

        public static List<List<Direction>> DoubleDirections()
        {
            var list = new List<List<Direction>>();
            foreach (var direction in Directions)
            {
                foreach (var direction1 in Directions)
                {
                    if (!direction.IsReverse(direction1))
                    {
                        var l = new List<Direction> { direction, direction1 };
                        list.Add(l);
                    }
                }
            }
            return list;
        }
    }
}

