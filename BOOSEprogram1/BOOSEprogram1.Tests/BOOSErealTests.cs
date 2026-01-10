using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSE;
using BOOSEprogram1;

namespace BOOSEprogram1.Tests
{
    /// <summary>
    /// This class contains unit tests for the BOOSEreal command.
    /// These tests make sure decimal (real) variables are stored
    /// correctly and keep their precision.
    /// </summary>
    [TestClass]
    public class BOOSErealTests
    {
        /// <summary>
        /// This test creates a real variable called "pi"
        /// and assigns it a decimal value.
        /// It checks that the variable exists
        /// and that the stored value is accurate.
        /// </summary>
        [TestMethod]
        public void RealVariable_StoresDecimalValue()
        {
            // Create a canvas required for the BOOSE program
            var canvas = new BOOSEcanvas(200, 200);

            // Create a new BOOSE program using the canvas
            var program = new StoredProgram(canvas);

            // Create a real variable and assign a decimal value
            var cmd = new BOOSEreal();
            cmd.Set(program, "pi = 3.14159");

            // Compile and execute the command
            cmd.Compile();
            cmd.Execute();

            // Confirm that the variable exists in the program
            Assert.IsTrue(program.VariableExists("pi"));

            // Confirm that the stored value matches the expected decimal value
            Assert.AreEqual(3.14159, cmd.Value, 0.00001);
        }
    }
}
