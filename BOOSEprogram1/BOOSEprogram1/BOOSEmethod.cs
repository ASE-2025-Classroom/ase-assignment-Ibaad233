using BOOSE;
using System;
using System.Collections.Generic;

namespace BOOSEprogram1
{
    /// <summary>
    /// Method definition for BOOSE language.
    /// Supports syntax:
    ///   method int mulMethod int one, int two
    ///   ...
    ///   end method
    /// </summary>
    public class BOOSEmethod : Command
    {
        private string methodName;
        private string returnType; // "int" or "real"
        private readonly List<string> parameters = new List<string>();
        private List<ICommand> methodCommands = new List<ICommand>();

        private static readonly Dictionary<string, BOOSEmethod> definedMethods
            = new Dictionary<string, BOOSEmethod>(StringComparer.OrdinalIgnoreCase);

        public BOOSEmethod() : base() { }

        public void SetMethodCommands(List<ICommand> commands)
        {
            methodCommands = commands ?? new List<ICommand>();
        }

        public override void Compile()
        {
            parameters.Clear();

            string header = (this.ParameterList ?? "").Trim();
            if (string.IsNullOrWhiteSpace(header))
                throw new CanvasException("Method requires a signature");

            // We expect header like:
            // "int mulMethod int one, int two"
            // returnType methodName paramType paramName , paramType paramName ...
            var tokens = Tokenise(header);

            if (tokens.Count < 2)
                throw new CanvasException("Invalid method signature");

            returnType = tokens[0].ToLowerInvariant();  // int/real
            methodName = tokens[1];

            if (string.IsNullOrWhiteSpace(methodName))
                throw new CanvasException("Method requires a name");

            // Parse parameters: pairs (type, name)
            // tokens from index 2 onwards: int one int two ...
            for (int i = 2; i + 1 < tokens.Count; i += 2)
            {
                string pType = tokens[i].ToLowerInvariant();
                string pName = tokens[i + 1];

                // store just the param name (you already treat params as vars)
                if (!string.IsNullOrWhiteSpace(pName))
                    parameters.Add(pName);
            }

            // ✅ IMPORTANT: declare the return variable (same name as method)
            // so "mulMethod = one * two" compiles inside the method body.
            if (!this.Program.VariableExists(methodName))
            {
                if (returnType == "real")
                {
                    var rv = new BOOSEreal();
                    rv.Set(this.Program, $"{methodName} = 0.0");
                    rv.Compile();
                }
                else
                {
                    var rv = new BOOSEint();
                    rv.Set(this.Program, $"{methodName} = 0");
                    rv.Compile();
                }
            }

            definedMethods[methodName] = this;
        }

        public override void Execute()
        {
            // Method definition line does nothing at runtime
        }

        public void Call(List<string> args)
        {
            if (args == null) args = new List<string>();

            if (args.Count != parameters.Count)
                throw new CanvasException($"Method '{methodName}' expects {parameters.Count} arguments");

            // Assign parameters into variables in the stored program
            for (int i = 0; i < parameters.Count; i++)
            {
                string paramName = parameters[i];
                string arg = args[i];

                // If arg is a variable name, use its value
                if (this.Program.VariableExists(arg))
                    arg = this.Program.GetVarValue(arg);

                // Create the param var if it doesn't exist
                if (!this.Program.VariableExists(paramName))
                {
                    // default to int param vars (matches your sample)
                    var pv = new BOOSEint();
                    pv.Set(this.Program, $"{paramName} = 0");
                    pv.Compile();
                }

                // Update param value (int or real)
                if (int.TryParse(arg, out int intVal))
                {
                    this.Program.UpdateVariable(paramName, intVal);
                }
                else if (double.TryParse(arg, out double dblVal))
                {
                    // if param is actually a BOOSEreal, set it directly
                    var v = this.Program.GetVariable(paramName);
                    if (v is BOOSEreal br) br.Value = dblVal;
                    else this.Program.UpdateVariable(paramName, dblVal);
                }
                else
                {
                    throw new CanvasException($"Invalid argument '{args[i]}' for parameter '{paramName}'");
                }
            }

            // Execute method body
            foreach (ICommand cmd in methodCommands)
                cmd.Execute();
        }

        public static BOOSEmethod GetMethod(string name)
        {
            if (name == null) return null;
            return definedMethods.TryGetValue(name.Trim(), out var m) ? m : null;
        }

        public static void ClearMethods()
        {
            definedMethods.Clear();
        }

        public override void CheckParameters(string[] parameter)
        {
            // no validation
        }

        /// <summary>
        /// Tokenises the method header, removing commas.
        /// Example: "int mulMethod int one, int two"
        /// -> ["int","mulMethod","int","one","int","two"]
        /// </summary>
        private List<string> Tokenise(string header)
        {
            var list = new List<string>();
            foreach (string raw in header.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries))
                list.Add(raw.Trim());
            return list;
        }
    }
}
