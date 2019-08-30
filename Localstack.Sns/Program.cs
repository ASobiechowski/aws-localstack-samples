using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Localstack.Sqs;

namespace Localstack.Sns
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var awsSnsClient = CreateAwsSnsClient();

            string topicName = "Localstack.Sns.Topic";
            var topicArn = await CreateSnsTopic(awsSnsClient, topicName);

            var awsSqsClient = CreateAwsSqsClient();
            SqsTestClient sqsClient = new SqsTestClient(awsSqsClient);

            string queueName = "Localstack.Sns.Sqs";
            await sqsClient.CreateQueueIfNotExistsAsync(queueName);
            var queueUrl = await sqsClient.GetQueueUrlAsync(queueName);
            
            await awsSnsClient.SubscribeQueueAsync(topicArn, awsSqsClient, queueUrl);
            await awsSnsClient.PublishAsync(topicArn, "Published to topic");
            var receivedMessage = sqsClient.ReceiveMessageAsync(queueUrl);
            Console.WriteLine($"Received: {receivedMessage}");
        }

        private static AmazonSQSClient CreateAwsSqsClient()
        {
            var amazonSqsConfig = new AmazonSQSConfig
            {
                //uncomment to use localstack
                ServiceURL = "http://localhost:4576",
            };

            AmazonSQSClient amazonSqsClient = new AmazonSQSClient(amazonSqsConfig);
            return amazonSqsClient;
        }

        private static AmazonSimpleNotificationServiceClient CreateAwsSnsClient()
        {
            AmazonSimpleNotificationServiceConfig config = new AmazonSimpleNotificationServiceConfig
            {
                //uncomment to use localstack
                ServiceURL = "http://localhost:4575",
            };
            AmazonSimpleNotificationServiceClient amazonSnsServiceClient =
                new AmazonSimpleNotificationServiceClient(config);
            return amazonSnsServiceClient;
        }

        private static async Task<string> CreateSnsTopic(AmazonSimpleNotificationServiceClient amazonSnsServiceClient, string topicName)
        {
            await amazonSnsServiceClient.CreateTopicAsync(topicName);

            var listTopicsResponse = await amazonSnsServiceClient.ListTopicsAsync();
            string topicArn = listTopicsResponse.Topics.FirstOrDefault()?.TopicArn;
            return topicArn;
        }
    }
}