using System;

namespace NRedisApi.Fluent.Test
{
    /// <summary>
    /// simple class for use Testing Redis GET and SET operations (class lifted from one of our solutions for convenience)
    /// </summary>
    public class SystemMonitorState
    {
        public SystemMonitorState(int numAlerts, string location, DateTime readingTimestamp, SystemMonitorStatus status)
        {
            Alerts = numAlerts;
            Location = location;
            ReadingTimestamp = readingTimestamp;
            Status = status;

        }

        public int Alerts { get; private set; }
        public string Location { get; private set; }
        public DateTime ReadingTimestamp { get; private set; }
        public SystemMonitorStatus Status { get; private set; }

        public void UpdateLastTrainProcessedState(DateTime readingTimestamp, string location, SystemMonitorStatus status)
        {
            Status = status;
            Location = location;
            ReadingTimestamp = readingTimestamp;
        }

        public void UpdateLastTrainProcessedStatus(SystemMonitorStatus status)
        {
            Status = status;
        }

        public void AddAlerts(int numAlerts)
        {
            Alerts += numAlerts;
        }

        public void ClearAlerts()
        {
            Alerts = 0;
        }
    }

    public enum SystemMonitorStatus
    {
        Normal,
        Warning,
        Critical
    }
}
