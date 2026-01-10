using BOOSE;
using System;
using System.Collections.Generic;

namespace BOOSEprogram1
{
    public sealed class ProgramManager
    {
        private static ProgramManager instance = null;
        private static readonly object padlock = new object();

        private StoredProgram currentProgram;
        private BOOSEcanvas currentCanvas;
        private BOOSEprogram1.BOOSEparser currentParser;
        private BOOSEprogram1.BOOSEcommandfactory currentFactory;

        private ProgramManager() { }

        public static ProgramManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                            instance = new ProgramManager();
                    }
                }
                return instance;
            }
        }

        public StoredProgram CurrentProgram => currentProgram;
        public BOOSEcanvas CurrentCanvas => currentCanvas;

        public void InitializeProgram(int width, int height)
        {
            currentCanvas = new BOOSEcanvas(width, height);
            BOOSEwrite.SetCanvas(currentCanvas);

            // FORCE our custom factory
            currentFactory = new BOOSEprogram1.BOOSEcommandfactory();

            // StoredProgram must be created with our canvas
            currentProgram = new StoredProgram(currentCanvas);

            // FORCE our custom parser
            currentParser = new BOOSEprogram1.BOOSEparser(currentFactory, currentProgram);

            BOOSEmethod.ClearMethods();
        }

        public void Reset()
        {
            currentProgram?.Clear();

            if (currentCanvas != null)
            {
                currentCanvas.Clear();
                currentCanvas.Reset();
            }

            BOOSEmethod.ClearMethods();

            // Recreate parser every run (prevents any fallback)
            if (currentFactory != null && currentProgram != null)
                currentParser = new BOOSEprogram1.BOOSEparser(currentFactory, currentProgram);
        }

        public void LoadCode(string code)
        {
            if (currentParser == null)
                throw new InvalidOperationException("Parser not initialized. Call InitializeProgram first.");

            // DO NOT clear canvas here (Reset already does that)
            string[] lines = code.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string raw in lines)
            {
                string line = raw.Trim();

                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                    continue;

                // FORCE our parser
                ICommand cmd = currentParser.ParseCommand(line);

                // Parser returns null for:
                // - lines inside blocks
                // - "else"
                // and returns the finished block command at "end if/endfor/endwhile/end method"
                if (cmd != null)
                {
                    cmd.Execute();
                }
            }
        }

        public object GetCanvasBitmap()
        {
            if (currentCanvas == null)
                throw new InvalidOperationException("Canvas not initialized");

            return currentCanvas.getBitmap();
        }
    }
}
