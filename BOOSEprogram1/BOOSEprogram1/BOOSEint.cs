using BOOSE;
using System;

namespace BOOSEprogram1
{
    /// <summary>
    /// Represents an integer variable in the BOOSE language.
    /// This class handles declaring, storing and updating whole-number values.
    /// </summary>
    public class BOOSEint : Evaluation, ICommand
    {
        /// <summary>
        /// Creates a new integer variable command.
        /// </summary>
        public BOOSEint() : base()
        {
        }

        /// <summary>
        /// Reads and processes the variable declaration.
        /// This extracts the variable name and any starting value.
        /// </summary>
        public override void Compile()
        {
            string text = ParameterList.Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new CanvasException("Variable declaration requires a name");
            }

            // If this variable was already created, only update its value
            if (!string.IsNullOrWhiteSpace(varName))
            {
                if (text.StartsWith("="))
                {
                    text = text.Substring(1).Trim();
                }

                expression = string.IsNullOrWhiteSpace(text) ? "0" : text;
                return;
            }

            int eq = text.IndexOf('=');

            if (eq >= 0)
            {
                varName = text.Substring(0, eq).Trim();
                expression = text.Substring(eq + 1).Trim();
            }
            else
            {
                varName = text.Trim();
                expression = "0";
            }

            // Register the variable with the program
            Program.AddVariable(this);
        }

        /// <summary>
        /// Evaluates the expression and stores the result as an integer.
        /// Updates the program’s variable table with the new value.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            if (!int.TryParse(evaluatedExpression, out int result))
            {
                throw new CanvasException(
                    $"Cannot convert '{evaluatedExpression}' to integer for variable '{varName}'");
            }

            value = result;
            Program.UpdateVariable(varName, result);
        }
    }
}
