using BOOSE;
using System;

namespace BOOSEprogram1
{
    /// <summary>
    /// Else command for if-else statements
    /// </summary>
    public class BOOSEelse : Command
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BOOSEelse() : base()
        {
        }

        /// <summary>
        /// Check parameters - no validation needed
        /// </summary>
        public override void CheckParameters(string[] parameter)
        {
            // No validation needed
        }

        /// <summary>
        /// Compiles the else command
        /// </summary>
        public override void Compile()
        {
            // Nothing to compile
        }

        /// <summary>
        /// Executes the else command
        /// </summary>
        public override void Execute()
        {
            // Else is handled by the if statement
        }
    }
}