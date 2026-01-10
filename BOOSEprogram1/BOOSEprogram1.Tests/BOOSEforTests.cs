using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSE;
using BOOSEprogram1;
using System.Collections.Generic;

namespace BOOSEprogram1.Tests
{
    /// <summary>
    /// This class contains unit tests for the BOOSEfor class.
    /// These tests check that a for-loop is set up correctly
    /// and that the loop variable is created inside the program.
    /// </summary>
    [TestClass]
    public class BOOSEforTests
    {
        /// <summary>
        /// This test runs a simple for-loop that starts at 0,
        /// runs once, and increases the value by 1.
        /// The test checks that the loop variable "i"
        /// is created and stored in the program.
        /// </summary>
        [TestMethod]
        public void ForLoop_CreatesLoopVariable()
        {
            // Create a canvas (required by the BOOSE program)
            var canvas = new BOOSEcanvas(200, 200);

            // Create a new BOOSE program using the canvas
            var program = new StoredProgram(canvas);

            // Create a for-loop command with a simple condition
            var forCmd = new BOOSEfor();
            forCmd.Set(program, "i = 0; i < 1; i = i + 1");
            forCmd.Compile();

            // Provide an empty loop body so the loop can execute safely
            forCmd.SetLoopCommands(new List<ICommand>());

            // Execute the for-loop
            forCmd.Execute();

            // Check that the loop variable "i" exists in the program
            Assert.IsTrue(program.VariableExists("i"));
        }
    }
}
