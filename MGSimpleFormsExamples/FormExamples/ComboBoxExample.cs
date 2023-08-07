using MGSimpleForms.Attributes;
using MGSimpleForms.Form.Building;
using MGSimpleForms.MVVM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Label = System.Windows.Controls.Label;

namespace MGSimpleFormsExamples.FormExamples
{
    [Form("ComboBox Examples", TitleFontSize = 24)]
    internal class ComboBoxExample : FormViewModel
    {
        string[] testItems;
        TestingItem[] testComplex;
        public ComboBoxExample()
        {
            testItems = new string[] { "hello", "my", "name", "is", };
            testComplex = new TestingItem[] {
                new TestingItem(){Field1 = "l;asdkjf", Field2="1"},
                new TestingItem(){Field1 = "2", Field2="oejhfhdkjdf"},
                new TestingItem(){Field1 = "fjitndlikhbdsf", Field2="3"},
                new TestingItem(){Field1 = "4", Field2=";lasdkfjle"},
            };

            Name = "This is the Name";
        }

        public ICollection<string> list1 => testItems;
        
        [ComboBox]
        public ICollection<string> list2 => testItems;

        [Name("Name Goes Here")]
        [ComboBox]
        public ICollection<string> list3 => testItems;

        [Name("IEnumerable<> works")]
        [ComboBox]
        public IEnumerable<string> list4 => testItems;

        [Name("IEnumerable works")]
        [ComboBox]
        public IEnumerable list5 => testItems;


        [Name("ObservableCollection works")]
        [ComboBox]
        public ObservableCollection<string> list6 => new ObservableCollection<string>(testItems);


        [Name("NonEditable")]
        [ComboBox(IsEditable =false)]
        public ICollection<string> list7 => testItems;




        [Name("Selection")]
        [ComboBox(SelectedItem =nameof(Selected))]
        public ICollection<string> list8 => testItems;
        
        [Name("Selected Item")]
        [TextBox]
        public string Selected { get => GetProperty<string>(); set => SetProperty(value); }




        [Name("Make Invisible")]
        [CheckBox]
        public bool cboVisible { get => GetProperty<bool>(); set { SetProperty(value); OnPropertyChanged(nameof(NotCboVisible)); } } 
        public bool NotCboVisible => !cboVisible;
        

        [Name("Disappering")]
        [ComboBox(IsVisible =nameof(cboVisible))]
        public ICollection<string> list9 => testItems;
        [Name("Disappering Not")]
        [ComboBox(IsVisible = nameof(NotCboVisible))]
        public ICollection<string> list13 => testItems;

        [Name("Enabled")]
        [ComboBox(IsEnabled = nameof(cboVisible))]
        public ICollection<string> list14 => testItems;
        [Name("Enabled Not")]
        [ComboBox(IsEnabled = nameof(NotCboVisible))]
        public ICollection<string> list15 => testItems;




        [Name("this is not the name")]
        [ComboBox(NameBinding = nameof(Name))]
        public ICollection<string> list10 => testItems;

      

        public string Name { get => GetProperty<string>(); set => SetProperty(value); }



        [Name("Not Templated:")]
        [ComboBox(SelectedItem = nameof(testSelected))]
        public ICollection<TestingItem> list11 => testComplex;

        [Name("Templated:")]
        [ComboBox(DataTemplate = nameof(Template), DataTemplateTextPath = "Field2", SelectedItem = nameof(testSelected))]
        /*
         * NOTE: without DataTemplateTextPath, the value that is left after selecting a item in the combobox
         * will be the class.ToString() result.
         */
        public ICollection<TestingItem> list12 => testComplex;

        public DataTemplate Template => DataTemplateGenerator.CreateDataTemplate(() => {
            var lbl = new Label();
            lbl.IsTabStop = false;
            lbl.SetBinding(Label.ContentProperty, "Field2");
            return lbl;
        });

        TestingItem _testSelected;
        public TestingItem testSelected { get => GetProperty<TestingItem>(); set => SetProperty(value); }
    }



    class TestingItem
    {
        public string Field1 { get; set; }
        public string Field2 { get; set; }
    }
}
