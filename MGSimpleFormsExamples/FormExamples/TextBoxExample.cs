using MGSimpleForms.Attributes;
using MGSimpleForms.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MGSimpleFormsExamples.FormExamples
{
    [Form("TextBox Examples")]
    internal class TextBoxExample : FormViewModel
    {
        public TextBoxExample()
        {
            Visible = true;
        }
        public string teststring1 { get => GetProperty<string>(); set => SetProperty(value); }

        [TextBox]
        public string teststring2 { get => GetProperty<string>(); set => SetProperty(value); }
        [Name("Name for Displaying Goes Here:")]
        [TextBox]
        public string teststring3 { get => GetProperty<string>(); set => SetProperty(value); }

        [TextBox()]//(IsLarge = true, StarSize = 2)]
        public string teststring4 { get => GetProperty<string>(); set => SetProperty(value); }





        [Name("testing checkBox:")]
        [CheckBox]
        public bool Visible { get => GetProperty<bool>(); set { SetProperty(value); OnPropertyChanged(nameof(NotVisible)); } }
        public bool NotVisible => !Visible;


        [Name("Is Enable:")]
        [TextBox(IsEnabled = nameof(Visible))]
        public string teststring8 { get => GetProperty<string>(); set => SetProperty(value); }

        [Name("Is Enable Not:")]
        [TextBox(IsEnabled = nameof(NotVisible))]
        public string teststring9 { get => GetProperty<string>(); set => SetProperty(value); }


        [Name("Is Visible:")]
        [TextBox(IsVisible = nameof(Visible))]
        public string teststring10 { get => GetProperty<string>(); set => SetProperty(value); }

        [Name("Is Visible Not:")]
        [TextBox(IsVisible  = nameof(NotVisible))]
        public string teststring11 { get => GetProperty<string>(); set => SetProperty(value); }


        [Name("Is Collapsed:")]
        [TextBox(IsCollapsed = nameof(Visible))]
        public string teststring12 { get => GetProperty<string>(); set => SetProperty(value); }

        [Name("Is Collapsed Not:")]
        [TextBox(IsCollapsed = nameof(NotVisible))]
        public string teststring13 { get => GetProperty<string>(); set => SetProperty(value); }




        //[Name("strait disabled:")]
        //[TextBox(IsEnabled = false)]
        //public string teststring14 { get; set; }




    }
}
