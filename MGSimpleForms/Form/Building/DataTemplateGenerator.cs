using System;
using System.Windows.Controls;
using System.Windows;

namespace MGSimpleForms.Form.Building
{
    /// <summary>
    /// Class that helps the creation of control and data templates by using delegates.
    /// pulled from:https://stackoverflow.com/questions/8427972/how-do-i-create-a-datatemplate-with-content-programmatically
    /// Other Link:http://www.codeproject.com/Tips/808808/Create-Data-and-Control-Templates-using-Delegates
    /// </summary>
    public static class DataTemplateGenerator
    {
        public sealed class _TemplateGeneratorControl :
          ContentControl
        {
            internal static readonly DependencyProperty FactoryProperty = DependencyProperty.Register("Factory", typeof(Func<object>), typeof(_TemplateGeneratorControl), new PropertyMetadata(null, _FactoryChanged));

            private static void _FactoryChanged(DependencyObject instance, DependencyPropertyChangedEventArgs args)
            {
                var control = (_TemplateGeneratorControl)instance;
                if (args.NewValue != null)
                {
                    var factory = (Func<object>)args.NewValue;
                    control.Content = factory();
                }
            }
        }

        /// <summary>
        /// Creates a data-template that uses the given delegate to create new instances.
        /// </summary>
        public static DataTemplate CreateDataTemplate(Func<object> factory, ItemsControl control = null)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            var frameworkElementFactory = new FrameworkElementFactory(typeof(_TemplateGeneratorControl));
            frameworkElementFactory.SetValue(_TemplateGeneratorControl.FactoryProperty, factory);



            var dataTemplate = new DataTemplate(typeof(DependencyObject));
            dataTemplate.VisualTree = frameworkElementFactory;
            return dataTemplate;
        }

        /// <summary>
        /// found here:
        /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/664bb419-0da4-4de7-ab9d-fcee359b9d0e/set-itemscontrolitemspanel-from-codebehind?forum=wpf
        /// </summary>
        public static ItemsPanelTemplate CreateVirtualizingItemTemplate()
        {
            FrameworkElementFactory factoryPanel = new FrameworkElementFactory(typeof(VirtualizingStackPanel));

            factoryPanel.SetValue(Panel.IsItemsHostProperty, true);

            ItemsPanelTemplate template = new ItemsPanelTemplate();

            template.VisualTree = factoryPanel;

            return template;
        }


        public static void AddDataTemplate(this ItemsControl control, Func<object> factory)
        {
            var template = CreateDataTemplate(factory);

            control.ItemTemplate = template;
        }
    }
}
