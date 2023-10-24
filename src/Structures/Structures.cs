using OutSystems.ExternalLibraries.SDK;
using Newtonsoft.Json;

namespace psn.PH.Structures
{
    [OSStructure(Description = "Auth info")]
    public struct AWS_Authenticationinfo
    {
        [OSStructureField(DataType = OSDataType.Text, Description = "AWS Region", IsMandatory = true)]
        public string Region;
        [OSStructureField(DataType = OSDataType.Text, Description = "AWS Access Key ID", IsMandatory = true)]
        public string AccessKeyId;
        [OSStructureField(DataType = OSDataType.Text, Description = "AWS Secret Access Key", IsMandatory = true)]
        public string SecretAccessKey;
    }

    [OSStructure(Description = "Subscription value object")]
    public struct Subscription
    {
        public string Endpoint;
        public string Owner;
        public string Protocol;
        public string SubscriptionArn;
        public string TopicArn;
    }

    [OSStructure(Description = "Subscription listing response value holder")]
    public struct SubscriptionListResponse
    {
        public List<Subscription> Subscriptions;
        public string NextTokenPagination;
    }
}