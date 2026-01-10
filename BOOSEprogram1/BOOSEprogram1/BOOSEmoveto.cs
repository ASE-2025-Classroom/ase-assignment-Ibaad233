using BOOSE;
using System;

namespace BOOSEprogram1
{
    /// <summary>
    /// Custom MoveTo command
    /// </summary>
    public class BOOSEmoveto : CommandTwoParameters
    {
        public BOOSEmoveto() : base()
        {
        }

        public override void CheckParameters(string[] parameter)
        {
            // No validation
        }

        public override void Execute()
        {
            base.Execute();

            int x = Paramsint[0];
            int y = Paramsint[1];

            if (x < 0 || y < 0)
            {
                throw new CanvasException("MoveTo requires non-negative coordinates.");
            }

            Canvas.MoveTo(x, y);
        }
    }
}