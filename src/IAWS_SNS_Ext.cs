using OutSystems.ExternalLibraries.SDK;
using psn.PH.Structures;

namespace psn.PH
{
    /// <summary>
    /// Connect to the Amazon Simple Notification Service (SNS) and perform tasks including deliver messages from producers to consumers based on topics they subscribe to.
    /// </summary>
    [OSInterface(Description = "Connect to the Amazon Simple Notification Service (SNS) and perform tasks including deliver messages from producers to consumers based on topics they subscribe to.", Name = "AWS_SNS_ExternalLogic", IconResourceName = "psn.PH.AWS_SNS_ExtIcon.png")]
    public interface IAWS_SNS_Ext
    {
        /// <summary>
        /// Create a new topic
        /// </summary>
        public string Topic_Create_Ext(AWS_Authenticationinfo authInfo, string TopicName);
        /// <summary>
        /// Delete an existing topic
        /// </summary>
        public bool Topic_Delete_Ext(AWS_Authenticationinfo authInfo, string TopicArn);
        /// <summary>
        /// Publish a message to a topic
        /// </summary>
        public string Topic_Publish_Ext(AWS_Authenticationinfo authInfo, string TopicArn, string Message);
        /// <summary>
        /// Retrieve a list of existing topics
        /// </summary>
        public List<string> Topic_List_Ext(AWS_Authenticationinfo authInfo);
        /// <summary>
        /// Subscribe to a topic 
        /// </summary>
        public string Topic_Subscribe_Ext(AWS_Authenticationinfo authInfo, string Protocol, string Endpoint, string TopicArn);
        /// <summary>
        /// Confirm a subscription exist
        /// </summary>
        public string Subscription_Confirm_Ext(AWS_Authenticationinfo authInfo, string Topic_Arn, string token);
        /// <summary>
        /// Retrieve a list of subscriptions
        /// </summary>
        public SubscriptionListResponse Subscription_List_Ext(AWS_Authenticationinfo authInfo, string NextToken);
        /// <summary>
        /// Unsubscribe from a subscription
        /// </summary>
        public bool Subscription_Unsubscribe_Ext(AWS_Authenticationinfo authInfo, string SubscriptionArn);
        /// <summary>
        /// Unsubscribe from a subscription
        /// </summary>
        public string Endpoint_Create_Ext(AWS_Authenticationinfo authInfo, string PlatformApplicationArn, string CustomUserData, string token);

        public bool Endpoint_Delete_Ext(AWS_Authenticationinfo authInfo, string EndpointArn);

        //public bool Validate_Signature_Ext(AWS_Authenticationinfo authInfo, string SNSMessage);

        /// <summary>
        /// Retrieve unique build information of this custom library.
        /// </summary>
        [OSAction(Description = "Get unique build information of this custom library.", ReturnName = "buildInfo")]
        public string GetBuildInfo_Ext();

    }
}