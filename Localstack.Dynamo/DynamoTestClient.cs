using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

namespace Localstack.Dynamo
{
    public class DynamoTestClient
    {
        private const string TableName = "LocalstackTable";
        private readonly AmazonDynamoDBClient _client;

        public DynamoTestClient(AmazonDynamoDBClient client)
        {
            _client = client;
        }

        public async Task CreateTableIfNotExistsAsync()
        {
            var tables = await _client.ListTablesAsync();
            if (tables.TableNames.Contains(TableName))
                return;

            var request = new CreateTableRequest
            {
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Id",
                        AttributeType = "N",
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "Message",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Id",
                        KeyType = "HASH" //Partition key,
                    },
                    new KeySchemaElement
                    {
                        AttributeName = "Message",
                        KeyType = "RANGE" //Sort key
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 10,
                    WriteCapacityUnits = 5
                },
                TableName = TableName
            };
            await _client.CreateTableAsync(request);
        }

        public async Task<string> PutItemAsync(string message)
        {
            string id = "1";
            var item = new Dictionary<string, AttributeValue>()
            {
                {"Id", new AttributeValue {N = id}},
                {"Message", new AttributeValue {S = message}},
            };
            PutItemRequest putItemRequest = new PutItemRequest(TableName, item);
            await _client.PutItemAsync(putItemRequest);
            return id;
        }

        public async Task<string> GetItemMessageAsync(string id)
        {
            var request = new QueryRequest
            {
                TableName = TableName,
                KeyConditionExpression = "Id = :v_Id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":v_Id", new AttributeValue {N = id}}
                }
            };
            var response = await _client.QueryAsync(request);
            return response.Items.First()["Message"].S;
        }
    }
}