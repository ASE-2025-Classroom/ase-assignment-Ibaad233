using BOOSE;
using System;

namespace BOOSEprogram1
{
    public class BOOSEevaluation : Evaluation, ICommand
    {
        public BOOSEevaluation() : base()
        {
        }

        public override void Compile()
        {
            string fullExpression = this.ParameterList.Trim();

            if (string.IsNullOrWhiteSpace(fullExpression))
                throw new CanvasException("Assignment requires an expression");

            if (!fullExpression.Contains("="))
                throw new CanvasException("Assignment must contain '='");

            string[] parts = fullExpression.Split('=');
            this.VarName = parts[0].Trim();
            this.Expression = parts[1].Trim();

            if (!this.Program.VariableExists(this.VarName))
                throw new CanvasException($"Variable '{this.VarName}' must be declared before assignment");
        }

        public override void Execute()
        {
            string evaluated = EvaluateManually(this.Expression);

            if (int.TryParse(evaluated, out int intResult))
            {
                this.Program.UpdateVariable(this.VarName, intResult);
            }
            else if (double.TryParse(evaluated, out double realResult))
            {
                this.Program.UpdateVariable(this.VarName, realResult);
            }
            else
            {
                throw new CanvasException($"Cannot evaluate '{this.Expression}' for variable '{this.VarName}'");
            }
        }

        private string EvaluateManually(string expr)
        {
            string result = expr.Trim();

            // Split on common operators to find variable tokens
            string[] tokens = result.Split(
                new[] { ' ', '+', '-', '*', '/', '(', ')', ',' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokens)
            {
                if (this.Program.VariableExists(token))
                {
                    // BOOSE StoredProgram typically returns a string from EvaluateExpression,
                    // but since yours needs manual replacement we use GetVarValue like your example.
                    // If your StoredProgram doesn't have GetVarValue, tell me the correct method name.
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
