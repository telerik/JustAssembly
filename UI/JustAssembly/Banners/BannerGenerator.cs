using JustAssembly.WebServiceProxy.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JustAssembly.Banners
{
    internal class BannerGenerator
    {
        public static BannerConfig GetRandomFromBannerConfigs(IEnumerable<BannerConfig> bannerConfigs)
        {
            Random random = new Random((int)DateTime.Now.Ticks);

            if (bannerConfigs == null || !bannerConfigs.Any())
            {
                return null;
            }
            int capacity = bannerConfigs.Sum(b => b.DisplayPriority);

            var campaignBannerNames = new List<BannerConfig>(capacity);

            foreach (BannerConfig bannerConfig in bannerConfigs)
            {
                for (int i = 0; i < bannerConfig.DisplayPriority; i++)
                {
                    campaignBannerNames.Add(bannerConfig);
                }
            }
            //// NOTE: index is between [0, count) range.
            int bannerIndex = random.Next(0, campaignBannerNames.Count);

            return campaignBannerNames[bannerIndex];
        }
    }
}
