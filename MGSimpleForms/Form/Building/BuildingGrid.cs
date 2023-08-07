using MGSimpleForms.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using MGSimpleForms.MVVM;
using Border = MGSimpleForms.Attributes.Border;

namespace MGSimpleForms.Form.Building
{

    internal class BuildingGrid
    {

        public static void Build(Grid grid, FormViewModel ViewModel)
        {
            if (grid == null)
                throw new Exception("No Grid was given to fill");
            if (ViewModel == null)
                throw new Exception("No View Model was given to pull from");

            var builder = new BuilderStates(ViewModel, grid);

            AddTitleToGrid(builder);

            foreach (var prop in builder.ViewModelProperties)
            {
                var BuildConditions = prop.GetCustomAttributes(typeof(BaseFormAttribute), true).Select(i => i as BaseFormAttribute).ToList();

                if (!BuildConditions.Any())
                    continue;

                var Name = BuildConditions.FirstOrDefault(i => i is NameAttribute) as NameAttribute;
                if (Name != null)
                    BuildConditions.Remove(Name);

                var Location = BuildConditions.FirstOrDefault(i => i is LocationAttribute) as LocationAttribute;
                if (Location != null)
                    BuildConditions.Remove(Location);

                var Row = BuildConditions.FirstOrDefault(i => i is RowAttribute) as RowAttribute;
                if (Row != null)
                    BuildConditions.Remove(Row);

                if (BuildConditions.Count == 0)
                    throw new Exception($"Property {prop.Name} is missing Main Build option");


                builder.AddItems(Location, Row, GenerateControl(builder, BuildConditions, prop, ViewModel, Name).ToArray());
            }

            builder.FinishBorder();
        }


        private static void AddTitleToGrid(BuilderStates builder)
        {
            if (!string.IsNullOrEmpty(builder.FormOptions.Title))
            {
                builder.AddTitle(ControlBuilder.BuildTitle(builder.FormOptions.Title, builder.FormOptions.TitleFontSize));
            }
        }

