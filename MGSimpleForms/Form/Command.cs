using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MGSimpleForms.Form
{
    public class Command : ICommand
    {
        Action<object> _Execute = null;
        Predicate<object> _CanExecute = null;

        public string Name { get; set; }

        public Command(Action execute)
        {
            _Execute = (i) => execute();
        }
        public Command(Action execute, Predicate<object> canExecute)
        {
            _Execute = (i) => execute();
            _CanExecute = canExecute;
        }


        public Command(Predicate<object> canExecute)
        {
            _CanExecute = canExecute;
        }
        public Command(Action<object> execute)
        {
            _Execute = execute;
        }
        public Command(Action<object> execute, Predicate<object> canExecute)
        {
            _Execute = execute;
            _CanExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }


        public bool CanExecute(object parameter) => _CanExecute == null || _CanExecute(parameter);
        public void Execute(object parameter) => _Execute(parameter);

    }

    public class Command<T> : ICommand
    {
        Action<T> _Execute = null;
        Predicate<T> _CanExecute = null;

        public string Name { get; set; }
        public Command(Action<T> Commit)
        {
            _CanExecute = null;
            _Execute = Commit;
        }

        public Command(Predicate<T> BeforeCommit, Action<T> Commit)
        {
            _CanExecute = BeforeCommit;
            _Execute = Commit;
        }
        public Command(Predicate<T> BeforeCommit)
        {
            _CanExecute = BeforeCommit;
            _Execute = null;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }


        public bool CanExecute(object parameter) => (parameter is T || parameter == null) && (_CanExecute == null || _CanExecute((T)parameter));
        public void Execute(object parameter)
        {
            if (parameter is T)
                _Execute?.Invoke((T)parameter);
            else
                throw new Exception("Empty Exception thown");
        }
    }


}
