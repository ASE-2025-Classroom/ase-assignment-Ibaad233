using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BOOSE;

namespace BOOSEprogram1
{
    /// <summary>
    /// Main form. Lets the user type commands and shows the drawing.
    /// </summary>
    public partial class Form1 : Form
    {
        private BOOSEcanvas canvas;

        // UI controls
        private TextBox txtProgram;
        private TextBox txtOutput;
        private Button btnRun;
        private Button btnAbout;

        /// <summary>
        /// Sets up the form and creates the canvas.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            InitializeUi();

            this.DoubleBuffered = true;
            this.Paint += Form1_Paint;

            canvas = new BOOSEcanvas(1000, 600);
            canvas.Clear();
        }

        /// <summary>
        /// Creates the text boxes and buttons on the form.
        /// </summary>
        private void InitializeUi()
        {
            // Program input textbox
            txtProgram = new TextBox();
            txtProgram.Multiline = true;
            txtProgram.ScrollBars = ScrollBars.Vertical;
            txtProgram.Font = new Font("Consolas", 10);
            txtProgram.Left = 10;
            txtProgram.Top = 10;
            txtProgram.Width = 320;
            txtProgram.Height = this.ClientSize.Height - 80;
            txtProgram.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(txtProgram);

            // Output textbox
            txtOutput = new TextBox();
            txtOutput.Multiline = true;
            txtOutput.ReadOnly = true;
            txtOutput.ScrollBars = ScrollBars.Vertical;
            txtOutput.Font = new Font("Consolas", 9);
            txtOutput.Left = 10;
            txtOutput.Top = txtProgram.Bottom + 5;
            txtOutput.Width = txtProgram.Width;
            txtOutput.Height = 60;
            txtOutput.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.Controls.Add(txtOutput);

            // Run button
            btnRun = new Button();
            btnRun.Text = "Run";
            btnRun.Left = txtProgram.Right + 20;
            btnRun.Top = this.ClientSize.Height - 40;
            btnRun.Width = 80;
            btnRun.Height = 30;
            btnRun.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRun.Click += BtnRun_Click;
            this.Controls.Add(btnRun);

            // About button
            btnAbout = new Button();
            btnAbout.Text = "About";
            btnAbout.Left = btnRun.Right + 10;
            btnAbout.Top = btnRun.Top;
            btnAbout.Width = 80;
            btnAbout.Height = 30;
            btnAbout.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAbout.Click += BtnAbout_Click;
            this.Controls.Add(btnAbout);
        }

        /// <summary>
        /// Handles the Run button click.
        /// </summary>
        private void BtnRun_Click(object sender, EventArgs e)
        {
            LogOutput("Running script...");
            RunScript(txtProgram.Text);
            this.Invalidate(); // redraws the canvas
        }

        /// <summary>
        /// Shows a message in the output box and debug window.
        /// </summary>
        private void LogOutput(string message)
        {
            if (txtOutput.TextLength > 0)
            {
                txtOutput.AppendText(Environment.NewLine);
            }

            txtOutput.AppendText(message);
            Debug.WriteLine(message);
        }

        /// <summary>
        /// Draws the canvas bitmap on the form.
        /// </summary>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (canvas == null) return;

            Bitmap bmp = (Bitmap)canvas.getBitmap();
            if (bmp == null) return;

            int left = txtProgram.Right + 20;
            int top = 10;

            e.Graphics.DrawImage(bmp, left, top);
        }

        /// <summary>
        /// Calls the BOOSE about method and shows the result.
        /// </summary>
        private void BtnAbout_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: Use Object Browser to confirm the class and method.
                // Example (you must change this to match your DLL exactly):
                //
                // var about = new BOOSE.AboutBOOSE();
                // string text = about.About();
                // LogOutput(text);
                //
                LogOutput("About: replace this line with the real BOOSE about() call once you confirm it in Object Browser.");
            }
            catch (Exception ex)
            {
                LogOutput("Error calling about(): " + ex.Message);
            }
        }

        /// <summary>
        /// Reads each line of the program and runs the drawing commands.
        /// </summary>
        private void RunScript(string src)
        {
            canvas.Clear();

            string[] lines = src.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            int lineNumber = 0;

            foreach (string raw in lines)
            {
                lineNumber++;
                string line = raw.Trim();
                if (line.Length == 0) continue;                      // blank
                if (line.StartsWith("*") || line.StartsWith("#")) continue; // comment

                try
                {
                    int space = line.IndexOf(' ');
                    string cmd = (space < 0 ? line : line.Substring(0, space)).ToLowerInvariant();
                    string args = (space < 0 ? "" : line.Substring(space + 1)).Trim();

                    switch (cmd)
                    {
                        case "moveto":
                            {
                                int x, y;
                                ParseTwo(args, out x, out y);
                                canvas.MoveTo(x, y);
                                break;
                            }

                        case "drawto":
                            {
                                int x, y;
                                ParseTwo(args, out x, out y);
                                canvas.DrawTo(x, y);
                                break;
                            }

                        case "pen":
                        case "setcolour":
                            {
                                int r, g, b;
                                ParseThree(args, out r, out g, out b);
                                canvas.SetColour(r, g, b);
                                break;
                            }

                        case "circle":
                            {
                                int r = ParseOne(args);
                                canvas.Circle(r, false);
                                break;
                            }

                        case "rect":
                            {
                                int w, h;
                                ParseTwo(args, out w, out h);
                                canvas.Rect(w, h, false);
                                break;
                            }

                        case "tri":
                            {
                                int w, h;
                                ParseTwo(args, out w, out h);
                                canvas.Tri(w, h);
                                break;
                            }

                        case "write":
                            {
                                string text = args.Trim().Trim('"');
                                canvas.WriteText(text);
                                break;
                            }

                        case "clear":
                            canvas.Clear();
                            break;

                        case "reset":
                            canvas.Reset();
                            break;

                        default:
                            LogOutput("Unknown command '" + cmd + "' on line " + lineNumber);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LogOutput("Error on line " + lineNumber + ": " + ex.Message);
                    Debug.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Converts a single piece of text into one integer.
        /// </summary>
        private static int ParseOne(string s)
        {
            int value;
            if (!int.TryParse(s.Trim(), out value))
            {
                throw new FormatException("Invalid integer value '" + s + "'.");
            }
            return value;
        }

        /// <summary>
        /// Converts text like "10,20" into two integers.
        /// </summary>
        private static void ParseTwo(string s, out int a, out int b)
        {
            string[] parts = s.Split(',');

            if (parts.Length != 2)
            {
                throw new FormatException("Expected two comma-separated values, got '" + s + "'.");
            }

            if (!int.TryParse(parts[0].Trim(), out a) ||
                !int.TryParse(parts[1].Trim(), out b))
            {
                throw new FormatException("Invalid integer pair '" + s + "'.");
            }
        }

        /// <summary>
        /// Converts text like "255,0,0" into three integers.
        /// </summary>
        private static void ParseThree(string s, out int a, out int b, out int c)
        {
            string[] parts = s.Split(',');

            if (parts.Length != 3)
            {
                throw new FormatException("Expected three comma-separated values, got '" + s + "'.");
            }

            if (!int.TryParse(parts[0].Trim(), out a) ||
                !int.TryParse(parts[1].Trim(), out b) ||
                !int.TryParse(parts[2].Trim(), out c))
            {
                throw new FormatException("Invalid integer triple '" + s + "'.");
            }
        }
    }
}