        private static IEnumerable<UIElement> GenerateControl(BuilderStates builder, List<BaseFormAttribute> Attributes, PropertyInfo Prop, FormViewModel ViewModel, NameAttribute Name)
        {
            if (Attributes.Count != 1)
                throw new Exception($"More that one option on the Property: {Prop.Name}");

            var BuildType = Attributes[0];

            var VisualName = Name?.Value ?? string.Empty;

            List<UIElement> itemsToAdd = new List<UIElement>();

            switch (BuildType)
            {

                case LabelAttribute lblOptions:
                    if (!string.IsNullOrEmpty(VisualName))
                    {
                        itemsToAdd.Add(ControlBuilder.BuildLabel(lblOptions, null, ViewModel, VisualName));
                    }

                    itemsToAdd.Add(ControlBuilder.BuildLabel(lblOptions, Prop, ViewModel, null));
                    break;

                case ComboBoxAttribute cboOptions:

                    if (!string.IsNullOrWhiteSpace(cboOptions.NameBinding) || !string.IsNullOrEmpty(VisualName))
                    {
                        var lbl = new LabelAttribute() { IsVisible = cboOptions.IsVisible, IsCollapsed = cboOptions.IsCollapsed };
                        itemsToAdd.Add(ControlBuilder.BuildLabel(lbl, ViewModel.GetProperty(cboOptions.NameBinding), ViewModel, VisualName));
                    }

                    itemsToAdd.Add(ControlBuilder.BuildComboBox(cboOptions, Prop, ViewModel));
                    break;

                case ButtonAttribute btnOptions:

                    itemsToAdd.Add(ControlBuilder.BuildButton(btnOptions, Prop, ViewModel, VisualName));
                    break;

                case ButtonsAttribute btnsOptions:

                    itemsToAdd.Add(ControlBuilder.BuildButtons(btnsOptions, Prop, ViewModel));
                    break;

                /*NOTE: FilePicker needs to be above Textbox*/
                case FilePickerAttribute fpOptions:
                    var lblFilePicker = new LabelAttribute() { IsVisible = fpOptions.IsVisible, IsCollapsed = fpOptions.IsCollapsed };
                    itemsToAdd.Add(ControlBuilder.BuildLabel(lblFilePicker, null, ViewModel, string.IsNullOrWhiteSpace(VisualName) ? "File:" : VisualName));
                    var FilePickerTxt = ControlBuilder.BuildTextBox(fpOptions, Prop, ViewModel);

                    itemsToAdd.Add(FilePickerTxt);

                    itemsToAdd.Add(ControlBuilder.BuildFilePickerButton(fpOptions, FilePickerTxt, ViewModel));
                    break;

                /*NOTE: FolderPicker needs to be above Textbox*/
                case FolderPickerAttribute fdpOptions:
                    var lblFolderPicker = new LabelAttribute() { IsVisible = fdpOptions.IsVisible, IsCollapsed = fdpOptions.IsCollapsed };

                    itemsToAdd.Add(ControlBuilder.BuildLabel(lblFolderPicker, null, ViewModel, string.IsNullOrWhiteSpace(VisualName) ? "Folder:" : VisualName));
                    var FolderPickerTxt = ControlBuilder.BuildTextBox(fdpOptions, Prop, ViewModel);
                    FolderPickerTxt.IsTabStop = false;
                    itemsToAdd.Add(FolderPickerTxt);

                    itemsToAdd.Add(ControlBuilder.BuildFolderPickerButton(fdpOptions, FolderPickerTxt, ViewModel));
                    break;

                case TextBoxAttribute txtOptions:

                    if (!string.IsNullOrEmpty(VisualName))
                    {
                        var lbl = new LabelAttribute() { IsVisible = txtOptions.IsVisible, IsCollapsed = txtOptions.IsCollapsed };
                        itemsToAdd.Add(ControlBuilder.BuildLabel(lbl, null, ViewModel, VisualName));
                    }

                    itemsToAdd.Add(ControlBuilder.BuildTextBox(txtOptions, Prop, ViewModel));
                    break;

                case DatePickAttribute dpOptions:
                    if (!string.IsNullOrEmpty(VisualName))
                    {
                        var lbl = new LabelAttribute() { IsVisible = dpOptions.IsVisible };
                        itemsToAdd.Add(ControlBuilder.BuildLabel(lbl, null, ViewModel, VisualName));
                    }

                    itemsToAdd.Add(ControlBuilder.BuildDatePicker(dpOptions, Prop, ViewModel));
                    break;

                case CheckBoxAttribute:
                    if (!string.IsNullOrEmpty(VisualName))
                    {
                        itemsToAdd.Add(ControlBuilder.BuildLabel(VisualName));
                    }

                    itemsToAdd.Add(ControlBuilder.BuildCheckBox(Prop));
                    break;

                case GeneralControlAttribute:

                    itemsToAdd.Add(ControlBuilder.BuildGeneralControl(Prop));
                    break;


                case ListViewAttribute lvOptions:

                    var ListView = ControlBuilder.BuildListView(lvOptions, Prop, ViewModel);
                    PropertyInfo ListViewSearchProp;

                    if (lvOptions.SearchPropOusideOfClass)
                    {
                        ListViewSearchProp = ViewModel.GetPropertiesByAttribute<SearchPropertyAttribute>(i => i.Key == lvOptions.SearchPropName).FirstOrDefault();
                    }
                    else
                        ListViewSearchProp = ViewModel.GetProperty(lvOptions.SearchPropName);

                    if (ListViewSearchProp != null)
                    {
                        itemsToAdd.Add(ControlBuilder.BuildLabel("Search:"));
                        var searchTxt = ControlBuilder.BuildTextBox(new TextBoxAttribute(), ListViewSearchProp, ViewModel);
                        itemsToAdd.Add(searchTxt);

                        ControlBuilder.AddSearchFileToTextBox(lvOptions, ListView, searchTxt);

                        builder.AddItems(itemsToAdd.ToArray());
                        itemsToAdd.Clear();
                    }

                    itemsToAdd.Add(ListView);
                    break;

            }



            return itemsToAdd;
        }

    }

    class BuilderStates
    {
        //Needed for Vertical
        int ColumnStart = 0;
        int RowStart = 0;

        bool HasTitle = false;

        public BuilderStates(FormViewModel ViewModel, Grid grid)
        {
            FormOptions = ViewModel.GetType()
                .GetCustomAttributes<FormAttribute>().FirstOrDefault() ?? new FormAttribute(string.Empty);
            WorkingGrid = grid;
            ViewModelProperties = GetProperties(ViewModel, FormOptions.PropertyOrder).ToList();

            if (FormOptions.UseMetaDataReorder == true)
                ViewModelProperties = ViewModelProperties.OrderBy(i => i.MetadataToken).ToList();

            var Size = ViewModel.GetType().GetCustomAttributes<GridSizeAttribute>().FirstOrDefault() ?? new GridSizeAttribute(1, 1);
            CreateFormGrid(Size);
        }

