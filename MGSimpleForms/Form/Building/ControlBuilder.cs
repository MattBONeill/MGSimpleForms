using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using MGSimpleForms.Attributes;
using Microsoft.Win32;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;
using MGSimpleForms.MVVM;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.ComponentModel;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Security.Cryptography;

namespace MGSimpleForms.Form.Building
{

    public class ControlBuilder
    {
        public static FrameworkElement BuildTitle(string Title, int fontsize, string TitleBinding)
        {
            if(!string.IsNullOrEmpty(Title))
                return new TextBlock() { Text = Title, FontSize = fontsize, TextAlignment = System.Windows.TextAlignment.Center };
            else
            {
                var txt = new Label() { FontSize = fontsize,
                    HorizontalContentAlignment = HorizontalAlignment.Center, 
                    VerticalContentAlignment = VerticalAlignment.Center,
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(0),
                    Padding = new Thickness(0),
                };//, TextAlignment = System.Windows.TextAlignment.Center };
                txt.SetBinding(Label.ContentProperty, TitleBinding);

                //txt.changed
                var dp = DependencyPropertyDescriptor.FromProperty(Label.ContentProperty, typeof(Label));
                dp.AddValueChanged(txt, (sender, args) =>
                {
                    if (string.IsNullOrWhiteSpace(txt.Content.ToString()))
                    {
                        txt.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txt.Visibility = Visibility.Visible;
                        //MessageBox.Show("Visible");
                    }
                });
                return txt;
            }
        }

        public static FrameworkElement BuildLabel(string Name, PropertyInfo prop = null) => BuildLabel(new LabelAttribute(), prop, null, Name);

        public static FrameworkElement BuildLabel(LabelAttribute lblOptions, PropertyInfo prop, ViewModel viewModel, string Name)
        {
            var lbl = new Label() { VerticalAlignment = VerticalAlignment.Center };

            var txt = new TextBlock() { VerticalAlignment = VerticalAlignment.Center };
            if (prop != null)
                txt.SetBinding(TextBlock.TextProperty, prop.Name);
            else
                txt.Text = Name;

            if (lblOptions.FontSize > 0)
                txt.FontSize = lblOptions.FontSize;

            //var temp = new TextBlock();
            
            switch (lblOptions.Wrap)
            {
                case Attributes.TextWrap.Wrap:
                    txt.TextWrapping = TextWrapping.Wrap;
                    break;
                case Attributes.TextWrap.NoWrap:
                    txt.TextWrapping = TextWrapping.NoWrap;
                    break;
                case Attributes.TextWrap.WrapWithOverflow:
                    txt.TextWrapping = TextWrapping.WrapWithOverflow;
                    break;
            }


            switch (lblOptions.Alignment)
            {
                case Attributes.TextAlignment.Left:
                    txt.HorizontalAlignment = HorizontalAlignment.Left;
                    lbl.HorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case Attributes.TextAlignment.Center:
                    txt.HorizontalAlignment = HorizontalAlignment.Center;
                    lbl.HorizontalAlignment = HorizontalAlignment.Center;
                    break;
                case Attributes.TextAlignment.Right:
                    txt.HorizontalAlignment = HorizontalAlignment.Right;
                    lbl.HorizontalAlignment = HorizontalAlignment.Right;
                    break;
            }



            BindVisible(lblOptions.IsVisible, lbl, viewModel);

            BindCollapsed(lblOptions.IsCollapsed, lbl, viewModel);

            lbl.Content = txt;
            return lbl;
        }

        private static void FillOptions(AdvancedProps AdvancedProps, FrameworkElement element, ViewModel viewModel)
        {
            BindVisible(AdvancedProps.IsVisible, element, viewModel);

            BindCollapsed(AdvancedProps.IsCollapsed, element, viewModel);

            BindEnabled(AdvancedProps.IsEnabled, element, viewModel);
        }

        private static void BindVisible(string propName, FrameworkElement element, ViewModel viewModel)
        {
            var IsVisible = viewModel.GetProperty(propName);
            if (IsVisible != null)
            {
                element.SetBinding(FrameworkElement.VisibilityProperty, new Binding(propName) { Converter = IsVisible.PropertyType == typeof(bool) ? new BooleanToVisible() : null });
            }
        }

        private static void BindCollapsed(string propName, FrameworkElement element, ViewModel viewModel)
        {
            var IsCollapsed = viewModel.GetProperty(propName);
            if (IsCollapsed != null)
                element.SetBinding(FrameworkElement.VisibilityProperty, new Binding(propName) { Converter = IsCollapsed.PropertyType == typeof(bool) ? new BooleanToCollapsed() : null });
        }

