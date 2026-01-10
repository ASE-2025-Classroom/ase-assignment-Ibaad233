using BOOSE;
using System;
using System.Collections.Generic;

namespace BOOSEprogram1
{
    /// <summary>
    /// Custom parser that handles BOOSE language constructs
    /// </summary>
    public class BOOSEparser : Parser
    {
        private readonly StoredProgram storedProgram;
        private readonly CommandFactory commandFactory;
        private readonly Stack<ControlStructure> controlStack;

        private class ControlStructure
        {
            public string Type { get; set; }               // "if", "else", "while", "for", "method"
            public ICommand Command { get; }
            public List<ICommand> Commands { get; }        // body/then commands
            public List<ICommand> ElseCommands { get; }    // else commands

            public ControlStructure(string type, ICommand command)
            {
                Type = type;
                Command = command;
                Commands = new List<ICommand>();
                ElseCommands = new List<ICommand>();
            }
        }

        public BOOSEparser(CommandFactory factory, StoredProgram program)
            : base(factory, program)
        {
            storedProgram = program;
            commandFactory = factory;
            controlStack = new Stack<ControlStructure>();
        }

        private bool InsideControlBlock => controlStack.Count > 0;

        public override ICommand ParseCommand(string line)
        {
            System.Diagnostics.Debug.WriteLine("CUSTOM PARSER HIT: " + line);

            string trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine))
                return null;

            // -----------------------
            // CONTROL TERMINATORS
            // -----------------------

            // ELSE
            if (trimmedLine.Equals("else", StringComparison.OrdinalIgnoreCase))
            {
                if (controlStack.Count == 0)
                    throw new CanvasException("else without matching if");

                var current = controlStack.Peek();
                if (current.Type != "if" && current.Type != "else")
                    throw new CanvasException("else without matching if");

                current.Type = "else";
                return null;
            }