        public FormAttribute FormOptions { get; set; }
        public Grid WorkingGrid { get; set; }

        public List<PropertyInfo> ViewModelProperties { get; set; }
        private IEnumerable<PropertyInfo> GetProperties(FormViewModel ViewModel, PropertyOrder PropertyOrder)
        {
            IEnumerable<PropertyInfo> Props;

            switch (PropertyOrder)
            {
                case PropertyOrder.ParentFirst:
                    Props = ViewModel.GetType().GetPropertiesByParent();
                    break;
                case PropertyOrder.ChildFirst:
                    Props = ViewModel.GetType().GetPropertiesByChild();
                    break;
                case PropertyOrder.Default:
                default:
                    Props = ViewModel.GetType().GetProperties();
                    break;
            }

            return Props;
        }

        GridLength CreateBorder()
        {
            if (FormOptions.BorderSize <= 0)
                return new GridLength(1, GridUnitType.Star);
            else
                return new GridLength(FormOptions.BorderSize);
        }

        public void CreateFormGrid(GridSizeAttribute size)
        {
            WorkingGrid.Children.Clear();
            WorkingGrid.ColumnDefinitions.Clear();
            WorkingGrid.RowDefinitions.Clear();

            switch (FormOptions.Flow)
            {
                case FormFlow.Grid:

                    if (size.Rows == 0 && size.Columns == 0)
                        return;

                    if (FormOptions.Border.HasFlag(Border.LeftPadding) == true)
                        WorkingGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = CreateBorder() });
                    if (FormOptions.Border.HasFlag(Border.TopPadding) == true)
                        WorkingGrid.RowDefinitions.Add(new RowDefinition() { Height = CreateBorder() });

