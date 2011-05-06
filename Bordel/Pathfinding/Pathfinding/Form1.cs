using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Dark_Kingdom;

namespace Pathfinding
{
    public partial class Form1 : Form
    {
        Graphics gdc;

        Random RND = new Random(42);

        const int width = 800;
        const int height = 600;

        const int mapw = 80;
        const int maph = 60;

        const int tilew = 10;
        const int tileh = 10;

        int incursor = 256;

        Texture screen = new Texture(800, 600);
        Texture[] texs = new Texture[1040];

        PathFinder pathfinder = new PathFinder(mapw, maph);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);
            this.Size = new Size(1000, 700);

            this.pictureBox1.Location = new Point(0, 0);
            this.pictureBox1.Size = new Size(800, 600);

            this.pictureBox2.Size = new Size(32, 300);

            Bitmap b = new Bitmap(width, height);
            for (int x = 0; x < b.Width; x++)
                for (int y = 0; y < b.Height; y++)
                    b.SetPixel(x, y, Color.FromArgb(x % 256, y % 256, x * y % 256));
            texs[0] = Texture.FromBitmap(b);

            Bitmap legend = new Bitmap(width, height);
            for (int x = 0; x < legend.Width; x++)
                for (int y = 0; y < legend.Height; y++)
                    legend.SetPixel(x, y, getHColor(y - 22));

            pictureBox2.Image = Image.FromHbitmap(legend.GetHbitmap());

            pictureBox1.Image = Image.FromHbitmap(b.GetHbitmap());
            gdc = Graphics.FromImage(pictureBox1.Image);

            prepareTextures();

            genMap();

