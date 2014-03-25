using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Powers_Of_Five.Core;

namespace Powers_Of_Five.Infrastructure
{
    public class PropertyChangedBase : Disposable, INotifyPropertyChanged
    {
        private static readonly Dictionary<string, PropertyChangedEventArgs> Names;

        static PropertyChangedBase()
        {
            Names = new Dictionary<string, PropertyChangedEventArgs>();
        }

        private PropertyChangedEventHandler _propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }

        protected void NotifyOfPropertyChange<T>(Expression<Func<T>> property)
        {
            var propertyName = GetPropertyName(property);
            if (_propertyChanged != null)
            {
                RaiseEvent(propertyName);
            }
            AfterPropertyChanged(propertyName);
        }

        public static string GetPropertyName<T>(Expression<Func<T>> property)
        {
            return Extensions.GetPropertyName(property);
        }

        protected void RaiseEvent(string propertyName)
        {
            var temp = _propertyChanged;
            if (temp != null)
            {
                PropertyChangedEventArgs evntArgs;
                if (!Names.TryGetValue(propertyName, out evntArgs))
                {
                    evntArgs = new PropertyChangedEventArgs(propertyName);
                    lock (Names)
                    {
                        Names[propertyName] = evntArgs;
                    }
                }

                if (evntArgs != null)
                {
                    Execute.OnUIThread(() => temp(this, evntArgs));
                }
            }
        }

        protected virtual void AfterPropertyChanged(string property)
        {
        }

        protected bool SetValue<TValue, TPropertyHolder>(ref TValue field, TValue value, Expression<Func<TPropertyHolder>> property, bool useReferenceEquals = false)
        {
            var propertyName = GetPropertyName(property);

            bool valueChanged = false;
            if (useReferenceEquals)
            {
                if (!object.ReferenceEquals(field, value))
                {
                    valueChanged = true;
                }
            }
            else
            {
                valueChanged = !AreEqual(field, value);
            }

            if (valueChanged)
            {
                field = value;
                NotifyOfPropertyChange(property);
            }
            return valueChanged;
        }

        private static bool AreEqual<T>(T x, T y)
        {
            return EqualityComparer<T>.Default.Equals(x, y);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }
    }
}
