using MGSimpleForms.Attributes;
using MGSimpleForms.Form;
using MGSimpleForms.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MGSimpleForms.Special
{
    //[Form(Flow = FormFlow.Grid, BorderSize = 5)]
    //[GridSize(6, 6)]
    //class Wrapper : FormViewModel
    //{

    //    [GeneralControl]
    //    [Location(0, 0, 10, 5)]
    //    public FormUserControl test { get; }


    //    [Name("Save")]
    //    [Button]
    //    [Location(2, 5)]
    //    public ICommand Save => new Command(() => this.Close(true));

    //    [Name("Cancel")]
    //    [Button]
    //    [Location(3, 5)]
    //    public ICommand Cancel => new Command(() => this.Close(false));

    //    public Wrapper(FormViewModel testing)
    //    {
    //        test = new FormUserControl() { DataContext = testing };
    //    }

    //    public override void OnFormLoaded()
    //    {
    //        base.OnFormLoaded();
    //        var window = GetWindow() as FormWindow;

    //        if (window == null)
    //            return;

    //        test.CustomBuildOptions = window.CustomBuildOptions;
    //    }

    //}

}
