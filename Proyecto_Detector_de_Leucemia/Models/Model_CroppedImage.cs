using System.Windows.Media.Imaging;

namespace Proyecto_Detector_de_Leucemia.Models
{
    public class Model_CroppedImage:IGeneric
    {
        private BitmapImage sourceImage;
        private int width = 86;
        private BitmapImage baseImage;
        private bool haschanged = false;

        public bool HasChanged
        {
            get { return haschanged; }
            set { haschanged = value; }
        }

        public BitmapImage BaseImage
        {
            get { return baseImage; }
            set { baseImage = value;
                sourceImage = baseImage;
            }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public BitmapImage SourceImage
        {
            get { return sourceImage; }
            set { sourceImage = value;
                  baseImage = value; OnPropertyChanged();
            }
        }
    }
}
