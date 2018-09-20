using System.Collections.Generic;

namespace HRD
{
    public class MoveStep
    {
        public int MoveBlockId { get; }
        public Direction MoveDirection { get; }
        public int Step { get; }

        public MoveStep(int id, Direction dir, int step = 1)
        {
            this.MoveBlockId = id;
            this.MoveDirection = dir;
            this.Step = step;
        }

        public override string ToString()
        {
            return (this.MoveBlockId + " >> " + this.MoveDirection + " " + this.Step);
        }
    }

    public class MoveItem
    {
        public int MoveBlockId { get; }
        public List<Direction> MoveDirections { get; }

        public MoveItem(int moveBlockId, List<Direction> moveDirections)
        {
            MoveBlockId = moveBlockId;
            MoveDirections = moveDirections;
        }

        public override string ToString()
        {
            return this.MoveBlockId + " >> " + this.MoveDirections.String();
        }
    }
}

