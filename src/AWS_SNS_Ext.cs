using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using psn.PH.Structures;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace psn.PH
{
    public class AWS_SNS_Ext : IAWS_SNS_Ext
    {
        private IAmazonSimpleNotificationService getClient(AWS_Authenticationinfo authInfo)
        {
            AmazonSimpleNotificationServiceClient client = new AmazonSimpleNotificationServiceClient(authInfo.AccessKeyId.Trim(), authInfo.SecretAccessKey.Trim(), RegionEndpoint.GetBySystemName(authInfo.Region.Trim()));
            return client;
        }
        private static async Task<string> CreateSNSTopicAsync(IAmazonSimpleNotificationService client, string topicName)
        {
            var request = new CreateTopicRequest
            {
                Name = topicName,
            };

            var response = await client.CreateTopicAsync(request);

            return response.TopicArn;
        }
        public string Topic_Create_Ext(AWS_Authenticationinfo authInfo, string TopicName)
        {
            IAmazonSimpleNotificationService client = getClient(authInfo);
            var result = CreateSNSTopicAsync(client, TopicName);
            return result.Result;
        }
        private async Task<bool> DeleteTopicByArn(IAmazonSimpleNotificationService client, string topicArn)
        {
            var deleteResponse = await client.DeleteTopicAsync(
                new DeleteTopicRequest()
                {
                    TopicArn = topicArn
                });

            return deleteResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        public bool Topic_Delete_Ext(AWS_Authenticationinfo authInfo, string TopicArn)
        {
            IAmazonSimpleNotificationService client = getClient(authInfo);
            var request = DeleteTopicByArn(client, TopicArn);
            return request.Result;
        }
        private static async Task<string> PublishToTopicAsync(IAmazonSimpleNotificationService client, string topicArn, string messageText)
        {
            var request = new PublishRequest
            {
                TopicArn = topicArn,
                Message = messageText,
            };

            var response = await client.PublishAsync(request);

            return response.MessageId;
        }
        public string Topic_Publish_Ext(AWS_Authenticationinfo authInfo, string TopicArn, string Message)
        {
            IAmazonSimpleNotificationService client = getClient(authInfo);
            var result = PublishToTopicAsync(client, TopicArn, Message);
            return result.Result;
        }

        private static async Task<List<string>> GetTopicListAsync(IAmazonSimpleNotificationService client)
        {
            // If there are more than 100 Amazon SNS topics, the call to
            // ListTopicsAsync will return a value to pass to the
            // method to retrieve the next 100 (or less) topics.
            string nextToken = string.Empty;
            List<string> result = new List<string>();
            do
            {
                var response = await client.ListTopicsAsync(nextToken);
                foreach (var topic in response.Topics)
                {
                    result.Add(topic.TopicArn);
                }
                nextToken = response.NextToken;
            }
            while (!string.IsNullOrEmpty(nextToken));
            return result;
        }
        public List<string> Topic_List_Ext(AWS_Authenticationinfo authInfo)
        {
            IAmazonSimpleNotificationService client = getClient(authInfo);
            var result = GetTopicListAsync(client);
            return result.Result; // list of topic arn
        }
        private static async Task<SubscribeResponse> TopicSubscribeAsync(IAmazonSimpleNotificationService client, SubscribeRequest request, string topicArn)
        {
            var response = await client.SubscribeAsync(request);
            return response;
        }
        public string Topic_Subscribe_Ext(AWS_Authenticationinfo authInfo, string Protocol, string Endpoint, string TopicArn)
        {
            IAmazonSimpleNotificationService client = getClient(authInfo);
            SubscribeRequest request = new SubscribeRequest()
            {
                TopicArn = TopicArn,
                ReturnSubscriptionArn = true,
                Protocol = Protocol,
                Endpoint = Endpoint,
            };
            var response = TopicSubscribeAsync(client, request, TopicArn);
            return response.Result.SubscriptionArn;
        }

        public bool Validate_Signature_Ext(AWS_Authenticationinfo authInfo, string SNSMessage)
        {
            return false;
        }
        private static async Task<string> Subscription_Confirm(IAmazonSimpleNotificationService client, string Topic_Arn, string token)
        {
            var request = new ConfirmSubscriptionRequest()
            {
                TopicArn = Topic_Arn,
                Token = token,
            };
            var response = await client.ConfirmSubscriptionAsync(request);
            return response.SubscriptionArn;
        }
        public string Subscription_Confirm_Ext(AWS_Authenticationinfo authInfo, string Topic_Arn, string token)
        {
            IAmazonSimpleNotificationService client = getClient(authInfo);
            var result = Subscription_Confirm(client, Topic_Arn, token);
            return result.Result;
        }
        private static async Task<SubscriptionListResponse> Subscription_List(IAmazonSimpleNotificationService client, string token)
        {

            var response = await client.ListSubscriptionsAsync(token);
            List<psn.PH.Structures.Subscription> subscriptions = new List<psn.PH.Structures.Subscription>();
            foreach (var sub in response.Subscriptions)
            {
                psn.PH.Structures.Subscription vo = new psn.PH.Structures.Subscription()
                {
                    Endpoint = sub.Endpoint,
                    Owner = sub.Owner,
                    Protocol = sub.Protocol,
                    SubscriptionArn = sub.SubscriptionArn,
                    TopicArn = sub.TopicArn,
                };
                subscriptions.Add(vo);
            }
            SubscriptionListResponse result = new SubscriptionListResponse()
            {
                Subscriptions = subscriptions,
                NextTokenPagination = response.NextToken,
            };
            return result;
        }
        public SubscriptionListResponse Subscription_List_Ext(AWS_Authenticationinfo authInfo, string NextToken)
        {
            IAmazonSimpleNotificationService client = getClient(authInfo);
            var response = Subscription_List(client, NextToken);
            return response.Result;
        }

        private static async Task<bool> Subscription_Unsubscribe(IAmazonSimpleNotificationService client, string SubscriptionArn)
        {
            var request = new UnsubscribeRequest()
            {
                SubscriptionArn = SubscriptionArn,
            };
            var response = await client.UnsubscribeAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public bool Subscription_Unsubscribe_Ext(AWS_Authenticationinfo authInfo, string SubscriptionArn)
        {
            IAmazonSimpleNotificationService client = getClient(authInfo);
            var result = Subscription_Unsubscribe(client, SubscriptionArn);
            return result.Result;
        }

        private static async Task<string> Endpoint_Create(IAmazonSimpleNotificationService client, string PlatformApplicationArn, string CustomUserData, string token)
        {
            var request = new CreatePlatformEndpointRequest()
            {
                PlatformApplicationArn = PlatformApplicationArn,
                Token = token,
                CustomUserData = CustomUserData,
            };
            var response = await client.CreatePlatformEndpointAsync(request);
            return response.EndpointArn;
        }

        public string Endpoint_Create_Ext(AWS_Authenticationinfo authInfo, string PlatformApplicationArn, string CustomUserData, string token)
        {
            IAmazonSimpleNotificationService client = getClient(authInfo);
            var result = Endpoint_Create(client, PlatformApplicationArn, CustomUserData, token);
            return result.Result;
        }

        private static async Task<bool> Endpoint_Delete(IAmazonSimpleNotificationService client, string EndpointArn)
        {
            var request = new DeleteEndpointRequest()
            {
                EndpointArn = EndpointArn,
            };
            var response = await client.DeleteEndpointAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public bool Endpoint_Delete_Ext(AWS_Authenticationinfo authInfo, string EndpointArn)
        {
            IAmazonSimpleNotificationService client = getClient(authInfo);
            var result = Endpoint_Delete(client, EndpointArn);
            return result.Result;
        }

        private string ReadResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = name;
            if (assembly.GetManifestResourceStream(resourcePath) != null)
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourcePath)!)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            return string.Empty;
        }

        public string GetBuildInfo_Ext()
        {
            return ReadResource("psn.PH.buildinfo.txt");
        }
    }
}
