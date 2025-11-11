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
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            Xpos = 100;
            Ypos = 100;
            Pen = new Pen(Color.Blue);
        }
        public int Xpos { get => xPos; set => xPos = value; }
        public int Ypos { get => yPos; set => yPos = value; }
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
            g.DrawLine(Pen, Xpos, Ypos, x, y);
            Xpos = x;
            Ypos = y;
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
            var r = new Rectangle(Xpos, Ypos, width,height );
            if (filled)
            {
                using var b = new SolidBrush(Pen.Color);
                g.FillRectangle(b, r);
            }
            else
            {
                g.DrawRectangle(Pen, r);
            }
        }

        public void Reset()
        {
            Xpos = 0;
            Ypos = 0;

        }

        public void Set(int width, int height)
        {
            CanvasBitmap?.Dispose();
            g?.Dispose();

            CanvasBitmap = new Bitmap(width, height);
            g = Graphics.FromImage(CanvasBitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.Black);
        }

        public void SetColour(int red, int green, int blue)
        {
            Pen.Color = Color.FromArgb(red, green, blue);

        }

        public void Tri(int width, int height)
        {
           
            var p1 = new Point(Xpos, Ypos + width);          
            var p2 = new Point(Xpos +width / height, Ypos);      
            var p3 = new Point(Xpos + width, Ypos + height);  
            g.DrawPolygon(Pen, new[] { p1, p2, p3 });
        }

        public void WriteText(string text)
        {
            using var font = new Font(FontFamily.GenericSansSerif, 12f);
            using var b = new SolidBrush(Pen.Color);
            g.DrawString(text, font, b, new PointF(Xpos, Ypos));
        }
    }
}
