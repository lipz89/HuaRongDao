using System.Linq;

namespace HRD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;

    public class frmMain : Form
    {
        public static Image _cur_pic = null;
        public static frmMain _frmMain = null;
        private const int BlockSize = 80;
        private Button btnBack;
        private Button btnStart;
        private ComboBox comboBoxHRD;
        private IContainer components = null;
        private int demoStepNo;
        //private FrmPictures frmPic = new FrmPictures();
        private Game game;
        public GroupBox groupBox1;
        public GroupBox groupBox3;
        private PictureBox Img_b1;
        private PictureBox Img_b2;
        private PictureBox Img_b3;
        private PictureBox Img_b4;
        private PictureBox Img_cc;
        private PictureBox Img_gy;
        private PictureBox Img_J1;
        private PictureBox Img_J2;
        private PictureBox Img_J3;
        private PictureBox Img_J4;
        public Label lab31;
        public Label label1;
        public Label labSTEP;
        private Label lab出口;
        private Label lblFinishLocation;
        public ListView listView1;
        private MenuStrip menuStripMain;
        private bool moveEnabled = true;
        private Game orgGame;
        private Panel panel2;
        public Panel pnlGame;
        private MoveProcess process;
        private bool showTempPic = false;
        private System.Windows.Forms.Timer timer1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSeparator toolStripSeparator4;
        private TrackBar trackBarTime;
        private ToolStripMenuItem 保存ToolStripMenuItem;
        private ToolStripMenuItem 保存棋局toolStripMenuItem;
        private ToolStripMenuItem 功能ToolStripMenuItem;
        private ToolStripMenuItem 关闭游戏ToolStripMenuItem;
        private ToolStripMenuItem 关于ToolStripMenuItem;
        private ToolStripMenuItem 显示步骤计时ToolStripMenuItem;
        private ToolStripMenuItem 装载toolStripMenuItem;
        private ToolStripMenuItem 关于AToolStripMenuItem;
        private ToolStripMenuItem 自动模式ToolStripMenuItem;

        public frmMain()
        {
            this.InitializeComponent();
            _frmMain = this;
        }

        private void btn_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.moveEnabled)
            {
                PictureBox btn = (PictureBox)sender;
                Block block = (Block)btn.Tag;
                if (!this.d2(btn) || this.showTempPic)
                {
                    List<Direction> drs = this.game.BlockDirection(block).Keys.ToList();
                    if (this.showTempPic)
                    {
                        bool bl = false;
                        Direction dr = Direction.Up;
                        if (((e.Y < (btn.Height / 3)) && (e.X > (btn.Width / 3))) && (e.X < ((btn.Width / 3) * 2)))
                        {
                            bl = this.game.MoveBlock(block, Direction.Up);
                        }
                        if (((e.Y > ((btn.Height / 3) * 2)) && (e.X > (btn.Width / 3))) && (e.X < ((btn.Width / 3) * 2)))
                        {
                            bl = this.game.MoveBlock(block, Direction.Down);
                            dr = Direction.Down;
                        }
                        if (((e.X < (btn.Width / 3)) && (e.Y > (btn.Height / 3))) && (e.Y < ((btn.Height / 3) * 2)))
                        {
                            bl = this.game.MoveBlock(block, Direction.Left);
                            dr = Direction.Left;
                        }
                        if (((e.X > ((btn.Width / 3) * 2)) && (e.Y > (btn.Height / 3))) && (e.Y < ((btn.Height / 3) * 2)))
                        {
                            bl = this.game.MoveBlock(block, Direction.Right);
                            dr = Direction.Right;
                        }
                        if (bl)
                        {
                            btn.Image = Resource._Imgs[block.pictureID];
                            this.UpdateButtonLocation(btn);
                            this.showTempPic = false;
                            this.DoStep(block, dr);
                        }
                    }
                    else if (drs.Count < 2)
                    {
                        foreach (Direction dr in drs)
                        {
                            if (this.game.MoveBlock(block, dr))
                            {
                                this.UpdateButtonLocation(btn);
                                this.DoStep(block, dr);
                                this.showTempPic = false;
                            }
                        }
                    }
                    if (this.game.GameWin())
                    {
                        MessageBox.Show("游戏胜利！");
                    }
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (this.listView1.Items.Count >= 1)
            {
                var item = this.listView1.Items[this.listView1.Items.Count - 1];
                string[] idd = (item.SubItems[0].Text + "\t" + item.SubItems[1].Text + "\t" + item.SubItems[2].Text + "\t" + item.SubItems[3].Text).Split(new char[] { '\t' });
                this.DostepArr(true, idd);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.game = new Game(@"C:\Hrd_temp.xml");
            this.InitGame();
        }

        private void comboBoxHRD_SelectedIndexChanged(object sender, EventArgs e)
        {
            String projectName = Assembly.GetExecutingAssembly().GetName().Name.ToString();
            Ini.GetHrDFile(projectName + ".XML." + this.comboBoxHRD.Text + ".xml");
            this.game = new Game(@"C:\Hrd_temp.xml");
            this.InitGame();
        }

        private bool d2(PictureBox btn)
        {
            bool b = false;
            Block block = (Block)btn.Tag;
            List<Direction> drs = this.game.BlockDirection(block).Keys.ToList();
            foreach (Direction dr in drs)
            {
                if (drs.Count > 1)
                {
                    switch (dr)
                    {
                        case Direction.Up:
                            block.showDirection(dr, btn, Resource.Up);
                            break;

                        case Direction.Down:
                            block.showDirection(dr, btn, Resource.Down);
                            break;

                        case Direction.Left:
                            block.showDirection(dr, btn, Resource.Left);
                            break;

                        case Direction.Right:
                            block.showDirection(dr, btn, Resource.Right);
                            break;
                    }
                    b = true;
                }
                else
                {
                    btn.Image = Resource._Imgs[block.pictureID];
                }
            }
            this.showTempPic = b;
            return b;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void doLV()
        {
            this.listView1.View = View.Details;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Columns.Add("步", 0x23, HorizontalAlignment.Center);
            this.listView1.Columns.Add("棋子", 0x24, HorizontalAlignment.Left);
            this.listView1.Columns.Add("索", 0x19, HorizontalAlignment.Center);
            this.listView1.Columns.Add("行动", 0x2b, HorizontalAlignment.Center);
        }
        private void DoStep(Block block, Direction dr, int step = 1)
        {
            ListViewItem item = new ListViewItem("步" + (this.listView1.Items.Count + 1));
            item.SubItems.Add("" + block.Text);
            item.SubItems.Add("" + block.index);
            item.SubItems.Add("" + dr.String() + step);
            this.listView1.Items.Add(item);
            this.listView1.Items[this.listView1.Items.Count - 1].Selected = true;
            this.listView1.HideSelection = false;
            this.labSTEP.Text = this.listView1.Items.Count.ToString();
        }
        private void DoStep(Block block, MoveItem dr)
        {
            ListViewItem item = new ListViewItem("步" + (this.listView1.Items.Count + 1));
            item.SubItems.Add("" + block.Text);
            item.SubItems.Add("" + block.index);
            item.SubItems.Add("" + dr.MoveDirections.String());
            this.listView1.Items.Add(item);
            this.listView1.Items[this.listView1.Items.Count - 1].Selected = true;
            this.listView1.HideSelection = false;
            this.labSTEP.Text = this.listView1.Items.Count.ToString();
        }

        private void DostepArr(bool DoBack, string[] a)
        {
            Application.DoEvents();
            if (a[0].Length >= 1)
            {
                Direction dr = Direction.Up;
                string cs_4_0001 = a[3];
                if (cs_4_0001 == null)
                {
                    return;
                }
                var dir = cs_4_0001[0];
                var step = 1;
                var flag = int.TryParse(cs_4_0001[1] + "", out step);
                dr = ParseDirection(dir, DoBack);

                PictureBox btn = null;
                foreach (Control p in this.pnlGame.Controls)
                {
                    if (p.Text == a[1])
                    {
                        btn = (PictureBox)p;
                    }
                }

                var m = this.game.MoveBlock(Convert.ToInt32(a[2]), dr, step);
                if (!flag)
                {
                    dr = ParseDirection(cs_4_0001[1], DoBack);
                    m &= this.game.MoveBlock(Convert.ToInt32(a[2]), dr);
                }
                if ((btn != null) && m)
                {
                    this.UpdateButtonLocation(btn);
                    if (DoBack)
                    {
                        this.listView1.Items.RemoveAt(this.listView1.Items.Count - 1);
                        this.labSTEP.Text = this.listView1.Items.Count.ToString();
                        if (this.listView1.Items.Count > 0)
                        {
                            this.listView1.Items[this.listView1.Items.Count - 1].Selected = true;
                        }
                    }
                }
            }
        }

        private static Direction ParseDirection(char dir, bool doBack = false)
        {
            Direction dr;
            if (dir == '下')
            {
                dr = doBack ? Direction.Up : Direction.Down;
            }
            else if (dir == '左')
            {
                dr = doBack ? Direction.Right : Direction.Left;
            }
            else if (dir == '右')
            {
                dr = doBack ? Direction.Left : Direction.Right;
            }
            else
            {
                dr = doBack ? Direction.Down : Direction.Up;
            }
            return dr;
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            base.Height = 460;

            string[] s = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            string[] st = null;
            foreach (string str in s)
            {
                if (str.EndsWith("xml"))
                {
                    st = str.Split('.');
                    this.comboBoxHRD.Items.Add(st[2]);
                }
            }

            comboBoxHRD.SelectedIndex = 0;
            this.game = new Game(@"C:\Hrd_temp.xml");
            this.InitGame();
            this.doLV();
        }

        private void InitGame()
        {
            this.listView1.Items.Clear();
            this.pnlGame.Controls.Clear();
            this.pnlGame.Width = 320;
            this.pnlGame.Height = 400;
            int index = 0;
            foreach (Block block in this.game.Blocks)
            {
                PictureBox btn = new PictureBox();
                switch (block.BType)
                {
                    case BlockType.One:
                        btn.Width = BlockSize;
                        btn.Height = BlockSize;
                        break;

                    case BlockType.TwoH:
                        btn.Width = BlockSize * 2;
                        btn.Height = BlockSize;
                        break;

                    case BlockType.TwoV:
                        btn.Width = BlockSize;
                        btn.Height = BlockSize * 2;
                        break;

                    case BlockType.Four:
                        btn.Width = BlockSize * 2;
                        btn.Height = BlockSize * 2;
                        break;
                }
                block.index = index;
                index++;
                btn.Text = block.Text;
                btn.BorderStyle = BorderStyle.Fixed3D;
                btn.Tag = block;
                btn.BackColor = SystemColors.Control;
                btn.Image = Resource._Imgs[block.pictureID];
                btn.SizeMode = PictureBoxSizeMode.StretchImage;
                this.UpdateButtonLocation(btn);
                btn.MouseClick += new MouseEventHandler(this.btn_MouseClick);
                this.pnlGame.Controls.Add(btn);
            }
            this.pnlGame.Controls.Add(this.lab出口);
            this.lblFinishLocation = new Label();
            this.lblFinishLocation.Left = this.game.FinishPoint.X * BlockSize;
            this.lblFinishLocation.Top = this.game.FinishPoint.Y * BlockSize;
            switch (this.game.Blocks[0].BType)
            {
                case BlockType.One:
                    this.lblFinishLocation.Width = BlockSize;
                    this.lblFinishLocation.Height = BlockSize;
                    break;

                case BlockType.TwoH:
                    this.lblFinishLocation.Width = BlockSize * 2;
                    this.lblFinishLocation.Height = BlockSize;
                    break;

                case BlockType.TwoV:
                    this.lblFinishLocation.Width = BlockSize;
                    this.lblFinishLocation.Height = BlockSize * 2;
                    break;

                case BlockType.Four:
                    this.lblFinishLocation.Width = BlockSize * 2;
                    this.lblFinishLocation.Height = BlockSize * 2;
                    break;
            }
            this.lblFinishLocation.BackColor = Color.Red;
            this.lblFinishLocation.Visible = false;
            this.pnlGame.Controls.Add(this.lblFinishLocation);
            this.labSTEP.Text = "0";
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlGame = new System.Windows.Forms.Panel();
            this.Img_cc = new System.Windows.Forms.PictureBox();
            this.Img_J1 = new System.Windows.Forms.PictureBox();
            this.Img_J2 = new System.Windows.Forms.PictureBox();
            this.Img_J3 = new System.Windows.Forms.PictureBox();
            this.Img_J4 = new System.Windows.Forms.PictureBox();
            this.Img_gy = new System.Windows.Forms.PictureBox();
            this.Img_b1 = new System.Windows.Forms.PictureBox();
            this.Img_b2 = new System.Windows.Forms.PictureBox();
            this.Img_b3 = new System.Windows.Forms.PictureBox();
            this.Img_b4 = new System.Windows.Forms.PictureBox();
            this.lab出口 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.功能ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.显示步骤计时ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.自动模式ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.装载toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存棋局toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.关闭游戏ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.listView1 = new System.Windows.Forms.ListView();
            this.btnBack = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBarTime = new System.Windows.Forms.TrackBar();
            this.labSTEP = new System.Windows.Forms.Label();
            this.lab31 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.comboBoxHRD = new System.Windows.Forms.ComboBox();
            this.关于AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlGame.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Img_cc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Img_J1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Img_J2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Img_J3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Img_J4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Img_gy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Img_b1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Img_b2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Img_b3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Img_b4)).BeginInit();
            this.menuStripMain.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTime)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlGame
            // 
            this.pnlGame.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.pnlGame.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlGame.Controls.Add(this.Img_cc);
            this.pnlGame.Controls.Add(this.Img_J1);
            this.pnlGame.Controls.Add(this.Img_J2);
            this.pnlGame.Controls.Add(this.Img_J3);
            this.pnlGame.Controls.Add(this.Img_J4);
            this.pnlGame.Controls.Add(this.Img_gy);
            this.pnlGame.Controls.Add(this.Img_b1);
            this.pnlGame.Controls.Add(this.Img_b2);
            this.pnlGame.Controls.Add(this.Img_b3);
            this.pnlGame.Controls.Add(this.Img_b4);
            this.pnlGame.Controls.Add(this.lab出口);
            this.pnlGame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGame.Location = new System.Drawing.Point(0, 25);
            this.pnlGame.Name = "pnlGame";
            this.pnlGame.Size = new System.Drawing.Size(485, 408);
            this.pnlGame.TabIndex = 0;
            // 
            // Img_cc
            // 
            this.Img_cc.BackColor = System.Drawing.SystemColors.Desktop;
            this.Img_cc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Img_cc.Location = new System.Drawing.Point(81, 1);
            this.Img_cc.Name = "Img_cc";
            this.Img_cc.Size = new System.Drawing.Size(160, 160);
            this.Img_cc.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Img_cc.TabIndex = 1;
            this.Img_cc.TabStop = false;
            this.Img_cc.Tag = "1";
            // 
            // Img_J1
            // 
            this.Img_J1.BackColor = System.Drawing.SystemColors.Desktop;
            this.Img_J1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Img_J1.Location = new System.Drawing.Point(1, 1);
            this.Img_J1.Name = "Img_J1";
            this.Img_J1.Size = new System.Drawing.Size(80, 160);
            this.Img_J1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Img_J1.TabIndex = 2;
            this.Img_J1.TabStop = false;
            this.Img_J1.Tag = "0";
            // 
            // Img_J2
            // 
            this.Img_J2.BackColor = System.Drawing.SystemColors.Desktop;
            this.Img_J2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Img_J2.Location = new System.Drawing.Point(240, 4);
            this.Img_J2.Name = "Img_J2";
            this.Img_J2.Size = new System.Drawing.Size(80, 160);
            this.Img_J2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Img_J2.TabIndex = 3;
            this.Img_J2.TabStop = false;
            this.Img_J2.Tag = "2";
            // 
            // Img_J3
            // 
            this.Img_J3.BackColor = System.Drawing.SystemColors.Desktop;
            this.Img_J3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Img_J3.Location = new System.Drawing.Point(1, 161);
            this.Img_J3.Name = "Img_J3";
            this.Img_J3.Size = new System.Drawing.Size(80, 160);
            this.Img_J3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Img_J3.TabIndex = 4;
            this.Img_J3.TabStop = false;
            this.Img_J3.Tag = "3";
            // 
            // Img_J4
            // 
            this.Img_J4.BackColor = System.Drawing.SystemColors.Desktop;
            this.Img_J4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Img_J4.Location = new System.Drawing.Point(241, 161);
            this.Img_J4.Name = "Img_J4";
            this.Img_J4.Size = new System.Drawing.Size(80, 160);
            this.Img_J4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Img_J4.TabIndex = 5;
            this.Img_J4.TabStop = false;
            this.Img_J4.Tag = "5";
            // 
            // Img_gy
            // 
            this.Img_gy.BackColor = System.Drawing.SystemColors.Desktop;
            this.Img_gy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Img_gy.Location = new System.Drawing.Point(81, 161);
            this.Img_gy.Name = "Img_gy";
            this.Img_gy.Size = new System.Drawing.Size(160, 80);
            this.Img_gy.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Img_gy.TabIndex = 6;
            this.Img_gy.TabStop = false;
            this.Img_gy.Tag = "4";
            // 
            // Img_b1
            // 
            this.Img_b1.BackColor = System.Drawing.SystemColors.Desktop;
            this.Img_b1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Img_b1.Location = new System.Drawing.Point(1, 321);
            this.Img_b1.Name = "Img_b1";
            this.Img_b1.Size = new System.Drawing.Size(80, 80);
            this.Img_b1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Img_b1.TabIndex = 7;
            this.Img_b1.TabStop = false;
            this.Img_b1.Tag = "6";
            // 
            // Img_b2
            // 
            this.Img_b2.BackColor = System.Drawing.SystemColors.Desktop;
            this.Img_b2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Img_b2.Location = new System.Drawing.Point(241, 321);
            this.Img_b2.Name = "Img_b2";
            this.Img_b2.Size = new System.Drawing.Size(80, 80);
            this.Img_b2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Img_b2.TabIndex = 8;
            this.Img_b2.TabStop = false;
            this.Img_b2.Tag = "9";
            // 
            // Img_b3
            // 
            this.Img_b3.BackColor = System.Drawing.SystemColors.Desktop;
            this.Img_b3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Img_b3.Location = new System.Drawing.Point(81, 241);
            this.Img_b3.Name = "Img_b3";
            this.Img_b3.Size = new System.Drawing.Size(80, 80);
            this.Img_b3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Img_b3.TabIndex = 9;
            this.Img_b3.TabStop = false;
            this.Img_b3.Tag = "7";
            // 
            // Img_b4
            // 
            this.Img_b4.BackColor = System.Drawing.SystemColors.Desktop;
            this.Img_b4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Img_b4.Location = new System.Drawing.Point(161, 241);
            this.Img_b4.Name = "Img_b4";
            this.Img_b4.Size = new System.Drawing.Size(80, 80);
            this.Img_b4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Img_b4.TabIndex = 10;
            this.Img_b4.TabStop = false;
            this.Img_b4.Tag = "8";
            // 
            // lab出口
            // 
            this.lab出口.Font = new System.Drawing.Font("楷体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab出口.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lab出口.Location = new System.Drawing.Point(119, 337);
            this.lab出口.Name = "lab出口";
            this.lab出口.Size = new System.Drawing.Size(76, 60);
            this.lab出口.TabIndex = 21;
            this.lab出口.Text = "曹  营        出  口";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 5000;
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.功能ToolStripMenuItem,
            this.关于ToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(485, 25);
            this.menuStripMain.TabIndex = 1;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // 功能ToolStripMenuItem
            // 
            this.功能ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.显示步骤计时ToolStripMenuItem,
            this.toolStripSeparator2,
            this.自动模式ToolStripMenuItem,
            this.toolStripSeparator3,
            this.装载toolStripMenuItem,
            this.保存ToolStripMenuItem,
            this.保存棋局toolStripMenuItem,
            this.toolStripSeparator4,
            this.关闭游戏ToolStripMenuItem});
            this.功能ToolStripMenuItem.Name = "功能ToolStripMenuItem";
            this.功能ToolStripMenuItem.Size = new System.Drawing.Size(58, 21);
            this.功能ToolStripMenuItem.Text = "功能(&F)";
            // 
            // 显示步骤计时ToolStripMenuItem
            // 
            this.显示步骤计时ToolStripMenuItem.Enabled = false;
            this.显示步骤计时ToolStripMenuItem.Name = "显示步骤计时ToolStripMenuItem";
            this.显示步骤计时ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.显示步骤计时ToolStripMenuItem.Text = "显示步骤计时";
            this.显示步骤计时ToolStripMenuItem.Click += new System.EventHandler(this.显示步骤计时ToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(157, 6);
            // 
            // 自动模式ToolStripMenuItem
            // 
            this.自动模式ToolStripMenuItem.Name = "自动模式ToolStripMenuItem";
            this.自动模式ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.自动模式ToolStripMenuItem.Text = "自动模式";
            this.自动模式ToolStripMenuItem.Click += new System.EventHandler(this.自动模式ToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(157, 6);
            // 
            // 装载toolStripMenuItem
            // 
            this.装载toolStripMenuItem.Name = "装载toolStripMenuItem";
            this.装载toolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.装载toolStripMenuItem.Text = "装载文件走步";
            this.装载toolStripMenuItem.Click += new System.EventHandler(this.装载toolStripMenuItem_Click);
            // 
            // 保存ToolStripMenuItem
            // 
            this.保存ToolStripMenuItem.Name = "保存ToolStripMenuItem";
            this.保存ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.保存ToolStripMenuItem.Text = "保存步骤到文件";
            this.保存ToolStripMenuItem.Click += new System.EventHandler(this.保存ToolStripMenuItem1_Click);
            // 
            // 保存棋局toolStripMenuItem
            // 
            this.保存棋局toolStripMenuItem.Name = "保存棋局toolStripMenuItem";
            this.保存棋局toolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.保存棋局toolStripMenuItem.Text = "保存目前棋局";
            this.保存棋局toolStripMenuItem.Click += new System.EventHandler(this.保存棋局toolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(157, 6);
            // 
            // 关闭游戏ToolStripMenuItem
            // 
            this.关闭游戏ToolStripMenuItem.Name = "关闭游戏ToolStripMenuItem";
            this.关闭游戏ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.关闭游戏ToolStripMenuItem.Text = "关闭游戏";
            this.关闭游戏ToolStripMenuItem.Click += new System.EventHandler(this.关闭游戏ToolStripMenuItem_Click);
            // 
            // 关于ToolStripMenuItem
            // 
            this.关于ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.关于AToolStripMenuItem});
            this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            this.关于ToolStripMenuItem.Size = new System.Drawing.Size(60, 21);
            this.关于ToolStripMenuItem.Text = "帮助(&B)";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.listView1);
            this.panel2.Controls.Add(this.btnBack);
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(327, 25);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(158, 408);
            this.panel2.TabIndex = 25;
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listView1.ForeColor = System.Drawing.Color.Blue;
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(0, 267);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(154, 137);
            this.listView1.TabIndex = 34;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            // 
            // btnBack
            // 
            this.btnBack.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBack.Location = new System.Drawing.Point(48, 218);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(64, 23);
            this.btnBack.TabIndex = 32;
            this.btnBack.Text = "撤消一步";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.trackBarTime);
            this.groupBox3.Controls.Add(this.labSTEP);
            this.groupBox3.Controls.Add(this.lab31);
            this.groupBox3.Location = new System.Drawing.Point(15, 112);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(131, 94);
            this.groupBox3.TabIndex = 31;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "行走信息";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(15, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 10);
            this.label1.TabIndex = 29;
            this.label1.Text = "自动走棋时间间隔:";
            // 
            // trackBarTime
            // 
            this.trackBarTime.Location = new System.Drawing.Point(16, 45);
            this.trackBarTime.Maximum = 600;
            this.trackBarTime.Minimum = 100;
            this.trackBarTime.Name = "trackBarTime";
            this.trackBarTime.Size = new System.Drawing.Size(88, 45);
            this.trackBarTime.SmallChange = 100;
            this.trackBarTime.TabIndex = 28;
            this.trackBarTime.Value = 200;
            // 
            // labSTEP
            // 
            this.labSTEP.AutoSize = true;
            this.labSTEP.Location = new System.Drawing.Point(80, 25);
            this.labSTEP.Name = "labSTEP";
            this.labSTEP.Size = new System.Drawing.Size(11, 12);
            this.labSTEP.TabIndex = 26;
            this.labSTEP.Text = "0";
            // 
            // lab31
            // 
            this.lab31.AutoSize = true;
            this.lab31.Location = new System.Drawing.Point(17, 25);
            this.lab31.Name = "lab31";
            this.lab31.Size = new System.Drawing.Size(65, 12);
            this.lab31.TabIndex = 24;
            this.lab31.Text = "当前步数：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Controls.Add(this.comboBoxHRD);
            this.groupBox1.Location = new System.Drawing.Point(15, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(131, 88);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "当前阵营";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(33, 55);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(64, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "重新开始";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // comboBoxHRD
            // 
            this.comboBoxHRD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.comboBoxHRD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxHRD.FormattingEnabled = true;
            this.comboBoxHRD.Location = new System.Drawing.Point(19, 29);
            this.comboBoxHRD.Name = "comboBoxHRD";
            this.comboBoxHRD.Size = new System.Drawing.Size(94, 20);
            this.comboBoxHRD.Sorted = true;
            this.comboBoxHRD.TabIndex = 0;
            this.comboBoxHRD.SelectedIndexChanged += new System.EventHandler(this.comboBoxHRD_SelectedIndexChanged);
            // 
            // 关于AToolStripMenuItem
            // 
            this.关于AToolStripMenuItem.Name = "关于AToolStripMenuItem";
            this.关于AToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.关于AToolStripMenuItem.Text = "关于(&A)";
            this.关于AToolStripMenuItem.Click += new System.EventHandler(this.关于ToolStripMenuItem_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.HighlightText;
            this.ClientSize = new System.Drawing.Size(485, 433);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pnlGame);
            this.Controls.Add(this.menuStripMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStripMain;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "华容道";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.pnlGame.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Img_cc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Img_J1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Img_J2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Img_J3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Img_J4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Img_gy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Img_b1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Img_b2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Img_b3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Img_b4)).EndInit();
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTime)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void UpdateButtonLocation(PictureBox btn)
        {
            Block block = (Block)btn.Tag;
            btn.Left = block.Location.X * BlockSize;
            btn.Top = block.Location.Y * BlockSize;
            btn.Refresh();
        }

        private void 保存ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Ini.saveFile(this.listView1);
        }

        private void 保存棋局toolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.game.SaveGame(@"C:\1.xml");
        }

        private void 关闭游戏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            base.Close();
            Application.Exit();
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FrmAbout().ShowDialog(this);
        }

        private void 显示步骤计时ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void 装载toolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveProcess mp = new MoveProcess(this.game);
            this.listView1.Items.Clear();
            Ini.DoStepFile();
            for (int i = 0; i < this.listView1.Items.Count; i++)
            {
                string s = this.listView1.Items[i].SubItems[0].Text + "\t" + this.listView1.Items[i].SubItems[1].Text + "\t" + this.listView1.Items[i].SubItems[2].Text + "\t" + this.listView1.Items[i].SubItems[3].Text;
                Thread.Sleep(this.trackBarTime.Value);
                string[] aa = s.Split(new char[] { '\t' });
                this.DostepArr(false, aa);
            }
        }

        private void 自动模式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            AutoSolve2();
        }

        private void AutoSolve1()
        {
            this.process = new Solver().SolveGame(this.game);
            this.listView1.Items.Clear();
            if (this.process == null)
            {
                MessageBox.Show("无解。");
            }
            else
            {
                var demoSteps = this.process.GetSteps();
                this.demoStepNo = 0;
                while (true)
                {
                    if (this.demoStepNo > (demoSteps.Count - 1))
                    {
                        break;
                    }

                    MoveStep step = demoSteps[this.demoStepNo];
                    this.game.MoveBlock(step.MoveBlockId, step.MoveDirection, step.Step);
                    this.DoStep(this.game.Blocks[step.MoveBlockId], step.MoveDirection, step.Step);
                    foreach (Control ctrl in this.pnlGame.Controls)
                    {
                        if (ctrl is PictureBox)
                        {
                            this.UpdateButtonLocation((PictureBox)ctrl);
                        }
                    }

                    Application.DoEvents();
                    Thread.Sleep(this.trackBarTime.Value);
                    this.demoStepNo++;
                }

                this.Cursor = Cursors.Default;
            }
        }
        private void AutoSolve2()
        {
            this.process = new Solver().SolveGame2(this.game);
            this.listView1.Items.Clear();
            if (this.process == null)
            {
                MessageBox.Show("无解。");
            }
            else
            {
                var demoSteps = this.process.GetSteps2();
                this.demoStepNo = 0;
                while (true)
                {
                    if (this.demoStepNo > (demoSteps.Count - 1))
                    {
                        break;
                    }

                    MoveItem step = demoSteps[this.demoStepNo];
                    foreach (var direction in step.MoveDirections)
                    {
                        this.game.MoveBlock(step.MoveBlockId, direction);
                    }
                    this.DoStep(this.game.Blocks[step.MoveBlockId], step);
                    foreach (Control ctrl in this.pnlGame.Controls)
                    {
                        if (ctrl is PictureBox)
                        {
                            this.UpdateButtonLocation((PictureBox)ctrl);
                        }
                    }

                    Application.DoEvents();
                    Thread.Sleep(this.trackBarTime.Value);
                    this.demoStepNo++;
                }

                this.Cursor = Cursors.Default;
            }
        }
    }
}

