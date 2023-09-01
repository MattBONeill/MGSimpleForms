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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MGSimpleForms.Form
{
    /// <summary>
    /// Interaction logic for FormUserControl.xaml
    /// </summary>
    public partial class FormUserControl : UserControl
    {

        bool HasLoaded = false;
        public FormUserControl()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is not FormViewModel viewModel)
                return;

            viewModel.Parent = this;
            var typ = DataContext.GetType().GetFormViewModelGenericType();
            if (typ != null)
                //typeof(BuildingGrid).GetMethod("BuildItems")?.MakeGenericMethod(typ).Invoke(null, new object[] { grdForm, viewModel, null, null });
                BuildingGrid.BuildItems(grdForm, viewModel, typ);
            else
                BuildingGrid.Build(grdForm, viewModel);

            if (HasLoaded)
                CallLoadForm();

        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            HasLoaded = true;
            CallLoadForm();
        }


        void CallLoadForm()
        {
            if (DataContext is not FormViewModel viewModel)
                return;

            viewModel.OnFormLoaded();
        }
    }
}
