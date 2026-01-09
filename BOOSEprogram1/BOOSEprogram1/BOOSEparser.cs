using BOOSE;
using System;

namespace BOOSEprogram1
{
    public class BOOSEparser : Parser
    {
        private StoredProgram storedProgram;

        public BOOSEparser(CommandFactory factory, StoredProgram program)
            : base(factory, program)
        {
            this.storedProgram = program;
        }

        public override ICommand ParseCommand(string line)
        {
            string trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                return null;
            }

            // Check if this is a standalone assignment (e.g. width = 2 * radius)
            if (IsStandaloneAssignment(trimmedLine))
            {
                BOOSEevaluation eval = new BOOSEevaluation();
                eval.Set(storedProgram, trimmedLine);
                eval.Compile();
                return eval;
            }

            return base.ParseCommand(line);
        }

        private bool IsStandaloneAssignment(string line)
        {
            if (!line.Contains("="))
                return false;

            string[] parts = line.Split('=');
            if (parts.Length < 2)
                return false;

            string varName = parts[0].Trim().Split(' ')[0];

            // Ignore declarations
            string[] keywords = { "int", "real", "boolean", "array", "poke" };
            foreach (string kw in keywords)
            {
                if (line.TrimStart().ToLowerInvariant().StartsWith(kw + " "))
                    return false;
            }

            // Only allow assignment if variable already exists
            return storedProgram.VariableExists(varName);
        }
    }
}
