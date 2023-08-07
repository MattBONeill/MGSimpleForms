using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGSimpleForms.Attributes
{

    public abstract class BaseFormAttribute : Attribute { }

    /// <summary>
    /// Generate a Label in front of Object
    /// </summary>
    public sealed class NameAttribute : BaseFormAttribute
    {
        public string Value { get; set; }

        public NameAttribute(string name)
        {
            Value = name;
        }

    }

    public sealed class LocationAttribute : BaseFormAttribute
    {
        public int Column { get; }
        public int Row { get; }
        public int ColumnSpan { get; }
        public int RowSpan { get; }

        public LocationAttribute(int column, int row, int columnspan = 1, int rowspan = 1)
        {
            Column = column;
            Row = row;
            ColumnSpan = columnspan;
            RowSpan = rowspan;
        }

    }


    public sealed class RowAttribute : BaseFormAttribute
    {
        public RowSize RowSize { get; set; } = RowSize.Auto;
        public int StarSize { get; set; } = 1;

    }
    
    public class AdvancedProps : BaseFormAttribute
    {
        public string IsEnabled { get; set; }
        public string IsVisible { get; set; }
        public string IsCollapsed { get; set; }

        public string NameBinding { get; set; }
    }

    public enum TextAlignment
    {
        Left,
        Center,
        Right
    }


    public sealed class ComboBoxAttribute : AdvancedProps
    {
        public bool IsEditable { get; set; } = true;
        public string DataTemplate { get; set; }
        public string DataTemplateTextPath { get; set; }
        public string SelectedItem { get; set; }

    }

    public sealed class TextBoxAttribute : AdvancedProps
    {
        public TextAlignment Alignment { get; set; } = TextAlignment.Center;
        public int MaxLength { get; set; } = -1;
    }

    public sealed class FilePickerAttribute : AdvancedProps
    {
        public string Filter { get; set; }
        public bool ReadOnly { get; set; } = false;
    }

    public sealed class FolderPickerAttribute : AdvancedProps
    {
        public bool ReadOnly { get; set; } = false;
    }

    public sealed class ButtonAttribute : BaseFormAttribute
    {
        public string TextBinding { get; set; }
        public string IsEnabled { get; set; }
    }

    public sealed class ButtonsAttribute : BaseFormAttribute
    {
        public bool MultiRow { get; set; } = false;

        public int MaxColumnCount { get; set; } = 3;

    }


    public sealed class DatePickAttribute : BaseFormAttribute
    {
        public string IsVisible { get; set; }
    }


    public sealed class CheckBoxAttribute : BaseFormAttribute { }

    public sealed class GeneralControlAttribute : BaseFormAttribute
    {
    }


    public sealed class SearchPropertyAttribute : BaseFormAttribute
    {
        public string Key { get; set; }
        public SearchPropertyAttribute(string key)
        {
            Key = key;
        }
    }
    public enum TextWrap
    {
        //
        // Summary:
        //     Line-breaking occurs if the line overflows beyond the available block width.
        //     However, a line may overflow beyond the block width if the line breaking algorithm
        //     cannot determine a line break opportunity, as in the case of a very long word
        //     constrained in a fixed-width container with no scrolling allowed.
        WrapWithOverflow = 0,
        //
        // Summary:
        //     No line wrapping is performed.
        NoWrap = 1,
        //
        // Summary:
        //     Line-breaking occurs if the line overflows beyond the available block width,
        //     even if the standard line breaking algorithm cannot determine any line break
        //     opportunity, as in the case of a very long word constrained in a fixed-width
        //     container with no scrolling allowed.
        Wrap = 2
    }

    public sealed class LabelAttribute : BaseFormAttribute
    {
        public int FontSize { get; set; } = 0;
        public TextAlignment Alignment { get; set; } = TextAlignment.Left;
        public TextWrap Wrap { get; set; } = TextWrap.WrapWithOverflow;
        

        public string IsVisible { get; set; }
        public string IsCollapsed { get; set; } 
    }

    public abstract class ListBaseAttribute : BaseFormAttribute
    {
        public string SelectedItem { get; set; }

        public string SearchPropName { get; set; }

        public string ToSearchPropertyName { get; set; }

        public bool SearchPropOusideOfClass { get; set; } = false;


    }


    public sealed class ListViewAttribute : ListBaseAttribute
    {
        public string DataTemplatePropName { get; set; }
        public string DataTemplateTextPath { get; set; }

        public bool IsGridView { get; set; }
        public string IsGridViewProp { get; set; }

        public bool EnableField { get; set; } = false;

        public string ContextMenuItemPropName { get; set; }

        /// <summary>
        /// Needs to be a Function(Func<PropertyInfo, int>),
        /// Parameter: PropertyInfo
        /// Return: bool
        /// false for Column to be Hidden
        /// True for Column to be shown
        /// </summary>
        public string GridFieldCheckPropName { get; set; }
    }
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class FormAttribute : BaseFormAttribute
    {
        public FormAttribute()
        {
            
        }
        public FormAttribute(string title = "")
        {
            Title = title;
        }

        public string Title { get; set; } = string.Empty;
        public int TitleFontSize { get; set; } = 16;
        public FormFlow Flow { get; set; } = FormFlow.Vertical;
        public Border Border { get; set; } = Border.FullPadding;
        public double BorderSize { get; set; } = 0;
        public PropertyOrder PropertyOrder { get; set; } = PropertyOrder.ParentFirst;
        public bool UseMetaDataReorder { get; set; }
    }

    public enum FormFlow
    {
        Vertical,
        Horizontal,
        Grid
    }

    public enum PropertyOrder
    {
        Default,
        ParentFirst,
        ChildFirst,
    }

    [Flags]
    public enum Border : int
    {
        None = 0,
        LeftPadding = 1,
        RightPadding = 2,
        TopPadding = 4,
        BottomPadding = 8,

        SidesPadding = 3,
        TopBottomPadding = 12,
        FullPadding = 15,
    }

    public enum RowSize
    {
        Auto,
        Star
    }

    public sealed class GridSizeAttribute : BaseFormAttribute
    {
        public int Columns { get; }
        public int Rows { get; }

        public GridSizeAttribute(int columns, int rows)
        {
            Columns = columns;
            Rows = rows;
        }
    }

}
