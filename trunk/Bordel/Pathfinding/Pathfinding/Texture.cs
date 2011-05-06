using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Dark_Kingdom
{
    public class Texture
    {
        int width;
        int height;

        byte[] data;

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public Texture(int width, int height)
            : this(width, height, new byte[width * height * 4])
        {

        }

        public Texture(int width, int height, byte[] data)
        {
            this.width = width;
            this.height = height;
            this.data = data; //(byte[])data.Clone();
            if (data.Length != width * height * 4) throw new Exception("Nesouhlasí data a rozměr textury!");
        }

        public Texture(Bitmap bmp)
            : this(bmp.Size.Width, bmp.Size.Height, FromBitmap(bmp).data)
        {
            // constructor
        }

        public void FillWith(Color c)
        {
            for (int i = 0; i < data.Length; i++ )
            {
                switch (i%4)
                {
                    case 0: data[i] = c.A; break;
                    case 1: data[i] = c.R; break;
                    case 2: data[i] = c.G; break;
                    case 3: data[i] = c.B; break;
                }
            }
        }

        public Bitmap ToBitmap()
        {
            Bitmap bmp = new Bitmap(width, height);
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(data, 0, bd.Scan0, width * height * 4);
            bmp.UnlockBits(bd);

            return bmp;
        }

        public static Texture FromBitmap(Bitmap bmp)
        {
            byte[] data = new byte[bmp.Size.Width * bmp.Size.Height * 4];
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Size.Width, bmp.Size.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(bd.Scan0, data, 0, bmp.Size.Width * bmp.Size.Height * 4);

            bmp.UnlockBits(bd);

            Texture t = new Texture(bmp.Size.Width, bmp.Size.Height, data);

            return t;
        }

        /// <summary>
        /// Draws actual texture to specified texture.
        /// </summary>
        /// <param name="txt">Texture into which will be drawn</param>
        /// <param name="xpos">X position</param>
        /// <param name="ypos">Z position</param>
        /// <param name="width">Drawn width</param>
        /// <param name="height">Drawn height</param>
        /// <param name="transparent">Use only alpha for pixels only as visible/hidden</param>
        /// <param name="alpha">Use real alpha</param>
        public void DrawTo(Texture txt, int xpos, int ypos, int width, int height, DrawMode mode)
        {
            int dl, dt; // dest leftop pixel
            int sl, st; // source leftop pixel
            int w, h; // size of really drawn area

            sl = xpos < 0 ? -xpos : 0; // pokud vykreslujem pred nulu, tak to pred nulou oriznem
            st = ypos < 0 ? -ypos : 0;

            dl = xpos < 0 ? 0 : xpos; // pokud vykreslujem pod nulu, tak to nevykreslime a zacnem az u nuly
            dt = ypos < 0 ? 0 : ypos;

            w = width; // sirka nakonec vykreslenyho vyrezu
            h = height;

            w = this.width < width ? this.width : width; // pokud to co kreslime je mensi
            h = this.height < height ? this.height : height;

            w = xpos < 0 ? w+xpos : w; // pokud to kreslime pred nulu
            h = ypos < 0 ? h+ypos : h;

            w = txt.width < dl + w ? txt.width - dl : w; // pokud se to tam cely nevejde
            h = txt.height < dt + h ? txt.height - dt : h; // pokud se to tam cely nevejde

            if (mode != DrawMode.NO_ALPHA)
            {
                //todo
                throw new Exception("not implemented yet");
                for (; ; )
                    for (; ; )
                    {
                        //txt.
                    }
            }
            else // mode == no_alpha
            {
                for (int y = 0; y < h; y++) // projdem vsechny radky teto textury
                {
                    Buffer.BlockCopy(
                        this.data, 
                        (sl + (st + y)*this.width) * 4, 
                        txt.data, 
                        (dl + (dt + y)*txt.width) * 4,
                        w*4);
                    //bacha, jsou to quady
                }
            }
        }
    }

    public enum DrawMode
    {
        NO_ALPHA,
        BINARY_ALPHA,
        FULL_ALPHA,
    }
}
