using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Ross.Windows
{
    public class DelegateCommand<T> : ICommand
    {
        private Action<T> _execute;
        private Func<T, bool> _canExecute;

        #region construction
        /// <summary>
        /// Creates a new instance, allowing for a method to be executed
        /// </summary>
        /// <param name="execute">Method to execute</param>
        /// <param name="function">The <see cref="SecureUseCaseFunction"/> to have check access against</param>
        public DelegateCommand(Action<T> execute)
        {
            _execute = execute;
        }

        /// <summary>
        /// Creates a new instance, allowing for a method to be executed and it's execution to be determined
        /// </summary>
        /// <param name="execute">Method to execute</param>
        /// <param name="canExecute">Method to test to see if the execute can be performed</param>
        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion

        /// <summary>
        /// Event to help manage command execution
        /// </summary>
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// Returns true if the method can be executed
        /// </summary>
        /// <param name="parameter">A parameter related to the execution</param>
        /// <returns>True if the command can be executed, otherwise false</returns>
        public virtual bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        /// <summary>
        /// Executes the attached method, if allowed to
        /// </summary>
        /// <param name="parameter">The CommandParameter to be passed to the <see cref="Action<>"/></param>
        public virtual void Execute(object parameter)
        {
            SetBusyStatus(true);
            try
            {
                _execute((T)parameter);
            }
            finally
            {
                SetBusyStatus(false);
            }
        }

        protected Action<T> ActionToExecute
        {
            get
            {
                return _execute;
            }
        }


        protected void SetBusyStatus(bool isBusy)
        {
         //   BusyStatus.IsBusy = isBusy;
        }

        internal void Invalidate()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
