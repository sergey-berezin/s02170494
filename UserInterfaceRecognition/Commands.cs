using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows;
using ImageRecognitionLibrary;

namespace UserInterfaceRecognition
{
    public class Commands : ICommand
    {
        private readonly Predicate<object> canExecute;
        private readonly Action<object> execute;

        public Commands(Action<object> execute): this(execute, null)
        {
        }

        public Commands(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }


        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
            {
                return true;
            }

            return canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }


        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
