using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Ross.Infrastructure.Core;
using Ross.Infrastructure.Helpers;

namespace Ross.Infrastructure
{
    public class PropertyChangedBase : Disposable, INotifyPropertyChanged, ITrackChanges, IDirtyPropertyChanged
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

        /// <summary>
        /// Trakc collection
        /// NOTE: We track only add, currenly used for comments
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TPropertyHolder"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="property"></param>
        protected void SetValue<TValue, TPropertyHolder>(ref ObservableCollection<TValue> field, ObservableCollection<TValue> value, Expression<Func<TPropertyHolder>> property)
        {
            if (value != null)
            {
                value.CollectionChanged += ((o, e) =>
                {
                    if (!IsTracking) return;
                    var key = GetPropertyName(property);

                    if (!_changes.ContainsKey(key))
                    {
                        _changes.Add(key, value);
                    }

                    //if (value.Count == 0)
                    //    _changes.Remove(GetPropertyName(property));
                    //else
                    //    _changes[GetPropertyName(property)] = value;

                    IsDirty = IsDirtyRefreshProperty;
                });
            }

            field = value;

            NotifyOfPropertyChange(property);
        }

        private readonly List<IDirtyPropertyChanged> _dirtyObjects = new List<IDirtyPropertyChanged>();

        void RefreshObject_IsDirtyChanged(object sender, EventArgs e)
        {
            var value = sender as IDirtyPropertyChanged;
            if (value == null)
                return;

            if (AddOrRemoveDirtyObject(value))
            {
                IsDirty = IsDirtyRefreshProperty;
            }
        }

        private bool AddOrRemoveDirtyObject(IDirtyPropertyChanged value)
        {
            var changed = false;
            var contains = _dirtyObjects.Contains(value);
            var dirty = value.IsDirty;

            if (dirty)
            {
                if (!contains)
                {
                    _dirtyObjects.Add(value);
                    changed = true;
                }
            }
            else
            {
                if (contains)
                {
                    _dirtyObjects.Remove(value);
                    changed = true;
                }
            }
            return changed;
        }

        public void TrackDirtyObject(IDirtyPropertyChanged potentialDirtyObject)
        {
            potentialDirtyObject.IsDirtyChanged += SubPropertyDirtyChanged;
        }

        public void StopTrackingDirtyObject(IDirtyPropertyChanged potentialDirtyObject)
        {
            potentialDirtyObject.IsDirtyChanged -= SubPropertyDirtyChanged;
        }

        private bool SwapPotentialDirtyProperty<TValue>(TValue field, TValue value)
        {
            var changed = false;
            if (field is IDirtyPropertyChanged || value is IDirtyPropertyChanged)
            {
                var f = (IDirtyPropertyChanged)field;
                var v = (IDirtyPropertyChanged)value;

                if (!ReferenceEquals(f, v))
                {
                    if (f != null)
                    {
                        f.IsDirtyChanged -= SubPropertyDirtyChanged;
                    }

                    if (v != null)
                    {
                        v.IsDirtyChanged += SubPropertyDirtyChanged;
                    }
                }
                if (_dirtyObjects.Contains(f))
                {
                    _dirtyObjects.Remove(f);
                    changed = true;
                }
                if (v != null && v.IsDirty)
                {
                    _dirtyObjects.Add(v);
                    changed = true;
                }
            }
            return changed;
        }

        private void SubPropertyDirtyChanged(object sender, EventArgs e)
        {
            var v = (IDirtyPropertyChanged)sender;
            var contains = _dirtyObjects.Contains(v);
            if (v.IsDirty)
            {
                if (!contains)
                {
                    _dirtyObjects.Add(v);
                    RefreshIsDirty();
                }
            }
            else
            {
                if (contains)
                {
                    _dirtyObjects.Remove(v);
                    if (_dirtyObjects.Count == 0)
                        RefreshIsDirty();
                }
            }
        }

        protected bool SetValueNonTracking<TValue, TPropertyHolder>(ref TValue field, TValue value, Expression<Func<TPropertyHolder>> property)
        {
            return SetValue(ref field, value, property, false, false, false);
        }

        protected bool SetValueAlways<TValue, TPropertyHolder>(ref TValue field, TValue value, Expression<Func<TPropertyHolder>> property, bool tracking = false, bool setAsOriginal = false)
        {
            return SetValue(ref field, value, property, tracking, setAsOriginal, true);
        }