            //button2_Click(null, null);
        }

        private int[,] smooth(int[,] tmp)
        {
            int[,] ret = new int[mapw, maph];

            // smooth
            for (int x = 0; x < mapw; x++)
                for (int y = 0; y < maph; y++)
                {
                    int h = 0;
                    int s = 0;

                    h += tmp[x, y] * 2;
                    s += 2;

                    if (x + 1 < mapw)
                    {
                        h += tmp[x + 1, y];
                        s++;
                    }

                    if (x - 1 > 0)
                    {
                        h += tmp[x - 1, y];
                        s++;
                    }

                    if (y + 1 < maph)
                    {
                        h += tmp[x, y + 1];
                        s++;
                    }

                    if (y - 1 > 0)
                    {
                        h += tmp[x, y - 1];
                        s++;
                    }

                    if (s == 0)
                    {
                        ret[x, y] = tmp[x, y];
                    }
                    else
                    {
                        h = h / s;
                        ret[x, y] = h;
                    }

                }
            return ret;
        }

        private void genMap()
        {
            int[,] tmp = new int[mapw, maph];

            RND = new Random((int)numericUpDown1.Value);

            // gen
            for (int x = 0; x < mapw; x++)
                for (int y = 0; y < maph; y++)
                {
                    tmp[x, y] = RND.Next((int)numericUpDown4.Value);
                }

            // smoothing
            for (int i = 0; i < numericUpDown2.Value; i++)
                tmp = smooth(tmp);

            // raise water
            for (int x = 0; x < mapw; x++)
                for (int y = 0; y < maph; y++)
                {
                    tmp[x, y] = Math.Min(255, tmp[x, y] + (int)numericUpDown3.Value);
                }

            pathfinder.map = tmp;
        }

        private Color getHColor(int i)
        {
            byte rc = 0;
            byte gc = 0;
            byte bc = 0;

            if (i <= 0)
            {
                i = -i;
                rc = (byte)Math.Max(0,64-i*2);
                gc = (byte)Math.Max(0,64-i*2);
                bc = (byte)Math.Max(127,255 - i * 2);
                i = -i;
            }

            if (i > 0)
            {
                rc = (byte)Math.Min(i * 2, 96);
                gc = (byte)(255 - i * 2);
                bc = (byte)Math.Max((64 - i), 0);
            }

            if (i >= 128)
            {
                rc = (byte)((i - 64) * 4 / 3);
                gc = (byte)((i - 64) * 4 / 3);
                bc = (byte)((i - 64) * 4 / 3);
            }

            if (i == 255)
            {
                rc = 255;
                gc = 255;
                bc = 255;
            }

            if (i > 255)
            {
                rc = 0;
                gc = 0;
                bc = 0;
            }

            return Color.FromArgb(rc, gc, bc);
        }

        private void prepareTextures()
        {
            Bitmap b = new Bitmap(tilew, tileh);
            for (int x = 0; x < b.Width; x++)
                for (int y = 0; y < b.Height; y++)
                    b.SetPixel(x, y, Color.FromArgb(255, 127, 63));
            texs[1030] = Texture.FromBitmap(b);

            b = new Bitmap(tilew, tileh);
            for (int x = 0; x < b.Width; x++)
                for (int y = 0; y < b.Height; y++)
                    b.SetPixel(x, y, Color.FromArgb(255, 255, 0));
            texs[1031] = Texture.FromBitmap(b);

            b = new Bitmap(tilew, tileh);
            for (int x = 0; x < b.Width; x++)
                for (int y = 0; y < b.Height; y++)
                    b.SetPixel(x, y, Color.FromArgb(255, 0, 255));
            texs[1032] = Texture.FromBitmap(b);

            b = new Bitmap(tilew, tileh);
            for (int x = 0; x < b.Width; x++)
                for (int y = 0; y < b.Height; y++)
                    b.SetPixel(x, y, Color.FromArgb(0, 255, 255));
            texs[1033] = Texture.FromBitmap(b);

            b = new Bitmap(tilew, tileh);
            for (int x = 0; x < b.Width; x++)
                for (int y = 0; y < b.Height; y++)
                    b.SetPixel(x, y, Color.FromArgb(255, 0, 0));
            texs[1034] = Texture.FromBitmap(b);

            for (int i = 0; i < 1025; i++)
            {
                Text = "Generuji texturu " + i;

                b = new Bitmap(tilew, tileh);
                for (int x = 0; x < b.Width; x++)
                    for (int y = 0; y < b.Height; y++)
                        b.SetPixel(x, y, getHColor(i-512));
                texs[i] = Texture.FromBitmap(b);
            }
            Text = "Pathfinder";
        }

        private void drawmap()
        {
            for (int y = 0; y < maph; y++)
                for (int x = 0; x < mapw; x++)
                {
                    int index = pathfinder.map[x, y]+512 > 1023 ? 1024 : pathfinder.map[x, y]+512;
                    if (index<0) index=0;
                    texs[index].DrawTo(screen, x * tilew, y * tileh, tilew, tileh, Dark_Kingdom.DrawMode.NO_ALPHA);
                }

            // start, cil
            texs[1030].DrawTo(screen, pathfinder.start.X * tilew, pathfinder.start.Y * tileh, tilew, tileh, Dark_Kingdom.DrawMode.NO_ALPHA);
            texs[1031].DrawTo(screen, pathfinder.finish.X * tilew, pathfinder.finish.Y * tileh, tilew, tileh, Dark_Kingdom.DrawMode.NO_ALPHA);

            if (checkBox1.Checked)
            {
                foreach (Point p in pathfinder.openset)
                {
                    texs[1032].DrawTo(screen, p.X * tilew + tilew / 4, p.Y * tileh + tileh / 4, tilew / 2, tileh / 2, Dark_Kingdom.DrawMode.NO_ALPHA);
                }
            }

            if (checkBox2.Checked)
            {
                foreach (Point p in pathfinder.closedset)
                {
                    texs[1033].DrawTo(screen, p.X * tilew + (tilew / 4), p.Y * tileh + (tileh / 4), tilew / 2, tileh / 2, Dark_Kingdom.DrawMode.NO_ALPHA);
                }
            }

            if (checkBox3.Checked)
            {
                foreach (Point p in pathfinder.lastPath)
                {
                    texs[1034].DrawTo(screen, p.X * tilew + (tilew / 4), p.Y * tileh + (tileh / 4), tilew / 2, tileh / 2, Dark_Kingdom.DrawMode.NO_ALPHA);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Graphics g = gdc;

            drawmap();

            //texs[0].DrawTo(screen, 0, 0, texs[0].Width, texs[0].Height, Dark_Kingdom.DrawMode.NO_ALPHA);

            gdc.DrawImageUnscaled(screen.ToBitmap(), 0, 0, screen.Width, screen.Height);

            pictureBox1.Refresh();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!checkBox5.Checked)
            {
                if (e.Button == MouseButtons.Left) pathfinder.start.X = e.X / tilew;
                if (e.Button == MouseButtons.Left) pathfinder.start.Y = e.Y / tileh;

                if (e.Button == MouseButtons.Right) pathfinder.finish.X = e.X / tilew;
                if (e.Button == MouseButtons.Right) pathfinder.finish.Y = e.Y / tileh;

                pathfinder.FindPath();
            }
            else
            {
                pathfinder.map[e.X / tilew, e.Y / tileh] = incursor;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pathfinder.Clear();
            genMap();
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            incursor = e.Y - 22;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            pathfinder.diagonals = checkBox4.Checked;
            pathfinder.FindPath();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (checkBox5.Checked)
                if (e.Button == MouseButtons.Left)
                    pathfinder.map[e.X / tilew, e.Y / tileh] = incursor;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            pathfinder.useHeuristic = checkBox6.Checked;
            pathfinder.FindPath();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pathfinder.Clear();
            for (int x = 0; x < mapw; x++)
                for (int y = 0; y < maph; y++)
                    pathfinder.map[x, y] = 1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            About a = new About();
            a.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pathfinder.map = smooth(pathfinder.map);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pathfinder.costEst = (byte)comboBox1.SelectedIndex;
            pathfinder.FindPath();
        }

    }
}
