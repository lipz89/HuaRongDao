using System.Linq;
using System.Security.AccessControl;

namespace HRD
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Xml;

    public class Game
    {
        public readonly List<Block> Blocks;
        private byte[,] gameView;
        public const int Height = 5;
        public const int Width = 4;


        public Game()
        {
            this.Blocks = new List<Block>();
            this.gameView = null;
            this.FinishPoint = new Point(1, 3);
        }

        public Game(string url)
        {
            this.Blocks = new List<Block>();
            this.gameView = null;
            this.LoadGame(url);
        }

        public Game(Game game)
        {
            this.Blocks = new List<Block>();
            this.gameView = null;
            this.FinishPoint = game.FinishPoint;
            foreach (Block b in game.Blocks)
            {
                Block nb = new Block(b);
                this.Blocks.Add(nb);
            }
        }

        public bool AddBlock(Block block)
        {
            if (this.Blocks.Contains(block))
            {
                return false;
            }
            if (!block.IsValid(4, 5))
            {
                return false;
            }
            foreach (Block b in this.Blocks)
            {
                if (b.Intersects(block))
                {
                    return false;
                }
            }
            this.Blocks.Add(block);
            this.gameView = null;
            return true;
        }

        public Dictionary<Direction, List<int>> BlockDirection(int blockId)
        {
            return this.BlockDirection(this.Blocks[blockId]);
        }

        public Dictionary<Direction, List<int>> BlockDirection(Block block)
        {
            Dictionary<Direction, List<int>> moves = new Dictionary<Direction, List<int>>();
            foreach (Direction dr in DirectionExtensions.Directions)
            {
                var list = new List<int>();
                Point orgLocation = block.Location;
                int step = 1;
                while (true)
                {
                    var moveOK = CanMove(block, dr, step);
                    if (!moveOK)
                    {
                        block.Location = orgLocation;
                        break;
                    }
                    else
                    {
                        list.Add(step);
                    }
                    block.Location = orgLocation;
                    step++;
                }

                if (list.Any())
                {
                    list.Reverse();
                    moves.Add(dr, list);
                }
            }
            return moves;
        }

        public List<List<Direction>> BlockMoves(int blockId)
        {
            return this.BlockMoves(this.Blocks[blockId]);
        }
        public List<List<Direction>> BlockMoves(Block block)
        {
            List<List<Direction>> moves = new List<List<Direction>>();
            Point orgLocation = block.Location;
            foreach (var grp in DirectionExtensions.DoubleDirections().GroupBy(x => x.First()))
            {
                var moveOK = CanMove(block, grp.Key);
                if (!moveOK)
                {
                    block.Location = orgLocation;
                    continue;
                }

                var orgLocation2 = block.Location;

                foreach (var drs in grp)
                {
                    moveOK = CanMove(block, drs[1]);
                    if (!moveOK)
                    {
                        block.Location = orgLocation2;
                        continue;
                    }
                    moves.Add(drs);
                    block.Location = orgLocation2;
                }
                moves.Add(new List<Direction> { grp.Key });

                block.Location = orgLocation;
            }
            return moves;
        }

        private bool CanMove(Block block, Direction dr, int step = 1)
        {
            switch (dr)
            {
                case Direction.Up:
                    block.Location.Y -= step;
                    break;

                case Direction.Down:
                    block.Location.Y += step;
                    break;

                case Direction.Left:
                    block.Location.X -= step;
                    break;

                case Direction.Right:
                    block.Location.X += step;
                    break;
            }

            bool moveOK = true;
            if (!block.IsValid(4, 5))
            {
                moveOK = false;
            }
            else
            {
                foreach (Block b in this.Blocks)
                {
                    if ((b != block) && block.Intersects(b))
                    {
                        moveOK = false;
                        break;
                    }
                }
            }

            return moveOK;
        }

        public void ClearBlock()
        {
            this.Blocks.Clear();
            this.gameView = null;
        }

        public override bool Equals(object obj)
        {
            if (!((obj != null) && (obj is Game)))
            {
                return false;
            }
            Game g = (Game)obj;
            if (this.FinishPoint != g.FinishPoint)
            {
                return false;
            }
            if ((this.Blocks.Count != 0) || (g.Blocks.Count != 0))
            {
                if (this.Blocks.Count != g.Blocks.Count)
                {
                    return false;
                }
                if (!this.Blocks[0].Equals(g.Blocks[0]))
                {
                    return false;
                }
                if (this.gameView == null)
                {
                    this.gameView = this.GetGameView();
                }
                if (g.gameView == null)
                {
                    g.gameView = g.GetGameView();
                }
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (this.gameView[i, j] != g.gameView[i, j])
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool GameWin()
        {
            return this.Blocks[0].Location.Equals(this.FinishPoint);
        }

        private byte[,] GetGameView()
        {
            byte[,] view = new byte[4, 5];
            foreach (Block b in this.Blocks)
            {
                byte typeCode = 0;
                switch (b.BType)
                {
                    case BlockType.One:
                        typeCode = 1;
                        break;

                    case BlockType.TwoH:
                        typeCode = 2;
                        break;

                    case BlockType.TwoV:
                        typeCode = 3;
                        break;

                    case BlockType.Four:
                        typeCode = 4;
                        break;
                }
                foreach (Point p in b.GetPoints())
                {
                    view[p.X, p.Y] = typeCode;
                }
            }
            return view;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (Block b in this.Blocks)
            {
                hash += b.GetHashCode();
            }
            return (hash + this.FinishPoint.GetHashCode());
        }

        private void LoadGame(string url)
        {
            XmlReader xr = null;
            try
            {
                XmlReaderSettings xrs = new XmlReaderSettings();
                xrs.IgnoreComments = true;
                xrs.IgnoreWhitespace = true;
                xrs.IgnoreProcessingInstructions = true;
                xr = XmlReader.Create(url, xrs);
                xr.Read();
                xr.ReadStartElement("game");
                xr.ReadStartElement("finish");
                xr.ReadStartElement("x");
                int x = Convert.ToInt32(xr.ReadString());
                xr.ReadEndElement();
                xr.ReadStartElement("y");
                int y = Convert.ToInt32(xr.ReadString());
                xr.ReadEndElement();
                xr.ReadEndElement();
                this.FinishPoint = new Point(x, y);
                while (xr.Read())
                {
                    if (xr.IsStartElement())
                    {
                        xr.ReadStartElement("location");
                        xr.ReadStartElement("x");
                        x = Convert.ToInt32(xr.ReadString());
                        xr.ReadEndElement();
                        xr.ReadStartElement("y");
                        y = Convert.ToInt32(xr.ReadString());
                        xr.ReadEndElement();
                        xr.ReadEndElement();
                        xr.ReadStartElement("type");
                        string type = xr.ReadString();
                        xr.ReadEndElement();
                        xr.ReadStartElement("text");
                        string text = xr.ReadString();
                        xr.ReadEndElement();
                        xr.ReadStartElement("pictureID");
                        int picID = Convert.ToInt32(xr.ReadString());
                        xr.ReadEndElement();
                        xr.ReadEndElement();
                        BlockType bType = (BlockType)Enum.Parse(typeof(BlockType), type);
                        //(type == "One") ? BlockType.One : ((type == "TwoH") ? BlockType.TwoH : ((type == "TwoV") ? BlockType.TwoV : BlockType.Four));
                        this.AddBlock(new Block(new Point(x, y), bType, text, picID));
                    }
                }
            }
            finally
            {
                xr?.Close();
            }
        }

        public bool MoveBlock(int blockId, Direction direction, int step = 1)
        {
            return this.MoveBlock(this.Blocks[blockId], direction, step);
        }

        public bool MoveBlock(Block block, Direction direction, int step = 1)
        {
            if (!this.Blocks.Contains(block))
            {
                throw new Exception("非此游戏中的块！");
            }
            Point orgLocation = block.Location;
            switch (direction)
            {
                case Direction.Up:
                    block.Location.Y -= step;
                    break;

                case Direction.Down:
                    block.Location.Y += step;
                    break;

                case Direction.Left:
                    block.Location.X -= step;
                    break;

                case Direction.Right:
                    block.Location.X += step;
                    break;
            }
            bool moveOK = true;
            if (!block.IsValid(4, 5))
            {
                moveOK = false;
            }
            else
            {
                foreach (Block b in this.Blocks)
                {
                    if ((b != block) && block.Intersects(b))
                    {
                        moveOK = false;
                        break;
                    }
                }
            }
            if (!moveOK)
            {
                block.Location = orgLocation;
                return moveOK;
            }
            this.gameView = null;
            return moveOK;
        }

        public bool RemoveBlock(int blockId)
        {
            if ((blockId < 0) || (blockId >= this.Blocks.Count))
            {
                return false;
            }
            this.Blocks.RemoveAt(blockId);
            this.gameView = null;
            return true;
        }

        public bool RemoveBlock(Block block)
        {
            if (!this.Blocks.Contains(block))
            {
                return false;
            }
            this.Blocks.Remove(block);
            this.gameView = null;
            return true;
        }

        public void SaveGame(string url)
        {
            XmlWriter xw = null;
            try
            {
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.Indent = true;
                xws.IndentChars = "\t";
                xw = XmlWriter.Create(url, xws);
                xw.WriteStartDocument();
                xw.WriteStartElement("game");
                xw.WriteStartElement("finish");
                xw.WriteStartElement("x");
                xw.WriteString(this.FinishPoint.X.ToString());
                xw.WriteEndElement();
                xw.WriteStartElement("y");
                xw.WriteString(this.FinishPoint.Y.ToString());
                xw.WriteEndElement();
                xw.WriteEndElement();
                foreach (Block b in this.Blocks)
                {
                    xw.WriteStartElement("block");
                    xw.WriteStartElement("location");
                    xw.WriteStartElement("x");
                    xw.WriteString(b.Location.X.ToString());
                    xw.WriteEndElement();
                    xw.WriteStartElement("y");
                    xw.WriteString(b.Location.Y.ToString());
                    xw.WriteEndElement();
                    xw.WriteEndElement();
                    xw.WriteStartElement("type");
                    xw.WriteString(b.BType.ToString());
                    xw.WriteEndElement();
                    xw.WriteStartElement("text");
                    xw.WriteString(b.Text);
                    xw.WriteEndElement();
                    xw.WriteStartElement("pictureID");
                    xw.WriteString(b.pictureID.ToString());
                    xw.WriteEndElement();
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
            }
            finally
            {
                xw?.Close();
            }
        }

        public Point FinishPoint { get; set; }
    }
}

