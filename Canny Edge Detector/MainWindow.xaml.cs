using Emgu.CV;
using Emgu.CV.CvEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Canny_Edge_Detector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const double c_alphaDownscale = 1 / 256d;

        Mat m_image;

        public MainWindow()
        {
            InitializeComponent();
            LoadImage();

            scThresh1.Setup("Threshold 1", 1, 255);
            scThresh1.ValueUpdated += ValueUpdated;
            scThresh2.Setup("Threshold 2", 1, 255);
            scThresh2.ValueUpdated += ValueUpdated;

            scHoughMin.Setup("Radius Minimum", 1, 200);
            scHoughMin.ValueUpdated += ValueUpdated;
            scHoughMax.Setup("Radius Maximum", 1, 200);
            scHoughMax.ValueUpdated += ValueUpdated;
            scHoughDist.Setup("Min Distance", 1, 200);
            scHoughDist.ValueUpdated += ValueUpdated;

        }

        private void ValueUpdated(object sender, EventArgs e)
        {
            try
            {
                UpdateImage();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception: " + exc.Message);
            }
        }

        private void LoadImage()
        {
            m_image = new Mat();
            // FILEPATH TO IMAGE HERE
            string filePath = "";
            Mat inputImage = new Mat(filePath, LoadImageType.AnyDepth | LoadImageType.Grayscale);

            // Uncomment this to scale from 16 bit to 8 bit.
            // inputImage.ConvertTo(m_image, DepthType.Cv8U, c_alphaDownscale);
            //CvInvoke.Imshow("m_image", m_image);

            UpdateImage();
        }

        private void UpdateImage()
        {
            Mat displayImage = new Mat(m_image.Size, DepthType.Cv8U, 3);
            CvInvoke.CvtColor(m_image, displayImage, ColorConversion.Gray2Bgr);

            if(cbEnableCanny.IsChecked.Value)
            {
                CvInvoke.Canny(m_image, displayImage, scThresh1.Value, scThresh2.Value);
            }
            if(cbEnableCircles.IsChecked.Value)
            {
                try
                {
                    var circs = CvInvoke.HoughCircles(m_image, HoughType.Gradient, 1, (int)scHoughDist.Value, (int)scThresh1.Value, (int)scThresh2.Value, (int)scHoughMin.Value, (int)scHoughMax.Value);
                    foreach (var circle in circs)
                    {
                        CvInvoke.Circle(displayImage, new System.Drawing.Point((int)circle.Center.X, (int)circle.Center.Y), (int)circle.Radius, new Emgu.CV.Structure.MCvScalar(0, 0, 255), 3);
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine("HoughCircle Exception: " + exc.Message);
                }
            }
            imgPreview.Source = ConvertImage.ToBitmapSource(displayImage);
        }

        private void cbEnableCanny_Click(object sender, RoutedEventArgs e)
        {
            UpdateImage();
        }

        private void sliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateImage();
        }
    }

    public static class ConvertImage
    {
        [System.Runtime.InteropServices.DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap();

                BitmapSource bitmapSrc = Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr);
                return bitmapSrc;
            }
        }
    }
    
}
