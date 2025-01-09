using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace MGSimpleForms.Tools
{
    public static class VTreeHelper
    {
        public static IEnumerable<(FrameworkElement, string)> GetAllChildrenForBinding(this DependencyObject start)
        {
            var stack = new Stack<(DependencyObject obj, int Index, int Count, string Header)>();

            stack.Push((start, 0, VisualTreeHelper.GetChildrenCount(start), String.Empty));

            while (stack.Any())
            {
                var item = stack.Pop();
                for (; item.Index < item.Count; item.Index++)
                {
                    var newObj = VisualTreeHelper.GetChild(item.obj, item.Index);
                    var Bindings = GetBindingStuff(newObj as FrameworkElement);
                    foreach (var bind in Bindings.Item2)
                        yield return (bind.Item1, (!string.IsNullOrWhiteSpace(item.Header) ? item.Header + "." : String.Empty) + bind.Item2);// item.Header + "." +


                    var newCount = VisualTreeHelper.GetChildrenCount(newObj);
                    if (newCount > 0)
                    {
                        item.Index++;
                        stack.Push(item);
                        var header = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.Header))
                            header = Bindings.Item1;
                        else
                            header = item.Header + (!string.IsNullOrWhiteSpace(Bindings.Item1) ? "." + Bindings.Item1 : String.Empty);

                        stack.Push((newObj, 0, newCount, header));//+ item.obj.GetType().Name
                        break;
                    }
                }
            }
        }

        static (string, List<(FrameworkElement, string)>) GetBindingStuff(FrameworkElement obj)
        {
            var lst = new List<(FrameworkElement, string)>();
            if (obj == null)
                return (string.Empty, lst);
            var type = obj.GetType();
            string DataContentType = string.Empty;

            if (type.GetProperty("DataContext") != null)
            {
                var binding = obj.GetBindingExpression(FrameworkElement.DataContextProperty);
                if (binding != null)
                {
                    var name = binding.ResolvedSource?.GetType().Name;
                    lst.Add((obj, name + "." + binding.ResolvedSourcePropertyName));
                    DataContentType = name;
                }
            }

            if (obj is ComboBox && type.GetProperty("SelectedItem") != null)
            {
                var binding = obj.GetBindingExpression(ComboBox.SelectedItemProperty);
                if (binding != null)
                {
                    lst.Add((obj, binding.ResolvedSource?.GetType().Name + "." + binding.ResolvedSourcePropertyName));
                }
            }

            if (obj is TextBox && type.GetProperty("Text") != null)
            {
                var binding = obj.GetBindingExpression(TextBox.TextProperty);
                if (binding != null)
                {
                    lst.Add((obj, binding.ResolvedSource?.GetType().Name + "." + binding.ResolvedSourcePropertyName));
                }
            }



            return (DataContentType, lst);
        }

        public static IEnumerable<T> GetAllChildren<T>(this DependencyObject start, Func<DependencyObject, T> IsItem = null) where T : class
        {
            if (start == null)
                yield break;

            var stack = new Stack<(DependencyObject obj, int Index, int Count)>();

            stack.Push((start, 0, VisualTreeHelper.GetChildrenCount(start)));

            while (stack.Any())
            {
                var item = stack.Pop();
                for (; item.Index < item.Count; item.Index++)
                {
                    var newObj = VisualTreeHelper.GetChild(item.obj, item.Index);
                    if (IsItem != null)
                    {
                        var check = IsItem(newObj);
                        if (check != null)
                        {
                            yield return check;
                        }
                    }
                    else
                    {
                        if (newObj is T)
                            yield return newObj as T;
                    }

                    var newCount = VisualTreeHelper.GetChildrenCount(newObj);
                    if (newCount > 0)
                    {
                        item.Index++;
                        stack.Push(item);
                        stack.Push((newObj, 0, newCount));
                        break;
                    }
                }
            }
        }




        public static DependencyObject GetParentByType(this IInputElement element, Type type) => GetParentByType((DependencyObject)element, type);
        public static DependencyObject GetParentByType(this FrameworkElement element, Type type) => GetParentByType((DependencyObject)element, type);
        public static DependencyObject GetParentByType(this DependencyObject element, Type type)
        {
            DependencyObject parentDepObj = element;
            while ((parentDepObj = VisualTreeHelper.GetParent(parentDepObj)) != null)
            {
                if (type == parentDepObj.GetType())
                    return parentDepObj;
            }
            return null;
        }


        public static bool IsParentType(this IInputElement element, Type type) => IsParentType((DependencyObject)element, type);
        public static bool IsParentType(this FrameworkElement element, Type type) => IsParentType((DependencyObject)element, type);
        public static bool IsParentType(this DependencyObject element, Type type)
        {
            DependencyObject parentDepObj = element;
            while ((parentDepObj = VisualTreeHelper.GetParent(parentDepObj)) != null)
            {
                if (type == parentDepObj.GetType())
                    return true;
            }
            return false;
        }


        public static T GetParent<T>(this DependencyObject parent) where T : class
        {
            if(parent == null)
                return default(T);
            
            if (parent is T)
                return parent as T;

            while ((parent = VisualTreeHelper.GetParent(parent)) != null)
            {
                if (parent is T)
                    return parent as T;
            }
            return default(T);
        }

    }
}
