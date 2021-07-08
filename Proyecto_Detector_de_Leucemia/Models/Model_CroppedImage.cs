using System.Windows.Media.Imaging;

namespace Proyecto_Detector_de_Leucemia.Models
{
    public class Model_CroppedImage:IGeneric
    {
        private BitmapImage sourceImage;
        private int width = 86;
        private BitmapImage baseImage;
        private bool haschanged = false;
        private int cantInfectedBloodCells = 0;
        private string diagnostic = "Diagnostico:\r\n";


        public bool HasChanged
        {
            get { return haschanged; }
            set { haschanged = value; }
        }

        public int CantInfectedBloodCells
        {
            get { return cantInfectedBloodCells; }
            set { cantInfectedBloodCells = value; 
                OnPropertyChanged();
            }
        }

        public string Diagnostic
        {
            get { return diagnostic; }
            set { diagnostic = value; OnPropertyChanged(); }
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
