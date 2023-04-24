using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PNI_Procesare_Imagini
{
    public partial class Form1 : Form
    {
        private Queue<Point> points;

        public Form1()
        {
            InitializeComponent();
            points = new Queue<Point>();
        }

        private void buttonLoadPicture_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() != DialogResult.Cancel)
            {
                pictureBoxSource.Image = new Bitmap(dialog.FileName);

                chartSource.UpdateChartByPictureBox(pictureBoxSource);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (pictureBoxDestination.Image != null)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.DefaultExt = "bmp";

                if (dialog.ShowDialog() != DialogResult.Cancel)
                {
                    pictureBoxDestination.Image.Save(dialog.FileName);
                }
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            pictureBoxDestination.Image = pictureBoxSource.Image;
            chartDestination.UpdateChartByPictureBox(pictureBoxDestination);
        }

        private void pictureBoxSource_MouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBoxSource.Image == null) return;

            chartSource.UpdateChartByPictureBox(pictureBoxSource, e.Location);

            if (points.Count == 2)
            {
                points.Dequeue();
            }
            points.Enqueue(e.Location);
        }

        private void pictureBoxDestination_MouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBoxDestination.Image != null)
            {
                chartDestination.UpdateChartByPictureBox(pictureBoxDestination, e.Location);
            }
        }

        private void checkBoxStretchSource_CheckedChanged(object sender, EventArgs e)
        {
            pictureBoxSource.SizeMode = checkBoxStretchSource.Checked ? PictureBoxSizeMode.StretchImage : PictureBoxSizeMode.Zoom;
        }

        private void checkBoxStretchDestination_CheckedChanged(object sender, EventArgs e)
        {
            pictureBoxDestination.SizeMode = checkBoxStretchDestination.Checked ? PictureBoxSizeMode.StretchImage : PictureBoxSizeMode.Zoom;
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            if (pictureBoxSource.Image != null)
            {
                switch (comboBoxAction.Text)
                {
                    case "Rotate":
                        pictureBoxDestination.Image = pictureBoxSource.Image.Rotate90();
                        break;

                    case "Zoom In":
                        if (points.Count != 2)
                        {
                            MessageBox.Show(@"Not enough points!");
                            break;
                        }
                        pictureBoxDestination.Image = pictureBoxSource.ZoomIn(points.Dequeue(), points.Dequeue());
                        break;

                    case "Mozaic":
                        pictureBoxDestination.Image = pictureBoxSource.Mozaic(25);
                        break;

                    default:
                        MessageBox.Show(@"Selected item: " + comboBoxAction.Text);
                        break;
                }
                chartDestination.UpdateChartByPictureBox(pictureBoxDestination);
            }
        }

        private void comboBoxGraph_SelectedIndexChanged(object sender, EventArgs e)
        {
            int graphLength = 256;
            int[] y = new int[256];
            switch (comboBoxGraph.Text)
            {
                case "Liniar":
                    for (int i = 0; i < graphLength; i++)
                    {
                        y[i] = graphLength - i - 1;
                    }
                    break;

                case "Invers":
                    for (int i = 0; i < graphLength; i++)
                    {
                        y[i] = i;
                    }
                    break;

                case "Logaritm":
                    for (int i = 0; i < graphLength; i++)
                    {
                        y[i] = (graphLength - 1) - (int)((graphLength - 1) * Math.Log10(1 + i) / Math.Log10(graphLength));
                    }
                    break;

                default:
                    MessageBox.Show(@"Not implemented");
                    break;
            }

            var bitmap = new Bitmap(graphLength, graphLength);
            using (Graphics gfx = Graphics.FromImage(bitmap))
            {
                using (SolidBrush brush = new SolidBrush(Color.DarkGray))
                {
                    gfx.FillRectangle(brush, 0, 0, graphLength, graphLength);
                    var pen = new Pen(Color.Black);
                    pen.DashPattern = new float[] { 5, 5 };
                    gfx.DrawLine(pen, 0, graphLength, graphLength, 0);
                    var point1 = new Point(0, y[0]);
                    for (int i = 1; i < graphLength; i++)
                    {
                        var point2 = new Point(i, y[i]);

                        gfx.DrawLine(new Pen(Color.Black), point1, point2);
                        point1 = point2;
                    }
                }
            }
            pictureBoxGraph.Image = bitmap;
        }
    }
}
