using BOOSE;
using System;
using System.Collections.Generic;

namespace BOOSEprogram1
{
    /// <summary>
    /// If-else statement implementation for BOOSE language
    /// </summary>
    public class BOOSEif : Command
    {
        private string condition;
        private List<ICommand> thenCommands;
        private List<ICommand> elseCommands;

        public BOOSEif() : base()
        {
            thenCommands = new List<ICommand>();
            elseCommands = new List<ICommand>();
        }

        public void SetThenCommands(List<ICommand> commands)
        {
            thenCommands = commands;
        }

        public void SetElseCommands(List<ICommand> commands)
        {
            elseCommands = commands;
        }

        public override void Compile()
        {
            this.condition = this.ParameterList.Trim();

            if (string.IsNullOrWhiteSpace(this.condition))
            {
                throw new CanvasException("If statement requires a condition");
            }

            if (this.condition.StartsWith("(") && this.condition.EndsWith(")"))
            {
                this.condition = this.condition.Substring(1, this.condition.Length - 2).Trim();
            }
        }

        public override void Execute()
        {
            if (EvaluateCondition(this.condition))
            {
                foreach (ICommand cmd in thenCommands)
                {
                    cmd.Execute();
                }
            }
            else if (elseCommands.Count > 0)
            {
                foreach (ICommand cmd in elseCommands)
                {
                    cmd.Execute();
                }
            }
        }

        private bool EvaluateCondition(string cond)
        {
            string evaluated = cond.Trim();

            string[] tokens = evaluated.Split(new[] { ' ', '<', '>', '=', '!', '&', '|', '(', ')' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokens)
            {
                if (this.Program.VariableExists(token))
                {
                    evaluated = evaluated.Replace(token, this.Program.GetVarValue(token));
                }
            }

            try
            {
                var dataTable = new System.Data.DataTable();
                var result = dataTable.Compute(evaluated, "");
                return Convert.ToBoolean(result);
            }
            catch
            {
                throw new CanvasException($"Cannot evaluate if condition: {cond}");
            }
        }
            /// <summary>
/// Check parameters - override to remove validation
/// </summary>
public override void CheckParameters(string[] parameter)
        {
           
        }
    }

    }
