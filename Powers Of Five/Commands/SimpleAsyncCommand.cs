using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Ross.Windows
{
    /// <summary>
    /// Provideds a simplified way of executing a command that takes no arguments on a background thread.
    /// </summary>
    public class SimpleAsyncCommand : SimpleCommand
    {
        private volatile bool _isExecuting;

        #region construction
        /// <summary>
        /// Creates a new instance, allowing for a method to be executed
        /// </summary>
        /// <param name="execute">Method to execute</param>
        public SimpleAsyncCommand(Action execute) : base(execute)
        {
        }

        /// <summary>
        /// Creates a new instance, allowing for a method to be executed and it's execution to be determined
        /// </summary>
        /// <param name="execute">Method to execute</param>
        /// <param name="canExecute">Method to test to see if the execute can be performed</param>
        public SimpleAsyncCommand(Action execute, Func<bool> canExecute) : base(execute, canExecute)
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


        /// <summary>
        /// Executes the command. If the command is already executing then no execution will be performed.
        /// Ensures that the busy state is changed accordingly
        /// </summary>
        public override void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                _isExecuting = true;
                SetBusyStatus(true);

                Task.Factory.StartNew(() =>
                {
                    ActionToExecute();
                }).ContinueWith(task =>
                {
                    if (task.Exception != null)
                        Trace.WriteLine(task.Exception.Message);

                    SetBusyStatus(false);
                    _isExecuting = false;

                    Infrastructure.Execute.OnUIThread(() => base.Invalidate());
                });
            }
        }
    }

}
