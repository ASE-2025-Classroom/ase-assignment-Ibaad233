using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSE;
using BOOSEprogram1;
using System.Collections.Generic;

namespace BOOSEprogram1.Tests
{
    /// <summary>
    /// This class contains unit tests for the BOOSEif command.
    /// It checks that IF statements correctly execute the
    /// "then" commands when the condition evaluates to true.
    /// </summary>
    [TestClass]
    public class BOOSEifTests
    {
        /// <summary>
        /// This test creates an IF statement where the condition is true.
        /// It verifies that the commands inside the THEN block run,
        /// and that the ELSE block is ignored.
        /// </summary>
        [TestMethod]
        public void IfTrue_ExecutesThenCommands()
        {
            // Create a canvas required by the BOOSE program
            var canvas = new BOOSEcanvas(200, 200);

            // Create a new program that uses the canvas
            var program = new StoredProgram(canvas);

            // Declare an integer variable x and set it to 1
            var x = new BOOSEint();
            x.Set(program, "x = 1");
            x.Compile();
            x.Execute();

            // Create an IF command that checks if x equals 1
            // Note: BOOSE uses "=" instead of "==" for comparisons
            var ifCmd = new BOOSEif();
            ifCmd.Set(program, "x = 1");
            ifCmd.Compile();

            // Create a command for the THEN block that sets x to 10
            var thenAssign = new BOOSEevaluation();
            thenAssign.Set(program, "x = 10");
            thenAssign.Compile();

            // Create a command for the ELSE block that sets x to 20
            var elseAssign = new BOOSEevaluation();
            elseAssign.Set(program, "x = 20");
            elseAssign.Compile();

            // Attach the THEN and ELSE commands to the IF statement
            ifCmd.SetThenCommands(new List<ICommand> { thenAssign });
            ifCmd.SetElseCommands(new List<ICommand> { elseAssign });

            // Execute the IF statement
            ifCmd.Execute();

            // Confirm that the THEN block ran by checking x is now 10
            Assert.AreEqual("10", program.GetVarValue("x"));
        }
    }
}
