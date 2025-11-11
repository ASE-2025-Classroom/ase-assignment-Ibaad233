using BOOSE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BOOSEprogram1
{
    internal class BOOSEcanvas : ICanvas

    {
        Bitmap CanvasBitmap;
        Graphics g;
        private int xPos, yPos;
        Pen Pen;
        
        public BOOSEcanvas(int xsize, int ysize)
        {
        CanvasBitmap = new Bitmap(xsize, ysize);
        g = Graphics.FromImage(CanvasBitmap);
            Xpos = 100;
            Ypos = 100;
            Pen = new Pen(Color.Blue);
        }
        public int Xpos { get => xPos; set => xPos = value; }
        public int Ypos { get => yPos; set => xPos = value; }
        public object PenColour { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Circle(int radius, bool filled)
        {
            g.DrawEllipse(Pen, Xpos, Ypos, radius * 2, radius * 2);
        }

        public void Clear()
        {
           //g.Clear(Color.White);
        }

        public void DrawTo(int x, int y)
        {
            throw new NotImplementedException();
        }

        public object getBitmap()
        {
            return CanvasBitmap;
        }

        public void MoveTo(int x, int y)
        {
            Xpos = x;
            Ypos = y;
        }

        public void Rect(int width, int height, bool filled)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            Xpos = 0;
            Ypos = 0;

        }

        public void Set(int width, int height)
        {
            throw new NotImplementedException();
        }

        public void SetColour(int red, int green, int blue)
        {
            throw new NotImplementedException();
        }

        public void Tri(int width, int height)
        {
            throw new NotImplementedException();
        }

        public void WriteText(string text)
        {
            throw new NotImplementedException();
        }
    }
}