        private static void BindEnabled(string propName, FrameworkElement element, ViewModel viewModel)
        {
            var IsEnabled = viewModel.GetProperty(propName);
            if (IsEnabled != null)
                element.SetBinding(FrameworkElement.IsEnabledProperty, new Binding(propName));
        }

        public static ComboBox BuildComboBox(ComboBoxAttribute cboOptions, PropertyInfo prop, FormViewModel viewModel)
        {
            if (!prop.PropertyType.IsParent(typeof(IEnumerable)))
                throw new Exception($"{prop.Name} can't be a ComboBox; ensure it has IEnumerable as Parent");

            var cbo = new ComboBox();
            viewModel.AddItemToDisable(cbo);

            if (cboOptions.IsEditable)
            {
                cbo.IsEditable = true;
                cbo.IsTextSearchEnabled = true;
                cbo.IsTextSearchCaseSensitive = false;
                cbo.StaysOpenOnEdit = true;
                cbo.GotFocus += ControlBuilderHelper.cbo_OpenDropdown;
                cbo.LostFocus += ControlBuilderHelper.cbo_CloseDropdown;
            }
            cbo.SetBinding(ItemsControl.ItemsSourceProperty, prop.Name);

            //Set up Template
            var Template = viewModel.GetProperty(cboOptions.DataTemplate);
            if (Template == null)
            {
                Type subType = prop.PropertyType.GenericTypeArguments.FirstOrDefault();

                if (subType != null && subType.IsClass && !subType.IsPrimitive && subType != typeof(string))
                {
                    var props = subType.GetProperties();
                    string propName = "";
                    if (!string.IsNullOrWhiteSpace(cboOptions.DataTemplateTextPath))
                        propName = cboOptions.DataTemplateTextPath;
                    else if (props.Length == 0)
                        throw new Exception($"No Properties found for {subType.Name}");
                    else
                    {
                        propName = props.Length == 1
                            ? props[0].Name
                            : props.First(i => i.Name != "ID").Name;
                    }

                    TextSearch.SetTextPath(cbo, propName);
                    cbo.AddDataTemplate(() =>
                    {
                        var lbl = new Label();
                        lbl.IsTabStop = false;
                        lbl.SetBinding(ContentControl.ContentProperty, propName);
                        return lbl;
                    });
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(cboOptions.DataTemplateTextPath))
                    TextSearch.SetTextPath(cbo, cboOptions.DataTemplateTextPath);
                cbo.SetBinding(ItemsControl.ItemTemplateProperty, Template.Name);
            }

            //Set up Selected Item
            var SelectedItem = viewModel.GetProperty(cboOptions.SelectedItem);
            if (SelectedItem != null)
            {
                cbo.SetBinding(System.Windows.Controls.Primitives.Selector.SelectedItemProperty, cboOptions.SelectedItem);
            }

            cbo.ItemsPanel = DataTemplateGenerator.CreateVirtualizingItemTemplate();

            //Set Up Visibility
            FillOptions(cboOptions, cbo, viewModel);
            return cbo;
        }

        public static Button BuildButton(ButtonAttribute btnOptions, PropertyInfo prop, FormViewModel viewModel, string Name)
        {
            if (!prop.PropertyType.IsParent(typeof(ICommand)))
                throw new Exception($"{prop.Name} can't be a Button; ensure it has ICommand as Parent");

            var btn = new Button();
            btn.SetBinding(System.Windows.Controls.Primitives.ButtonBase.CommandProperty, prop.Name);

            //set up Text 
            var TextProp = viewModel.GetProperty(btnOptions.TextBinding);
            if (TextProp != null)
                btn.SetBinding(ContentControl.ContentProperty, btnOptions.TextBinding);
            else
                btn.Content = Name;



            BindEnabled(btnOptions.IsEnabled, btn, viewModel);

            viewModel.AddItemToDisable(btn);


            return btn;
        }

