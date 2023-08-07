using MGSimpleForms.Attributes;
using MGSimpleForms.Form;
using MGSimpleForms.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MGSimpleFormsExamples.FormExamples
{
    [Form("Button Examples", TitleFontSize = 24)]
    internal class ButtonExample : FormViewModel
    {
        public ButtonExample()
        {
            btnName = "Am i Really a Name";
        }
        public ICommand btn1 => new Command(WontRun);

        [Button]
        public ICommand btn2 => new Command(OnClick);

        [Name("Name is Set Here")]
        [Button]
        public ICommand btn3 => new Command(OnClick);

        public string btnName { get => GetProperty<string>(); set => SetProperty(value); }


        [Button(TextBinding = nameof(btnName))]
        public ICommand btn4 => new Command(OnClick);

        public bool btnEnabled { get => GetProperty<bool>(); set => SetProperty(value); }

        [Name("Hello User")]
        [Button(IsEnabled = nameof(btnEnabled))]
        public ICommand btn5 => new Command(OnClick);



        [Button(TextBinding = nameof(btnName), IsEnabled = nameof(btnEnabled))]
        public ICommand btn6 => new Command(OnClick);

        [Name("Hello User")]
        [Button(TextBinding = nameof(btnName), IsEnabled = nameof(btnEnabled))]
        public ICommand btn7 => new Command(OnClick);



        [Name("None ICommand")]
        [Button]
        public Command btn8 => new Command(OnClick);


        int cnt = 0;
        private void OnClick()
        {
            //MessageBox.Show("Ran");
            btnName = $"Am i Really a Name, Clicked {cnt++} Times";
            btnEnabled = !btnEnabled;
        }



        private void WontRun()
        {
            throw new NotImplementedException();
        }


        /*
         * NOTE: Name in Command only works for "Buttons". it Doesn't work for "Button"
         */
        [Buttons]
        public List<Command> buttons => new List<Command>() {
            new Command(OnClick){ Name ="Left" },
            new Command(OnClick){ Name ="Center" },
            new Command(OnClick){ Name ="Right" },
        };




    }
}
