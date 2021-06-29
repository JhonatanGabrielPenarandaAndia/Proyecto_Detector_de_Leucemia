using System.Windows.Media.Imaging;

namespace Proyecto_Detector_de_Leucemia.Models
{
    public class Model_CroppedImage
    {
        public BitmapImage SourceImage { get; set; }
        private int width = 86;

        public int Width
        {
            get { return width; }
            set { width = value; }
        }


    }
}
