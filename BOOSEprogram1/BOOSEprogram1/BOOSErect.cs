using BOOSE;
using System;

namespace BOOSEprogram1
{
    /// <summary>
    /// Custom rectangle command that draws a rectangle 
    /// </summary>
    public class BOOSErect : CommandTwoParameters
    {
        public BOOSErect() : base()
        {
        }

        public override void CheckParameters(string[] parameter)
        {
            // No validation
        }

        public override void Execute()
        {
            base.Execute();

            int width = Paramsint[0];
            int height = Paramsint[1];

            if (width <= 0 || height <= 0)
            {
                throw new CanvasException("Rectangle width and height must be greater than zero.");
            }

            Canvas.Rect(width, height, false);
        }
    }
}