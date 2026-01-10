using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSE;
using BOOSEprogram1;

namespace BOOSEprogram1.Tests
{
    /// <summary>
    /// This class contains unit tests for the BOOSEint command.
    /// These tests make sure integer variables are created
    /// and stored correctly inside the BOOSE program.
    /// </summary>
    [TestClass]
    public class BOOSEintTests
    {
        /// <summary>
        /// This test creates an integer variable called "x"
        /// and assigns it the value 10.
        /// It then checks that the variable exists
        /// and that the stored value is correct.
        /// </summary>
        [TestMethod]
        public void IntVariable_StoresCorrectValue()
        {
            // Create a canvas (required for the BOOSE program to run)
            var canvas = new BOOSEcanvas(200, 200);

            // Create a new BOOSE program that uses the canvas
            var program = new StoredProgram(canvas);

            // Create an integer command to declare x and set it to 10
            var cmd = new BOOSEint();
            cmd.Set(program, "x = 10");

            // Compile and execute the command
            cmd.Compile();
            cmd.Execute();

            // Check that the variable exists in the program
            Assert.IsTrue(program.VariableExists("x"));

            // Check that the value stored for x is "10"
            Assert.AreEqual("10", program.GetVarValue("x"));
        }
    }
}
