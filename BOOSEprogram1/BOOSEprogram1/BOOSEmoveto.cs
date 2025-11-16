using BOOSE;
using System;

namespace BOOSEprogram1
{
    /// <summary>
    /// Custom MoveTo command that repositions the canvas cursor
    /// to the specified coordinates without drawing anything.
    /// </summary>
    public class BOOSEmoveto : CommandTwoParameters
    {
        private int x;
        private int y;

        /// <summary>
        /// Default constructor used by the command factory.
        /// </summary>
        public BOOSEmoveto() : base()
        {
        }

        /// <summary>
        /// Optional constructor if you decide to instantiate manually.
        /// </summary>
        public BOOSEmoveto(Canvas c, int x, int y) : base(c)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Carries out the MoveTo command, shifting the cursor position.
        /// </summary>
        public override void Execute()
        {
            // Allow BOOSE to read and prepare the parameter values
            base.Execute();

            x = Paramsint[0];
            y = Paramsint[1];

            // Coordinate bounds check
            if (x < 0 || y < 0)
            {
                throw new CanvasException("MoveTo requires non-negative coordinates.");
            }

            // Update the canvas cursor position
            Canvas.MoveTo(x, y);
        }
    }
}
