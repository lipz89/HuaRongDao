using System;

namespace HRD
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    public class Block
    {
        public BlockType BType { get; set; }
        public Point Location;
        private Block() { }
        public Block(Block block)
        {
            this.ID = block.ID;
            this.Location = block.Location;
            this.BType = block.BType;
            this.Text = block.Text;
            this.PictureId = block.PictureId;
        }

        public Block(int id, Point location, BlockType blockType, string name, int picId)
        {
            this.ID = id;
            this.Location = location;
            this.BType = blockType;
            this.Text = name;
            this.PictureId = picId;
        }

        public bool Contains(Point point)
        {
            return this.GetPoints().Contains(point);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Block))
            {
                return false;
            }
            Block block = (Block)obj;
            return ((this.Location == block.Location) && (this.BType == block.BType));
        }

        public override int GetHashCode()
        {
            return (this.Location.GetHashCode() + this.BType.GetHashCode());
        }

        //public int GetIndex(string name, Game game)
        //{
        //    int index = -1;
        //    foreach (Block b in game.Blocks)
        //    {
        //        if (b.Text == name)
        //        {
        //            this.ID = b.ID;
        //            return index;
        //        }
        //    }
        //    return index;
        //}

        public List<Point> GetPoints()
        {
            List<Point> pList = new List<Point>();
            if (this.BType == BlockType.One)
            {
                pList.Add(this.Location);
                return pList;
            }
            if (this.BType == BlockType.Horizontal)
            {
                pList.Add(this.Location);
                pList.Add(new Point(this.Location.X + 1, this.Location.Y));
                return pList;
            }
            if (this.BType == BlockType.Vertical)
            {
                pList.Add(this.Location);
                pList.Add(new Point(this.Location.X, this.Location.Y + 1));
                return pList;
            }
            if (this.BType == BlockType.Target)
            {
                pList.Add(this.Location);
                pList.Add(new Point(this.Location.X + 1, this.Location.Y));
                pList.Add(new Point(this.Location.X, this.Location.Y + 1));
                pList.Add(new Point(this.Location.X + 1, this.Location.Y + 1));
            }
            return pList;
        }

        public bool Intersects(Block block)
        {
            List<Point> myPoints = this.GetPoints();
            List<Point> otherPoints = block.GetPoints();
            foreach (Point p in otherPoints)
            {
                if (myPoints.Contains(p))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsValid(int width, int height)
        {
            List<Point> points = this.GetPoints();
            foreach (Point p in points)
            {
                if ((((p.X < 0) || (p.X >= width)) || (p.Y < 0)) || (p.Y >= height))
                {
                    return false;
                }
            }
            return true;
        }

        public void ShowDirection(Direction dr, PictureBox pBtn, Image img)
        {
            Point p = new Point(0, 0);
            switch (dr)
            {
                case Direction.Up:
                    p = new Point(pBtn.Width / 3, 0);
                    break;

                case Direction.Down:
                    p = new Point(pBtn.Width / 3, (pBtn.Height / 3) * 2);
                    break;

                case Direction.Left:
                    p = new Point(0, pBtn.Height / 3);
                    break;

                case Direction.Right:
                    p = new Point((pBtn.Width / 3) * 2, pBtn.Height / 3);
                    break;
            }
            Bitmap b = new Bitmap(pBtn.Image, pBtn.Width, pBtn.Height);
            Graphics g = Graphics.FromImage(b);
            Rectangle origReg = new Rectangle(0, 0, img.Size.Width, img.Size.Height);
            Rectangle destReg = new Rectangle(p.X, p.Y, pBtn.Width / 3, pBtn.Height / 3);
            g.DrawImage(img, destReg, origReg, GraphicsUnit.Pixel);
            //frmMain._cur_pic = new Bitmap(pBtn.Image, pBtn.Image.Size.Width, pBtn.Image.Size.Height);
            pBtn.Image = b;
            g.Dispose();
        }

        public int ID { get; set; }

        public int PictureId { get; set; }

        public string Text { get; set; }

        public override string ToString()
        {
            return $"id={ID};x={Location.X};y={Location.Y};text={Text};picid={PictureId};type={BType}";
        }

        public static Block From(string text)
        {
            var block = new Block();
            var sps = text.Split(';');
            foreach (var sp in sps)
            {
                var kv = sp.Split('=');
                var val = kv[1].Trim();
                switch (kv[0].Trim())
                {
                    case "id":
                        block.ID = int.Parse(val);
                        break;
                    case "x":
                        block.Location.X = int.Parse(val);
                        break;
                    case "y":
                        block.Location.Y = int.Parse(val);
                        break;
                    case "picid":
                        block.PictureId = int.Parse(val);
                        break;
                    case "type":
                        block.BType = (BlockType)Enum.Parse(typeof(BlockType), val);
                        break;
                    case "text":
                        block.Text = val;
                        break;
                }
            }

            return block;
        }
    }
}

