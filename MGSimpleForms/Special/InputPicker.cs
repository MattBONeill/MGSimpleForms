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
using TextAlignment = MGSimpleForms.Attributes.TextAlignment;

namespace MGSimpleForms.Special
{
    [Form(BorderSize = 5, Flow = FormFlow.Grid)]
    [GridSize(6, 2)]
    public class InputPicker : FormViewModel
    {

        [Label(Alignment = TextAlignment.Center, Wrap = TextWrap.Wrap)]
        [Location(0, 0, 6)]
        public string Question { get => GetProperty<string>(); set => SetProperty(value); }


        [Label(Alignment = TextAlignment.Right)]
        [Location(0, 1)]
        public string lbl => "Text:";

        [TextBox(Alignment = TextAlignment.Left)]
        [Location(1, 1, 4)]
        public string SelectedText { get => GetProperty<string>(); set => SetProperty(value); }

        [Name("Save")]
        [Button]
        [Location(5, 1)]
        public ICommand SaveButton => new Command(_SaveButton);

        private void _SaveButton()
        {
            this.Close(true);
        }




        private InputPicker(string question, string selectedText)
        {
            Question = question;
            SelectedText = selectedText;
        }

        public override void OnFormLoaded()
        {
            base.OnFormLoaded();

            var window = GetWindow();
            if (window == null)
                return;
            window.Title = "Input";
            window.MinWidth = 250;
            window.MaxWidth = 700;
            window.MinHeight = 50;
            window.MaxHeight = 500;

            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            window.ResizeMode = ResizeMode.NoResize;
            window.Topmost = true;
            window.WindowStyle = WindowStyle.ToolWindow;
            window.SizeToContent = SizeToContent.WidthAndHeight;
        }


        public static string GetInput(string Question, string SelectedText = "")
        {
            var viewModel = new InputPicker(Question, SelectedText);

            if (viewModel.ShowInWindowDialog() == true)
            {
                return viewModel.SelectedText;
            }
            return null;
        }
    }
}
