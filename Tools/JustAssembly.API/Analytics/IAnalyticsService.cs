using System;
using System.Collections.Generic;

namespace JustAssembly.API.Analytics
{
    public interface IAnalyticsService
    {
        IAnalyticsMonitorStatus Status { get; }

        void ForceSync();

        void SetInstallationInfo(string installationId);

        void SetInstallationInfo(string installationId, IDictionary<string, string> propertyDictionary);

        void Start();

        void Stop();

        void Stop(TimeSpan waitForCompletion);

        void TrackException(Exception exception);

        void TrackException(Exception exception, string contextMessage);

        void TrackException(Exception exception, string contextMessageFormat, params object[] args);

        void TrackFeature(string featureName);

        void TrackFeatureCancel(string featureName);

        void TrackFeatures(string[] featureNames);

        ITimingScope TrackFeatureStart(string featureName);

        TimeSpan TrackFeatureStop(string featureName);

        void TrackFeatureValue(string featureName, long trackedValue);
    }
}
