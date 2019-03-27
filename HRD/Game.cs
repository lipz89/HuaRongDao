using System.Linq;

namespace HRD
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public class Game
    {
        private byte[,] gameView;
        private const int Height = 5;
        private const int Width = 4;


        public Game()
        {
            this.Blocks = new List<Block>();
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

            if (!block.IsValid(Width, Height))
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
            if (!block.IsValid(Width, Height))
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

        //public void ClearBlock()
        //{
        //    this.Blocks.Clear();
        //    this.gameView = null;
        //}

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

                    case BlockType.Horizontal:
                        typeCode = 2;
                        break;

                    case BlockType.Vertical:
                        typeCode = 3;
                        break;

                    case BlockType.Target:
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

        //private void LoadGame(string url)
        //{
        //    XmlReader xr = null;
        //    try
        //    {
        //        XmlReaderSettings xrs = new XmlReaderSettings();
        //        xrs.IgnoreComments = true;
        //        xrs.IgnoreWhitespace = true;
        //        xrs.IgnoreProcessingInstructions = true;
        //        xr = XmlReader.Create(url, xrs);
        //        xr.Read();
        //        xr.ReadStartElement("game");
        //        xr.ReadStartElement("finish");
        //        xr.ReadStartElement("x");
        //        int x = Convert.ToInt32(xr.ReadString());
        //        xr.ReadEndElement();
        //        xr.ReadStartElement("y");
        //        int y = Convert.ToInt32(xr.ReadString());
        //        xr.ReadEndElement();
        //        xr.ReadEndElement();
        //        this.FinishPoint = new Point(x, y);
        //        var id = 0;
        //        while (xr.Read())
        //        {
        //            if (xr.IsStartElement())
        //            {
        //                xr.ReadStartElement("location");
        //                xr.ReadStartElement("x");
        //                x = Convert.ToInt32(xr.ReadString());
        //                xr.ReadEndElement();
        //                xr.ReadStartElement("y");
        //                y = Convert.ToInt32(xr.ReadString());
        //                xr.ReadEndElement();
        //                xr.ReadEndElement();
        //                xr.ReadStartElement("type");
        //                string type = xr.ReadString();
        //                xr.ReadEndElement();
        //                xr.ReadStartElement("text");
        //                string text = xr.ReadString();
        //                xr.ReadEndElement();
        //                xr.ReadStartElement("pictureID");
        //                int picID = Convert.ToInt32(xr.ReadString());
        //                xr.ReadEndElement();
        //                xr.ReadEndElement();
        //                BlockType bType = (BlockType)Enum.Parse(typeof(BlockType), type);
        //                //(type == "One") ? BlockType.One : ((type == "Horizontal") ? BlockType.Horizontal : ((type == "Vertical") ? BlockType.Vertical : BlockType.Target));
        //                this.AddBlock(new Block(id, new Point(x, y), bType, text, picID));
        //                id++;
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        xr?.Close();
        //    }
        //}

        public bool MoveBlock(int blockId, Direction direction, bool isBack = false)
        {
            return this.MoveBlock(this.Blocks[blockId], direction, isBack);
        }

        public bool MoveBlock(Block block, Direction direction, bool isBack = false)
        {
            if (!this.Blocks.Contains(block))
            {
                throw new Exception("非此游戏中的块！");
            }

            Point orgLocation = block.Location;
            var step = isBack ? -1 : 1;
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

        //public void SaveGame(string url)
        //{
        //    XmlWriter xw = null;
        //    try
        //    {
        //        XmlWriterSettings xws = new XmlWriterSettings();
        //        xws.Indent = true;
        //        xws.IndentChars = "\t";
        //        xw = XmlWriter.Create(url, xws);
        //        xw.WriteStartDocument();
        //        xw.WriteStartElement("game");
        //        xw.WriteStartElement("finish");
        //        xw.WriteStartElement("x");
        //        xw.WriteString(this.FinishPoint.X.ToString());
        //        xw.WriteEndElement();
        //        xw.WriteStartElement("y");
        //        xw.WriteString(this.FinishPoint.Y.ToString());
        //        xw.WriteEndElement();
        //        xw.WriteEndElement();
        //        foreach (Block b in this.Blocks)
        //        {
        //            xw.WriteStartElement("block");
        //            xw.WriteStartElement("location");
        //            xw.WriteStartElement("x");
        //            xw.WriteString(b.Location.X.ToString());
        //            xw.WriteEndElement();
        //            xw.WriteStartElement("y");
        //            xw.WriteString(b.Location.Y.ToString());
        //            xw.WriteEndElement();
        //            xw.WriteEndElement();
        //            xw.WriteStartElement("type");
        //            xw.WriteString(b.BType.ToString());
        //            xw.WriteEndElement();
        //            xw.WriteStartElement("text");
        //            xw.WriteString(b.Text);
        //            xw.WriteEndElement();
        //            xw.WriteStartElement("pictureID");
        //            xw.WriteString(b.PictureId.ToString());
        //            xw.WriteEndElement();
        //            xw.WriteEndElement();
        //        }

        //        xw.WriteEndElement();
        //    }
        //    finally
        //    {
        //        xw?.Close();
        //    }
        //}

        public Point FinishPoint { get; set; }

        public List<Block> Blocks { get; set; }

        public override string ToString()
        {
            var str = "hrdmap".PadRight(8, ' ');
            str += "\r\n";
            str += $"target:x={FinishPoint.X};y={FinishPoint.Y}";
            str += "\r\n";
            foreach (var block in this.Blocks)
            {
                str += "block:" + block;
                str += "\r\n";
            }

            return str;
        }

        public static Game From(string text)
        {
            var game = new Game();
            var lines = text.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.StartsWith("hrdmap")) continue;
                if (line.StartsWith("target:"))
                {
                    var sps = line.Replace("target:", "").Split(';');
                    int x = 1;
                    int y = 3;
                    foreach (var sp in sps)
                    {
                        var kv = sp.Split('=');
                        var val = kv[1].Trim();
                        switch (kv[0].Trim())
                        {
                            case "x":
                                x = int.Parse(val);
                                break;
                            case "y":
                                y = int.Parse(val);
                                break;
                        }
                    }

                    game.FinishPoint = new Point(x, y);
                }

                if (line.StartsWith("block:"))
                {
                    game.Blocks.Add(Block.From(line.Replace("block:", "")));
                }
            }

            return game;
        }
    }
}