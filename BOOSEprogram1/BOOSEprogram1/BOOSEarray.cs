using BOOSE;
using System;
using System.Collections.Generic;

namespace BOOSEprogram1
{
    /// <summary>
    /// Array variable for BOOSE language - supports "array type name size" syntax
    /// </summary>
    public class BOOSEarray : Evaluation, ICommand
    {
        private Dictionary<int, object> arrayValues;
        private int arraySize;

        /// <summary>
        /// Gets the size of the array
        /// </summary>
        public int Size
        {
            get { return arraySize; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BOOSEarray() : base()
        {
            arrayValues = new Dictionary<int, object>();
        }

        /// <summary>
        /// Gets or sets a value at the specified index
        /// </summary>
        public object this[int index]
        {
            get
            {
                if (index < 0 || index >= arraySize)
                {
                    throw new CanvasException($"Array index {index} out of bounds");
                }
                return arrayValues.ContainsKey(index) ? arrayValues[index] : 0;
            }
            set
            {
                if (index < 0 || index >= arraySize)
                {
                    throw new CanvasException($"Array index {index} out of bounds");
                }
                arrayValues[index] = value;
            }
        }

        /// <summary>
        /// Compiles the array declaration
        /// Supports both "array name[size]" and "array type name size" syntax
        /// </summary>
        public override void Compile()
        {
            string fullExpression = this.ParameterList.Trim();

            if (string.IsNullOrWhiteSpace(fullExpression))
            {
                throw new CanvasException("Array declaration requires parameters");
            }

            // Check if it uses bracket syntax: array name[size]
            if (fullExpression.Contains("[") && fullExpression.Contains("]"))
            {
                int bracketStart = fullExpression.IndexOf('[');
                int bracketEnd = fullExpression.IndexOf(']');

                this.varName = fullExpression.Substring(0, bracketStart).Trim();
                string sizeStr = fullExpression.Substring(bracketStart + 1, bracketEnd - bracketStart - 1).Trim();

                if (this.Program.VariableExists(sizeStr))
                {
                    sizeStr = this.Program.GetVarValue(sizeStr);
                }

                if (!int.TryParse(sizeStr, out arraySize) || arraySize <= 0)
                {
                    throw new CanvasException($"Array size must be a positive integer");
                }
            }
            else
            {
                // Parse "array type name size" syntax
                // Example: "int nums 10" or "real prices 10"
                string[] parts = fullExpression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 3)
                {
                    // Format: type name size
                    string type = parts[0].Trim();      // int or real (ignored)
                    this.varName = parts[1].Trim();     // array name
                    string sizeStr = parts[2].Trim();   // size

                    if (this.Program.VariableExists(sizeStr))
                    {
                        sizeStr = this.Program.GetVarValue(sizeStr);
                    }

                    if (!int.TryParse(sizeStr, out arraySize) || arraySize <= 0)
                    {
                        throw new CanvasException($"Array size must be a positive integer");
                    }
                }
                else if (parts.Length == 2)
                {
                    // Format: name size
                    this.varName = parts[0].Trim();
                    string sizeStr = parts[1].Trim();

                    if (this.Program.VariableExists(sizeStr))
                    {
                        sizeStr = this.Program.GetVarValue(sizeStr);
                    }

                    if (!int.TryParse(sizeStr, out arraySize) || arraySize <= 0)
                    {
                        throw new CanvasException($"Array size must be a positive integer");
                    }
                }
                else
                {
                    throw new CanvasException("Array declaration format: array [type] name size or array name[size]");
                }
            }

            this.Program.AddVariable(this);
        }

        /// <summary>
        /// Executes the array initialization
        /// </summary>
        public override void Execute()
        {
            for (int i = 0; i < arraySize; i++)
            {
                arrayValues[i] = 0;
            }
        }

        /// <summary>
        /// Check parameters - no validation
        /// </summary>
        public override void CheckParameters(string[] parameter)
        {
            // No validation
        }
    }
}