            // END IF / ENDIF
            if (trimmedLine.Equals("endif", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.Equals("end if", StringComparison.OrdinalIgnoreCase))
            {
                if (controlStack.Count == 0 ||
                    (controlStack.Peek().Type != "if" && controlStack.Peek().Type != "else"))
                    throw new CanvasException("end if without matching if");

                var structure = controlStack.Pop();
                var ifCmd = (BOOSEif)structure.Command;

                ifCmd.SetThenCommands(structure.Commands);
                if (structure.ElseCommands.Count > 0)
                    ifCmd.SetElseCommands(structure.ElseCommands);

                AddCommandToCurrentScope(ifCmd);
                return InsideControlBlock ? null : ifCmd;
            }

            // END WHILE
            if (trimmedLine.Equals("endwhile", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.Equals("end while", StringComparison.OrdinalIgnoreCase))
            {
                if (controlStack.Count == 0 || controlStack.Peek().Type != "while")
                    throw new CanvasException("end while without matching while");

                var structure = controlStack.Pop();
                ((BOOSEwhile)structure.Command).SetLoopCommands(structure.Commands);

                AddCommandToCurrentScope(structure.Command);
                return InsideControlBlock ? null : structure.Command;
            }

            // END FOR
            if (trimmedLine.Equals("endfor", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.Equals("end for", StringComparison.OrdinalIgnoreCase))
            {
                if (controlStack.Count == 0 || controlStack.Peek().Type != "for")
                    throw new CanvasException("end for without matching for");

                var structure = controlStack.Pop();
                ((BOOSEfor)structure.Command).SetLoopCommands(structure.Commands);

                AddCommandToCurrentScope(structure.Command);
                return InsideControlBlock ? null : structure.Command;
            }

            // END METHOD
            if (trimmedLine.Equals("endmethod", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.Equals("end method", StringComparison.OrdinalIgnoreCase))
            {
                if (controlStack.Count == 0 || controlStack.Peek().Type != "method")
                    throw new CanvasException("end method without matching method");

                var structure = controlStack.Pop();
                ((BOOSEmethod)structure.Command).SetMethodCommands(structure.Commands);

                AddCommandToCurrentScope(structure.Command);
                return InsideControlBlock ? null : structure.Command;
            }

            // -----------------------
            // CONTROL STARTERS
            // -----------------------

            // WHILE
            if (trimmedLine.StartsWith("while ", StringComparison.OrdinalIgnoreCase))
            {
                var whileCmd = new BOOSEwhile();
                whileCmd.Set(storedProgram, trimmedLine.Substring(6).Trim());
                whileCmd.Compile();

                controlStack.Push(new ControlStructure("while", whileCmd));
                return null;
            }

            // FOR
            if (trimmedLine.StartsWith("for ", StringComparison.OrdinalIgnoreCase))
            {
                var forCmd = new BOOSEfor();
                forCmd.Set(storedProgram, trimmedLine.Substring(4).Trim());
                forCmd.Compile();

                controlStack.Push(new ControlStructure("for", forCmd));
                return null;
            }

            // IF
            if (trimmedLine.StartsWith("if ", StringComparison.OrdinalIgnoreCase))
            {
                var ifCmd = new BOOSEif();
                ifCmd.Set(storedProgram, trimmedLine.Substring(3).Trim());
                ifCmd.Compile();

                controlStack.Push(new ControlStructure("if", ifCmd));
                return null;
            }

            // METHOD  ✅ this is the one you need
            if (trimmedLine.StartsWith("method ", StringComparison.OrdinalIgnoreCase))
            {
                var methodCmd = new BOOSEmethod();
                methodCmd.Set(storedProgram, trimmedLine.Substring(7).Trim());
                methodCmd.Compile();

                controlStack.Push(new ControlStructure("method", methodCmd));
                return null;
            }

            // -----------------------
            // NORMAL COMMANDS
            // -----------------------

            ICommand cmd = ParseNonBlockCommand(trimmedLine);
            if (cmd == null) return null;

            AddCommandToCurrentScope(cmd);

            // don’t return nested commands
            return InsideControlBlock ? null : cmd;
        }

        private ICommand ParseNonBlockCommand(string trimmedLine)
        {
            // INT
            if (trimmedLine.StartsWith("int ", StringComparison.OrdinalIgnoreCase))
            {
                var v = new BOOSEint();
                v.Set(storedProgram, trimmedLine.Substring(4).Trim());
                v.Compile();
                return v;
            }

            // REAL
            if (trimmedLine.StartsWith("real ", StringComparison.OrdinalIgnoreCase))
            {
                var v = new BOOSEreal();
                v.Set(storedProgram, trimmedLine.Substring(5).Trim());
                v.Compile();
                return v;
            }

            // ARRAY
            if (trimmedLine.StartsWith("array ", StringComparison.OrdinalIgnoreCase))
            {
                var v = new BOOSEarray();
                v.Set(storedProgram, trimmedLine.Substring(6).Trim());
                v.Compile();
                return v;
            }

            // POKE
            if (trimmedLine.StartsWith("poke ", StringComparison.OrdinalIgnoreCase))
            {
                var c = new BOOSEpoke();
                c.Set(storedProgram, trimmedLine.Substring(5).Trim());
                c.Compile();
                return c;
            }

            // PEEK
            if (trimmedLine.StartsWith("peek ", StringComparison.OrdinalIgnoreCase))
            {
                var c = new BOOSEpeek();
                c.Set(storedProgram, trimmedLine.Substring(5).Trim());
                c.Compile();
                return c;
            }

            // CALL
            if (trimmedLine.StartsWith("call ", StringComparison.OrdinalIgnoreCase))
            {
                var c = new BOOSEcall();
                c.Set(storedProgram, trimmedLine.Substring(5).Trim());
                c.Compile();
                return c;
            }

            // WRITE
            if (trimmedLine.StartsWith("write ", StringComparison.OrdinalIgnoreCase))
            {
                var c = new BOOSEwrite();
                c.Set(storedProgram, trimmedLine.Substring(6).Trim());
                c.Compile();
                return c;
            }

            // Standalone assignment
            if (IsStandaloneAssignment(trimmedLine))
            {
                var e = new BOOSEevaluation();
                e.Set(storedProgram, trimmedLine);
                e.Compile();
                return e;
            }

            // Factory command (circle, moveto, rect, pen, etc.)
            string[] parts = trimmedLine.Split(new[] { ' ' }, 2);
            string cmdName = parts[0].ToLowerInvariant();
            string parms = parts.Length > 1 ? parts[1] : "";

            ICommand cmd = commandFactory.MakeCommand(cmdName);
            if (cmd is Command ccmd)
            {
                ccmd.Set(storedProgram, parms);
                ccmd.Compile();
                return cmd;
            }

            return null;
        }

        private void AddCommandToCurrentScope(ICommand command)
        {
            if (command == null) return;

            if (controlStack.Count > 0)
            {
                var current = controlStack.Peek();
                if (current.Type == "else")
                    current.ElseCommands.Add(command);
                else
                    current.Commands.Add(command);
            }
        }

        private bool IsStandaloneAssignment(string line)
        {
            if (!line.Contains("="))
                return false;

            string[] parts = line.Split('=');
            if (parts.Length < 2)
                return false;

            string varName = parts[0].Trim().Split(' ')[0];

            string[] keywords = {
                "int", "real", "boolean", "array", "poke", "peek",
                "method", "call", "if", "while", "for", "write"
            };

            foreach (string kw in keywords)
            {
                if (line.TrimStart().StartsWith(kw + " ", StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return storedProgram.VariableExists(varName);
        }
    }
}
