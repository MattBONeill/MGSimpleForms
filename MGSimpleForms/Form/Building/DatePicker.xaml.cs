using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MGSimpleForms.Form.Building
{
    /// <summary>
    /// Interaction logic for DatePicker.xaml
    /// Inspiration: https://jobijoy.blogspot.com/2007/10/time-picker-user-control.html
    /// </summary>
    public partial class DatePicker : UserControl, INotifyPropertyChanged
    {
        public DatePicker()
        {
            InitializeComponent();
            txtDate.DataContext = this;
        }

        public DateTime Value
        {
            get => (DateTime)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register(nameof(Value), 
                typeof(DateTime),typeof(DatePicker),
                new FrameworkPropertyMetadata(
                                new DateTime(1900, 1, 1),
                                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            DatePicker control = obj as DatePicker;
            if (control.Value > new DateTime(1900, 1, 1))
                control.RawDate = $"{control.Value.Month:00}/{control.Value.Day:00}/{control.Value.Year:0000}";//control.Value.ToString("MM/dd/yyyy");
            else
                control.RawDate = "__/__/____";
        }


        public string RawDate
        {
            get => (string)GetValue(RawDateProperty);
            set
            {
                SetValue(RawDateProperty, value);
                OnPropertyChanged();
            }
        }

        public static readonly DependencyProperty RawDateProperty =
        DependencyProperty.Register(nameof(RawDate), typeof(string), typeof(DatePicker),
        new UIPropertyMetadata("__/__/____", new PropertyChangedCallback(OnDateChanged)));


        private static void OnDateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            DatePicker control = obj as DatePicker;

            //control.Value = DateTime.TryParse(control.RawDate, System.Globalization.DateTimeStyles.None, out var date) ?
            control.Value = DateTime.TryParseExact(control.RawDate, "MM/dd/yyyy",
                null, System.Globalization.DateTimeStyles.None, out var date) ?
                date :
                new DateTime(1900, 1, 1);

        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
        

        private const int Slash1Loc = 2;
        private const int Slash2Loc = 5;
        private bool CaretHandled { get; set; } = false;
        
        private void txtDate_KeyDown(object sender, KeyEventArgs e)
        {
            var caret = txtDate.CaretIndex;
            var KeyInt = (int)e.Key;
            var tStart = (int)Key.D0;
            var tEnd = (int)Key.D9;
            var pStart = (int)Key.NumPad0;
            var pEnd = (int)Key.NumPad9;

            int Length = 10;

            if ((KeyInt >= tStart && KeyInt <= tEnd) ||
                (KeyInt >= pStart && KeyInt <= pEnd))
            {
                if (KeyInt >= pStart && KeyInt <= pEnd)
                    KeyInt -= pStart - tStart;
                if (caret < Length)
                {
                    if (caret == Slash1Loc || caret == Slash2Loc)
                        throw new Exception("Unable to process Date");
                    var Num = (char)KeyInterop.VirtualKeyFromKey((Key)KeyInt);
                    txtDate.Text = txtDate.Text.Remove(caret, 1).Insert(caret, "" + Num);
                    txtDate.CaretIndex = caret + 1;
                }
            }
            else if (e.Key == Key.Back)
            {
                if (caret == Slash1Loc + 1 || caret == Slash2Loc + 1)
                {
                    caret--;
                }

                if (caret != 0)
                {
                    txtDate.Text = caret == Length ?
                        txtDate.Text.Substring(0, Length-1) + "_" :
                        txtDate.Text.Remove(caret - 1, 1).Insert(caret-1, "_");

                    txtDate.CaretIndex = caret - 1;
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                if (caret == Slash1Loc - 1 || caret == Slash2Loc - 1)
                {
                    CaretHandled = true;
                    txtDate.CaretIndex++;
                    CaretHandled = false;
                }
                return;
            }
            else if (e.Key == Key.Left)
            {

                if (caret == Slash1Loc + 1 || caret == Slash2Loc + 1)
                {
                    CaretHandled = true;
                    txtDate.CaretIndex--;
                    CaretHandled = false;
                }
                return;
            }
            else if (e.Key == Key.Tab || e.Key == Key.Home || e.Key == Key.End)
                return;//make sure functionality isn't lost

            if (DateTime.TryParseExact(txtDate.Text, "MM/dd/yyyy",
                null, System.Globalization.DateTimeStyles.None, out var date))
                Value = date;

            e.Handled = true;
        }

        private void txtDate_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (!CaretHandled)
            {
                var caret = txtDate.CaretIndex;
                if (caret == Slash1Loc || caret == Slash2Loc)
                    txtDate.CaretIndex++;
            }
        }

        private void txtDate_GotFocus(object sender, RoutedEventArgs e) => (sender as TextBox).SelectAll();

        private void UserControl_GotFocus(object sender, RoutedEventArgs e) => txtDate.Focus();
    }
}
