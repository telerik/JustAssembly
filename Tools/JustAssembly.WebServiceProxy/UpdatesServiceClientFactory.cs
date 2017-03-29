using JustAssembly.WebServiceProxy.Service;
using System;
using System.Linq;
using System.ServiceModel;
using System.Xml;
using System.Xml.Linq;

namespace JustAssembly.WebServiceProxy
{
    public static class UpdatesServiceClientFactory
    {
        private const string WebServiceAddress = "http://justdecompile.telerik.com/AutoUpdatesService/UpdatesService.svc";

        public static IUpdatesService CreateNew()
        {
            var binding = new BasicHttpBinding();
            binding.CloseTimeout = TimeSpan.FromSeconds(15);
            binding.OpenTimeout = TimeSpan.FromSeconds(15);
            binding.ReceiveTimeout = TimeSpan.FromSeconds(25);
            binding.SendTimeout = TimeSpan.FromSeconds(15);
            binding.AllowCookies = true;
            binding.BypassProxyOnLocal = false;

            binding.MaxBufferPoolSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.ReaderQuotas = new XmlDictionaryReaderQuotas
            {
                MaxDepth = 32,
                MaxStringContentLength = 2147483647,
                MaxArrayLength = 2147483647,
                MaxBytesPerRead = 2147483647,
                MaxNameTableCharCount = 2147483647
            };
            binding.TransferMode = TransferMode.Buffered;
            binding.UseDefaultWebProxy = true;

#if !USELOCALUPDATESERVICE
            try
            {
                string configFileLocation = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

                string value = XElement.Load(configFileLocation)
                                       .Descendants("endpoint")
                                       .Select(x => x.Attribute("address"))
                                       .FirstOrDefault()
                                       .Value;

                return new UpdatesServiceClient(binding, new EndpointAddress(value));
            }
            catch
            {
                EndpointAddress address = new EndpointAddress(WebServiceAddress);

                return new UpdatesServiceClient(binding, address);
            }
#else
            EndpointAddress address = new EndpointAddress("");

            return new UpdatesServiceClient(binding, address);
#endif
        }
    }
}
