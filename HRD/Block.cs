using System.Linq;

namespace HRD
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    public class Block
    {
        public readonly BlockType BType;
        public Point Location;

        public Block(Block block)
        {
            this.index = 0;
            this.direction = Direction.Up;
            this.Pardent = null;
            this.backPicture = null;
            this.Location = block.Location;
            this.BType = block.BType;
            this.Text = block.Text;
            this.pictureID = block.pictureID;
        }

        public Block(Point location, BlockType blockType, string name, int PicID)
        {
            this.index = 0;
            this.direction = Direction.Up;
            this.Pardent = null;
            this.backPicture = null;
            this.Location = location;
            this.BType = blockType;
            this.Text = name;
            this.pictureID = PicID;
        }

        public bool Contains(Point point)
        {
            return this.GetPoints().Contains(point);
        }

        public override bool Equals(object obj)
        {
            if (!((obj != null) && (obj is Block)))
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

        public int getIndex(string name, Game game)
        {
            int index = -1;
            foreach (Block b in game.Blocks)
            {
                if (b.Text == name)
                {
                    this.index = b.index;
                    return index;
                }
            }
            return index;
        }

        public List<Point> GetPoints()
        {
            List<Point> pList = new List<Point>();
            if (this.BType == BlockType.One)
            {
                pList.Add(this.Location);
                return pList;
            }
            if (this.BType == BlockType.TwoH)
            {
                pList.Add(this.Location);
                pList.Add(new Point(this.Location.X + 1, this.Location.Y));
                return pList;
            }
            if (this.BType == BlockType.TwoV)
            {
                pList.Add(this.Location);
                pList.Add(new Point(this.Location.X, this.Location.Y + 1));
                return pList;
            }
            if (this.BType == BlockType.Four)
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

        public void showDirection(Direction dr, PictureBox p_Btn, Image img)
        {
            Point p = new Point(0, 0);
            switch (dr)
            {
                case Direction.Up:
                    p = new Point(p_Btn.Width / 3, 0);
                    break;

                case Direction.Down:
                    p = new Point(p_Btn.Width / 3, (p_Btn.Height / 3) * 2);
                    break;

                case Direction.Left:
                    p = new Point(0, p_Btn.Height / 3);
                    break;

                case Direction.Right:
                    p = new Point((p_Btn.Width / 3) * 2, p_Btn.Height / 3);
                    break;
            }
            Bitmap b = new Bitmap(p_Btn.Image, p_Btn.Width, p_Btn.Height);
            Graphics g = Graphics.FromImage(b);
            Rectangle origReg = new Rectangle(0, 0, img.Size.Width, img.Size.Height);
            Rectangle destReg = new Rectangle(p.X, p.Y, p_Btn.Width / 3, p_Btn.Height / 3);
            g.DrawImage(img, destReg, origReg, GraphicsUnit.Pixel);
            frmMain._cur_pic = new Bitmap(p_Btn.Image, p_Btn.Image.Size.Width, p_Btn.Image.Size.Height);
            p_Btn.Image = b;
            g.Dispose();
        }

        public Image backPicture { get; set; }

        public Direction direction { get; set; }

        public int index { get; set; }

        public Control Pardent { get; set; }

        public int pictureID { get; set; }

        public string Text { get; set; }

        public override string ToString()
        {
            return this.Text + " " + this.Location;
        }
    }
}

