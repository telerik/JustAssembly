using JustAssembly.API.Analytics;

namespace JustAssembly.Infrastructure.Analytics
{
    internal class EmptyTimingScope : ITimingScope
    {
        public string FeatureName
        {
            get { return string.Empty; }
        }

        public void Cancel()
        {
        }

        public void Complete()
        {
        }
    }
}
