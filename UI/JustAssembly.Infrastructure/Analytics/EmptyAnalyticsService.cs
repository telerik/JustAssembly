using JustAssembly.API.Analytics;
using System;
using System.Collections.Generic;

namespace JustAssembly.Infrastructure.Analytics
{
    internal class EmptyAnalyticsService : IAnalyticsService
    {
        public IAnalyticsMonitorStatus Status
        {
            get { return new EmptyAnalyticsMonitorStatus(); }
        }

        public void ForceSync()
        {
        }

        public void SetInstallationInfo(string installationId)
        {
        }

        public void SetInstallationInfo(string installationId, IDictionary<string, string> propertyDictionary)
        {
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Stop(TimeSpan waitForCompletion)
        {
        }

        public void TrackException(Exception exception)
        {
        }

        public void TrackException(Exception exception, string contextMessage)
        {
        }

        public void TrackException(Exception exception, string contextMessageFormat, params object[] args)
        {
        }

        public void TrackFeature(string featureName)
        {
        }

        public void TrackFeatureCancel(string featureName)
        {
        }

        public void TrackFeatures(string[] featureNames)
        {
        }

        public ITimingScope TrackFeatureStart(string featureName)
        {
            return new EmptyTimingScope();
        }

        public TimeSpan TrackFeatureStop(string featureName)
        {
            return TimeSpan.Zero;
        }

        public void TrackFeatureValue(string featureName, long trackedValue)
        {
        }
    }
}
