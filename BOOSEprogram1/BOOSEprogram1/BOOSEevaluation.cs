using BOOSE;
using System;

namespace BOOSEprogram1
{
    /// <summary>
    /// Custom evaluation class for handling standalone variable assignments.
    /// Allows assignments like "width = 2*radius" after variable declaration.
    /// </summary>
    public class BOOSEevaluation : Evaluation, ICommand
    {
        /// <summary>
        /// Constructor for creating evaluation objects.
        /// </summary>
        public BOOSEevaluation() : base()
        {
        }

        /// <summary>
        /// Check parameters - override to remove validation.
        /// </summary>
        public override void CheckParameters(string[] parameter)
        {
            
        }

        /// <summary>
        /// Compiles the variable assignment statement.
        /// Validates that the variable exists before assignment.
        /// </summary>
        public override void Compile()
        {
            string fullExpression = this.ParameterList.Trim();

            if (string.IsNullOrWhiteSpace(fullExpression))
            {
                throw new CanvasException("Assignment requires an expression");
            }

            if (fullExpression.Contains("="))
            {
                string[] parts = fullExpression.Split('=');
                this.varName = parts[0].Trim();
                this.expression = parts[1].Trim();
            }
            else
            {
                throw new CanvasException("Assignment must contain '='");
            }

            if (!this.Program.VariableExists(this.varName))
            {
                throw new CanvasException($"Variable '{this.varName}' must be declared before assignment");
            }
        }

        /// <summary>
        /// Executes the variable assignment.
        /// Evaluates the expression and updates the variable value.
        /// </summary>
        public override void Execute()
        {
            string evaluated = EvaluateManually(this.expression);

            Evaluation existingVar = this.Program.GetVariable(this.varName);

           
            if (existingVar is Real || existingVar is BOOSEreal)
            {
                if (double.TryParse(evaluated, out double doubleResult))
                {
                    this.Program.UpdateVariable(this.varName, doubleResult);
                }
                else
                {
                    throw new CanvasException(
                        $"Cannot evaluate '{this.expression}' as real for variable '{this.varName}'");
                }
            }
            else if (int.TryParse(evaluated, out int intResult))
            {
                this.value = intResult;
                this.Program.UpdateVariable(this.varName, intResult);
            }
            else if (double.TryParse(evaluated, out double dblResult))
            {
                this.Program.UpdateVariable(this.varName, dblResult);
            }
            else
            {
                throw new CanvasException(
                    $"Cannot evaluate '{this.expression}' for variable '{this.varName}'");
            }
        }

        /// <summary>
        /// Manually evaluates expressions containing variables and arithmetic operations.
        /// Replaces variable names with their values and computes the result.
        /// </summary>
        private string EvaluateManually(string expr)
        {
            string result = expr.Trim();

            string[] tokens = result.Split(
                new[] { ' ', '+', '-', '*', '/', '(', ')' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokens)
            {
                if (this.Program.VariableExists(token))
                {
                    string value = this.Program.GetVarValue(token);
                    result = result.Replace(token, value);
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
                throw new CanvasException($"Cannot evaluate expression: {result}");
            }
        }
    }
}
