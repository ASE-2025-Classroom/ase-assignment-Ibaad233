using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSE;
using BOOSEprogram1;

namespace BOOSEprogram1.Tests
{
    /// <summary>
    /// This class contains tests for the BOOSEarray class.
    /// It is used to make sure that arrays are created correctly
    /// and that their size is stored properly inside the program.
    /// </summary>
    [TestClass]
    public class BOOSEarrayTests
    {
        /// <summary>
        /// This test creates an array called "nums" with a size of 10.
        /// It then checks that the program knows the array exists
        /// and that the array has been given the correct size.
        /// </summary>
        [TestMethod]
        public void Array_CreatesWithCorrectSize()
        {
            // Create a canvas for drawing (required by StoredProgram)
            var canvas = new BOOSEcanvas(200, 200);

            // Create a new BOOSE program that uses this canvas
            var program = new StoredProgram(canvas);

            // Create an array command and tell it to create "nums" with size 10
            var arr = new BOOSEarray();
            arr.Set(program, "int nums 10");

            // Process the array command
            arr.Compile();
            arr.Execute();

            // Check that the array exists in the program
            Assert.IsTrue(program.VariableExists("nums"));

            // Check that the array size was set to 10
            Assert.AreEqual(10, arr.Size);
        }
    }
}
