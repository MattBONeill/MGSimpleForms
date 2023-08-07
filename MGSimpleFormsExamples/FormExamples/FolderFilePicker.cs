using MGSimpleForms.Attributes;
using MGSimpleForms.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGSimpleFormsExamples.FormExamples
{
    [Form("Folder File Picker", TitleFontSize = 24)]
    internal class FolderFilePicker : FormViewModel
    {
        public FolderFilePicker()
        {
            FileArea = "FilePicker";
            FolderArea = "FolderPicker";
        }

        [Name("testing checkBox:")]
        [CheckBox]
        public bool visible { get => GetProperty<bool>(); set { SetProperty(value); OnPropertyChanged(nameof(NotVisible)); } }
        public bool NotVisible => !visible;


        [Label(FontSize = 18)]
        public string FileArea { get => GetProperty<string>(); set => SetProperty(value); } 

        [FilePicker(Filter = "Excel Files|*.xls;*.xlsx;*.csv")]
        public string teststring1 { get => GetProperty<string>(); set => SetProperty(value); }

        [Name("Read Only TextBox")]
        [FilePicker(Filter = "Excel Files|*.xls;*.xlsx;*.csv", ReadOnly = true)]
        public string teststring2 { get => GetProperty<string>(); set => SetProperty(value); }


        /*
            * NOTE: you will have to use _teststring7 for full path
            */
        [Name("Show's Only the Filename")]
        [FilePicker(Filter = "Excel Files|*.xls;*.xlsx;*.csv")]
        public string teststring3 { get => Path.GetFileName(GetProperty<string>()); set => SetProperty(value); }
        



        [Name("Is Enable:")]
        [FilePicker(IsEnabled = nameof(visible))]
        public string teststring4 { get => GetProperty<string>(); set => SetProperty(value); }

        [Name("Is Enable Not:")]
        [FilePicker(IsEnabled = nameof(NotVisible))]
        public string teststring5 { get => GetProperty<string>(); set => SetProperty(value); }


        [Name("Is Visible:")]
        [FilePicker(IsVisible = nameof(visible))]
        public string teststring6 { get => GetProperty<string>(); set => SetProperty(value); }

        [Name("Is Visible Not:")]
        [FilePicker(IsVisible = nameof(NotVisible))]
        public string teststring7 { get => GetProperty<string>(); set => SetProperty(value); }


        [Name("Is Collapsed:")]
        [FilePicker(IsCollapsed = nameof(visible))]
        public string teststring8 { get => GetProperty<string>(); set => SetProperty(value); }

        [Name("Is Collapsed Not:")]
        [FilePicker(IsCollapsed= nameof(NotVisible))]
        public string teststring9 { get => GetProperty<string>(); set => SetProperty(value); }


        [Label(FontSize = 18)]
        public string FolderArea { get => GetProperty<string>(); set => SetProperty(value); } 

        [FolderPicker()]
        public string teststring10 { get => GetProperty<string>(); set => SetProperty(value); }

        [Name("Read Only TextBox")]
        [FolderPicker(ReadOnly = true)]
        public string teststring11 { get => GetProperty<string>(); set => SetProperty(value); }


        /*
            * NOTE: you will have to use _teststring7 for full path
            */
        [Name("Show's Only the Filename")]
        [FolderPicker()]
        public string teststring12 { get => Path.GetFileName(GetProperty<string>()); set => SetProperty(value); }


        [Name("Is Enable:")]
        [FilePicker(IsEnabled = nameof(visible))]
        public string teststring13 { get => GetProperty<string>(); set => SetProperty(value); }

        [Name("Is Enable Not:")]
        [FilePicker(IsEnabled = nameof(NotVisible))]
        public string teststring14 { get => GetProperty<string>(); set => SetProperty(value); }


        [Name("Is Visible:")]
        [FilePicker(IsVisible = nameof(visible))]
        public string teststring15 { get => GetProperty<string>(); set => SetProperty(value); }

        [Name("Is Visible Not:")]
        [FilePicker(IsVisible = nameof(NotVisible))]
        public string teststring16 { get => GetProperty<string>(); set => SetProperty(value); }


        [Name("Is Collapsed:")]
        [FilePicker(IsCollapsed = nameof(visible))]
        public string teststring17 { get => GetProperty<string>(); set => SetProperty(value); }

        [Name("Is Collapsed Not:")]
        [FilePicker(IsCollapsed = nameof(NotVisible))]
        public string teststring18 { get => GetProperty<string>(); set => SetProperty(value); }




    }
}
