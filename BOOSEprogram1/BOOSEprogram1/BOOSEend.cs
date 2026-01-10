using BOOSE;
using System;

namespace BOOSEprogram1
{
    /// <summary>
    /// End command that marks the end of the program
    /// </summary>
    public class BOOSEend : Command
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BOOSEend() : base()
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
        /// Compiles the end command
        /// </summary>
        public override void Compile()
        {
            // Nothing to compile
        }

        /// <summary>
        /// Executes the end command - stops program execution
        /// </summary>
        public override void Execute()
        {
            // End command stops execution
        }
    }
}
