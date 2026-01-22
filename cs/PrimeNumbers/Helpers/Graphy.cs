using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace PrimeNumbers.Helpers {
    public class Graphy {
        public Form Form { get; set; }
        public Graphics Graphics { get; set; }

        private PictureBox _drawingArea = new PictureBox();

        public Graphy() {
            Form = new Form();
            Form.Show();

            Form.BackColor = Color.White;
            Form.FormBorderStyle = FormBorderStyle.None;
            Form.ShowIcon = false;
            Form.ShowInTaskbar = false;

            Form.Bounds = new Rectangle(0, 0, 720, 480);

            Form.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens) {
                if (scrn.Bounds.Contains(Form.Location)) {
                    Form.Location = new Point(scrn.Bounds.Right - Form.Width, scrn.Bounds.Top);
                    break;
                }
            }

            Application.EnableVisualStyles();
            Application.Run(Form);

            DrawString("HERE");

            //Form form2 = new Form();
            ////form2.Show();

            //var graphics = form2.CreateGraphics();
            //var rectangle = new System.Drawing.Rectangle(100, 100, 200, 200);

            //graphics.DrawEllipse(System.Drawing.Pens.Black, rectangle);
            //graphics.DrawRectangle(System.Drawing.Pens.Red, rectangle);

        }

        private void Form_Load(object sender, EventArgs e) {
            _drawingArea.Dock = DockStyle.Fill;
            _drawingArea.BackColor = Color.White;
            //_drawingArea.Paint += new PaintEventHandler(DrawingArea_Paint);

            // Add the PictureBox control to the Form.
            Form.Controls.Add(_drawingArea);
        }

        public void DrawString(string value) {
            var x = 10f;
            var y = 10f;

            var drawFormat = new StringFormat();

            using (var formGraphics = Form.CreateGraphics()) {
                using (var drawFont = new Font("Arial", 16)) {
                    using (var drawBrush = new SolidBrush(Color.Yellow)) {
                        formGraphics.DrawString(value, drawFont, drawBrush, x, y, drawFormat);
                    }
                }
            };

            //_drawingArea.Refresh();
        }

        private void Form_Paint(object sender, PaintEventArgs e) {
            Graphics = e.Graphics;

            //var rectangle = new System.Drawing.Rectangle(10, 10, 100, 100);

            //Graphics.DrawEllipse(Pens.Black, rectangle);
            //Graphics.DrawRectangle(Pens.Red, rectangle);
        }

        private void DrawingArea_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
            // Create a local version of the graphics object for the PictureBox.
            Graphics g = e.Graphics;

            // Draw a string on the PictureBox.
            //g.DrawString("This is a diagonal line drawn on the control", fnt, System.Drawing.Brushes.Blue, new Point(30, 30));

            // Draw a line in the PictureBox.
            g.DrawLine(System.Drawing.Pens.Red, _drawingArea.Left, _drawingArea.Top, _drawingArea.Right, _drawingArea.Bottom);
        }
    }
}
