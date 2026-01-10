using BOOSE;
using System;

namespace BOOSEprogram1
{
    /// <summary>
    /// Poke command - sets a value in an array
    /// Supports both "poke arrayName[index] = value" and "poke arrayName index = value"
    /// </summary>
    public class BOOSEpoke : Evaluation, ICommand
    {
        private string arrayName;
        private string indexExpression;
        private string valueExpression;

        /// <summary>
        /// Constructor
        /// </summary>
        public BOOSEpoke() : base()
        {
        }

        /// <summary>
        /// Compiles the poke command
        /// </summary>
        public override void Compile()
        {
            string fullExpression = this.ParameterList.Trim();

            if (!fullExpression.Contains("="))
            {
                throw new CanvasException("Poke command requires '='");
            }

            string[] parts = fullExpression.Split('=');
            string leftSide = parts[0].Trim();
            this.valueExpression = parts[1].Trim();

            // Check if using bracket syntax: arrayName[index]
            if (leftSide.Contains("[") && leftSide.Contains("]"))
            {
                int bracketStart = leftSide.IndexOf('[');
                int bracketEnd = leftSide.IndexOf(']');

                this.arrayName = leftSide.Substring(0, bracketStart).Trim();
                this.indexExpression = leftSide.Substring(bracketStart + 1, bracketEnd - bracketStart - 1).Trim();
            }
            else
            {
                // Parse "arrayName index = value" syntax
                string[] leftParts = leftSide.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (leftParts.Length >= 2)
                {
                    this.arrayName = leftParts[0].Trim();
                    this.indexExpression = leftParts[1].Trim();
                }
                else
                {
                    throw new CanvasException("Poke requires array name and index");
                }
            }

            if (!this.Program.VariableExists(this.arrayName))
            {
                throw new CanvasException($"Array '{this.arrayName}' does not exist");
            }
        }

        /// <summary>
        /// Executes the poke command
        /// </summary>
        public override void Execute()
        {
            BOOSEarray array = this.Program.GetVariable(this.arrayName) as BOOSEarray;
            if (array == null)
            {
                throw new CanvasException($"Variable '{this.arrayName}' is not an array");
            }

            int index = EvaluateExpressionAsInt(this.indexExpression);
            string evaluatedValue = EvaluateExpression(this.valueExpression);

            if (int.TryParse(evaluatedValue, out int intValue))
            {
                array[index] = intValue;
            }
            else if (double.TryParse(evaluatedValue, out double dblValue))
            {
                array[index] = dblValue;
            }
            else
            {
                throw new CanvasException($"Cannot evaluate value: {this.valueExpression}");
            }
        }

        /// <summary>
        /// Evaluates an expression as an integer
        /// </summary>
        private int EvaluateExpressionAsInt(string expr)
        {
            string evaluated = EvaluateExpression(expr);
            if (!int.TryParse(evaluated, out int result))
            {
                throw new CanvasException($"Array index must be an integer: {expr}");
            }
            return result;
        }

        /// <summary>
        /// Evaluates an expression
        /// </summary>
        private string EvaluateExpression(string expr)
        {
            string result = expr.Trim();

            string[] tokens = result.Split(new[] { ' ', '+', '-', '*', '/', '(', ')', '[', ']' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokens)
            {
                if (this.Program.VariableExists(token))
                {
                    result = result.Replace(token, this.Program.GetVarValue(token));
                }
            }

            try
            {
                var dataTable = new System.Data.DataTable();
                var evalResult = dataTable.Compute(result, "");
                return evalResult.ToString();
            }
            catch
            {
                return result;
            }
        }

        /// <summary>
        /// </summary>
        public override void CheckParameters(string[] parameter)
        {
        }
    }
}