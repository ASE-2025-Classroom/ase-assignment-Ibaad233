using BOOSE;
using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BOOSEprogram1
{
    /// <summary>
    /// Custom write command that supports:
    /// - numbers (including real variables)
    /// - arithmetic expressions
    /// - string literals in quotes
    /// - string concatenation with +
    /// Example: write "£"+y
    /// </summary>
    public class BOOSEwrite : Evaluation, ICommand
    {
        private static BOOSEcanvas staticCanvas;

        public static void SetCanvas(BOOSEcanvas canvas)
        {
            staticCanvas = canvas;
        }

        public BOOSEwrite() : base() { }

        public override void CheckParameters(string[] parameter)
        {
            // no validation
        }

        public override void Compile()
        {
            this.expression = this.ParameterList.Trim();
        }

        public override void Execute()
        {
            if (staticCanvas == null) return;

            string expr = this.expression?.Trim() ?? "";
            if (expr.Length == 0) return;

            string output = EvaluateWriteExpression(expr);
            staticCanvas.WriteText(output);
        }

        /// <summary>
        /// Evaluates a BOOSE write expression.
        /// Supports string literals and + concatenation.
        /// Falls back to numeric evaluation for pure maths.
        /// </summary>
        private string EvaluateWriteExpression(string expr)
        {
            // If it contains quotes, treat as string concatenation mode
            if (expr.Contains("\""))
            {
                return EvaluateAsStringConcat(expr);
            }

            // Otherwise treat as numeric expression
            return EvaluateAsNumeric(expr);
        }

        private string EvaluateAsStringConcat(string expr)
        {
            // Split by + but NOT inside quotes
            // Example: "£"+y  -> ["\"£\"", "y"]
            var parts = Regex.Split(expr, @"\s*\+\s*(?=(?:[^""]*""[^""]*"")*[^""]*$)");

            string result = "";

            foreach (string rawPart in parts)
            {
                string part = rawPart.Trim();
                if (part.Length == 0) continue;

                // If it's a quoted string, add it directly (without quotes)
                if (part.StartsWith("\"") && part.EndsWith("\"") && part.Length >= 2)
                {
                    result += part.Substring(1, part.Length - 2);
                    continue;
                }

                // If it's a variable, append its value
                if (this.Program.VariableExists(part))
                {
                    var v = this.Program.GetVariable(part);

                    if (v is BOOSEreal br)
                        result += br.Value.ToString(CultureInfo.InvariantCulture);
                    else if (v is Real rr)
                        result += rr.Value.ToString(CultureInfo.InvariantCulture);
                    else
                        result += this.Program.GetVarValue(part);

                    continue;
                }

                // Otherwise, attempt numeric evaluation (supports things like: "Area="+(x*y)
                result += EvaluateAsNumeric(part);
            }

            return result;
        }

        private string EvaluateAsNumeric(string expr)
        {
            string evaluated = ReplaceVariables(expr);

            try
            {
                var dt = new DataTable();
                var value = dt.Compute(evaluated, "");
                return Convert.ToString(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                // If it's just a number, return it
                if (double.TryParse(evaluated, NumberStyles.Any, CultureInfo.InvariantCulture, out double d))
                    return d.ToString(CultureInfo.InvariantCulture);

                // Otherwise return raw text 
                return evaluated;
            }
        }

        private string ReplaceVariables(string expr)
        {
            string result = expr;

            foreach (Match m in Regex.Matches(expr, @"\b[A-Za-z_]\w*\b"))
            {
                string name = m.Value;

                if (this.Program.VariableExists(name))
                {
                    var v = this.Program.GetVariable(name);

                    string replacement;
                    if (v is BOOSEreal br)
                        replacement = br.Value.ToString(CultureInfo.InvariantCulture);
                    else if (v is Real rr)
                        replacement = rr.Value.ToString(CultureInfo.InvariantCulture);
                    else
                        replacement = this.Program.GetVarValue(name);

                    result = Regex.Replace(result, $@"\b{Regex.Escape(name)}\b", replacement);
                }
            }

            return result;
        }
    }
}
