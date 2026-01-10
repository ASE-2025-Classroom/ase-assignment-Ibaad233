using BOOSE;
using System;

namespace BOOSEprogram1
{
    /// <summary>
    /// Peek command - retrieves a value from an array
    /// Supports both:
    ///   peek var = arrayName[index]
    ///   peek var = arrayName index
    /// </summary>
    public class BOOSEpeek : Evaluation, ICommand
    {
        private string targetVariable;
        private string arrayName;
        private string indexExpression;

        public BOOSEpeek() : base() { }

        public override void Compile()
        {
            string fullExpression = this.ParameterList.Trim();

            if (!fullExpression.Contains("="))
                throw new CanvasException("Peek command requires '='");

            string[] parts = fullExpression.Split('=');
            this.targetVariable = parts[0].Trim();
            string rightSide = parts[1].Trim();

            // arrayName[index]
            if (rightSide.Contains("[") && rightSide.Contains("]"))
            {
                int bracketStart = rightSide.IndexOf('[');
                int bracketEnd = rightSide.IndexOf(']');

                this.arrayName = rightSide.Substring(0, bracketStart).Trim();
                this.indexExpression = rightSide.Substring(bracketStart + 1, bracketEnd - bracketStart - 1).Trim();
            }
            else
            {
                // arrayName index
                string[] rightParts = rightSide.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (rightParts.Length >= 2)
                {
                    this.arrayName = rightParts[0].Trim();
                    this.indexExpression = rightParts[1].Trim();
                }
                else
                {
                    throw new CanvasException("Peek requires array name and index");
                }
            }

            if (!this.Program.VariableExists(this.targetVariable))
                throw new CanvasException($"Target variable '{this.targetVariable}' does not exist");

            if (!this.Program.VariableExists(this.arrayName))
                throw new CanvasException($"Array '{this.arrayName}' does not exist");
        }

        public override void Execute()
        {
            // Get array
            BOOSEarray array = this.Program.GetVariable(this.arrayName) as BOOSEarray;
            if (array == null)
                throw new CanvasException($"Variable '{this.arrayName}' is not an array");

            // Resolve index
            string indexStr = this.indexExpression.Trim();
            if (this.Program.VariableExists(indexStr))
                indexStr = this.Program.GetVarValue(indexStr);

            if (!int.TryParse(indexStr, out int index))
                throw new CanvasException($"Array index must be an integer: {this.indexExpression}");

            object arrayValue = array[index];

            // Get the target variable object
            Evaluation target = this.Program.GetVariable(this.targetVariable);

            
            if (target is BOOSEreal br)
            {
                br.Value = Convert.ToDouble(arrayValue);
                return;
            }

            
            if (target is Real)
            {
                this.Program.UpdateVariable(this.targetVariable, Convert.ToDouble(arrayValue));
                return;
            }

            
            if (target is BOOSEint)
            {
                this.Program.UpdateVariable(this.targetVariable, Convert.ToInt32(arrayValue));
                return;
            }

            if (arrayValue is int i)
            {
                this.Program.UpdateVariable(this.targetVariable, i);
            }
            else
            {
                this.Program.UpdateVariable(this.targetVariable, Convert.ToDouble(arrayValue));
            }
        }

        public override void CheckParameters(string[] parameter)
        {
        }
    }
}
