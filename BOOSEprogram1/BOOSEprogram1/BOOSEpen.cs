using BOOSE;
using System;

namespace BOOSEprogram1
{
    /// <summary>
    /// Pen command to change pen color
    /// </summary>
    public class BOOSEpen : CommandThreeParameters
    {
        public BOOSEpen() : base()
        {
        }

        public override void CheckParameters(string[] parameter)
        {
            // No validation
        }

        public override void Execute()
        {
            base.Execute();

            int red = Paramsint[0];
            int green = Paramsint[1];
            int blue = Paramsint[2];

            if (red < 0 || red > 255 || green < 0 || green > 255 || blue < 0 || blue > 255)
            {
                throw new CanvasException("Pen color values must be between 0 and 255");
            }

            Canvas.SetColour(red, green, blue);
        }
    }
}