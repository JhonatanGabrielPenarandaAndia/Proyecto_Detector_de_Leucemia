using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Proyecto_Detector_de_Leucemia
{
    public class IGeneric : IEnlace
    {
        #region Implementacion de INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
