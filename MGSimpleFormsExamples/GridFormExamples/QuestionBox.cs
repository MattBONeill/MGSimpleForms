using MGSimpleForms;
using MGSimpleForms.Attributes;
using MGSimpleForms.Form;
using MGSimpleForms.Form.Building;
using MGSimpleForms.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TextAlignment = MGSimpleForms.Attributes.TextAlignment;

namespace MGSimpleFormsExamples.GridFormExamples
{

    [Form(Flow = FormFlow.Grid, BorderSize = 7)]
    [GridSize(1,2)]
    internal class QuestionBox : FormViewModel
    {
        [Location(0,0)]
        [Label(Alignment = TextAlignment.Center)]
        public string Message { get; set; }


        [Location(0,1)]
        [Buttons]
        public List<Command> Buttons { get; set; }

        public int Result { get; set; }

        string Title { get; set; }

        private QuestionBox(string message, string title, IEnumerable<QuestionBoxButton> buttons)
        {
            Message = message;
            Title = title;
            Buttons = buttons.WithIndex().Select(btn => {
                
                return new Command(() =>
                {
                    Result = btn.Index;
                    if (btn.Item.OnClick == null || !btn.Item.OnClick())
                        this.Close();

                }) { Name = btn.Item.Name };
            }).ToList();
        }

        public override void OnFormLoaded()
        {
            base.OnFormLoaded();
            var window = GetWindow();
            window.Title = Title;

            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.ResizeMode = ResizeMode.NoResize;
            window.MaxWidth = 550;
            window.MinWidth = 200;
            window.MinHeight = 150;
            window.WindowStyle = WindowStyle.ToolWindow;
        }

        private QuestionBox(string message, string title, QuestionBoxButtons buttons) : this(message, title, ParseMessageBoxOptions(buttons))
        {
            
        }
        public static (string Option, int Index) Show(string Message, string Title = "", QuestionBoxButtons buttons = QuestionBoxButtons.OK, int buttonDefault = 0)
        {
            var msgbox = new QuestionBox(Message, Title, buttons);

            return msgbox.ShowInWindowDialog() == true ? ((string Option, int Index))(msgbox.Buttons[msgbox.Result].Name, msgbox.Result) : ((string Option, int Index))(null, -1);
        }


        public static (string Option, int Index) Show(string Message, string Title, IEnumerable<QuestionBoxButton> ButtonOptions, int buttonDefault = 0)
        {
            var msgbox = new QuestionBox(Message, Title, ButtonOptions);
            return msgbox.ShowInWindowDialog() == true ? ((string Option, int Index))(msgbox.Buttons[msgbox.Result].Name, msgbox.Result) : ((string Option, int Index))(null, -1);
        }

        static IEnumerable<QuestionBoxButton> ParseMessageBoxOptions(QuestionBoxButtons buttons)
        {
            switch (buttons)
            {
                case QuestionBoxButtons.OK:
                    return new QuestionBoxButton[] { "OK" };
                case QuestionBoxButtons.OKCancel:
                    return new QuestionBoxButton[] { "OK", "Cancel" };
                case QuestionBoxButtons.YesNo:
                    return new QuestionBoxButton[] { "Yes", "No" };
                case QuestionBoxButtons.YesNoCancel:
                    return new QuestionBoxButton[] { "Yes", "No", "Cancel" };
            }
            throw new Exception("Unknown Option");
        }
    }

    public enum QuestionBoxButtons
    {
        //
        // Summary:
        //     The message box displays an OK button.
        OK = 0,
        //
        // Summary:
        //     The message box displays OK and Cancel buttons.
        OKCancel = 1,
        //
        // Summary:
        //     The message box displays Yes, No, and Cancel buttons.
        YesNoCancel = 3,
        //
        // Summary:
        //     The message box displays Yes and No buttons.
        YesNo = 4
    }

    public readonly struct QuestionBoxButton
    {
        public string Name { get; }

        public Func<bool> OnClick { get; }

        public QuestionBoxButton(string name)
        {
            Name = name;
            OnClick = null;
        }

        public QuestionBoxButton(string name, Func<bool> onClick)
        {
            Name = name;
            OnClick = onClick;
        }

        public static implicit operator QuestionBoxButton(string name) => new QuestionBoxButton(name);
        public static implicit operator QuestionBoxButton((string, Func<bool>) item) => new QuestionBoxButton(item.Item1, item.Item2);
    }
}
