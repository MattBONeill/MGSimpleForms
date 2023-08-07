using MGSimpleForms.Attributes;
using MGSimpleForms.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGSimpleFormsExamples.FormExamples
{
    [Form("Date Picker and Check Box Examples", TitleFontSize = 24)]
    internal class DatePickerCheckBoxExample : FormViewModel
    {
        public DatePickerCheckBoxExample()
        {
            date2 = new DateTime(2022, 08, 02);
        }
        public DateTime date1 { get => GetProperty<DateTime>(); set => SetProperty(value); }

        [DatePick]
        public DateTime date2 { get => GetProperty<DateTime>(); set => SetProperty(value); } 

        [Name("Well Hello")]
        [DatePick]
        public DateTime date3 { get => GetProperty<DateTime>(); set => SetProperty(value); }

        public bool Check1 { get => GetProperty<bool>(); set => SetProperty(value); }
        [CheckBox]
        public bool Check2 { get => GetProperty<bool>(); set => SetProperty(value); }

        [Name("There")]

        [CheckBox]
        public bool Check3 { get => GetProperty<bool>(); set => SetProperty(value); }

    }
}
