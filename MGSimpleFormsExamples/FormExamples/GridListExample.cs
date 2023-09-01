using MGSimpleForms.Attributes;
using MGSimpleForms.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGSimpleFormsExamples.FormExamples
{
    [Form("Grid List Examples", TitleFontSize = 24)]
    internal class GridListExample : FormViewModel
    {
        DisplayTestItem[] testItems;

        public GridListExample()
        {
            testItems = new DisplayTestItem[] {
                new DisplayTestItem(){
                    ID = 0,
                    Name = "First",
                    Data = new testingItem(){ Field1 = "asdlkjfh", Field2 = "l;kjhsadf;kj" },
                    Active = true,
                    Amount = 100,
                    client = new Client(){ ID = 0, Name="Unknown"}
                },
                new DisplayTestItem(){
                    ID = 1,
                    Name = "Second",
                    Data = new testingItem(){ Field1 = "fghhjgf", Field2 = "jkug" },
                    Active = false,
                    Amount = 1000,
                    client = new Client(){ ID = 0, Name="Unknown"}
                },
                new DisplayTestItem(){
                    ID = 2,
                    Name = "Third",
                    Data = new testingItem(){ Field1 = "yujrt", Field2 = ";.gerjhkn.jm" },
                    Active = false,
                    Amount = 10000,
                    client = new Client(){ ID = 0, Name="Unknown"}
                },

            };
        }


        public ICollection<DisplayTestItem> list1 => new ObservableCollection<DisplayTestItem>(testItems);



        public DisplayTestItem Selected { get => GetProperty<DisplayTestItem>(); set => SetProperty(value); }

        [ListView(IsGridView =true, SelectedItem= nameof(Selected))]
        public ICollection<DisplayTestItem> list2 => new ObservableCollection<DisplayTestItem>(testItems);


        public string SearchField { get; set; }
        [ListView(IsGridView = true, SearchPropName = nameof(SearchField), ToSearchPropertyName = "Name")]
        public ICollection<DisplayTestItem> list3 => new ObservableCollection<DisplayTestItem>(testItems);



        [ListView(IsGridView = true)]
        public ICollection<DisplayTestItem> list4 => new ObservableCollection<DisplayTestItem>(testItems);



        /*
         * 
         */
        [SearchProperty("1234567890")]
        public string SearchField2 { get; set; }

        [ListView(IsGridView = true, SearchPropOusideOfClass = true, SearchPropName = "1234567890", ToSearchPropertyName = "Name")]
        public ICollection<DisplayTestItem> list5 => new ObservableCollection<DisplayTestItem>(testItems);



    }




    class DisplayTestItem
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public testingItem Data { get; set; }

        public bool Active { get; set; }

        public decimal Amount { get; set; }
        public Client client { get; set; }

    }

    class Client
    {
        public int ID { get; set; }
        public string Name { get; set; }

    }

    class testingItem
    {
        public string Field1 { get; set; }
        public string Field2 { get; set; }
    }


}
