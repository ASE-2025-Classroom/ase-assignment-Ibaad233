using BOOSE;
namespace BOOSEprogram1
{
    /// <summary>
    /// Custom rectangle command that draws a rectangle 
    /// using the application's canvas instead of BOOSE's default one.
    /// </summary>
    public class BOOSErect : CommandTwoParameters
    {
        /// <summary>
        /// Blank constructor used by the command factory.
        /// </summary>
        public BOOSErect() : base()
        {
        }
        /// <summary>
        /// Constructor used when creating the command manually.
        /// </summary>
        /// <param name="c">The canvas used for drawing.</param>
        /// <param name="width">Rectangle width.</param>
        /// <param name="height">Rectangle height.</param>
        public BOOSErect(Canvas c, int width, int height) : base(c)
        {
            Paramsint = new[] { width, height };
        }
        /// <summary>
        /// Executes the rectangle command. Draws an unfilled rectangle
        /// starting at the current cursor position.
        /// </summary>
        /// <exception cref="CanvasException">Thrown if width or height are invalid.</exception>
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
