using BOOSE;
using System;

namespace BOOSEprogram1
{
    public class BOOSEint : Evaluation, ICommand
    {
        public BOOSEint() : base()
        {
        }

        public override void Compile()
        {
            string text = ParameterList.Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new CanvasException("Variable declaration requires a name");
            }

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

            Program.AddVariable(this);
        }

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