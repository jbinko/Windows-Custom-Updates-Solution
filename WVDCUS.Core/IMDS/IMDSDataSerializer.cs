using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace WVDCUS.Core
{
    internal sealed class IMDSData
    {
        public string VMResourceName { get; internal set; }
        public string ResourceGroupName { get; internal set; }
        public string SubscriptionId { get; internal set; }
    }

    [DataContract]
    internal sealed class IMDSDataContract
    {
        [DataMember(Name = "compute")]
        public IMDSDataComputeContract Compute { get; set; }
    }

    [DataContract]
    internal sealed class IMDSDataComputeContract
    {
        [DataMember(Name = "name")]
        public string VMResourceName { get; set; }

        [DataMember(Name = "resourceGroupName")]
        public string ResourceGroupName { get; set; }

        [DataMember(Name = "subscriptionId")]
        public string SubscriptionId { get; set; }
    }

    internal sealed class IMDSDataSerializer
    {
        public static IMDSData Deserialize(string json)
        {
            var serializer = new DataContractJsonSerializer(typeof(IMDSDataContract));

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var data = (IMDSDataContract)serializer.ReadObject(ms);

                if (data != null && data.Compute != null)
                {
                    return new IMDSData
                    {
                        VMResourceName = data.Compute.VMResourceName,
                        ResourceGroupName = data.Compute.ResourceGroupName,
                        SubscriptionId = data.Compute.SubscriptionId,
                    };
                }
            }

            return null;
        }
    }
}
