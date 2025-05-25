using System.ComponentModel;
using System;
using System.Runtime.CompilerServices; // Для CallerMemberName
using System.Collections.Generic;    // <--- ВОТ ЭТА СТРОКА БЫЛА ДОБАВЛЕНА

namespace CNO.ViewModel // Замените CNO.ViewModels на ваше реальное пространство имен
{
    public class RecordViewModel : INotifyPropertyChanged
    {
        private int _recordId;
        public int RecordID
        {
            get => _recordId;
            set { SetField(ref _recordId, value); }
        }

        private string _patientName;
        public string PatientName
        {
            get => _patientName;
            set { SetField(ref _patientName, value); }
        }

        private string _doctorName;
        public string DoctorName
        {
            get => _doctorName;
            set { SetField(ref _doctorName, value); }
        }

        private DateTime? _recordDate;
        public DateTime? RecordDate
        {
            get => _recordDate;
            set
            {
                if (SetField(ref _recordDate, value))
                {
                    OnPropertyChanged(nameof(FormattedRecordDate)); // Обновить и форматированную дату
                }
            }
        }

        public string FormattedRecordDate => RecordDate?.ToString("dd.MM.yyyy") ?? "N/A";

        private string _notes;
        public string Notes
        {
            get => _notes;
            set { SetField(ref _notes, value); }
        }

        private string _stage;
        public string Stage // Стадия Канбана
        {
            get => _stage;
            set { SetField(ref _stage, value); }
        }

        // Исходные ID для связи с БД, если имена Patient/Doctor загружаются отдельно
        public int OriginalPatientID { get; set; }
        public int OriginalDoctorID { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Здесь используется EqualityComparer<T>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}