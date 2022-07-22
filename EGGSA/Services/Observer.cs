using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EGGSA.Services
{
    internal class Observer : INotifyPropertyChanged

    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
