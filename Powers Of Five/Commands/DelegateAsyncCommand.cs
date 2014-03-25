using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Powers_Of_Five.Windows
{
    public class DelegateAsyncCommand<T> : DelegateCommand<T>
    {
        #region construction
        /// <summary>
        /// Creates a new instance, allowing for a method to be executed
        /// </summary>
        /// <param name="execute">Method to execute</param>
        public DelegateAsyncCommand(Action<T> execute) : base(execute)
        {
        }

        /// <summary>
        /// Creates a new instance, allowing for a method to be executed and it's execution to be determined
        /// </summary>
        /// <param name="execute">Method to execute</param>
        /// <param name="canExecute">Method to test to see if the execute can be performed</param>
        public DelegateAsyncCommand(Action<T> execute, Func<T, bool> canExecute) : base(execute, canExecute)
        {
        }
        #endregion

        /// <summary>
        /// Returns true if the command can be executed and it is not already executing
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            if (_isExecuting) return false;

            return base.CanExecute(parameter);
        }

        private volatile bool _isExecuting;

        /// <summary>
        /// Executes the attached method, if allowed to, on a background thread
        /// </summary>
        /// <param name="parameter">The CommandParameter to be passed to the <see cref="Action<>"/></param>
        public override void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                _isExecuting = true;

                SetBusyStatus(true);
                Task.Factory.StartNew(() =>
                {
                    ActionToExecute((T)parameter);
                }).ContinueWith(task =>
                {
                    if (task.Exception == null)
                    {
                        SetBusyStatus(false);
                        _isExecuting = false;
                        Infrastructure.Execute.OnUIThread(() => base.Invalidate());
                    }
                    else
                    {
                        //Logger.LogException(task.Exception);
                    }
                });
            }
        }
    }
}
