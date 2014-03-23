using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Ross.Windows
{
    /// <summary>
    /// Provides a bindable command that takes no parameters
    /// </summary>
    public class SimpleCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        #region construction
        /// <summary>
        /// Creates a new instance, allowing for a method to be executed
        /// </summary>
        /// <param name="execute">Method to execute</param>
        public SimpleCommand(Action execute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
        }

        /// <summary>
        /// Creates a new instance, allowing for a method to be executed and it's execution to be determined
        /// </summary>
        /// <param name="execute">Method to execute</param>
        /// <param name="canExecute">Method to test to see if the execute can be performed</param>
        public SimpleCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            if (canExecute == null)
                throw new ArgumentNullException("canExecute");

            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion

        /// <summary>
        /// Returns true if the method can be executed
        /// </summary>
        /// <param name="parameter">Ignored</param>
        /// <returns>True if the command can be executed, otherwise false</returns>
        public virtual bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

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
        /// Executes the attached method, if allowed to
        /// </summary>
        /// <param name="parameter">Ignored</param>
        public virtual void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                SetBusyStatus(true);
                try
                {
                    _execute();
                }
                finally
                {
                    SetBusyStatus(false);
                }
            }
        }

        /// <summary>
        /// Returns the <see cref="Action"/> that this command will execute
        /// </summary>
        protected Action ActionToExecute
        {
            get
            {
                return _execute;
            }
        }

        protected void SetBusyStatus(bool isBusy)
        {
           // BusyStatus.IsBusy = isBusy;
        }

        protected void Invalidate()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
