using BOOSE;
using System;
using System.Collections.Generic;

namespace BOOSEprogram1
{
    /// <summary>
    /// Method call command for BOOSE language
    /// Supports:
    ///   call myMethod 10 5
    ///   call myMethod(10,5)
    /// </summary>
    public class BOOSEcall : Command
    {
        private string methodName;
        private readonly List<string> arguments = new List<string>();

        public BOOSEcall() : base() { }

        public override void Compile()
        {
            arguments.Clear();

            string full = (this.ParameterList ?? "").Trim();
            if (string.IsNullOrWhiteSpace(full))
                throw new CanvasException("Call requires a method name");

            // Parentheses style: name(a,b)
            int parenStart = full.IndexOf('(');
            int parenEnd = full.LastIndexOf(')');

            if (parenStart >= 0 && parenEnd > parenStart)
            {
                methodName = full.Substring(0, parenStart).Trim();

                string inside = full.Substring(parenStart + 1, parenEnd - parenStart - 1).Trim();
                if (!string.IsNullOrWhiteSpace(inside))
                {
                    foreach (string a in inside.Split(','))
                        arguments.Add(a.Trim());
                }
                return;
            }

            // Space style: name 10 5
            // (also allows commas accidentally: "10, 5")
            string[] parts = full.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            methodName = parts[0].Trim();

            for (int i = 1; i < parts.Length; i++)
            {
                string arg = parts[i].Trim().TrimEnd(',');
                if (!string.IsNullOrWhiteSpace(arg))
                    arguments.Add(arg);
            }

            if (string.IsNullOrWhiteSpace(methodName))
                throw new CanvasException("Call requires a method name");
        }

        public override void Execute()
        {
            BOOSEmethod method = BOOSEmethod.GetMethod(methodName);
            if (method == null)
                throw new CanvasException($"Method '{methodName}' is not defined");

            method.Call(arguments);
        }

        public override void CheckParameters(string[] parameter)
        {
            // no validation
        }
    }
}
