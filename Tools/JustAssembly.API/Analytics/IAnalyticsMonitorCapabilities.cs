namespace JustAssembly.API.Analytics
{
    public interface IAnalyticsMonitorCapabilities
    {
        int MaxAllowedBandwidthUsagePerDayInKB { get; }

        int MaxInstallationIDSize { get; }

        int MaxKeySizeOfInstallationPropertyKey { get; }

        int MaxLengthOfExceptionContextMessage { get; }

        int MaxLengthOfFeatureName { get; }

        int MaxNumberOfInstallationProperties { get; }

        int MaxStorageSizeInKB { get; }
    }
}
