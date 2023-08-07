using MGSimpleForms.Form;
using MGSimpleForms.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MGSimpleForms
{
    public class FormLoader
    {
        public static Window GetWindow(FormViewModel viewModel)
        {
            var window = new FormWindow();
            window.DataContext = viewModel;

            return window;
        }

        public static void ShowInWindow(FormViewModel viewModel)
        { 
            var window = GetWindow(viewModel);

            window.Show();
        }

        public static bool? ShowInWindowDialog(FormViewModel viewModel)
        {
            var window = GetWindow(viewModel);

            return window.ShowDialog();
        }

        public static UserControl MakeUserControl(FormViewModel viewModel)
        {
            return new FormUserControl() { DataContext = viewModel };
        }
    }

    public static class FormLoaderUtil
    {
        public static Window GetWindow(this FormViewModel viewModel) => FormLoader.GetWindow(viewModel);
        public static void ShowInWindow(this FormViewModel viewModel) => FormLoader.ShowInWindow(viewModel);
        public static bool? ShowInWindowDialog(this FormViewModel viewModel) => FormLoader.ShowInWindowDialog(viewModel);
        public static UserControl MakeUserControl(this FormViewModel viewModel) => FormLoader.MakeUserControl(viewModel);
    }
}
