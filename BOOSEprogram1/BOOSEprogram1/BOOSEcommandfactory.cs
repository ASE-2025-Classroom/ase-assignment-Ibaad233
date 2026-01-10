using BOOSE;
using System;

namespace BOOSEprogram1
{
    /// <summary>
    /// Factory that creates our custom BOOSE commands
    /// </summary>
    public class BOOSEcommandfactory : CommandFactory
    {
        /// <summary>
        /// Makes a command object based on the command name
        /// </summary>
        public override ICommand MakeCommand(string commandType)
        {
            commandType = commandType.ToLowerInvariant();

            if (commandType == "circle")
                return new BOOSEcircle();
            
            if (commandType == "moveto")
                return new BOOSEmoveto();
            
            if (commandType == "rect")
                return new BOOSErect();
            
            if (commandType == "int")
                return new BOOSEint();
            
            if (commandType == "real")
                return new BOOSEreal();
            
            if (commandType == "array")
                return new BOOSEarray();
            
            if (commandType == "poke")
                return new BOOSEpoke();
            
            if (commandType == "peek")
                return new BOOSEpeek();
            
            if (commandType == "while")
                return new BOOSEwhile();
            
            if (commandType == "for")
                return new BOOSEfor();
            
            if (commandType == "if")
                return new BOOSEif();
            
            if (commandType == "method")
                return new BOOSEmethod();
            
            if (commandType == "call")
                return new BOOSEcall();
            
            if (commandType == "write")
                return new BOOSEwrite();

            if (commandType == "end")
                return new BOOSEend();

            if (commandType == "else")
                return new BOOSEelse();

            if (commandType == "pen")
                return new BOOSEpen();

            return base.MakeCommand(commandType);
        }
    }
}