        public static UIElement BuildButtons(ButtonsAttribute btnsOptions, PropertyInfo prop, FormViewModel viewModel)
        {
            if (!prop.PropertyType.IsParent(typeof(IEnumerable)))
                throw new Exception($"{prop.Name} can't be a Series Button; ensure it has IEnumerable as Parent");

            if (!prop.PropertyType.GenericTypeArguments[0].IsParent(typeof(ICommand)))// != typeof(Command))
                throw new Exception($"{prop.Name} can't be a Series Button; ensure it has IEnumerable<ICommand> as Parent");

            var Buttons = (IEnumerable<ICommand>)prop.GetValue(viewModel);

            Grid grid = new Grid();
            int ColumnIndex = 0;
            int LastRowColumn = -1;
            int cnt = -1;

            var btnCount = Buttons.Count();

            if (btnsOptions.MultiRow)
            {
                if (btnsOptions.MaxColumnCount < 1)
                    throw new Exception($"{prop.Name} Column count must be 1 or more");

                for (int columns = 0; columns < Math.Min(btnCount, btnsOptions.MaxColumnCount); columns++)
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                LastRowColumn = (int)Math.Ceiling((double)btnCount / btnsOptions.MaxColumnCount);
                for (int columns = 0; columns < LastRowColumn; columns++)
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            }
            foreach (var button in Buttons)
            {
                cnt++;
                var thickness = new Thickness(cnt == 0 ? 0 : 1, 0, cnt == btnCount ? 0 : 1, 0);
                var btn = new Button() { };

                btn.SetBinding(System.Windows.Controls.Primitives.ButtonBase.CommandProperty, prop.Name + $"[{cnt}]");
                btn.SetBinding(ContentControl.ContentProperty, prop.Name + $"[{cnt}].Name");


                if (btnsOptions.MultiRow)
                {
                    ColumnIndex = cnt % btnsOptions.MaxColumnCount;
                    int RowIndex = cnt / btnsOptions.MaxColumnCount;

                    Grid.SetColumn(btn, ColumnIndex);
                    Grid.SetRow(btn, RowIndex);

                    if (ColumnIndex == 0)
                        thickness.Left = 0;
                    else if (ColumnIndex == btnsOptions.MaxColumnCount - 1)
                        thickness.Right = 0;

                    thickness.Bottom = RowIndex == LastRowColumn - 1 ? 0 : 2;
                }
                else
                {
                    //Data column
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                    Grid.SetColumn(btn, ColumnIndex);

                    ColumnIndex = grid.ColumnDefinitions.Count;
                }

                btn.Margin = thickness;

                grid.Children.Add(btn);
                viewModel.AddItemToDisable(btn);
            }

            return grid;
        }

        public static TextBox BuildTextBox(AdvancedProps txtOptions, PropertyInfo prop, FormViewModel viewModel)
        {
            var txt = new TextBox();
            if(prop.CanWrite)
                txt.SetBinding(TextBox.TextProperty, new Binding(prop.Name) { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.LostFocus });
            else
                txt.SetBinding(TextBox.TextProperty, new Binding(prop.Name) { Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.LostFocus });

            FillOptions(txtOptions, txt, viewModel);
            if (!prop.CanWrite)
                txt.IsReadOnly = true;
            //var IsCollapsed = viewModel.GetProperty(txtOptions.IsCollapsed);
            //if (IsCollapsed != null)
            //{
            //    //txt.SetBinding(TextBox.VisibilityProperty, new Binding("teststring11") { Converter = new BooleanToCollapsed() });
            //    var temp2= txt.SetBinding(TextBox.VisibilityProperty, new Binding(txtOptions.IsCollapsed) { Converter = new BooleanToCollapsed() });

            //    txt.DataContextChanged += (s, e) =>
            //    {
            //        {
            //            txt.Text = txt.Text;
            //            var temp = txt.GetBindingExpression(TextBox.VisibilityProperty);

            //        }

            //    };
            //}

            viewModel.AddItemToDisable(txt);
            return txt;
        }

        public static TextBox BuildTextBox(TextBoxAttribute txtOptions, PropertyInfo prop, FormViewModel viewModel)
        {
            var txt = BuildTextBox((AdvancedProps)txtOptions, prop, viewModel);

            if (txtOptions.MaxLength > 0)
                txt.MaxLength = txtOptions.MaxLength;

            txt.TextWrapping = TextWrapping.Wrap;

            switch (txtOptions.Alignment)
            {
                case Attributes.TextAlignment.Center:
                    txt.VerticalContentAlignment = VerticalAlignment.Center;
                    txt.HorizontalContentAlignment = HorizontalAlignment.Center;
                    break;
                case Attributes.TextAlignment.Left:
                    txt.HorizontalContentAlignment = HorizontalAlignment.Left;
                    txt.VerticalContentAlignment = VerticalAlignment.Center;
                    break;
                case Attributes.TextAlignment.Right:
                    txt.HorizontalContentAlignment = HorizontalAlignment.Right;
                    txt.VerticalContentAlignment = VerticalAlignment.Center;
                    break;
            }

            switch (txtOptions.Wrap)
            {
                case Attributes.TextWrap.Wrap:
                    txt.TextWrapping = TextWrapping.Wrap;
                    break;
                case Attributes.TextWrap.NoWrap:
                    txt.TextWrapping = TextWrapping.NoWrap;
                    break;
                case Attributes.TextWrap.WrapWithOverflow:
                    txt.TextWrapping = TextWrapping.WrapWithOverflow;
                    break;
            }

            return txt;
        }

