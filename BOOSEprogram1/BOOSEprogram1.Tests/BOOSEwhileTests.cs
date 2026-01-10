using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSE;
using BOOSEprogram1;
using System.Collections.Generic;

namespace BOOSEprogram1.Tests
{
    /// <summary>
    /// This class contains tests for the BOOSEwhile command.
    /// These tests make sure while loops repeat correctly
    /// and stop when their condition becomes false.
    /// </summary>
    [TestClass]
    public class BOOSEwhileTests
    {
        /// <summary>
        /// This test starts a variable at 0 and runs a while loop
        /// that keeps increasing it until it reaches 3.
        /// It checks that the loop runs the correct number of times
        /// and stops when the condition is no longer true.
        /// </summary>
        [TestMethod]
        public void WhileLoop_IncrementsUntilConditionFalse()
        {
            // Create a canvas for the BOOSE program
            var canvas = new BOOSEcanvas(200, 200);

            // Create a program that will store variables and run commands
            var program = new StoredProgram(canvas);

            // Create an integer variable x and set it to 0
            var x = new BOOSEint();
            x.Set(program, "x = 0");
            x.Compile();
            x.Execute();

            // Create a while loop that runs while x is less than 3
            var loop = new BOOSEwhile();
            loop.Set(program, "x < 3");
            loop.Compile();

            // Create a command that increases x by 1 each loop
            var inc = new BOOSEevaluation();
            inc.Set(program, "x = x + 1");
            inc.Compile();

            // Add the increment command to the loop body
            loop.SetLoopCommands(new List<ICommand> { inc });

            // Run the while loop
            loop.Execute();

            // Check that x ended up as 3 after the loop finished
            Assert.AreEqual("3", program.GetVarValue("x"));
        }
    }
}
