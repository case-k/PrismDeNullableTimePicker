using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace PrismDeNullableTimePicker.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        private TimeSpan _time1;
        public TimeSpan Time1
        {
            get { return _time1; }
            set { SetProperty(ref _time1, value); }
        }

        private TimeSpan? _time2;
        public TimeSpan? Time2
        {
            get { return _time2; }
            set { SetProperty(ref _time2, value); }
        }


        public ICommand SetNullCommand { get; }
        public ICommand TimePickerTapCommand { get; }


        public MainPageViewModel()
        {
            SetNullCommand = new DelegateCommand(SetNull);
            TimePickerTapCommand = new DelegateCommand(TimePickerTap);

            Time1 = DateTime.Now.TimeOfDay;
            Time2 = null;
        }


        private void SetNull()
        {
            Time2 = null;
        }


        private void TimePickerTap()
        {
            // Pickerがnullだった場合初期値を設定してあげたり
            if (Time2 == null)
                Time2 = DateTime.Now.TimeOfDay;
        }
    }
}