        public static Button BuildFilePickerButton(FilePickerAttribute fpOptions, TextBox txt, FormViewModel viewModel)
        {
            var btn = new Button() { Content = "...", Focusable = true };

            txt.IsReadOnly = fpOptions.ReadOnly;

            txt.IsTabStop = false;

            viewModel.AddItemToDisable(btn);

            btn.Click += (sender, e) =>
            {
                //txt.Text = null;
                var ofd = new OpenFileDialog();
                ofd.Filter = fpOptions.Filter;
                ofd.FileName = txt.Text;
                if (!string.IsNullOrWhiteSpace(txt.Text))
                {
                    var dir = Path.GetDirectoryName(txt.Text);
                    while (!Directory.Exists(dir))
                    {
                        dir = Path.GetDirectoryName(dir);
                    }
                    ofd.InitialDirectory = dir;
                }

                if (ofd.ShowDialog() == true)
                {
                    txt.Text = ofd.FileName;
                    //idk why i need this line, but without it the String field in the ViewModel doesn't get updated.
                    txt.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                }
            };


            FillOptions(fpOptions, btn, viewModel);

            return btn;
        }

        public static Button BuildFolderPickerButton(FolderPickerAttribute fpOptions, TextBox txt, FormViewModel viewModel)
        {
            var btn = new Button() { Content = "..." };
            viewModel.AddItemToDisable(btn);
            txt.IsReadOnly = fpOptions.ReadOnly;

            btn.Click += (sender, e) =>
            {
                // Create a "Save As" dialog for selecting a directory (HACK)
                var dialog = new SaveFileDialog();

                if (!string.IsNullOrWhiteSpace(txt.Text))
                {
                    var dir = Path.GetDirectoryName(txt.Text);
                    while (!Directory.Exists(dir))
                    {
                        dir = Path.GetDirectoryName(dir);
                    }
                    dialog.InitialDirectory = dir;
                }
                //dialog.InitialDirectory = txt.Text; // Use current value for initial dir
                //txt.Text = null;

                dialog.Title = "Select a Directory"; // instead of default "Save As"
                dialog.Filter = "Directory|*.this.directory"; // Preve displaying files
                dialog.FileName = "select"; // Filename will then be "select.this.directory"
                if (dialog.ShowDialog() == true)
                {
                    string path = dialog.FileName;
                    // Remove fake filename from resulting path
                    path = path.Replace("\\select.this.directory", "");
                    path = path.Replace(".this.directory", "");

                    txt.Text = path;
                    //idk why i need this line, but without it the String field in the ViewModel doesn't get updated.
                    txt.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                }
            };


            FillOptions(fpOptions, btn, viewModel);
            return btn;
        }

        public static DatePicker BuildDatePicker(DatePickAttribute dpOptions, PropertyInfo prop, ViewModel viewModel)
        {
            if (prop.PropertyType != typeof(DateTime))
                throw new Exception($"{prop.Name} can't be a DatePicker; ensure it is a DateTime");

            var dp = new DatePicker();
            dp.SetBinding(DatePicker.ValueProperty, prop.Name);


            //Set Up Visibility

            var IsVisible = viewModel.GetProperty(dpOptions.IsVisible);
            if (IsVisible != null)
                dp.SetBinding(UIElement.VisibilityProperty, new Binding(dpOptions.IsVisible) { Converter = IsVisible.PropertyType != typeof(bool) ? null : new BooleanToVisible() });



            return dp;
        }

        public static CheckBox BuildCheckBox(PropertyInfo prop, FormViewModel viewModel)
        {
            var chk = new CheckBox() { VerticalAlignment = VerticalAlignment.Center };
            chk.SetBinding(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty, prop.Name);
            viewModel.AddItemToDisable(chk);
            return chk;
        }

        public static ContentPresenter BuildGeneralControl(PropertyInfo prop)
        {
            var cp = new ContentPresenter();
            cp.SetBinding(ContentPresenter.ContentProperty, prop.Name);
            return cp;
        }

        public static void AddSearchFileToTextBox(ListBaseAttribute options, ListBox gridList, TextBox searchText)
        {
            searchText.TextChanged += (sender, e) =>
            {
                AddFilterToList(options.ToSearchPropertyName, gridList, searchText);
            };
        }

