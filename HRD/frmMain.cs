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
        public static frmMain _frmMain = null;
        private const int BlockSize = 80;
        private Button btnBack;
        private Button btnStart;
        private ComboBox comboBoxHRD;
        private IContainer components = null;
        private int demoStepNo;
        private Game game;
        public GroupBox groupBox1;
        public GroupBox grpAuto;
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
        private ToolStripMenuItem 功能ToolStripMenuItem;
        private ToolStripMenuItem 关闭游戏ToolStripMenuItem;
        private ToolStripMenuItem 关于ToolStripMenuItem;
        private ToolStripMenuItem 显示步骤计时ToolStripMenuItem;
        private ToolStripMenuItem 关于AToolStripMenuItem;
        private Button btnPrev;
        private Button btnNext;
        private Button btnContinue;
        private Button btnPause;
        private ToolStripMenuItem 自动模式ToolStripMenuItem;

        public frmMain()
        {
            CheckForIllegalCrossThreadCalls = false;
            this.InitializeComponent();
            _frmMain = this;
        }

        private void btn_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.moveEnabled)
            {
                PictureBox btn = (PictureBox)sender;
                Block block = (Block)btn.Tag;
                List<Direction> drs = this.game.BlockDirection(block).Keys.ToList();
                if (drs.Count == 0)
                {
                    return;
                }
                if (drs.Count == 1)
                {
                    if (this.game.MoveBlock(block, drs[0]))
                    {
                        this.UpdateButtonLocation(btn);
                        this.DoStep(block, drs[0]);
                        this.showTempPic = false;
                    }
                }
                else
                {
                    bool bl = false;
                    Direction dr = Direction.Up;
                    if (((e.Y < (btn.Height / 3)) && (e.X > (btn.Width / 3))) && (e.X < ((btn.Width / 3) * 2)))
                    {
                        bl = this.game.MoveBlock(block, Direction.Up);
                    }
                    else if (((e.Y > ((btn.Height / 3) * 2)) && (e.X > (btn.Width / 3))) && (e.X < ((btn.Width / 3) * 2)))
                    {
                        bl = this.game.MoveBlock(block, Direction.Down);
                        dr = Direction.Down;
                    }
                    else if (((e.X < (btn.Width / 3)) && (e.Y > (btn.Height / 3))) && (e.Y < ((btn.Height / 3) * 2)))
                    {
                        bl = this.game.MoveBlock(block, Direction.Left);
                        dr = Direction.Left;
                    }
                    else if (((e.X > ((btn.Width / 3) * 2)) && (e.Y > (btn.Height / 3))) && (e.Y < ((btn.Height / 3) * 2)))
                    {
                        bl = this.game.MoveBlock(block, Direction.Right);
                        dr = Direction.Right;
                    }
                    else if (!this.showTempPic)
                    {
                        this.ShowBlockDirections(btn, drs);
                    }

                    if (bl)
                    {
                        btn.Image = Resource.Imgs[block.PictureId];
                        this.UpdateButtonLocation(btn);
                        this.showTempPic = false;
                        this.DoStep(block, dr);
                    }
                }
                if (this.game.GameWin())
                {
                    MessageBox.Show("游戏胜利！");
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

        private void comboBoxHRD_SelectedIndexChanged(object sender, EventArgs e)
        {
            var projectName = Assembly.GetExecutingAssembly().GetName().Name;
            var map = Ini.GetMap(projectName + ".maps." + this.comboBoxHRD.Text + ".map");
            this.game = Game.From(map);
            this.InitGame();
        }

        private void ShowBlockDirections(PictureBox btn, List<Direction> drs)
        {
            Block block = (Block)btn.Tag;
            if (drs.Count > 1)
            {
                foreach (Direction dr in drs)
                {
                    switch (dr)
                    {
                        case Direction.Up:
                            block.ShowDirection(dr, btn, Resource.Up);
                            break;

                        case Direction.Down:
                            block.ShowDirection(dr, btn, Resource.Down);
                            break;

                        case Direction.Left:
                            block.ShowDirection(dr, btn, Resource.Left);
                            break;

                        case Direction.Right:
                            block.ShowDirection(dr, btn, Resource.Right);
                            break;
                    }
                }
                this.showTempPic = true;
            }
            else
            {
                btn.Image = Resource.Imgs[block.PictureId];
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void DoLv()
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
            item.SubItems.Add("" + block.ID);
            item.SubItems.Add("" + dr.String() + step);
            this.listView1.Items.Add(item);
            this.listView1.Items[this.listView1.Items.Count - 1].Selected = true;
            this.listView1.HideSelection = false;
            this.labSTEP.Text = this.listView1.Items.Count.ToString();
        }

        private void DoStep(Block block, MoveItem dr, bool isBack = false)
        {
            if (isBack)
            {
                this.listView1.Items.RemoveAt(this.listView1.Items.Count - 1);
            }
            else
            {
                ListViewItem item = new ListViewItem("步" + (this.listView1.Items.Count + 1));
                item.SubItems.Add("" + block.Text);
                item.SubItems.Add("" + block.ID);
                item.SubItems.Add("" + dr.MoveDirections.String());
                this.listView1.Items.Add(item);
            }

            if (this.listView1.Items.Count > 0)
            {
                this.listView1.Items[this.listView1.Items.Count - 1].Selected = true;
            }

            this.labSTEP.Text = this.listView1.Items.Count.ToString();
        }

        private void DoStep(MoveItem step, bool isBack = false)
        {
            var block = this.game.Blocks[step.MoveBlockId];
            this.DoStep(block, step, isBack);
            if (isBack)
            {
                for (int i = step.MoveDirections.Count - 1; i >= 0; i--)
                {
                    this.game.MoveBlock(step.MoveBlockId, step.MoveDirections[i], true);
                }
            }
            else
            {
                foreach (var direction in step.MoveDirections)
                {
                    this.game.MoveBlock(step.MoveBlockId, direction);
                }
            }
            this.UpdateButtonLocation(dic[step.MoveBlockId]);
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
                var drs = ParseDirection(cs_4_0001);

                PictureBox btn = null;
                foreach (Control p in this.pnlGame.Controls)
                {
                    if (p.Text == a[1])
                    {
                        btn = (PictureBox)p;
                    }
                }

                var id = Convert.ToInt32(a[2]);
                foreach (var direction in drs)
                {
                    this.game.MoveBlock(id, direction, DoBack);
                }

                if (btn != null)
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
        private static List<Direction> ParseDirection(string dirs)
        {
            var drs = new List<Direction>();
            Direction? last = null;
            foreach (var dir in dirs)
            {
                if (ParseDirection(dir, out Direction dr))
                {
                    drs.Add(dr);
                    last = dr;
                }
                else if (last != null && int.TryParse(dir.ToString(), out int i))
                {
                    for (int j = 0; j < i; j++)
                    {
                        drs.Add(last.Value);
                    }
                }
                else
                {
                    break;
                }
            }

            return drs;
        }

        private static bool ParseDirection(char dir, out Direction dr)
        {
            if (dir == '下')
            {
                dr = Direction.Down;
                return true;
            }
            else if (dir == '左')
            {
                dr = Direction.Left;
                return true;
            }
            else if (dir == '右')
            {
                dr = Direction.Right;
                return true;
            }
            else if (dir == '上')
            {
                dr = Direction.Up;
                return true;
            }

            dr = Direction.Up;
            return false;
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (thAuto != null && thAuto.IsAlive)
            {
                thAuto.Abort();
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            base.Height = 460;

            string[] s = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            string[] st = null;
            foreach (string str in s)
            {
                if (str.EndsWith("map"))
                {
                    st = str.Split('.');
                    this.comboBoxHRD.Items.Add(st[2]);
                }
            }

            comboBoxHRD.SelectedIndex = 0;
            this.comboBoxHRD_SelectedIndexChanged(sender, e);
            this.DoLv();
        }

        private IDictionary<int, PictureBox> dic = new Dictionary<int, PictureBox>();

        private void InitGame()
        {
            this.listView1.Items.Clear();
            this.pnlGame.Controls.Clear();
            this.pnlGame.Width = 320;
            this.pnlGame.Height = 400;
            //int index = 0;
            this.moves = null;
            this.demoStepNo = 0;
            if (thAuto != null && thAuto.IsAlive)
            {
                thAuto.Abort();
            }
            this.grpAuto.Enabled = false;
            this.btnBack.Enabled = true;
            dic.Clear();
            foreach (Block block in this.game.Blocks)
            {
                PictureBox btn = new PictureBox();
                switch (block.BType)
                {
                    case BlockType.One:
                        btn.Width = BlockSize;
                        btn.Height = BlockSize;
                        break;

                    case BlockType.Horizontal:
                        btn.Width = BlockSize * 2;
                        btn.Height = BlockSize;
                        break;

                    case BlockType.Vertical:
                        btn.Width = BlockSize;
                        btn.Height = BlockSize * 2;
                        break;

                    case BlockType.Target:
                        btn.Width = BlockSize * 2;
                        btn.Height = BlockSize * 2;
                        break;
                }
                //block.ID = index;
                //index++;
                btn.Text = block.Text;
                btn.BorderStyle = BorderStyle.Fixed3D;
                btn.Tag = block;
                btn.BackColor = SystemColors.Control;
                btn.Image = Resource.Imgs[block.PictureId];
                btn.SizeMode = PictureBoxSizeMode.StretchImage;
                this.UpdateButtonLocation(btn);
                btn.MouseClick += new MouseEventHandler(this.btn_MouseClick);
                dic.Add(block.ID, btn);
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

                case BlockType.Horizontal:
                    this.lblFinishLocation.Width = BlockSize * 2;
                    this.lblFinishLocation.Height = BlockSize;
                    break;

                case BlockType.Vertical:
                    this.lblFinishLocation.Width = BlockSize;
                    this.lblFinishLocation.Height = BlockSize * 2;
                    break;

                case BlockType.Target:
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
            this.保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.关闭游戏ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.listView1 = new System.Windows.Forms.ListView();
            this.btnBack = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.comboBoxHRD = new System.Windows.Forms.ComboBox();
            this.grpAuto = new System.Windows.Forms.GroupBox();
            this.btnContinue = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPause = new System.Windows.Forms.Button();
            this.trackBarTime = new System.Windows.Forms.TrackBar();
            this.labSTEP = new System.Windows.Forms.Label();
            this.lab31 = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
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
            this.groupBox1.SuspendLayout();
            this.grpAuto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTime)).BeginInit();
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
            this.pnlGame.Location = new System.Drawing.Point(0, 28);
            this.pnlGame.Margin = new System.Windows.Forms.Padding(4);
            this.pnlGame.Name = "pnlGame";
            this.pnlGame.Size = new System.Drawing.Size(606, 513);
            this.pnlGame.TabIndex = 0;
            // 
            // Img_cc
            // 
            this.Img_cc.BackColor = System.Drawing.SystemColors.Desktop;
            this.Img_cc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Img_cc.Location = new System.Drawing.Point(101, 1);
            this.Img_cc.Margin = new System.Windows.Forms.Padding(4);
            this.Img_cc.Name = "Img_cc";
            this.Img_cc.Size = new System.Drawing.Size(200, 200);
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
            this.Img_J1.Margin = new System.Windows.Forms.Padding(4);
            this.Img_J1.Name = "Img_J1";
            this.Img_J1.Size = new System.Drawing.Size(100, 200);
            this.Img_J1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Img_J1.TabIndex = 2;
            this.Img_J1.TabStop = false;
            this.Img_J1.Tag = "0";
            // 
            // Img_J2
            // 
            this.Img_J2.BackColor = System.Drawing.SystemColors.Desktop;
            this.Img_J2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Img_J2.Location = new System.Drawing.Point(300, 5);
            this.Img_J2.Margin = new System.Windows.Forms.Padding(4);
            this.Img_J2.Name = "Img_J2";
            this.Img_J2.Size = new System.Drawing.Size(100, 200);
            this.Img_J2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Img_J2.TabIndex = 3;
            this.Img_J2.TabStop = false;
            this.Img_J2.Tag = "2";
            // 
            // Img_J3
            // 
            this.Img_J3.BackColor = System.Drawing.SystemColors.Desktop;
            this.Img_J3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Img_J3.Location = new System.Drawing.Point(1, 201);
            this.Img_J3.Margin = new System.Windows.Forms.Padding(4);
            this.Img_J3.Name = "Img_J3";
            this.Img_J3.Size = new System.Drawing.Size(100, 200);
            this.Img_J3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Img_J3.TabIndex = 4;
            this.Img_J3.TabStop = false;
            this.Img_J3.Tag = "3";
            // 
            // Img_J4
            // 
            this.Img_J4.BackColor = System.Drawing.SystemColors.Desktop;
            this.Img_J4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Img_J4.Location = new System.Drawing.Point(301, 201);
            this.Img_J4.Margin = new System.Windows.Forms.Padding(4);
            this.Img_J4.Name = "Img_J4";
            this.Img_J4.Size = new System.Drawing.Size(100, 200);
            this.Img_J4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Img_J4.TabIndex = 5;
            this.Img_J4.TabStop = false;
            this.Img_J4.Tag = "5";
            // 
            // Img_gy
            // 
            this.Img_gy.BackColor = System.Drawing.SystemColors.Desktop;
            this.Img_gy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Img_gy.Location = new System.Drawing.Point(101, 201);
            this.Img_gy.Margin = new System.Windows.Forms.Padding(4);
            this.Img_gy.Name = "Img_gy";
            this.Img_gy.Size = new System.Drawing.Size(200, 100);
            this.Img_gy.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Img_gy.TabIndex = 6;
            this.Img_gy.TabStop = false;
            this.Img_gy.Tag = "4";
            // 
            // Img_b1
            // 
            this.Img_b1.BackColor = System.Drawing.SystemColors.Desktop;
            this.Img_b1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Img_b1.Location = new System.Drawing.Point(1, 401);
            this.Img_b1.Margin = new System.Windows.Forms.Padding(4);
            this.Img_b1.Name = "Img_b1";
            this.Img_b1.Size = new System.Drawing.Size(100, 100);
            this.Img_b1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Img_b1.TabIndex = 7;
            this.Img_b1.TabStop = false;
            this.Img_b1.Tag = "6";
            // 
            // Img_b2
            // 
            this.Img_b2.BackColor = System.Drawing.SystemColors.Desktop;
            this.Img_b2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Img_b2.Location = new System.Drawing.Point(301, 401);
            this.Img_b2.Margin = new System.Windows.Forms.Padding(4);
            this.Img_b2.Name = "Img_b2";
            this.Img_b2.Size = new System.Drawing.Size(100, 100);
            this.Img_b2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Img_b2.TabIndex = 8;
            this.Img_b2.TabStop = false;
            this.Img_b2.Tag = "9";
            // 
            // Img_b3
            // 
            this.Img_b3.BackColor = System.Drawing.SystemColors.Desktop;
            this.Img_b3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Img_b3.Location = new System.Drawing.Point(101, 301);
            this.Img_b3.Margin = new System.Windows.Forms.Padding(4);
            this.Img_b3.Name = "Img_b3";
            this.Img_b3.Size = new System.Drawing.Size(100, 100);
            this.Img_b3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Img_b3.TabIndex = 9;
            this.Img_b3.TabStop = false;
            this.Img_b3.Tag = "7";
            // 
            // Img_b4
            // 
            this.Img_b4.BackColor = System.Drawing.SystemColors.Desktop;
            this.Img_b4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Img_b4.Location = new System.Drawing.Point(201, 301);
            this.Img_b4.Margin = new System.Windows.Forms.Padding(4);
            this.Img_b4.Name = "Img_b4";
            this.Img_b4.Size = new System.Drawing.Size(100, 100);
            this.Img_b4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Img_b4.TabIndex = 10;
            this.Img_b4.TabStop = false;
            this.Img_b4.Tag = "8";
            // 
            // lab出口
            // 
            this.lab出口.Font = new System.Drawing.Font("楷体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab出口.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lab出口.Location = new System.Drawing.Point(149, 421);
            this.lab出口.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lab出口.Name = "lab出口";
            this.lab出口.Size = new System.Drawing.Size(95, 75);
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
            this.menuStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.功能ToolStripMenuItem,
            this.关于ToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStripMain.Size = new System.Drawing.Size(606, 28);
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
            this.保存ToolStripMenuItem,
            this.toolStripSeparator4,
            this.关闭游戏ToolStripMenuItem});
            this.功能ToolStripMenuItem.Name = "功能ToolStripMenuItem";
            this.功能ToolStripMenuItem.Size = new System.Drawing.Size(69, 24);
            this.功能ToolStripMenuItem.Text = "功能(&F)";
            // 
            // 显示步骤计时ToolStripMenuItem
            // 
            this.显示步骤计时ToolStripMenuItem.Enabled = false;
            this.显示步骤计时ToolStripMenuItem.Name = "显示步骤计时ToolStripMenuItem";
            this.显示步骤计时ToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.显示步骤计时ToolStripMenuItem.Text = "显示步骤计时";
            this.显示步骤计时ToolStripMenuItem.Click += new System.EventHandler(this.显示步骤计时ToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(213, 6);
            // 
            // 自动模式ToolStripMenuItem
            // 
            this.自动模式ToolStripMenuItem.Name = "自动模式ToolStripMenuItem";
            this.自动模式ToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.自动模式ToolStripMenuItem.Text = "自动模式";
            this.自动模式ToolStripMenuItem.Click += new System.EventHandler(this.自动模式ToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(213, 6);
            // 
            // 保存ToolStripMenuItem
            // 
            this.保存ToolStripMenuItem.Name = "保存ToolStripMenuItem";
            this.保存ToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.保存ToolStripMenuItem.Text = "保存步骤到文件";
            this.保存ToolStripMenuItem.Click += new System.EventHandler(this.保存ToolStripMenuItem1_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(213, 6);
            // 
            // 关闭游戏ToolStripMenuItem
            // 
            this.关闭游戏ToolStripMenuItem.Name = "关闭游戏ToolStripMenuItem";
            this.关闭游戏ToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.关闭游戏ToolStripMenuItem.Text = "关闭游戏";
            this.关闭游戏ToolStripMenuItem.Click += new System.EventHandler(this.关闭游戏ToolStripMenuItem_Click);
            // 
            // 关于ToolStripMenuItem
            // 
            this.关于ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.关于AToolStripMenuItem});
            this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            this.关于ToolStripMenuItem.Size = new System.Drawing.Size(70, 24);
            this.关于ToolStripMenuItem.Text = "帮助(&B)";
            // 
            // 关于AToolStripMenuItem
            // 
            this.关于AToolStripMenuItem.Name = "关于AToolStripMenuItem";
            this.关于AToolStripMenuItem.Size = new System.Drawing.Size(135, 26);
            this.关于AToolStripMenuItem.Text = "关于(&A)";
            this.关于AToolStripMenuItem.Click += new System.EventHandler(this.关于ToolStripMenuItem_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.listView1);
            this.panel2.Controls.Add(this.btnBack);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.grpAuto);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(410, 28);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(196, 513);
            this.panel2.TabIndex = 25;
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listView1.ForeColor = System.Drawing.Color.Blue;
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(0, 338);
            this.listView1.Margin = new System.Windows.Forms.Padding(4);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(192, 171);
            this.listView1.TabIndex = 34;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            // 
            // btnBack
            // 
            this.btnBack.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBack.Location = new System.Drawing.Point(54, 301);
            this.btnBack.Margin = new System.Windows.Forms.Padding(4);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(80, 29);
            this.btnBack.TabIndex = 32;
            this.btnBack.Text = "撤消一步";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Controls.Add(this.comboBoxHRD);
            this.groupBox1.Location = new System.Drawing.Point(19, 16);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(164, 94);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "当前阵营";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(41, 54);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(80, 29);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "重新开始";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += comboBoxHRD_SelectedIndexChanged;
            // 
            // comboBoxHRD
            // 
            this.comboBoxHRD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.comboBoxHRD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxHRD.FormattingEnabled = true;
            this.comboBoxHRD.Location = new System.Drawing.Point(24, 21);
            this.comboBoxHRD.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxHRD.Name = "comboBoxHRD";
            this.comboBoxHRD.Size = new System.Drawing.Size(116, 23);
            this.comboBoxHRD.Sorted = true;
            this.comboBoxHRD.TabIndex = 0;
            this.comboBoxHRD.SelectedIndexChanged += new System.EventHandler(this.comboBoxHRD_SelectedIndexChanged);
            // 
            // grpAuto
            // 
            this.grpAuto.Controls.Add(this.btnContinue);
            this.grpAuto.Controls.Add(this.label1);
            this.grpAuto.Controls.Add(this.btnPause);
            this.grpAuto.Controls.Add(this.trackBarTime);
            this.grpAuto.Controls.Add(this.labSTEP);
            this.grpAuto.Controls.Add(this.lab31);
            this.grpAuto.Controls.Add(this.btnNext);
            this.grpAuto.Controls.Add(this.btnPrev);
            this.grpAuto.Location = new System.Drawing.Point(19, 118);
            this.grpAuto.Margin = new System.Windows.Forms.Padding(4);
            this.grpAuto.Name = "grpAuto";
            this.grpAuto.Padding = new System.Windows.Forms.Padding(4);
            this.grpAuto.Size = new System.Drawing.Size(164, 170);
            this.grpAuto.TabIndex = 31;
            this.grpAuto.TabStop = false;
            this.grpAuto.Text = "自动求解";
            this.grpAuto.Enabled = false;
            // 
            // btnContinue
            // 
            this.btnContinue.Location = new System.Drawing.Point(84, 106);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(80, 29);
            this.btnContinue.TabIndex = 38;
            this.btnContinue.Text = "继续&G";
            this.btnContinue.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(17, 51);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "自动走棋时间间隔:";
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(0, 106);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(80, 29);
            this.btnPause.TabIndex = 37;
            this.btnPause.Text = "暂停&T";
            this.btnPause.UseVisualStyleBackColor = true;
            // 
            // trackBarTime
            // 
            this.trackBarTime.Location = new System.Drawing.Point(20, 67);
            this.trackBarTime.Margin = new System.Windows.Forms.Padding(4);
            this.trackBarTime.Maximum = 600;
            this.trackBarTime.Minimum = 100;
            this.trackBarTime.Name = "trackBarTime";
            this.trackBarTime.Size = new System.Drawing.Size(110, 56);
            this.trackBarTime.SmallChange = 100;
            this.trackBarTime.TabIndex = 28;
            this.trackBarTime.TickFrequency = 100;
            this.trackBarTime.Value = 200;
            // 
            // labSTEP
            // 
            this.labSTEP.AutoSize = true;
            this.labSTEP.Location = new System.Drawing.Point(100, 25);
            this.labSTEP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labSTEP.Name = "labSTEP";
            this.labSTEP.Size = new System.Drawing.Size(15, 15);
            this.labSTEP.TabIndex = 26;
            this.labSTEP.Text = "0";
            // 
            // lab31
            // 
            this.lab31.AutoSize = true;
            this.lab31.Location = new System.Drawing.Point(21, 25);
            this.lab31.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lab31.Name = "lab31";
            this.lab31.Size = new System.Drawing.Size(82, 15);
            this.lab31.TabIndex = 24;
            this.lab31.Text = "当前步数：";
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(84, 139);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(80, 29);
            this.btnNext.TabIndex = 36;
            this.btnNext.Text = "下一步&N";
            this.btnNext.UseVisualStyleBackColor = true;
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(0, 139);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(80, 29);
            this.btnPrev.TabIndex = 35;
            this.btnPrev.Text = "上一步&P";
            this.btnPrev.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.HighlightText;
            this.ClientSize = new System.Drawing.Size(606, 541);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pnlGame);
            this.Controls.Add(this.menuStripMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStripMain;
            this.Margin = new System.Windows.Forms.Padding(4);
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
            this.groupBox1.ResumeLayout(false);
            this.grpAuto.ResumeLayout(false);
            this.grpAuto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTime)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

            this.btnNext.Click += BtnNext_Click;
            this.btnPrev.Click += BtnPrev_Click;
            this.btnPause.Click += BtnPause_Click;
            this.btnContinue.Click += BtnContinue_Click;
        }

        private void BtnContinue_Click(object sender, EventArgs e)
        {
            this.isAutoRunning = true;
            this.btnPrev.Enabled = false;
            this.btnNext.Enabled = false;
            this.btnContinue.Enabled = false;
            this.btnPause.Enabled = true;
            this.resetEvent.Set();
        }

        private void BtnPause_Click(object sender, EventArgs e)
        {
            this.isAutoRunning = false;
            this.btnPrev.Enabled = this.demoStepNo > 0;
            this.btnNext.Enabled = this.demoStepNo < this.moves.Count - 1;
            this.btnContinue.Enabled = true;
            this.btnPause.Enabled = false;
        }

        private void BtnPrev_Click(object sender, EventArgs e)
        {
            this.demoStepNo--;
            MoveItem step = this.moves[this.demoStepNo];
            this.DoStep(step, true);
            Application.DoEvents();
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            MoveItem step = this.moves[this.demoStepNo];
            this.DoStep(step);
            this.demoStepNo++;
            Application.DoEvents();
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
            Ini.SaveFile(this.listView1);
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

        private void 自动模式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            AutoSolve();
        }

        private List<MoveItem> moves;
        private AutoResetEvent resetEvent = new AutoResetEvent(true);
        private bool isAutoRunning = true;
        private Thread thAuto;
        //public static Bitmap _cur_pic;

        private void AutoSolve()
        {
            this.process = new Solver().SolveGame(this.game);
            this.listView1.Items.Clear();
            if (this.process == null)
            {
                MessageBox.Show("无解。");
            }
            else
            {
                this.moves = this.process.GetSteps();
                this.demoStepNo = 0;
                this.btnPrev.Enabled = this.btnNext.Enabled = this.btnContinue.Enabled = false;
                this.grpAuto.Enabled = true;
                this.isAutoRunning = true;
                this.btnBack.Enabled = false;
                Application.DoEvents();
                RunAuto();

                this.Cursor = Cursors.Default;
            }
        }

        private void RunAuto()
        {
            if (thAuto != null && thAuto.IsAlive)
            {
                thAuto.Abort();
            }
            thAuto = new Thread(() =>
            {
                this.Invoke(new Action(() =>
                {
                    while (true)
                    {
                        if (!isAutoRunning)
                        {
                            this.btnPrev.Enabled = this.demoStepNo > 0;
                            this.btnNext.Enabled = this.demoStepNo < this.moves.Count - 1;
                        }

                        if (this.demoStepNo < this.moves.Count)
                        {
                            if (isAutoRunning)
                            {
                                MoveItem step = this.moves[this.demoStepNo];
                                this.DoStep(step);
                                //foreach (var direction in step.MoveDirections)
                                //{
                                //    this.game.MoveBlock(step.MoveBlockId, direction);
                                //    this.UpdateButtonLocation(dic[step.MoveBlockId]);
                                //}

                                //this.DoStep(this.game.Blocks[step.MoveBlockId], step);
                                //foreach (Control ctrl in this.pnlGame.Controls)
                                //{
                                //    if (ctrl is PictureBox)
                                //    {
                                //        this.UpdateButtonLocation((PictureBox)ctrl);
                                //    }
                                //}
                                this.demoStepNo++;

                                Application.DoEvents();
                                Thread.Sleep(this.trackBarTime.Value);
                            }
                        }
                        else if (isAutoRunning)
                        {
                            this.isAutoRunning = false;
                        }

                        if (!isAutoRunning)
                        {
                            resetEvent.WaitOne();
                        }

                    }
                }));
            });
            thAuto.Start();
        }
    }
}

