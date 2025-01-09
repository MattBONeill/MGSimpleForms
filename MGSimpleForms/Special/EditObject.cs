using MGSimpleForms.Attributes;
using MGSimpleForms.Form;
using MGSimpleForms.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Xml.Serialization;
using MGSimpleForms.Form.Building;
using System.Windows.Controls;
using Border = MGSimpleForms.Attributes.Border;

namespace MGSimpleForms.Special
{
    [Form(Title = "", Flow = FormFlow.Columned, BorderSize = 5, Border = Border.BottomPadding | Border.RightPadding)]
    [ColumnedSize(2)]
    public abstract class BaseEditObject<T> : FormViewModel<T>
    {
        public BaseEditObject(T item)
        {
            Item = item;
        }
        public override void OnFormLoaded()
        {
            base.OnFormLoaded();

            var window = GetWindow();
            if (window == null)
                return;
            window.Title = typeof(T).Name;
            window.MinWidth = 400;
            window.MaxWidth = 1000;
            window.MinHeight = 50;

            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            window.ResizeMode = ResizeMode.NoResize;
            window.Topmost = true;
            window.WindowStyle = WindowStyle.ToolWindow;
            window.SizeToContent = SizeToContent.WidthAndHeight;
        }
    }

    [Form(Flow = FormFlow.Grid, BorderSize = 5)]
    [GridSize(6, 6)]
    internal class EditObjectView<T> : FormViewModel
    {

        [GeneralControl]
        [Location(0, 0, 10, 5)]
        public FormUserControl test { get; }


        [Name("Save")]
        [Button]
        [Location(2, 5)]
        public ICommand Save => new Command(() => this.Close(true));

        [Name("Cancel")]
        [Button]
        [Location(3, 5)]
        public ICommand Cancel => new Command(() => this.Close(false));

        public EditObjectView(BaseEditObject<T> testing)
        {
            test = new FormUserControl() { DataContext = testing };
        }

        public override void OnFormLoaded()
        {
            base.OnFormLoaded();
            var window = GetWindow() as FormWindow;

            if (window == null)
                return;

            test.CustomBuildOptions = window.CustomBuildOptions;
        }

    }



    public static class EditObject
    {
        public static Window MakeEditWindow<T>(this BaseEditObject<T> viewModel, ICustomBuildOptions buildOptions = null) => new EditObjectView<T>(viewModel).GetWindow(buildOptions);
        public static bool? ShowEditDialog<T>(this BaseEditObject<T> viewModel, ICustomBuildOptions buildOptions = null) => new EditObjectView<T>(viewModel).ShowInWindowDialog(buildOptions);
        public static UserControl MakeUserControl<T>(this BaseEditObject<T> viewModel, ICustomBuildOptions buildOptions = null) => FormLoader.MakeUserControl(new EditObjectView<T>(viewModel), buildOptions);
    }
}
