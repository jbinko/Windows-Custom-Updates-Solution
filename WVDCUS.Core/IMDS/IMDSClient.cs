using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WVDCUS.Core
{
    internal sealed class IMDSClient
    {
        private IMDSClient()
        {
        }

        public static async Task<IMDSData> GetInfoAsync()
        {
            const string Request = "http://169.254.169.254/metadata/instance?api-version=2017-08-01";

            try
            {
                Logger.WriteInfo("IMDS Data Downloading ....");

                using (var httpClient = new HttpClient())
                {
                    // 10s should be enough - if not running in Azure VM -> it will timeout
                    httpClient.Timeout = new TimeSpan(0, 0, 10);
                    httpClient.DefaultRequestHeaders.Add("Metadata", "true");

                    using (var response = await httpClient.GetAsync(Request))
                    {
                        response.EnsureSuccessStatusCode();
                        using (var content = response.Content)
                        {
                            string responseJsonBody = await response.Content.ReadAsStringAsync();
                            var data = IMDSDataSerializer.Deserialize(responseJsonBody);

                            if (data != null)
                            {
                                Logger.WriteInfo("IMDS Data Downloaded");
                                return data;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.WriteException(e);
            }

            Logger.WriteInfo("IMDS Data Downloaded - Failed");
            return null;
        }
    }
}
