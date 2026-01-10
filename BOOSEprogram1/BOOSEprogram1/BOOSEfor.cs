using BOOSE;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BOOSEprogram1
{
    /// <summary>
    /// Unrestricted FOR loop for BOOSE.
    /// Supports:
    ///   for var = start to end step stepValue
    /// and also supports:
    ///   for (init; condition; increment)
    /// </summary>
    public class BOOSEfor : Command
    {
        private List<ICommand> loopCommands = new List<ICommand>();

        // BOOSE style parts
        private string loopVarName = "";
        private string startExpr = "";
        private string endExpr = "";
        private string stepExpr = "1";

        // Old style parts (optional)
        private string initialization = "";
        private string condition = "";
        private string increment = "";

        private bool isBooseStyle = true;

        public BOOSEfor() : base() { }

        public void SetLoopCommands(List<ICommand> commands)
        {
            loopCommands = commands ?? new List<ICommand>();
        }

        public override void CheckParameters(string[] parameter)
        {
            // No validation needed
        }

        public override void Compile()
        {
            string fullParams = this.ParameterList.Trim();

            if (string.IsNullOrWhiteSpace(fullParams))
                throw new CanvasException("For loop requires parameters");

            // Remove surrounding brackets if present
            if (fullParams.StartsWith("(") && fullParams.EndsWith(")"))
                fullParams = fullParams.Substring(1, fullParams.Length - 2).Trim();

            // If it contains ';' then treat as old style: init; condition; increment
            if (fullParams.Contains(";"))
            {
                isBooseStyle = false;

                string[] parts = fullParams.Split(';');
                if (parts.Length != 3)
                    throw new CanvasException("For loop requires 3 parts: for (init; condition; increment)");

                initialization = parts[0].Trim();
                condition = parts[1].Trim();
                increment = parts[2].Trim();
                return;
            }

            // Otherwise treat as BOOSE style:
            // for count = 1 to 20 step 2
            isBooseStyle = true;

            // Split into "var = start" and "end step ..."
            // Expect "... to ..."
            int toIndex = IndexOfWord(fullParams, "to");
            if (toIndex < 0)
                throw new CanvasException("For loop syntax: for var = start to end step stepValue");

            string left = fullParams.Substring(0, toIndex).Trim();
            string right = fullParams.Substring(toIndex + 2).Trim(); // after "to"

            // left must contain '='
            int eqIndex = left.IndexOf('=');
            if (eqIndex < 0)
                throw new CanvasException("For loop syntax: for var = start to end step stepValue");

            loopVarName = left.Substring(0, eqIndex).Trim();
            startExpr = left.Substring(eqIndex + 1).Trim();

            if (string.IsNullOrWhiteSpace(loopVarName))
                throw new CanvasException("For loop requires a loop variable name");

            // right can be "end" or "end step X"
            int stepIndex = IndexOfWord(right, "step");
            if (stepIndex < 0)
            {
                endExpr = right.Trim();
                stepExpr = "1";
            }
            else
            {
                endExpr = right.Substring(0, stepIndex).Trim();
                stepExpr = right.Substring(stepIndex + 4).Trim(); // after "step"
                if (string.IsNullOrWhiteSpace(stepExpr))
                    throw new CanvasException("For loop 'step' requires a value");
            }

            if (string.IsNullOrWhiteSpace(startExpr) || string.IsNullOrWhiteSpace(endExpr))
                throw new CanvasException("For loop requires start and end values");
        }

        public override void Execute()
        {
            if (!isBooseStyle)
            {
                ExecuteOldStyleFor();
                return;
            }

            int start = EvaluateAsInt(startExpr);
            int end = EvaluateAsInt(endExpr);
            int step = EvaluateAsInt(stepExpr);

            if (step == 0)
                throw new CanvasException("For loop step cannot be 0");

            // Ensure loop variable exists (create it if missing)
            EnsureIntVariable(loopVarName, start);

            int iterations = 0;
            int maxIterations = 1_000_000;

            // Decide loop direction based on step
            if (step > 0)
            {
                for (int i = start; i <= end; i += step)
                {
                    if (iterations++ > maxIterations)
                        throw new CanvasException("For loop exceeded maximum iterations");

                    Program.UpdateVariable(loopVarName, i);

                    foreach (ICommand cmd in loopCommands)
                        cmd.Execute();
                }
            }
            else
            {
                for (int i = start; i >= end; i += step) // step is negative
                {
                    if (iterations++ > maxIterations)
                        throw new CanvasException("For loop exceeded maximum iterations");

                    Program.UpdateVariable(loopVarName, i);

                    foreach (ICommand cmd in loopCommands)
                        cmd.Execute();
                }
            }
        }

        // -------------------------
        // Helpers
        // -------------------------

        private void ExecuteOldStyleFor()
        {
            ExecuteAssignment(initialization);

            int iterations = 0;
            int maxIterations = 1_000_000;

            while (EvaluateCondition(condition))
            {
                if (iterations++ > maxIterations)
                    throw new CanvasException("For loop exceeded maximum iterations");

                foreach (ICommand cmd in loopCommands)
                    cmd.Execute();

                ExecuteAssignment(increment);
            }
        }

        private int EvaluateAsInt(string expr)
        {
            string evaluated = ReplaceVariables(expr);

            // allow things like "count * 10"
            try
            {
                var dt = new System.Data.DataTable();
                object result = dt.Compute(evaluated, "");
                return Convert.ToInt32(result);
            }
            catch
            {
                if (int.TryParse(evaluated, out int direct))
                    return direct;

                throw new CanvasException($"Cannot evaluate expression: {expr}");
            }
        }

        private string ReplaceVariables(string expr)
        {
            string evaluated = expr.Trim();

            // Replace variable names with values safely (word boundaries)
            string[] tokens = Regex.Split(evaluated, @"([^\w]+)");
            for (int i = 0; i < tokens.Length; i++)
            {
                string t = tokens[i];
                if (Regex.IsMatch(t, @"^\w+$") && Program.VariableExists(t))
                {
                    tokens[i] = Program.GetVarValue(t);
                }
            }
            return string.Concat(tokens);
        }

        private void EnsureIntVariable(string name, int initialValue)
        {
            if (!Program.VariableExists(name))
            {
                // create int variable
                var v = new BOOSEint();
                v.Set(Program, $"{name} = {initialValue}");
                v.Compile();
                v.Execute();
            }
            else
            {
                Program.UpdateVariable(name, initialValue);
            }
        }

        private void ExecuteAssignment(string assignment)
        {
            if (!assignment.Contains("="))
                throw new CanvasException($"Invalid assignment: {assignment}");

            string[] parts = assignment.Split('=');
            string varName = parts[0].Trim();
            string expression = parts[1].Trim();

            int value = EvaluateAsInt(expression);

            if (!Program.VariableExists(varName))
            {
                EnsureIntVariable(varName, value);
            }
            else
            {
                Program.UpdateVariable(varName, value);
            }
        }

        private bool EvaluateCondition(string cond)
        {
            string evaluated = ReplaceVariables(cond);

            
            evaluated = evaluated.Replace("==", "=").Replace("!=", "<>");

            try
            {
                var dt = new System.Data.DataTable();
                object result = dt.Compute(evaluated, "");
                return Convert.ToBoolean(result);
            }
            catch (Exception ex)
            {
                throw new CanvasException($"Cannot evaluate for condition: {cond} (evaluated to: {evaluated}) - {ex.Message}");
            }
        }

        private static int IndexOfWord(string text, string word)
        {
            // finds " word " or "word " or " word" in a case-insensitive way
            var m = Regex.Match(text, $@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase);
            return m.Success ? m.Index : -1;
        }
    }
}
