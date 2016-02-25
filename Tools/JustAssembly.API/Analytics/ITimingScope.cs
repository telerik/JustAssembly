namespace JustAssembly.API.Analytics
{
    public interface ITimingScope
    {
        string FeatureName { get; }

        void Cancel();

        void Complete();
    }
}
