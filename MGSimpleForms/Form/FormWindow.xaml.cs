using MGSimpleForms.Attributes;
using MGSimpleForms.Form.Building;
using MGSimpleForms.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MGSimpleForms.Form
{
    /// <summary>
    /// Interaction logic for FormWindow.xaml
    /// </summary>
    public partial class FormWindow : Window
    {
        public FormWindow()
        {
            InitializeComponent();
        }

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is not FormViewModel viewModel)
                return;

            var FormOptions = viewModel.GetType().GetCustomAttributes<FormAttribute>().FirstOrDefault() ?? new FormAttribute(string.Empty);

            if (!string.IsNullOrEmpty(FormOptions.Title))
            {
                this.Title = FormOptions.Title;
            }
            else
            {
                this.SetBinding(Window.TitleProperty, FormOptions.TitleBinding);
            }


        }



        public static readonly DependencyProperty CustomBuildOptionsProperty = DependencyProperty.Register(nameof(CustomBuildOptions),
            typeof(ICustomBuildOptions),
            typeof(FormWindow),
            new UIPropertyMetadata(null, new PropertyChangedCallback(OnBuildOptionsChanged)));

        private static void OnBuildOptionsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            FormWindow control = obj as FormWindow;
            if (control == null) return;

            control.Window_DataContextChanged(null, e);
        }

        public ICustomBuildOptions CustomBuildOptions
        {
            get => (ICustomBuildOptions)GetValue(CustomBuildOptionsProperty);
            set => SetValue(CustomBuildOptionsProperty, value);
        }

    }
}
