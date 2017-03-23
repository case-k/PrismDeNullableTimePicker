using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PrismDeNullableTimePicker.Controls
{
    class NullableTimePicker : ContentView
    {
        private const string DisplayFormat = "HH時 mm分";
        private const string NullTimeLabel = "--時 --分";

        private readonly TimePicker _timePicker;
        private readonly Entry _entry;


        public static readonly BindableProperty TimeProperty =
            BindableProperty.Create(
                "Time",
                typeof(TimeSpan?),
                typeof(NullableTimePicker),
                null,
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var nullableTimePicker = bindable as NullableTimePicker;
                    if (nullableTimePicker != null && (TimeSpan?)oldValue != (TimeSpan?)newValue)
                    {
                        nullableTimePicker.Time = (TimeSpan?)newValue;
                    }
                });

        public TimeSpan? Time
        {
            get
            {
                return GetValue(TimeProperty) as TimeSpan?;
            }
            set
            {
                SetValue(TimeProperty, value);

                if (value.HasValue && _timePicker.Time != value)
                {
                    _timePicker.Time = value.Value;
                }

                SetLabelText(value);
            }
        }


        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(NullableTimePicker));

        public object CommandParameter
        {
            get { return this.GetValue(CommandParameterProperty); }
            set { this.SetValue(CommandParameterProperty, value); }
        }


        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(NullableTimePicker));

        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, (object)value); }
        }


        public string Placeholder
        {
            get { return _entry.Placeholder; }
            set { _entry.Placeholder = value; }
        }

        public TextAlignment HorizontalTextAlignment
        {
            get { return _entry.HorizontalTextAlignment; }
            set { _entry.HorizontalTextAlignment = value; }
        }


        public NullableTimePicker()
        {
            _entry = new Entry
            {
                Text = NullTimeLabel,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Start,
                //VerticalTextAlignment = TextAlignment.Center,
                TextColor = Color.Black,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };

            _timePicker = new TimePicker
            {
                IsVisible = false,
                //IsEnabled = false,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Fill
            };

            Content = new StackLayout
            {
                Children =
                {
                   _entry,
                   _timePicker
                },
                Padding = 0,
                Spacing = 0,
                Margin = 0
            };

            Padding = 0;
            Margin = 0;

            _entry.Focused += OnFocused;

            _timePicker.PropertyChanged += timePicker_PropertyChanged;

            PropertyChanged += NullableTimePicker_PropertyChanged;
        }

        private void NullableTimePicker_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_entry != null && e.PropertyName == IsEnabledProperty.PropertyName)
            {
                _entry.TextColor = IsEnabled ? Color.Black : Color.Gray;
            }
        }

        private void timePicker_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == TimePicker.TimeProperty.PropertyName && _timePicker != null)
            {
                Time = _timePicker.Time;
            }
        }

        async private void OnFocused(object sender, EventArgs e)
        {
            if (IsEnabled == false)
            {
                return;
            }

            Device.BeginInvokeOnMainThread(() => {
                _entry.Unfocus();
            });

            if (this.Command != null)
            {
                await Task.Run(() => this.Command.Execute(this.CommandParameter ?? this));
            }

            if (Time == null)
                SetValue(TimeProperty, _timePicker.Time);

            Device.BeginInvokeOnMainThread(() => {
                _timePicker.Focus();
            });
        }

        private void SetLabelText(TimeSpan? value)
        {
            _entry.Text = value.HasValue ? ConvertTimeSpanToString(value.Value) : NullTimeLabel;
        }

        private string ConvertTimeSpanToString(TimeSpan value)
        {
            return new DateTime(0).Add(value).ToString(DisplayFormat);
        }
    }
}
