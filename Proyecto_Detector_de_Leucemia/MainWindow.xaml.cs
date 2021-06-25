using Microsoft.Win32;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Vision.Motion;
using AForge;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using AForge.Imaging;

namespace Proyecto_Detector_de_Leucemia
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables
        BitmapImage imgOrigin;
        BitmapImage imgResult;
        EuclideanColorFiltering colorFilter;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
        }

        #region methods

        public static BitmapImage ToBitmapImage(System.Drawing.Image bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;



                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new System.Drawing.Bitmap(outStream);



                return new Bitmap(bitmap);
            }
        }

        public BitmapImage RemoveBackground(BitmapImage imgProcessed)
        {
            colorFilter = new EuclideanColorFiltering();
            colorFilter.CenterColor = new RGB(163, 57, 176, 255); //160, 50, 160
            colorFilter.Radius = 100;   
            //colorFilter.FillColor = new RGB(255,255,255);
            imgProcessed = ToBitmapImage(colorFilter.Apply(BitmapImage2Bitmap(imgProcessed)));
            return imgProcessed;
        }

        #endregion

        #region Events

        private void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (ofd.ShowDialog() == true)
            {
                string fileName = ofd.FileName;
                imgOrigin = new BitmapImage(new Uri(fileName));
                Img_Processed.Source = imgOrigin;
            }
        }

        private void btnDetectAbnormalities_Click(object sender, RoutedEventArgs e)
        {
            imgResult = RemoveBackground(imgOrigin);
            Img_Processed.Source = imgResult;
        }

        #endregion
    }
}
