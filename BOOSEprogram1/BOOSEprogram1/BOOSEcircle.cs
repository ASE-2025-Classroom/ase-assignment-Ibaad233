using BOOSE;
using System;
using System.Drawing;

namespace BOOSEprogram1
{
    /// <summary>
    /// Custom circle command that draws using our canvas
    /// without enforcing the original BOOSE radius limits.
    /// </summary>
    public class BOOSEcircle : CommandOneParameter
    {
        private int radius;

        /// <summary>
        /// Default constructor used by the factory system.
        /// </summary>
        public BOOSEcircle() : base()
        {
        }

        /// <summary>
        /// Manual-use constructor (not normally required).
        /// </summary>
        public BOOSEcircle(Canvas c, int radius) : base(c)
        {
            this.radius = radius;
        }

        /// <summary>
        /// Performs the circle drawing action.
        /// </summary>
        public override void Execute()
        {
            // Let BOOSE process the parameter list
            base.Execute();

            // Assign the parsed radius value
            radius = Paramsint[0];

            // Prevent invalid radius values
            if (radius < 1)
            {
                throw new CanvasException("Radius must be a positive integer.");
            }

            // Call the canvas to render the circle
            Canvas.Circle(radius, false);
        }
    }
}
