using MGSimpleForms.Attributes;
using MGSimpleForms.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MGSimpleFormsExamples.EditExamples
{
    [Form(Flow = FormFlow.Columned, BorderSize =7, Border = Border.None)]
    [ColumnedSize(4)]
    internal class ShowItemExample : FormViewModel<TestItem>
    {
        public ShowItemExample()
        {
            Item = new TestItem();
            var testing = GetParent<Window>();
        }


        public List<subItem> Items => new List<subItem>()
        {
            new subItem(){ ID = 1, Name = "Test1"},
            new subItem(){ ID = 2, Name = "Test2"},
            new subItem(){ ID = 4, Name = "Test3"},
        };

        public override void OnFormLoaded()
        {
            base.OnFormLoaded();
            var window = GetWindow();

            window.SizeToContent = SizeToContent.Height;
        }
    }
    class TestItem
    {
        public int Field1 { get; set; }
        public bool Field2 { get; set; }
        public string Field3 { get; set; }
        public decimal Field4 { get; set; }
        public subItem Field5 { get; set; }
        public decimal Field6 { get; set; }
        public decimal Field7 { get; set; }
        public decimal Field8 { get; set; }
        public decimal Field9 { get; set; }
        public decimal Field10 { get; set; }
    }


    class subItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
