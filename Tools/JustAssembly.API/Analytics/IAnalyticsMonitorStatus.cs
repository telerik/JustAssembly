using System;

namespace JustAssembly.API.Analytics
{
    public interface IAnalyticsMonitorStatus
    {
        IAnalyticsMonitorCapabilities Capabilities { get; }

        ConnectivityStatus Connectivity { get; }

        string CookieId { get; }

        bool IsStarted { get; }

        TimeSpan RunTime { get; }
    }
}
