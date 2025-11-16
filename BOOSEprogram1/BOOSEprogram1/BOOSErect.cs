using BOOSE;
using System;

namespace BOOSEprogram1
{
    /// <summary>
    /// Custom rectangle command that works with our canvas
    /// and removes the standard BOOSE size limitations.
    /// </summary>
    public class BOOSErect : CommandTwoParameters
    {
        private int width;
        private int height;

        /// <summary>
        /// Default constructor used by the factory.
        /// </summary>
        public BOOSErect() : base()
        {
        }

        /// <summary>
        /// Optional constructor for manually creating the command.
        /// </summary>
        public BOOSErect(Canvas c, int width, int height) : base(c)
        {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Runs the rectangle drawing operation.
        /// </summary>
        public override void Execute()
        {
            // Allow BOOSE to process and validate the parameters
            base.Execute();

            // Retrieve integer values from the parsed parameter list
            width = Paramsint[0];
            height = Paramsint[1];

            // Basic safeguard to avoid drawing invalid shapes
            if (width < 1 || height < 1)
            {
                throw new CanvasException("Both rectangle dimensions must be positive numbers.");
            }

            // Issue the draw call to the canvas
            Canvas.Rect(width, height, false);
        }
    }
}
