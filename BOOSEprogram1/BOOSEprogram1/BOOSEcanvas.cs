using BOOSE;
using System;
using System.Drawing;

namespace BOOSEprogram1
{
    /// <summary>
    /// Canvas class that draws shapes and text for the BOOSE program
    /// </summary>
    public class BOOSEcanvas : ICanvas
    {
        private Bitmap canvasBitmap;
        private Graphics graphics;
        private int xPos;
        private int yPos;
        private Pen pen;

        /// <summary>
        /// Creates a new canvas with the given width and height.
        /// </summary>
        /// <param name="width">Canvas width in pixels.</param>
        /// <param name="height">Canvas height in pixels.</param>
        public BOOSEcanvas(int width, int height)
        {
            canvasBitmap = new Bitmap(width, height);
            graphics = Graphics.FromImage(canvasBitmap);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Start settings
            graphics.Clear(Color.Gray);   // grey background
            Xpos = 100;
            Ypos = 100;
            pen = new Pen(Color.Blue);
        }

        /// <summary>
        /// Current X position of the pen.
        /// </summary>
        public int Xpos
        {
            get { return xPos; }
            set { xPos = value; }
        }

        /// <summary>
        /// Current Y position of the pen.
        /// </summary>
        public int Ypos
        {
            get { return yPos; }
            set { yPos = value; }
        }

        /// <summary>
        /// Current colour of the pen.
        /// </summary>
        public object PenColour
        {
            get { return pen.Color; }
            set
            {
                if (value is Color c)
                {
                    pen.Color = c;
                }
                else
                {
                    throw new ArgumentException("PenColour must be a System.Drawing.Color.");
                }
            }
        }

        /// <summary>
        /// Changes the size of the canvas.
        /// </summary>
        public void Set(int width, int height)
        {
            if (canvasBitmap != null) canvasBitmap.Dispose();
            if (graphics != null) graphics.Dispose();

            canvasBitmap = new Bitmap(width, height);
            graphics = Graphics.FromImage(canvasBitmap);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.Clear(Color.Gray);
        }

        /// <summary>
        /// Sets the pen colour using red, green and blue values.
        /// </summary>
        public void SetColour(int red, int green, int blue)
        {
            PenColour = Color.FromArgb(red, green, blue);
        }

        /// <summary>
        /// Moves the pen to a new position without drawing.
        /// </summary>
        public void MoveTo(int x, int y)
        {
            Xpos = x;
            Ypos = y;
        }

        /// <summary>
        /// Draws a line from the current pen position to a new position.
        /// </summary>
        public void DrawTo(int x, int y)
        {
            graphics.DrawLine(pen, Xpos, Ypos, x, y);
            Xpos = x;
            Ypos = y;
        }

        /// <summary>
        /// Clears the canvas to grey.
        /// </summary>
        public void Clear()
        {
            graphics.Clear(Color.Gray);
        }

        /// <summary>
        /// Resets the pen position and colour to their starting values.
        /// </summary>
        public void Reset()
        {
            Xpos = 0;
            Ypos = 0;
            pen.Color = Color.Blue;
        }

        /// <summary>
        /// Draws a circle with the current pen position as the centre.
        /// </summary>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="filled">True to fill the circle, false to only draw the outline.</param>
        public void Circle(int radius, bool filled)
        {
            // Xpos,Ypos are the centre of the circle
            Rectangle rect = new Rectangle(Xpos - radius, Ypos - radius, radius * 2, radius * 2);

            if (filled)
            {
                using (SolidBrush brush = new SolidBrush(pen.Color))
                {
                    graphics.FillEllipse(brush, rect);
                }
            }
            else
            {
                graphics.DrawEllipse(pen, rect);
            }
        }

        /// <summary>
        /// Draws a rectangle at the current pen position.
        /// </summary>
        public void Rect(int width, int height, bool filled)
        {
            graphics.DrawRectangle(pen, Xpos, Ypos, width, height);
        }

        /// <summary>
        /// Draws a simple triangle based on the current pen position.
        /// </summary>
        public void Tri(int width, int height)
        {
            Point p1 = new Point(Xpos, Ypos + height);          // left bottom
            Point p2 = new Point(Xpos + width / 2, Ypos);       // top
            Point p3 = new Point(Xpos + width, Ypos + height);  // right bottom

            graphics.DrawPolygon(pen, new[] { p1, p2, p3 });
        }

        /// <summary>
        /// Draws a piece of text at the current pen position.
        /// </summary>
        public void WriteText(string text)
        {
            using (Font font = new Font(FontFamily.GenericSansSerif, 12f))
            using (SolidBrush brush = new SolidBrush(pen.Color))
            {
                graphics.DrawString(text, font, brush, new PointF(Xpos, Ypos));
            }
        }

        /// <summary>
        /// Returns the bitmap so it can be shown on the form.
        /// </summary>
        public object getBitmap()
        {
            return canvasBitmap;
        }
    }
}