        public static void AddFilterToList(string PropertyToSearch, ListBox gridList, TextBox searchText)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(gridList.ItemsSource);
            if (view.Filter == null)
                view.Filter = obj =>
                {
                    if (string.IsNullOrWhiteSpace(searchText.Text))
                        return true;
                    var SearchObj = obj;
                    PropertyInfo prop;
                    if (!string.IsNullOrWhiteSpace(PropertyToSearch))
                    {
                        var tempprop = obj.GetType().GetProperty(PropertyToSearch);
                        if (tempprop != null)
                        {
                            if (tempprop.PropertyType.IsClass)
                            {
                                prop = tempprop.PropertyType.GetProperties().FirstOrDefault(i => i.Name != "ID");
                                SearchObj = tempprop.GetValue(obj);
                            }
                            else
                                prop = tempprop;
                        }
                        else
                            prop = obj.GetType().GetProperties().FirstOrDefault(i => i.Name != "ID");
                    }
                    else
                        prop = obj.GetType().GetProperties().FirstOrDefault(i => i.Name != "ID");

                    if (SearchObj == null)
                        return false;

                    var value = prop == null ? SearchObj.ToString() : prop.GetValue(SearchObj).ToString();
                    if (SearchObj is string)
                        value = SearchObj.ToString();

                    return Regex.IsMatch(value, searchText.Text, RegexOptions.IgnoreCase);
                };
            view.Refresh();

        }





        public static ListView GetListGridView(string Name, Type lstType, string Selected = "", bool AdjustSize = true)
        {
            var ItemContainerStyle = new Style(typeof(ListViewItem));
            ItemContainerStyle.Setters.Add(new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Center));

            var lst = new ListView()
            {
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                ItemContainerStyle = ItemContainerStyle,
            };
            lst.SetBinding(ItemsControl.ItemsSourceProperty, Name);
            if (!string.IsNullOrEmpty(Selected))
                lst.SetBinding(System.Windows.Controls.Primitives.Selector.SelectedItemProperty, Selected);

            var gridView = new GridView();
            //var lstType = PropertyType.GenericTypeArguments[0];
            var isValueTuple = lstType.Name.StartsWith("ValueTuple");
            IEnumerable<MemberInfo> Columns = isValueTuple ? lstType.GetFields() : lstType.GetProperties();
            Func<MemberInfo, Type> getType = isValueTuple ? fld => (fld as FieldInfo).FieldType : prop => (prop as PropertyInfo).PropertyType;

            List<MemberInfo> GroupByFields = new List<MemberInfo>();

            foreach (var prop in Columns)
            {
                var PropName = prop.Name;
                var type = getType(prop);

                //if (prop.CustomAttributes.Any(i => i.AttributeType == typeof(GroupBy)))
                //{
                //    GroupByFields.Add(prop);
                //    continue;
                //}


                if (type.IsClass && type != typeof(string))
                {
                    var AddName = type.GetProperties().FirstOrDefault(i => i.Name != "ID");
                    if (AddName != null)
                        PropName += $".{AddName.Name}";
                }


                var column = new GridViewColumn()
                {
                    Header = prop.Name,
                    DisplayMemberBinding = new Binding(isValueTuple ? "." : PropName)
                    {
                        Converter = isValueTuple ? new FieldValueConverter(prop.Name) : null
                    },
                    //HeaderStringFormat = (type == typeof(decimal)) ? "{}{0:0.00}" : string.Empty
                };

                if (type == typeof(bool))
                {
                    column.DisplayMemberBinding = null;

                    var template = DataTemplateGenerator.CreateDataTemplate(() =>
                    {

                        var chk = new CheckBox();
                        chk.IsTabStop = false;
                        chk.SetBinding(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty, new Binding(prop.Name)
                        {
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        });
                        return chk;
                    });

                    column.CellTemplate = template;
                }
                else if (type == typeof(decimal))
                {
                    column.DisplayMemberBinding = null;

                    var template = DataTemplateGenerator.CreateDataTemplate(() =>
                    {
                        var txt = new TextBlock();
                        txt.SetBinding(TextBlock.TextProperty, new Binding(prop.Name)
                        {
                            Mode = BindingMode.OneWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            StringFormat = "0.00"
                        });
                        return txt;
                    });

                    column.CellTemplate = template;
                }



                gridView.Columns.Add(column);
            }



