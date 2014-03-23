using System;
using System.Linq;
using System.Text;

namespace Ross.Infrastructure
{
    public interface IDirtyPropertyChanged
    {
        bool IsDirty { get; set; }

        event EventHandler IsDirtyChanged;
    }
}
