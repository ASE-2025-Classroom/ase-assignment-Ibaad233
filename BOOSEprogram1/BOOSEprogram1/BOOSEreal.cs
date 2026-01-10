using BOOSE;
using System;

namespace BOOSEprogram1
{
    /// <summary>
    /// Custom real (floating-point) variable class that replaces BOOSE's Real class.
    /// Removes restrictions on variable names and variable count limits.
    /// </summary>
    public class BOOSEreal : Evaluation, ICommand
    {
        private double doubleValue;

        /// <summary>
        /// Get or set the double value for this real number.
        /// </summary>
        public double Value
        {
            get { return doubleValue; }
            set { doubleValue = value; }
        }

        /// <summary>
        /// Constructor for creating real variable objects.
        /// </summary>
        public BOOSEreal() : base()
        {
        }

        /// <summary>
        /// Check parameters - override to remove validation restrictions.
        /// </summary>
        public override void CheckParameters(string[] parameter)
        {
        }

        /// <summary>
        /// Compiles the real variable declaration or initialization.
        /// Parses variable name and optional initial value expression.
        /// </summary>
        public override void Compile()
        {
            string fullExpression = this.ParameterList.Trim();

            if (string.IsNullOrWhiteSpace(fullExpression))
            {
                throw new CanvasException("Variable declaration requires a name");
            }

            if (fullExpression.Contains("="))
            {
                string[] parts = fullExpression.Split('=');
                this.varName = parts[0].Trim();
                this.expression = parts[1].Trim();
            }
            else
            {
                this.varName = fullExpression;
                this.expression = "0.0";
            }

            this.Program.AddVariable(this);
        }

        /// <summary>
        /// Executes the real variable initialization.
        /// Evaluates the expression and stores the floating-point value.
        /// </summary>
        public override void Execute()
        {
            string evaluated = this.expression.Trim();

            string[] tokens = evaluated.Split(new[] { ' ', '+', '-', '*', '/', '(', ')' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokens)
            {
                if (this.Program.VariableExists(token))
                {
                    Evaluation var = this.Program.GetVariable(token);
                    string value;

                    if (var is BOOSEreal)
                    {
                        value = ((BOOSEreal)var).Value.ToString();
                    }
                    else if (var is Real)
                    {
                        value = ((Real)var).Value.ToString();
                    }
                    else
                    {
                        value = this.Program.GetVarValue(token);
                    }

                    evaluated = evaluated.Replace(token, value);
                }
            }

            if (double.TryParse(evaluated, out double directResult))
            {
                this.Value = directResult;
            }
            else
            {
                try
                {
                    var dataTable = new System.Data.DataTable();
                    var result = dataTable.Compute(evaluated, "");
                    double dblResult = Convert.ToDouble(result);
                    this.Value = dblResult;
                }
                catch
                {
                    throw new CanvasException($"Cannot convert '{evaluated}' to real for variable '{this.varName}'");
                }
            }
        }
    }
}