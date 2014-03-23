using System.Collections.Generic;

namespace Ross.Infrastructure
{
    public interface ITrackChanges
    {
        void StartTracking(bool isNew = false);

        bool IsTracking { get; }

        void FinishTracking();

        //Dictionary<string, object> CancelChanges();
        void CancelChanges();
        void AcceptChanges();
        //List<string> Changes { get; }
        Dictionary<string, object> Changes { get; }
        object GetOriginalValue(string key);
    }
}