namespace HRD
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class FrmAbout : Form
    {
        private Button btn_about;
        private IContainer components = null;
        private Label label1;
        private Label label2;
        private Label label4;
        private Label label5;
        private Label label7;

        public FrmAbout()
        {
            this.InitializeComponent();

            this.label5.Text = "　华容道是一款变化多端、百玩不厌的古老的中国智力游戏。华容道游戏取自于著名的三国故事，曹操在赤壁大战中被刘备和孙权打败，被迫退逃到华容道，又遇上诸葛亮的伏兵，关羽为了报答曹操对他的恩情，明逼实让，终于帮助曹操逃出了华容道。游戏就是根据这一故事情节，通过移动各个棋子，帮助曹操从初始位置移到棋盘最下方中部，从出口逃走。“华容道”有一个带二十个小方格的棋盘，代表华容道。棋盘下方有一个两方格边长的出口，是供曹操逃走的。棋盘上共摆有十个大小不一样的棋子，它们分别代表曹操、关羽、张飞、赵云、马超、黄忠，还有四个兵。“华容道”有几十种布阵方法，如“横刀立马”、“水泄不通”、“巧过五关”等等。棋盘上仅有两个小方格空着，玩法就是利用这两个空格移动棋子，不允许跨越棋子，用最少的步数把曹操移到华容道出口。";
        }

        private void btn_about_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void F_About_Load(object sender, EventArgs e)
        {
        }

        private void InitializeComponent()
        {
            this.btn_about = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_about
            // 
            this.btn_about.Location = new System.Drawing.Point(219, 268);
            this.btn_about.Name = "btn_about";
            this.btn_about.Size = new System.Drawing.Size(76, 31);
            this.btn_about.TabIndex = 0;
            this.btn_about.Text = "确定";
            this.btn_about.Click += new System.EventHandler(this.btn_about_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.label1.Location = new System.Drawing.Point(12, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 28);
            this.label1.TabIndex = 6;
            this.label1.Text = "华容道";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Italic);
            this.label2.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label2.Location = new System.Drawing.Point(146, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Beta 1";
            // 
            // label5
            // 
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label5.Location = new System.Drawing.Point(12, 39);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(283, 201);
            this.label5.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label7.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(15, 277);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(161, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "E-Mail/QQ: 23955618@QQ.COM\r\n";
            // 
            // label4
            // 
            this.label4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(50, 240);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(230, 25);
            this.label4.TabIndex = 8;
            this.label4.Text = "游戏胜利条件:曹操棋子走到最下面中间！";
            // 
            // FrmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(307, 313);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_about);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmAbout";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "关于";
            this.Load += new System.EventHandler(this.F_About_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}

