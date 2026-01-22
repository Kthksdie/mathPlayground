using PrimeNumbers.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PrimeNumbers.Helpers.ColorSchema;
using static PrimeNumbers.Helpers.Graph3D;

namespace PrimeNumbers.Helpers {
    public class GraphForm : Form {
        private static Graph3D Graph3D;

        //public static Brush WhiteBrush = Brushes.White;

        public GraphForm() {
            Task.Run(() => {
                BackColor = Color.White;
                FormBorderStyle = FormBorderStyle.Sizable;
                ShowIcon = false;
                ShowInTaskbar = false;

                Bounds = new Rectangle(0, 0, 720, 480);

                StartPosition = FormStartPosition.Manual;
                foreach (var scrn in Screen.AllScreens) {
                    if (scrn.Bounds.Contains(Location)) {
                        Location = new Point(scrn.Bounds.Right - Width, scrn.Bounds.Top);
                        break;
                    }
                }

                Initialize();

                Application.EnableVisualStyles();
                Application.Run(this);
            });
        }

        private void Initialize() {
            Graph3D = new Graph3D() {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                ColorScheme = GetSchema(eSchema.Rainbow1)
            };

            Controls.Add(Graph3D);
        }

        public void SetScatterPoints(List<cScatter> scatterPoints, eNormalize e_Normalize) {
            Graph3D.PerformInvoke(() => {
                Graph3D.SetScatterPoints(scatterPoints, e_Normalize);
            });
        }
    }
}