                    for (int x = 0; x < size.Columns; x++)
                        WorkingGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    for (int y = 0; y < size.Rows; y++)
                        WorkingGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                    if (FormOptions.Border.HasFlag(Border.RightPadding) == true)
                        WorkingGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = CreateBorder() });

                    if (FormOptions.Border.HasFlag(Border.BottomPadding) == true)
                        WorkingGrid.RowDefinitions.Add(new RowDefinition() { Height = CreateBorder() });
                    break;

                case FormFlow.Horizontal:
                    throw new NotImplementedException();
                case FormFlow.Vertical:
                default:

                    if (FormOptions.Border.HasFlag(Border.LeftPadding) == true)
                    {
                        WorkingGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = CreateBorder() });
                        ColumnStart = 1;
                    }

                    WorkingGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                    WorkingGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
                    WorkingGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                    if (FormOptions.Border.HasFlag(Border.RightPadding) == true)
                        WorkingGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = CreateBorder() });

                    if (FormOptions.Border.HasFlag(Border.TopPadding) == true)
                    {
                        WorkingGrid.RowDefinitions.Add(new RowDefinition() { Height = CreateBorder() });
                        RowStart = 1;
                    }
                    break;
            }

        }

        public void AddItems(params UIElement[] items) => AddItems(null, null, items);
        public void AddItems(RowAttribute Row, params UIElement[] items) => AddItems(null, Row, items);
        public void AddItems(LocationAttribute location, params UIElement[] items) => AddItems(location, null, items);
        public void AddItems(LocationAttribute location, RowAttribute Row, params UIElement[] items)
        {
            if (items == null || items.Length == 0)
                throw new Exception("No Items are Give to Add to Form");

            switch (FormOptions.Flow)
            {
                case FormFlow.Grid:
                    if (items.Length > 1)
                        throw new Exception("Can't add more that 1 items When in Grid Move");
                    if (location == null)
                        throw new Exception("Need a location to Put Item in Grid Mode");

                    var GridItem = items.First();

                    Grid.SetColumn(GridItem, location.Column + (FormOptions.Border.HasFlag(Border.LeftPadding) == true ? 1 : 0));
                    Grid.SetRow(GridItem, location.Row + (HasTitle ? 1 : 0) + (FormOptions.Border.HasFlag(Border.TopPadding) == true ? 1 : 0));

                    if (location.ColumnSpan > 1)
                        Grid.SetColumnSpan(GridItem, location.ColumnSpan);
                    if (location.RowSpan > 1)
                        Grid.SetRowSpan(GridItem, location.RowSpan);

                    WorkingGrid.Children.Add(GridItem);
                    break;

                case FormFlow.Horizontal:
                    throw new NotImplementedException();
                case FormFlow.Vertical:
                default:

                    if (items.Length > 3)
                        throw new Exception("Can't add more that 3 items when in Vertical mode");

                    var CurrentRow = RowStart;
                    RowStart += GenerateRow(Row);

                    int cnt = 0;
                    foreach (var item in items)
                    {
                        Grid.SetColumn(item, ColumnStart + cnt);
                        Grid.SetRow(item, CurrentRow);

                        WorkingGrid.Children.Add(item);
                        cnt++;
                    }
                    if (items.Length == 1)
                        Grid.SetColumnSpan(items[0], 3);
                    else if (items.Length == 2)
                        Grid.SetColumnSpan(items[1], 2);


                    break;
            }
        }

        private int GenerateRow(RowAttribute Row)
        {
            switch (FormOptions.Flow)
            {
                case FormFlow.Grid:
                    return 0;
                case FormFlow.Horizontal:
                    throw new NotImplementedException();
                case FormFlow.Vertical:
                default:
                    return _GenerateRow(Row);
            }
        }
        private int _GenerateRow(RowAttribute Row, int padding = 5)
        {
            switch (Row?.RowSize)
            {
                case RowSize.Star:
                    return _GenerateRow(new GridLength(Row.StarSize, GridUnitType.Star), padding);
                case RowSize.Auto:
                default:
                    return _GenerateRow(GridLength.Auto, padding);
            }

        }
        private int _GenerateRow(GridLength length, int padding = 5)
        {
            WorkingGrid.RowDefinitions.Add(new RowDefinition() { Height = length });
            WorkingGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(padding, GridUnitType.Pixel) });
            return 2;
        }

        public void AddTitle(UIElement Title)
        {
            switch (FormOptions.Flow)
            {
                case FormFlow.Grid:

                    int insertRow = 0;
                    if (FormOptions.Border.HasFlag(Border.TopPadding))
                        insertRow = 1;

                    int insertColumn = 0;
                    if (FormOptions.Border.HasFlag(Border.LeftPadding))
                        insertColumn = 1;

                    WorkingGrid.RowDefinitions.Insert(insertRow, new RowDefinition() { Height = GridLength.Auto });
                    AddItems(new LocationAttribute(insertColumn, insertRow), Title);
                    HasTitle = true;
                    break;

                case FormFlow.Horizontal:
                    throw new NotImplementedException();
                case FormFlow.Vertical:
                default:
                    AddItems(Title);
                    break;
            }
        }

        public void FinishBorder()
        {
            switch (FormOptions.Flow)
            {
                case FormFlow.Grid:
                    return;
                case FormFlow.Horizontal:
                    throw new NotImplementedException();
                case FormFlow.Vertical:
                default:
                    if (FormOptions.Border.HasFlag(Border.BottomPadding) == true)
                        WorkingGrid.RowDefinitions.Add(new RowDefinition() { Height = CreateBorder() });
                    break;
            }
        }
    }

    public static class BuildGridExt
    {
        public static IEnumerable<PropertyInfo> GetPropertiesByParent(this Type T)
        {
            var props = T.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            return T.BaseType == null ? props : T.BaseType.GetPropertiesByParent().Concat(props);
        }

        public static IEnumerable<PropertyInfo> GetPropertiesByChild(this Type T)
        {
            var props = T.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            return T.BaseType == null ? props : props.Concat(T.BaseType.GetPropertiesByChild());
        }
        public static IEnumerable<PropertyInfo> GetPropertiesByAttribute<T>(this FormViewModel viewModel, Func<T, bool> search)
        {
            return viewModel
                .GetType()
                .GetProperties()
                .Where(prop =>
                {
                    foreach (var attr in prop.GetCustomAttributes(typeof(T), true))
                    {
                        if (search((T)attr))
                            return true;
                    }
                    return false;
                });
        }
        public static void AddItems(this Grid grid, int Column, int row, params UIElement[] items)
        {
            if (items.Length > 3)
                throw new Exception("Can't add more that 3 items to Grid at a time");
            int cnt = 0;
            foreach (var item in items)
            {
                Grid.SetColumn(item, Column + cnt);
                Grid.SetRow(item, row);

                grid.Children.Add(item);
                cnt++;
            }
            if (items.Length == 1)
                Grid.SetColumnSpan(items[0], 3);
            else if (items.Length == 2)
                Grid.SetColumnSpan(items[1], 2);

        }

    }
}
