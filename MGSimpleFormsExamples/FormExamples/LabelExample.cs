
using MGSimpleForms.Attributes;
using MGSimpleForms.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGSimpleFormsExamples.FormExamples
{
    [Form("Label Examples", TitleFontSize = 24)]
    internal class LabelExample : FormViewModel
    {
        public LabelExample()
        {
        }
        public string label1 { get; } = "adsfasdf";

        [Label]
        public string label2 { get; } = "xcvbdf";

        [Name("Name Goes Here")]
        [Label]
        public string label3 { get; } = "ytuyt";

        [Name("CLICK ME!!! - Make Visible:")]
        [CheckBox]
        public bool visible { get => GetProperty<bool>(); set { SetProperty(value); OnPropertyChanged(nameof(NotVisible)); } }
        public bool NotVisible => !visible;

        [Name("Name Goes Here:")]
        [Label(IsVisible = nameof(visible))]
        public string label4 { get; } = "Should be visible when checked.";
        
        [Name("Name Goes Here:")]
        [Label(IsVisible = nameof(NotVisible))]
        public string label5 { get; } = "Should be visible when Not checked.";

        [Name("this should be collapsed:")]
        [Label(IsCollapsed = nameof(visible))]
        public string label6 { get; } = "this should be collapsed.";
        
        [Name("this should be collapsed Not:")]
        [Label(IsCollapsed = nameof(NotVisible))]
        public string label7 { get; } = "this should be collapsed Not.";


        [Label(FontSize = 20)]
        public string label8 { get; } = "Large Font";

        [Label(FontSize = 20, Alignment = TextAlignment.Center)]
        public string label9 { get; } = "Large Font in the Center";


        [Label(FontSize = 20, Alignment = TextAlignment.Right)]
        public string label10 { get; } = "Large Font on the Right";

    }
}
