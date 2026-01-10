using System;
using System.Drawing;
using System.Windows.Forms;
using BOOSE;

namespace BOOSEprogram1
{
    public partial class Form1 : Form
    {
        private TextBox programTextBox;
        private PictureBox outputPictureBox;
        private RichTextBox debugOutputTextBox;
        private Button runButton;
        private Button clearButton;

        public Form1()
        {
            InitializeUI();
            InitializeProgramManager();
        }

        private void InitializeUI()
        {
            this.Text = "BOOSE Interpreter";
            this.Size = new Size(1200, 800);

            SplitContainer mainSplit = new SplitContainer();
            mainSplit.Dock = DockStyle.Fill;
            mainSplit.Orientation = Orientation.Horizontal;
            mainSplit.SplitterDistance = 400;

            SplitContainer topSplit = new SplitContainer();
            topSplit.Dock = DockStyle.Fill;
            topSplit.Orientation = Orientation.Vertical;
            topSplit.SplitterDistance = 500;

            programTextBox = new TextBox();
            programTextBox.Multiline = true;
            programTextBox.Dock = DockStyle.Fill;
            programTextBox.Font = new Font("Consolas", 10);
            programTextBox.ScrollBars = ScrollBars.Both;
            programTextBox.Text = "int x = 100\nint y = 100\nmoveto x y\ncircle 50";

            outputPictureBox = new PictureBox();
            outputPictureBox.Dock = DockStyle.Fill;
            outputPictureBox.BorderStyle = BorderStyle.FixedSingle;
            outputPictureBox.BackColor = Color.Gray;
            outputPictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            debugOutputTextBox = new RichTextBox();
            debugOutputTextBox.Dock = DockStyle.Fill;
            debugOutputTextBox.Font = new Font("Consolas", 9);
            debugOutputTextBox.ReadOnly = true;
            debugOutputTextBox.BackColor = Color.Black;
            debugOutputTextBox.ForeColor = Color.LightGreen;

            runButton = new Button();
            runButton.Text = "Run";
            runButton.Location = new Point(10, 10);
            runButton.Size = new Size(80, 30);
            runButton.Click += RunButton_Click;

            clearButton = new Button();
            clearButton.Text = "Clear";
            clearButton.Location = new Point(100, 10);
            clearButton.Size = new Size(80, 30);
            clearButton.Click += ClearButton_Click;

            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.Height = 50;
            buttonPanel.Controls.Add(runButton);
            buttonPanel.Controls.Add(clearButton);

            Panel bottomPanel = new Panel();
            bottomPanel.Dock = DockStyle.Fill;
            bottomPanel.Controls.Add(debugOutputTextBox);
            bottomPanel.Controls.Add(buttonPanel);

            topSplit.Panel1.Controls.Add(programTextBox);
            topSplit.Panel2.Controls.Add(outputPictureBox);
            mainSplit.Panel1.Controls.Add(topSplit);
            mainSplit.Panel2.Controls.Add(bottomPanel);
            this.Controls.Add(mainSplit);
        }

        private void InitializeProgramManager()
        {
            try
            {
                ProgramManager.Instance.InitializeProgram(640, 480);
                WriteDebugOutput("Ready");
            }
            catch (Exception ex)
            {
                WriteDebugOutput("Error: " + ex.Message, true);
            }
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            ProgramManager.Instance.Reset();
            WriteDebugOutput("Running...");

            try
            {
                ProgramManager.Instance.LoadCode(programTextBox.Text);

                // Always update image after running
                outputPictureBox.Image = (Bitmap)ProgramManager.Instance.GetCanvasBitmap();

                WriteDebugOutput("Success");
            }
            catch (Exception ex)
            {
                // ✅ IMPORTANT: still update the image even on error
                outputPictureBox.Image = (Bitmap)ProgramManager.Instance.GetCanvasBitmap();

                WriteDebugOutput("Error: " + ex.Message, true);
            }
        }


        private void ClearButton_Click(object sender, EventArgs e)
        {
            try
            {
                ProgramManager.Instance.CurrentCanvas.Clear();
                outputPictureBox.Image = (Bitmap)ProgramManager.Instance.GetCanvasBitmap();
            }
            catch (Exception ex)
            {
                WriteDebugOutput("Error: " + ex.Message, true);
            }
        }

        private void WriteDebugOutput(string message, bool isError = false)
        {
            debugOutputTextBox.SelectionColor = isError ? Color.Red : Color.LightGreen;
            debugOutputTextBox.AppendText(DateTime.Now.ToString("HH:mm:ss") + " " + message + "\n");
        }
    }
}