using System.Collections.Generic;

namespace HRD
{
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

