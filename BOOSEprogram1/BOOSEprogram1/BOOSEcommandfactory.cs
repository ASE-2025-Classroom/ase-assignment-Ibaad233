using BOOSE;
using System;

namespace BOOSEprogram1
{
    /// <summary>
    /// Factory that creates our custom BOOSE commands.
    /// </summary>
    internal class BOOSEcommandfactory : CommandFactory
    {
        /// <summary>
        /// Makes a command object based on the command name.
        /// </summary>
        public override ICommand MakeCommand(string commandType)
        {
            // Convert the name to lowercase so it matches correctly
            commandType = commandType.ToLowerInvariant();

            // Return the correct custom command
            if (commandType == "circle")
                return new BOOSEcircle();

            if (commandType == "moveto")
                return new BOOSEmoveto();

            if (commandType == "rect")
                return new BOOSErect();

            // If the command isn't one of ours, let the base factory handle it
            return base.MakeCommand(commandType);
        }
    }
}
