using JustAssembly.WebServiceProxy;
using JustAssembly.WebServiceProxy.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace JustAssembly.Banners
{
    public class BannerService
    {
        private const string DefaultBanner1ImagePath = @"pack://application:,,,/images/DefaultBanner.png";
        private const string DefaultBanner1Link = "http://www.telerik.com/products/wpf/overview.aspx?utm_medium=placement&utm_source=justassembly&utm_campaign=dt-devcraft-desktopui-product-oct16&utm_content=banner-wpf";

        private static BannerService instance;

        private readonly IUpdatesService serviceInstance;
        private readonly DirectoryInfo localBannersDirInfo;
        private readonly Dictionary<int, Banner> holderIdToDefaultBannerMap;
        private readonly Version currentBannerCampaignVersion;
        private readonly DirectoryInfo currentBannerCampaignDirInfo;
        private readonly Dictionary<int, List<BannerConfig>> holderIdToBannerConfigMap;

        private BannerService()
        {
            try
            {
                this.serviceInstance = UpdatesServiceClientFactory.CreateNew();

                this.localBannersDirInfo = new DirectoryInfo(Path.Combine(Configuration.JustAssemblyAppDataFolder, "Banners"));

                if (!localBannersDirInfo.Exists)
                {
                    localBannersDirInfo.Create();
                }

                this.holderIdToDefaultBannerMap = GenerateDefaultBannersMap();

                this.currentBannerCampaignVersion = GetCurrentBannerCampaignVersion();

                if (this.currentBannerCampaignVersion != null)
                {
                    // Delete banners from old campaigns
                    this.DeleteBannerCampaignVersions(str => !str.Equals(currentBannerCampaignVersion.ToString(), StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    // Just to be sure that the Banners folder is empty
                    this.DeleteAllBannerCampaignVersions();
                }

                if (this.currentBannerCampaignVersion != null)
                {
                    this.currentBannerCampaignDirInfo = new DirectoryInfo(Path.Combine(localBannersDirInfo.FullName, this.currentBannerCampaignVersion.ToString()));

                    this.holderIdToBannerConfigMap = GenerateHolderIdToConfigMap(this.currentBannerCampaignDirInfo);
                }

                this.CheckLatestServerCampaignVersionsAsync(OnCheckLatestServerCampaignVersionsAsync);
            }
            catch (Exception ex)
            {
                Configuration.Analytics.TrackException(ex);
            }
        }

        public static BannerService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BannerService();
                }

                return instance;
            }
        }

        public Banner GetBanner(int holderId)
        {
            if (this.holderIdToBannerConfigMap != null && this.holderIdToBannerConfigMap.ContainsKey(holderId))
            {
                return this.GetRandomBanner(holderId);
            }

            return GetDefaultBannerIfPresent(holderId);
        }

        private Banner GetRandomBanner(int holderId)
        {
            BannerConfig banner = BannerGenerator.GetRandomFromBannerConfigs(holderIdToBannerConfigMap[holderId]);

            if (banner == null || Path.GetFileNameWithoutExtension(banner.Name) == "D1BFFB30-517B-4ED5-A401-FD301D03E8AB")
            {
                return GetDefaultBannerIfPresent(holderId);
            }
            else
            {
                try
                {
                    using (FileStream stream = new FileStream(Path.Combine(this.currentBannerCampaignDirInfo.FullName, Path.GetFileName(banner.Name)), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        return new Banner(BitmapFrame.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad), banner.Link);
                    }
                }
                catch (Exception ex)
                {
                    Configuration.Analytics.TrackException(ex);

                    return GetDefaultBannerIfPresent(holderId);
                }
            }
        }

        private Banner GetDefaultBannerIfPresent(int holderId)
        {
            if (this.holderIdToDefaultBannerMap.ContainsKey(holderId))
            {
                return this.holderIdToDefaultBannerMap[holderId];
            }
            else
            {
                return null;
            }
        }

        private Dictionary<int, Banner> GenerateDefaultBannersMap()
        {
            Dictionary<int, Banner> result = new Dictionary<int, Banner>();

            result.Add(1, GetDefaultBanner(DefaultBanner1ImagePath, DefaultBanner1Link));

            return result;
        }

        private Banner GetDefaultBanner(string bannerImagePath, string bannerLink)
        {
            BitmapImage image = new BitmapImage();

            image.BeginInit();

            image.UriSource = new Uri(bannerImagePath, UriKind.RelativeOrAbsolute);

            image.EndInit();

            return new Banner(image, bannerLink);
        }

        private Version GetCurrentBannerCampaignVersion()
        {
            if (!localBannersDirInfo.Exists)
            {
                return null;
            }
            DirectoryInfo[] dirBanners = localBannersDirInfo.GetDirectories();

            if (dirBanners.Length == 0)
            {
                return null;
            }
            Version currentVersion = null;

            foreach (DirectoryInfo dirInfo in dirBanners)
            {
                Version v;
                if (Version.TryParse(dirInfo.Name, out v))
                {
                    if (currentVersion == null || v > currentVersion)
                    {
                        if (dirInfo.GetFiles().Length > 0)
                        {
                            currentVersion = v;
                        }
                    }
                }
            }
            return currentVersion;
        }

        private void DeleteAllBannerCampaignVersions()
        {
            this.DeleteBannerCampaignVersions(str => true);
        }

        private void DeleteBannerCampaignVersions(Func<string, bool> shouldDeleteDirectory)
        {
            if (shouldDeleteDirectory == null)
            {
                return;
            }
            foreach (DirectoryInfo dirInfo in this.localBannersDirInfo.EnumerateDirectories())
            {
                if (shouldDeleteDirectory(dirInfo.Name))
                {
                    dirInfo.Delete(true);
                }
            }
        }

        private Dictionary<int, List<BannerConfig>> GenerateHolderIdToConfigMap(DirectoryInfo bannerCampaignDirInfo)
        {
            IEnumerable<BannerConfig> bannerConfigs = GetBannerConfigsFromDirectory(bannerCampaignDirInfo);

            Dictionary<int, List<BannerConfig>> result = new Dictionary<int, List<BannerConfig>>();

            foreach (BannerConfig bannerConfig in bannerConfigs)
            {
                if (!result.ContainsKey(bannerConfig.HolderId))
                {
                    result.Add(bannerConfig.HolderId, new List<BannerConfig>());
                }

                result[bannerConfig.HolderId].Add(bannerConfig);
            }

            return result;
        }

        private void CheckLatestServerCampaignVersionsAsync(Action<BannerEntity> completedAction)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (this.serviceInstance.ShouldShowDefaultBanners())
                    {
                        this.DeleteAllBannerCampaignVersions();
                    }
                    else
                    {
                        BannerEntity result = this.serviceInstance.GetLatestCampaign();

                        completedAction(result);
                    }
                }
                catch (EndpointNotFoundException)
                {
                }
                catch (TimeoutException)
                {
                }
                catch (CommunicationException)
                {
                }
                catch (Exception ex)
                {
                    Configuration.Analytics.TrackException(ex);
                }
            });
        }

        private void OnCheckLatestServerCampaignVersionsAsync(BannerEntity bannerEntity)
        {
            if (bannerEntity == null)
            {
                return;
            }

            if (currentBannerCampaignVersion == null)
            {
                this.DownloadBannerEntityPackage(bannerEntity);
            }
            else
            {
                Version serverVersion;

                if (Version.TryParse(bannerEntity.Version, out serverVersion))
                {
                    if (currentBannerCampaignVersion < serverVersion)
                    {
                        this.DownloadBannerEntityPackage(bannerEntity);
                    }
                    else if (currentBannerCampaignVersion == serverVersion)
                    {
                        UpdateBannersConfigsByNewVersion(currentBannerCampaignVersion.ToString());
                    }
                }
            }
        }

        private void DownloadBannerEntityPackage(BannerEntity bannerEntity)
        {
            string localBannerPackageName = Path.Combine(localBannersDirInfo.FullName, Path.GetFileName(bannerEntity.DownloadUri));

            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += OnDownloadBannerEntityPackageCompleted;
            webClient.DownloadFileAsync(new Uri(bannerEntity.DownloadUri, UriKind.RelativeOrAbsolute), localBannerPackageName, localBannerPackageName);
        }

        private void OnDownloadBannerEntityPackageCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                string localBannerPackageName = e.UserState.ToString();
                if (new FileInfo(localBannerPackageName).Length == 0)
                {
                    File.Delete(localBannerPackageName);

                    throw new Exception("The downloaded banners zip file is empty (size - 0B).");
                }

                string zipFileNameWithoutExtension = Path.GetFileNameWithoutExtension(localBannerPackageName);

                string destinationDir = Path.Combine(Path.GetDirectoryName(localBannerPackageName), zipFileNameWithoutExtension);

                if (!Directory.Exists(destinationDir))
                {
                    Directory.CreateDirectory(destinationDir);
                }

                ZipFile.ExtractToDirectory(localBannerPackageName, destinationDir);

                File.Delete(localBannerPackageName);
            }
            catch (Exception ex)
            {
                Configuration.Analytics.TrackException(ex);
            }
        }

        private void UpdateBannersConfigsByNewVersion(string version)
        {
            BannerConfig[] bannerConfigs = this.serviceInstance.GetBannersConfigsPerVersion(version);

            DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(localBannersDirInfo.FullName, version));

            foreach (BannerConfig localBannerConfigs in GetBannerConfigsFromDirectory(dirInfo))
            {
                foreach (BannerConfig serverBannerConfig in bannerConfigs)
                {
                    if (string.Equals(localBannerConfigs.Name, serverBannerConfig.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        Version localBannerVersion;
                        Version serverBannerVersion;

                        bool isLocalValid = Version.TryParse(localBannerConfigs.Version, out localBannerVersion);
                        bool isServerValid = Version.TryParse(serverBannerConfig.Version, out serverBannerVersion);

                        if (isLocalValid && isServerValid)
                        {
                            if (localBannerVersion < serverBannerVersion)
                            {
                                localBannerConfigs.Version = serverBannerConfig.Version;

                                SaveServerBanner(localBannerConfigs, version);
                            }
                        }
                        else if (!isLocalValid && isServerValid)
                        {
                            localBannerConfigs.Version = serverBannerConfig.Version;

                            SaveServerBanner(localBannerConfigs, version);
                        }
                    }
                }
            }
        }

        private void SaveServerBanner(BannerConfig bannerConfigs, string bannerCampaignVersionString)
        {
            Byte[] bytes = this.serviceInstance.GetBannerBytesByVersion(bannerConfigs.Name, bannerCampaignVersionString);

            if (TrySaveBannerConfig(bannerConfigs))
            {
                string bannerPath = Path.Combine(localBannersDirInfo.FullName, bannerCampaignVersionString, bannerConfigs.Name);

                File.WriteAllBytes(bannerPath, bytes);
            }
        }

        private IEnumerable<BannerConfig> GetBannerConfigsFromDirectory(DirectoryInfo targetDir)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(BannerConfig));

            foreach (FileInfo xmlFileInfo in targetDir.GetFiles("*.xml"))
            {
                using (FileStream fileStream = new FileStream(xmlFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    BannerConfig bannerConfig;
                    try
                    {
                        bannerConfig = xmlSerializer.Deserialize(fileStream) as BannerConfig;

                        bannerConfig.LocalFileName = xmlFileInfo.FullName;
                    }
                    catch (Exception ex)
                    {
                        Configuration.Analytics.TrackException(ex);

                        ////NOTE: Consider downloadig the xml from the server in case it is broken on client machine.
                        continue;
                    }
                    yield return bannerConfig;
                }
            }
        }

        private bool TrySaveBannerConfig(BannerConfig bannerConfigs)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(BannerConfig));

            bool isSave = false;

            using (FileStream fileStream = new FileStream(bannerConfigs.LocalFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                try
                {
                    xmlSerializer.Serialize(fileStream, bannerConfigs);

                    isSave = true;
                }
                catch (Exception ex)
                {
                    Configuration.Analytics.TrackException(ex);
                }
            }
            return isSave;
        }
    }
}
