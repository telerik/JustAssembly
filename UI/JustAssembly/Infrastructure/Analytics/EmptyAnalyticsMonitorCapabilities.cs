using JustAssembly.API.Analytics;

namespace JustAssembly.Infrastructure.Analytics
{
    internal class EmptyAnalyticsMonitorCapabilities : IAnalyticsMonitorCapabilities
    {
        public int MaxAllowedBandwidthUsagePerDayInKB
        {
            get { return 0; }
        }

        public int MaxInstallationIDSize
        {
            get { return 0; }
        }

        public int MaxKeySizeOfInstallationPropertyKey
        {
            get { return 0; }
        }

        public int MaxLengthOfExceptionContextMessage
        {
            get { return 0; }
        }

        public int MaxLengthOfFeatureName
        {
            get { return 0; }
        }

        public int MaxNumberOfInstallationProperties
        {
            get { return 0; }
        }

        public int MaxStorageSizeInKB
        {
            get { return 0; }
        }
    }
}
