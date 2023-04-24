using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PNI_Procesare_Imagini
{
    internal static class MyExtensions
    {
        public static void UpdateChartByPictureBox(this Chart chart, PictureBox pictureBox)
        {
            int[] histoRed = new int[256];
            int[] histoGreen = new int[256];
            int[] histoBlue = new int[256];

            for (int x = 0; x < pictureBox.Image.Width; x++)
            {
                for (int y = 0; y < pictureBox.Image.Height; y++)
                {
                    histoRed[((Bitmap)pictureBox.Image).GetPixel(x, y).R]++;
                    histoGreen[((Bitmap)pictureBox.Image).GetPixel(x, y).G]++;
                    histoBlue[((Bitmap)pictureBox.Image).GetPixel(x, y).B]++;
                }
            }

            chart.Series["Red"].Points.Clear();
            chart.Series["Green"].Points.Clear();
            chart.Series["Blue"].Points.Clear();

            for (int i = 0; i < 256; i++)
            {
                chart.Series["Red"].Points.Add(histoRed[i]);
                chart.Series["Green"].Points.Add(histoGreen[i]);
                chart.Series["Blue"].Points.Add(histoBlue[i]);
            }
        }

        public static void UpdateChartByPictureBox(this Chart chart, PictureBox pictureBox, Point point)
        {
            int[] histoRed = new int[256];
            int[] histoGreen = new int[256];
            int[] histoBlue = new int[256];

            point.Y = point.Y * pictureBox.Image.Height / pictureBox.Height;

            for (int x = 0; x < pictureBox.Image.Width; x++)
            {
                histoRed[((Bitmap)pictureBox.Image).GetPixel(x, point.Y).R]++;
                histoGreen[((Bitmap)pictureBox.Image).GetPixel(x, point.Y).G]++;
                histoBlue[((Bitmap)pictureBox.Image).GetPixel(x, point.Y).B]++;
            }

            chart.Series["Red"].Points.Clear();
            chart.Series["Green"].Points.Clear();
            chart.Series["Blue"].Points.Clear();

            for (int i = 0; i < 256; i++)
            {
                chart.Series["Red"].Points.Add(histoRed[i]);
                chart.Series["Green"].Points.Add(histoGreen[i]);
                chart.Series["Blue"].Points.Add(histoBlue[i]);
            }
        }

        public static Image Rotate90(this Image image)
        {
            var sourceImage = new Bitmap(image);
            var destinationImage = new Bitmap(sourceImage.Height, sourceImage.Width, sourceImage.PixelFormat);
            for (int x = 0; x < sourceImage.Width; x++)
            {
                for (int y = 0; y < sourceImage.Height; y++)
                {
                    destinationImage.SetPixel(y, sourceImage.Width - x - 1, sourceImage.GetPixel(x, y));
                }
            }
            return destinationImage;
        }

        public static Image ZoomIn(this PictureBox pictureBox, Point point1, Point point2)
        {
            if (point1.X > point2.X)
            {
                (point1.X, point2.X) = (point2.X, point1.X);
            }

            if (point1.Y > point2.Y)
            {
                (point1.Y, point2.Y) = (point2.Y, point1.Y);
            }

            point1.X = point1.X * pictureBox.Image.Width / pictureBox.Width;
            point2.X = point2.X * pictureBox.Image.Width / pictureBox.Width;
            point1.Y = point1.Y * pictureBox.Image.Height / pictureBox.Height;
            point2.Y = point2.Y * pictureBox.Image.Height / pictureBox.Height;

            var sourceBitmap = (Bitmap)pictureBox.Image;
            var bitmap = new Bitmap(point2.X - point1.X, point2.Y - point1.Y, sourceBitmap.PixelFormat);

            for (int x = point1.X, newX = 0; x < point2.X; x++, newX++)
            {
                for (int y = point1.Y, newY = 0; y < point2.Y; y++, newY++)
                {
                    bitmap.SetPixel(newX, newY, sourceBitmap.GetPixel(x, y));
                }
            }
            return bitmap;
        }

        public static Image Mozaic(this PictureBox pictureBox, int size)
        {
            var sourceBitmap = (Bitmap)pictureBox.Image;
            var bitmap = new Bitmap(sourceBitmap);

            for (int x = 0; x < sourceBitmap.Width; x += size)
            {
                for (int y = 0; y < sourceBitmap.Height; y += size)
                {
                    int R = 0;
                    int G = 0;
                    int B = 0;
                    int counter = 0;
                    for(int smallX = 0; smallX < size && x + smallX < sourceBitmap.Width ; smallX++)
                    {
                        for (int smallY = 0; smallY < size && y + smallY < sourceBitmap.Height; smallY++)
                        {
                            counter++;
                            var pixel = sourceBitmap.GetPixel(x + smallX, y + smallY);
                            R += pixel.R;
                            G += pixel.G;
                            B += pixel.B;
                        }
                    }

                    var mozaicColor = Color.FromArgb(R/counter, G/counter, B/counter);
                    for (int smallX = 0; smallX < size && x + smallX < sourceBitmap.Width; smallX++)
                    {
                        for (int smallY = 0; smallY < size && y + smallY < sourceBitmap.Height; smallY++)
                        {
                            bitmap.SetPixel(x + smallX, y + smallY, mozaicColor);
                        }
                    }
                }
            }
            
            return bitmap;
        }
    }
}
