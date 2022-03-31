using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
	public partial class Form1 : Form
	{
        Map map; bool freeze;
        System.Timers.Timer t;


        public static Point Size { get; private set; }
		public Form1()
		{
			InitializeComponent();
            KeyPreview = true;
            KeyDown += new KeyEventHandler(SetVector);
            SizeChanged += new EventHandler(Resize);
          
		}

        void Resize(object sender, EventArgs e)
        {
            pictureBox1.Height = Height - 96;
            pictureBox1.Width = Width - 48;
        }

        void Init(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Size = new Point
            (
                pictureBox1.Width / 8,
                pictureBox1.Height / 8
            );
            map = new Map();
            
            t = new System.Timers.Timer(100);
            t.Elapsed += Step;
            t.AutoReset = true;
            t.Enabled = true;
        }

        void Step(object sender, EventArgs e)
        {
            if (!map.Step()) { t.Enabled = false; }
            pictureBox1.Image = map.Image;
            //label1.Text = "SCORE:  " + map.score.ToString();
            GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
        }

        void SetVector(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W: map.SetVector(0, -1); break;
                case Keys.A: map.SetVector(-1, 0); break;
                case Keys.D: map.SetVector(1, 0); break;
                case Keys.S: map.SetVector(0, 1); break;
            }
        }
	}
}