            //var groupTemplate = DataTemplateGenerator.CreateDataTemplate(() => {
            //    var grd = new Grid();
            //    foreach (var prop in GroupByFields)
            //    {
            //        var PropName = prop.Name;
            //        var type = getType(prop);
            //        grd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            //        txt
            //    }
            //});
            //lst.GroupStyle.Add(new GroupStyle()
            //{
            //    HeaderTemplate = groupTemplate
            //});

            lst.View = gridView;
            if (AdjustSize)
                lst.SizeChanged += (sender, e) =>
                {
                    var listview = sender as ListView;

                    var gridView = listview.View as GridView;
                    var width = listview.RenderSize.Width - listview.FontSize * 1.75;

                    var newWidth = (width - (gridView.Columns.Count + 1)) / gridView.Columns.Count;
                    if (gridView.Columns.Count == 1)
                        newWidth = width;
                    foreach (var column in gridView.Columns)
                    {
                        column.Width = newWidth;
                    }
                };
            return lst;
        }

        public static ListView BuildListView(ListViewAttribute lvOptions, PropertyInfo prop, FormViewModel viewModel)
        {
            if (!prop.PropertyType.IsParent(typeof(IEnumerable)))
                throw new Exception($"{prop.Name} can't be a List View; ensure it has IEnumerable as Parent");

            var lst = new ListView() { HorizontalContentAlignment = HorizontalAlignment.Stretch };

            lst.SetBinding(ItemsControl.ItemsSourceProperty, prop.Name);

            var SelectedItem = viewModel.GetProperty(lvOptions.SelectedItem);
            if (SelectedItem != null)
            {
                lst.SetBinding(System.Windows.Controls.Primitives.Selector.SelectedItemProperty, lvOptions.SelectedItem);
            }
            //set style targeting ListViewItem
            lst.ItemContainerStyle = new Style(typeof(ListViewItem));


            var GridViewProp = viewModel.GetProperty(lvOptions.IsGridViewProp);
            if (GridViewProp != null && (bool)viewModel.GetValue(lvOptions.IsGridViewProp) == true || lvOptions.IsGridView)
            {
                var ItemContainerStyle = lst.ItemContainerStyle;
                ItemContainerStyle.Setters.Add(new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Center));

                var GridItemType = prop.PropertyType.GenericTypeArguments[0];
                var gridView = new GridView();
                ScrollViewer.SetCanContentScroll(gridView, true);

                Predicate<PropertyInfo> FieldCheck;
                var FieldCheckProp = viewModel.GetProperty(lvOptions.GridFieldCheckPropName);
                if (FieldCheckProp != null)
                {
                    if (FieldCheckProp.PropertyType != typeof(Predicate<PropertyInfo>))
                    {
                        throw new Exception($"{prop.Name} need to be of Type Func<PropertyInfo, int>");
                    }
                    FieldCheck = (Predicate<PropertyInfo>)viewModel.GetValue(lvOptions.GridFieldCheckPropName);//FieldCheckProp.GetValue(viewModel);
                }
                else
                    FieldCheck = p => true;

                foreach (var gProp in GridItemType.GetProperties())
                {
                    var PropName = gProp.Name;
                    var type = gProp.PropertyType;

                    if (PropName.ToUpperInvariant().EndsWith("ID"))
                        continue;


                    if (!FieldCheck(gProp))
                        continue;


                    if (type.IsClass && type != typeof(string))
                    {
                        var AddName = type.GetProperties().FirstOrDefault(i => i.Name != "ID");
                        if (AddName != null)
                            PropName += $".{AddName.Name}";
                    }


                    var column = new GridViewColumn()
                    {
                        Header = gProp.Name,
                        DisplayMemberBinding = new Binding(PropName),
                    };

                    if (type == typeof(bool))
                    {
                        column.DisplayMemberBinding = null;

                        var template = DataTemplateGenerator.CreateDataTemplate(() =>
                        {

                            var chk = new CheckBox()
                            {
                                IsTabStop = false,
                                IsEnabled = lvOptions.EnableField
                            };
                            var binding = new Binding(gProp.Name)
                            {
                                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            };

                            binding.Mode = lvOptions.EnableField ? BindingMode.TwoWay : BindingMode.OneWay;

                            chk.SetBinding(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty, binding);
                            return chk;
                        });

                        column.CellTemplate = template;
                    }
                    else if (type == typeof(decimal))
                    {
                        column.DisplayMemberBinding = null;

                        var template = DataTemplateGenerator.CreateDataTemplate(() =>
                        {
                            var txt = new TextBlock() { IsEnabled = false };
                            txt.SetBinding(TextBlock.TextProperty, new Binding(gProp.Name)
                            {
                                Mode = BindingMode.OneWay,
                                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                StringFormat = "0.00"
                            });
                            return txt;
                        });

                        column.CellTemplate = template;
                    }



                    gridView.Columns.Add(column);
                }



                lst.View = gridView;
                lst.SizeChanged += (sender, e) =>
                {
                    var listview = sender as ListView;

                    var gridView = listview.View as GridView;
                    var width = listview.RenderSize.Width - listview.FontSize * 1.75;

                    var newWidth = (width - (gridView.Columns.Count + 1)) / gridView.Columns.Count;
                    if (gridView.Columns.Count == 1)
                        newWidth = width;
                    foreach (var column in gridView.Columns)
                    {
                        column.Width = newWidth;
                    }
                };
            }
            else
            {
                //Set up Template
                var Template = viewModel.GetProperty(lvOptions.DataTemplatePropName);
                if (Template == null)
                {
                    Type subType = prop.PropertyType.GenericTypeArguments.FirstOrDefault();

                    if (subType != null && subType.IsClass && !subType.IsPrimitive && subType != typeof(string))
                    {
                        var props = subType.GetProperties();
                        string propName = "";
                        if (!string.IsNullOrWhiteSpace(lvOptions.DataTemplateTextPath))
                            propName = lvOptions.DataTemplateTextPath;
                        else if (props.Length == 0)
                            throw new Exception($"No Properties found for {subType.Name}");
                        else
                        {
                            propName = props.Length == 1
                                ? props[0].Name
                                : props.First(i => i.Name != "ID").Name;
                        }

                        TextSearch.SetTextPath(lst, propName);
                        lst.AddDataTemplate(() =>
                        {
                            var txt = new TextBlock();
                            txt.SetBinding(TextBlock.TextProperty, propName);
                            return txt;
                        });
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(lvOptions.DataTemplateTextPath))
                        TextSearch.SetTextPath(lst, lvOptions.DataTemplateTextPath);
                    lst.SetBinding(ItemsControl.ItemTemplateProperty, Template.Name);
                }

            }

            var ContextMenuProp = viewModel.GetProperty(lvOptions.ContextMenuItemPropName);
            if (ContextMenuProp != null)
            {
                var style = lst.ItemContainerStyle;
                style.Setters.Add(new Setter()
                {
                    Property = FrameworkElement.ContextMenuProperty,
                    Value = viewModel.GetValue(lvOptions.ContextMenuItemPropName)
                });
            }
            return lst;
        }

        internal static System.Windows.Controls.ProgressBar BuildProgressBar(Attributes.ProgressBar progressBar, PropertyInfo prop, FormViewModel viewModel)
        {
            
            if (!prop.PropertyType.IsParent(typeof(IProgress<(int, int)>)))
                throw new Exception($"{prop.Name} can't be a ProgressBar; ensure it is a IProgress<(int count, int length)>");
            
            if (!prop.PropertyType.GenericTypeArguments[0].IsParent(typeof((int, int))))// != typeof(Command))
                throw new Exception($"{prop.Name} can't be a ProgressBar; ensure it is a IProgress<(int count, int length)>");
            //if (!prop.PropertyType.GenericTypeArguments[1].IsParent(typeof(int)))// != typeof(Command))
            //    throw new Exception($"{prop.Name} can't be a ProgressBar; ensure it is a IProgress<int, int>");

            var bar = new System.Windows.Controls.ProgressBar();

            var progress = new Progress<(int, int)>(item => {
                var count = item.Item1;
                var length = item.Item2;

                if(length > bar.Value)
                {
                    bar.Value = 0;
                }
                bar.Maximum = length;
                if (count < length)
                    bar.Value = count;
                else
                    bar.Value = length;
            });

            prop.SetValue(viewModel, progress);


            //throw new NotImplementedException();
            return bar;
        }
    }

    public static class ControlBuilderExt
    {
        public static PropertyInfo GetProperty(this ViewModel viewModel, string name)
        {
            if (string.IsNullOrWhiteSpace(name) || viewModel == null)
                return null;

            object value = viewModel;
            var Splits = name.Split('.');
            var prop = value?.GetType()?.GetProperty(Splits[0]);
            foreach (var item in Splits.Skip(1))
            {
                prop = prop.PropertyType.GetProperty(item);
            }

            return prop;
        }

        public static object GetValue(this ViewModel viewModel, string name)
        {
            if (string.IsNullOrWhiteSpace(name) || viewModel == null)
                return null;

            object value = viewModel;
            var Splits = name.Split('.');
            PropertyInfo prop = null;
            foreach (var item in Splits)
            {
                prop = value?.GetType()?.GetProperty(item);
                value = prop?.GetValue(value, null);
            }

            return value;
        }

        internal static object GetValue<T>(this T obj, PropertyInfo Field) => Field.GetValue(obj);

        public static string GetName(this ICommand command)
        {
            var prop = command?.GetType().GetProperty("Name");
            return prop?.GetValue(command)?.ToString() ?? string.Empty;
        }

        public static IEnumerable<(T Item, int Index)> WithIndex<T>(this IEnumerable<T> source)
        {
            int cnt = 0;
            foreach (var item in source)
            {
                yield return (item, cnt++);
            }
        }

        public static bool IsParent(this Type type, Type parent)
        {
            if (type == null)
                return false;
            if (type == parent)
                return true;
            foreach (var inter in type.GetInterfaces())
            {
                if (inter == parent)
                    return true;
            }
            return type.BaseType.IsParent(parent);
        }
        public static Type GetParentByStartOfName(this Type type, string StartOfName)
        {
            if (type == null)
                return null;
            if (type.Name.ToUpperInvariant().StartsWith(StartOfName.ToUpperInvariant()))
                return type;
            foreach (var inter in type.GetInterfaces())
            {
                if (inter.Name.ToUpperInvariant().StartsWith(StartOfName.ToUpperInvariant()))
                    return inter;
            }
            return type.BaseType.GetParentByStartOfName(StartOfName);
        }
    }
    public static class ControlBuilderHelper
    {

        public static void cbo_OpenDropdown(object sender, RoutedEventArgs e) => (sender as ComboBox).IsDropDownOpen = true;
        public static void cbo_CloseDropdown(object sender, RoutedEventArgs e) => (sender as ComboBox).IsDropDownOpen = false;

        public static void txt_SelectAll(object sender, RoutedEventArgs e) => (sender as TextBox).SelectAll();

        public static void txt_OnlyNumber(object sender, KeyEventArgs e)
        {
            var KeyInt = (int)e.Key;
            var tStart = (int)Key.D0;
            var tEnd = (int)Key.D9;
            var pStart = (int)Key.NumPad0;
            var pEnd = (int)Key.NumPad9;


            if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
            {
                if (e.Key == Key.Tab && e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
                    return;
            }
            else if (KeyInt >= tStart && KeyInt <= tEnd ||
                KeyInt >= pStart && KeyInt <= pEnd ||
                e.Key == Key.Back ||
                e.Key == Key.Delete ||
                e.Key == Key.Right ||
                e.Key == Key.Left ||
                e.Key == Key.Tab ||
                e.Key == Key.Home ||
                e.Key == Key.End ||
                e.Key == Key.Decimal ||
                e.Key == Key.OemPeriod)
            {

                return;
            }

            e.Handled = true;
        }

        public static void txt_Format_DecimalAmount(object sender, KeyboardFocusChangedEventArgs e)
        {
            var txtbox = sender as TextBox;
            if (decimal.TryParse(txtbox.Text, out var amount))
            {
                txtbox.Text = Math.Round(amount, 2).ToString("0.00");
            }
            else
                e.Handled = true;
        }

        public static void txt_Format_IntAmount(object sender, KeyboardFocusChangedEventArgs e)
        {
            var txtbox = sender as TextBox;
            if (decimal.TryParse(txtbox.Text, out var amount))
            {
                txtbox.Text = Math.Ceiling(amount).ToString("0");
            }
            else
                e.Handled = true;
        }
    }

    public class BooleanToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value is bool)
                return (bool)value == true ? Visibility.Visible : Visibility.Hidden;

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture) => throw new NotImplementedException();
    }


    public class BooleanToCollapsed : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value is bool)
                return (bool)value == true ? Visibility.Visible : Visibility.Collapsed;

            //throw new NotImplementedException();
            return null;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    class FieldValueConverter : IValueConverter
    {
        /*
         * Code Thanks to 
         * https://github.com/dotnet/wpf/issues/4312
         */
        private string _fieldName;

        public FieldValueConverter(string fieldName)
        {
            _fieldName = fieldName;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var mainVal = value?.GetType().GetField(_fieldName).GetValue(value);
                var type = mainVal.GetType();
                if (type.IsClass)
                {
                    var AddName = type.GetProperties().FirstOrDefault(i => i.Name != "ID");
                    if (AddName != null)
                    {
                        return AddName.GetValue(mainVal);
                    }
                }
                return mainVal;
            }
            return null;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
