using Xunit;
using Xunit.Abstractions;

using psn.PH.Structures;
namespace psn.PH;

public class AWS_SNS_ExtTests
{
    private string aws_sns_access_id = Environment.GetEnvironmentVariable("AWS_SNS_ACCESS_ID") ?? "Sample_value";
    private string aws_sns_secret_access_key = Environment.GetEnvironmentVariable("AWS_SNS_SECRET_ACCESS_KEY") ?? "Sample_value";
    private string aws_sns_region = Environment.GetEnvironmentVariable("AWS_SNS_REGION") ?? "Sample_value";
    private readonly ITestOutputHelper output;
    AWS_Authenticationinfo authInfo;

    public AWS_SNS_ExtTests(ITestOutputHelper output)
    {
        this.output = output;
        this.authInfo = getAuthInfo();
    }

    private AWS_Authenticationinfo getAuthInfo()
    {
        AWS_Authenticationinfo authInfo = new AWS_Authenticationinfo()
        {
            Region = aws_sns_region,
            AccessKeyId = aws_sns_access_id.Trim(),
            SecretAccessKey = aws_sns_secret_access_key.Trim(),
        };
        output.WriteLine("Region = " + authInfo.Region);
        return authInfo;
    }

    [Fact]
    public void Topic_Create_Delete_Ext_test1()
    {
        var ext = new AWS_SNS_Ext();
        string arn = ext.Topic_Create_Ext(authInfo, "Topic_Create_Ext_test1");
        output.WriteLine("Topic ARN = " + arn);
        Assert.True(arn.Length > 0);
        Assert.StartsWith("arn:aws:sns:", arn);
        Assert.EndsWith("Topic_Create_Ext_test1", arn);
        Assert.True(ext.Topic_Delete_Ext(authInfo, arn));
    }

    [Fact]
    public void Topic_Publish_Ext_test1()
    {
        var ext = new AWS_SNS_Ext();
        string topicArn = ext.Topic_Create_Ext(authInfo, "Topic_Publish_Ext_test1");
        string message = "Hello World!";
        string messageId = ext.Topic_Publish_Ext(authInfo, topicArn, message);
        output.WriteLine("MessageID = " + messageId);
        ext.Topic_Delete_Ext(authInfo, topicArn);
        Assert.True(messageId.Length > 0 && messageId.Length <= 100);
    }

    [Fact]
    public void Topic_List_Ext_test1()
    {
        var ext = new AWS_SNS_Ext();
        string[] topic_names = { "Topic_List_Ext_test1", "Topic_List_Ext_test2" };
        int match_count = 0;
        string topicArn1 = ext.Topic_Create_Ext(authInfo, topic_names[0]);
        string topicArn2 = ext.Topic_Create_Ext(authInfo, topic_names[1]);
        List<string> topics = ext.Topic_List_Ext(authInfo);
        ext.Topic_Delete_Ext(authInfo, topicArn1);
        ext.Topic_Delete_Ext(authInfo, topicArn2);

        foreach (var t in topics)
        {
            for (int i = 0; i < topic_names.Length; i++)
            {
                if (t.EndsWith(topic_names[i]))
                {
                    match_count++;
                    break;
                }
            }
            output.WriteLine("Topic [" + t + "]");
        }
        Assert.True(match_count == topic_names.Length);
    }


    [Fact]
    public void GetBuildInfo_Ext_test1()
    {
        var ext = new AWS_SNS_Ext();
        string buildInfo = ext.GetBuildInfo_Ext();
        output.WriteLine(buildInfo);
        Assert.True(buildInfo.Length > 0);
    }
}