        protected bool SetValue<TValue, TPropertyHolder>(ref TValue field, TValue value, Expression<Func<TPropertyHolder>> property, bool tracking = true, bool setAsOriginal = false, bool useReferenceEquals = false)
        {
            var propertyName = GetPropertyName(property);
            if (tracking)
            {
                if (!_isNew && !_originalValues.ContainsKey(propertyName) || (!IsTracking))
                    _originalValues[propertyName] = value;
            }

            if (setAsOriginal)
            {
                _originalValues[propertyName] = value;
            }

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
                if (tracking)
                {
                    TrackItem(value, propertyName);
                }
                var changed = SwapPotentialDirtyProperty(field, value);
                field = value;
                NotifyOfPropertyChange(property);
                if (changed)
                {
                    RefreshIsDirty();
                }
            }
            return valueChanged;
        }

        protected bool RevertValue<TValue, TPropertyHolder>(ref TValue field, Expression<Func<TPropertyHolder>> property)
        {
            var propertyName = GetPropertyName(property);
            if (_originalValues.ContainsKey(propertyName))
            {
                TValue value;
                if (_originalValues[propertyName] is TValue)
                {
                    value = (TValue)_originalValues[propertyName];
                }
                else
                {
                    value = default(TValue);
                }

                return SetValue(ref field, value, property, false, false, true);
            }
            return false;
        }

        private static bool AreEqual<T>(T x, T y)
        {
            return EqualityComparer<T>.Default.Equals(x, y);
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            _changes.Clear();
            _originalValues.Clear();
        }

        #region change tracking

        private readonly Dictionary<string, object> _changes = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _originalValues = new Dictionary<string, object>();
        private Dictionary<string, List<DependentEntity>> _changesListValues = new Dictionary<string, List<DependentEntity>>();

        public object GetOriginalValue(string propertyName)
        {
            return _originalValues.ContainsKey(propertyName) ? _originalValues[propertyName] : null;
        }

        private void TrackItem(object value, string propertyName)
        {
            if (IsTracking)
            {
                if (value is Interfaces.IIdentifiable)
                {
                    value = ((Interfaces.IIdentifiable)value).Id;
                }
                if (!_isNew && AreEqual(_originalValues[propertyName], value))
                    _changes.Remove(propertyName);
                else
                {
                    // If the value is an enum then get the underlying type value, otherwise it will return itself.
                    object trackValue = EnumHelper.ToUnderlyingType(value);
                    _changes[propertyName] = trackValue;
                }
                RefreshIsDirty();
            }
        }

        private bool SubObjectsDirty
        {
            get
            {
                foreach (var dirtyObject in _dirtyObjects)
                {
                    if (dirtyObject.IsDirty)
                        return true;
                }
                return false;
            }
        }

        private void RefreshIsDirty()
        {
            IsDirty = IsDirtyRefreshProperty;
        }

        private bool IsDirtyRefreshProperty
        {
            get { return (_changes != null && _changes.Count != 0) || _changesListValues.Count != 0 || SubObjectsDirty; }
        }

        private bool _isNew;
        private bool _isTracking;

        public void StartTracking(bool isNew = false)
        {
            if (!_isTracking)
            {
                _changes.Clear();
                _changesListValues.Clear();
                _isNew = isNew;
                _isTracking = true;
                OnStartTracking(isNew);
            }
        }

        protected virtual void OnStartTracking(bool isNew)
        {

        }

        public bool IsTracking
        {
            get { return _isTracking; }
        }

        public void FinishTracking()
        {
            if (_isTracking)
            {
                _isTracking = false;
            }
        }

        public void AcceptChanges()
        {
            if (_isTracking)
            {
                foreach (var change in _changes)
                {
                    _originalValues[change.Key] = change.Value;
                }

                OnAcceptChanges();

                _changesListValues.Clear();
                _changes.Clear();
                RefreshIsDirty();
            }
        }

        protected virtual void OnAcceptChanges()
        {
        }

        public void CancelChanges()
        {
            if (_isTracking)
            {
                //TODO: Before these are cleared they should be restoring the original values!
                OnCancelChanges();
                _changes.Clear();
                _changesListValues.Clear();
                RefreshIsDirty();
            }
        }

        protected virtual void OnCancelChanges()
        {

        }

        public Dictionary<string, object> Changes
        {
            get { return _changes; }
        }

        public virtual Dictionary<string, List<DependentEntity>> ChangesLists
        {
            get { return _changesListValues; }
        }

        public bool HasPropertyChanged<TPropertyHolder>(Expression<Func<TPropertyHolder>> property)
        {
            var name = GetPropertyName(property);
            return HasPropertyChanged(name);
        }

        public bool HasPropertyChanged(params string[] names)
        {
            foreach (var name in names)
            {
                if (_changes.ContainsKey(name))
                {
                    return true;
                }
            }
            return false;
        }

        #region IsDirty

        private bool _isDirty;

        public virtual bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    NotifyOfPropertyChange(() => IsDirty);
                    OnIsDirtyChanged();
                }
            }
        }

        private event EventHandler _isDirtyChanged;

        public event EventHandler IsDirtyChanged
        {
            add { _isDirtyChanged += value; }
            remove { _isDirtyChanged -= value; }
        }

        protected virtual void OnIsDirtyChanged()
        {
            EventHandler temp = _isDirtyChanged;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }

        #endregion IsDirty

        #endregion change tracking
    }
}
