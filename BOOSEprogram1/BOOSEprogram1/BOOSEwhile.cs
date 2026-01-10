using BOOSE;
using System;
using System.Collections.Generic;

namespace BOOSEprogram1
{
    /// <summary>
    /// While loop implementation for BOOSE language
    /// </summary>
    public class BOOSEwhile : Command
    {
        private string condition;
        private List<ICommand> loopCommands;

        public BOOSEwhile() : base()
        {
            loopCommands = new List<ICommand>();
        }

        public void SetLoopCommands(List<ICommand> commands)
        {
            loopCommands = commands;
        }

        /// <summary>
        /// Check parameters - override to remove validation
        /// </summary>
        public override void CheckParameters(string[] parameter)
        {
            // No validation needed
        }

        public override void Compile()
        {
            this.condition = this.ParameterList.Trim();

            if (string.IsNullOrWhiteSpace(this.condition))
            {
                throw new CanvasException("While loop requires a condition");
            }

            if (this.condition.StartsWith("(") && this.condition.EndsWith(")"))
            {
                this.condition = this.condition.Substring(1, this.condition.Length - 2).Trim();
            }
        }

        public override void Execute()
        {
            int iterations = 0;
            int maxIterations = 100000;

            System.Diagnostics.Debug.WriteLine($"Starting while loop with condition: {this.condition}");
            System.Diagnostics.Debug.WriteLine($"Loop has {loopCommands.Count} commands");

            while (EvaluateCondition(this.condition))
            {
                if (iterations++ > maxIterations)
                {
                    throw new CanvasException($"While loop exceeded maximum iterations (stopped at 10 for testing)");
                }

                System.Diagnostics.Debug.WriteLine($"Iteration {iterations}: Executing {loopCommands.Count} commands");

                foreach (ICommand cmd in loopCommands)
                {
                    System.Diagnostics.Debug.WriteLine($"  Executing command: {cmd.GetType().Name}");
                    cmd.Execute();
                }

                // Check the condition after commands execute
                System.Diagnostics.Debug.WriteLine($"After iteration {iterations}, condition still: {this.condition}");
            }

            System.Diagnostics.Debug.WriteLine("While loop completed successfully");
        }

        private bool EvaluateCondition(string cond)
        {
            string evaluated = cond.Trim();

            // Replace variables with their values
            string[] tokens = evaluated.Split(new[] { ' ', '<', '>', '=', '!', '&', '|', '(', ')' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokens)
            {
                if (this.Program.VariableExists(token))
                {
                    string value = this.Program.GetVarValue(token);
                    evaluated = evaluated.Replace(token, value);
                }
            }

            try
            {
                // Handle common comparison operators manually
                if (evaluated.Contains(">="))
                {
                    string[] parts = evaluated.Split(new[] { ">=" }, StringSplitOptions.None);
                    double left = double.Parse(parts[0].Trim());
                    double right = double.Parse(parts[1].Trim());
                    return left >= right;
                }
                else if (evaluated.Contains("<="))
                {
                    string[] parts = evaluated.Split(new[] { "<=" }, StringSplitOptions.None);
                    double left = double.Parse(parts[0].Trim());
                    double right = double.Parse(parts[1].Trim());
                    return left <= right;
                }
                else if (evaluated.Contains(">"))
                {
                    string[] parts = evaluated.Split('>');
                    double left = double.Parse(parts[0].Trim());
                    double right = double.Parse(parts[1].Trim());
                    return left > right;
                }
                else if (evaluated.Contains("<"))
                {
                    string[] parts = evaluated.Split('<');
                    double left = double.Parse(parts[0].Trim());
                    double right = double.Parse(parts[1].Trim());
                    return left < right;
                }
                else if (evaluated.Contains("=="))
                {
                    string[] parts = evaluated.Split(new[] { "==" }, StringSplitOptions.None);
                    double left = double.Parse(parts[0].Trim());
                    double right = double.Parse(parts[1].Trim());
                    return left == right;
                }
                else if (evaluated.Contains("!="))
                {
                    string[] parts = evaluated.Split(new[] { "!=" }, StringSplitOptions.None);
                    double left = double.Parse(parts[0].Trim());
                    double right = double.Parse(parts[1].Trim());
                    return left != right;
                }
                else
                {
                    var dataTable = new System.Data.DataTable();
                    var result = dataTable.Compute(evaluated, "");
                    return Convert.ToBoolean(result);
                }
            }
            catch (Exception ex)
            {
                throw new CanvasException($"Cannot evaluate while condition: {cond} (evaluated to: {evaluated}) - {ex.Message}");
            }
        }
    }
}