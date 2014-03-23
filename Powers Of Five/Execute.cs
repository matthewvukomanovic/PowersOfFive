using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ross.Infrastructure
{
    public static class Execute
    {
        private static bool? _inDesignMode;
        private static SynchronizationContext _context;
        private static Action<Action> _marshaller;
        private static TaskScheduler _scheduler;

        static Execute()
        {
            InitializeWithCurrentContext();
        }

        /// <summary>
        /// Indicates whether or not the framework is in design-time mode.
        /// </summary>
        public static bool InDesignMode
        {
            get
            {
                if (_inDesignMode == null)
                {
                    _inDesignMode = Process.GetCurrentProcess().ProcessName.StartsWith("devenv", StringComparison.Ordinal);
                }

                return _inDesignMode.GetValueOrDefault(false);
            }
        }

        /// <summary>
        /// Initializes the framework using the current <see cref="SynchronizationContext"/>.
        /// </summary>
        public static void InitializeWithCurrentContext()
        {
            _context = SynchronizationContext.Current;
            if (_context != null)
                _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        private static SynchronizationContext CurrentContext
        {
            get
            {
                if (_context == null)
                {
                    _context = new SynchronizationContext();
                    _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                }
                return _context;
            }
        }

        /// <summary>
        /// Sets a custom UI thread marshaller.
        /// </summary>
        /// <param name="marshaller">The marshaller.</param>
        public static void SetUIThreadMarshaller(Action<Action> marshaller)
        {
            _marshaller = marshaller;
        }

        /// <summary>
        /// Gets the ui <see cref="TaskScheduler"/>.
        /// </summary>
        public static TaskScheduler UIScheduler
        {
            get
            {
                return _scheduler;
            }
        }

        /// <summary>
        /// Executes the action on the UI thread.
        /// </summary>
        /// <param name = "action">The action to execute.</param>
        public static void OnUIThread(this Action action)
        {
            if (IsOnUIThread)
            {
                action();
            }
            else
            {
                if (_marshaller == null)
                {
                    CurrentContext.Send((obj) => action(), null);
                }
                else
                    _marshaller(action);
            }
        }

        public static void OnUIThread<T>(this Action<T> action, T paramater)
        {
            if (IsOnUIThread)
            {
                action(paramater);
            }
            else
            {
                //Dont ever try to use the marshaller
                CurrentContext.Send((obj) => action(paramater), null);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool IsOnUIThread
        {
            get
            {
                return (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA);
            }
        }

        /// <summary>
        /// Executes the action on the UI thread using an asynchronous message
        /// </summary>
        /// <param name = "action">The action to execute.</param>
        public static void PostOnUIThread(this Action action)
        {
            if (_marshaller == null)
                CurrentContext.Post((obj) => action(), null);
            else
                _marshaller(action);
        }

        public static void PostOnUIThreadWithDelay(this Action action, TimeSpan delay)
        {
            var timer = new System.Timers.Timer(delay.TotalMilliseconds);
            timer.Elapsed += (o, e) =>
            {
                timer.Stop();
                PostOnUIThread(action);
                timer.Dispose();
            };

            timer.Start();
        }

        /// <summary>
        /// Returns the UI <see cref="SynchornizationContext"/>
        /// </summary>
        public static SynchronizationContext Context
        {
            get
            {
                return CurrentContext;
            }
        }
    }
}
