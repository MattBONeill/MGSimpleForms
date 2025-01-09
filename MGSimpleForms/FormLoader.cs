using MGSimpleForms.Form;
using MGSimpleForms.Form.Building;
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
        public static Window GetWindow(FormViewModel viewModel, ICustomBuildOptions buildOptions = null)
        {
            return new FormWindow() { DataContext = viewModel, CustomBuildOptions = buildOptions };
        }

        public static void ShowInWindow(FormViewModel viewModel, ICustomBuildOptions buildOptions = null)
        { 
            var window = GetWindow(viewModel, buildOptions);

            window.Show();
        }

        public static bool? ShowInWindowDialog(FormViewModel viewModel, ICustomBuildOptions buildOptions = null)
        {
            var window = GetWindow(viewModel, buildOptions);

            return window.ShowDialog();
        }

        public static UserControl MakeUserControl(FormViewModel viewModel, ICustomBuildOptions buildOptions = null)
        {
            return new FormUserControl() { DataContext = viewModel, CustomBuildOptions = buildOptions };
        }
    }

    public static class FormLoaderUtil
    {
        public static Window GetWindow(this FormViewModel viewModel, ICustomBuildOptions buildOptions = null) => FormLoader.GetWindow(viewModel, buildOptions);
        public static void ShowInWindow(this FormViewModel viewModel, ICustomBuildOptions buildOptions = null) => FormLoader.ShowInWindow(viewModel, buildOptions);
        public static bool? ShowInWindowDialog(this FormViewModel viewModel, ICustomBuildOptions buildOptions = null) => FormLoader.ShowInWindowDialog(viewModel, buildOptions);
        public static UserControl MakeUserControl(this FormViewModel viewModel, ICustomBuildOptions buildOptions = null) => FormLoader.MakeUserControl(viewModel, buildOptions);
    }
}
