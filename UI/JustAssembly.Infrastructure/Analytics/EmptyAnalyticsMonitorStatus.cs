using JustAssembly.API.Analytics;
using System;

namespace JustAssembly.Infrastructure.Analytics
{
    internal class EmptyAnalyticsMonitorStatus : IAnalyticsMonitorStatus
    {
        public IAnalyticsMonitorCapabilities Capabilities
        {
            get { return new EmptyAnalyticsMonitorCapabilities(); }
        }

        public ConnectivityStatus Connectivity
        {
            get { return ConnectivityStatus.Unknown; }
        }

        public string CookieId
        {
            get { return string.Empty; }
        }

        public bool IsStarted
        {
            get { return false; }
        }

        public TimeSpan RunTime
        {
            get { return TimeSpan.Zero; }
        }
    }
}
