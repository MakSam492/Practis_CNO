// CNO.ViewModel.DoctorViewModel.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CNO.ViewModel
{
    public class DoctorViewModel : INotifyPropertyChanged
    {
        private int _id;
        public int ID
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        private string _doctorName;
        public string DoctorName // ФИО
        {
            get => _doctorName;
            set { _doctorName = value; OnPropertyChanged(); }
        }

        private string _spez;
        public string Spez // Специальность
        {
            get => _spez;
            set { _spez = value; OnPropertyChanged(); }
        }

        private string _num;
        public string Num // Телефон
        {
            get => _num;
            set { _num = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}