using BOOSE;
using System.Diagnostics;

namespace BOOSEprogram1
{
    public partial class Form1 : Form
    {
        BOOSEcanvas canvas;
        CommandFactory Factory;
        StoredProgram Program;
        IParser Parser;

        public Form1()
        {
            InitializeComponent();
            Debug.WriteLine(AboutBOOSE.about());
            canvas = new BOOSEcanvas(640, 400);
            canvas.Circle(100, true);
            Factory = new CommandFactory();
            Parser = new Parser(Factory, Program);
            Program = new StoredProgram(canvas);
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Bitmap b = (Bitmap)canvas.getBitmap();
            g.DrawImage(b, 0, 0);
        }
    }
}


