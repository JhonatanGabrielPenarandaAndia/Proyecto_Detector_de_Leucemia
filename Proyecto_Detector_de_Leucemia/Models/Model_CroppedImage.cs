using System.Windows.Media.Imaging;

namespace Proyecto_Detector_de_Leucemia.Models
{
    public class Model_CroppedImage
    {
        public BitmapImage SourceImage { get; set; }
        private string name ="asdasd";

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

    }
}
