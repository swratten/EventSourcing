using Core.DynamoDbEventStore.Events;
using EfficientDynamoDb;
using EfficientDynamoDb.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Logging;
using Core.BackgroundWorkers;
using Core.DynamoDbEventStore.Subscriptions;

namespace Core.DynamoDbEventStore;
public static class DynamoDBConfigExtensions
{
    private const string DefaultConfigKey = "EventStore";

    private static AmazonDynamoDBConfig config = new AmazonDynamoDBConfig {
        ServiceURL = "http://localhost:8000"
    };
    private static AmazonDynamoDBClient client = new AmazonDynamoDBClient("test", "test", config);
    // private async static void ListMyTables()
    //     {
    //         Console.WriteLine("\n*** listing tables ***");
    //         string lastTableNameEvaluated = String.Empty;
    //         do
    //         {
    //             var request = new ListTablesRequest
    //             {
    //                 Limit = 2
    //             };
    //             if(lastTableNameEvaluated != String.Empty)
    //             {
    //                 request.ExclusiveStartTableName = lastTableNameEvaluated;
    //             }

    //             var response = await client.ListTablesAsync(request);
    //             foreach (string name in response.TableNames)
    //                 GetTableInformation(name);

    //             lastTableNameEvaluated = response.LastEvaluatedTableName;
    //         } while (lastTableNameEvaluated != null);
    //     }

    //     private async static void GetTableInformation(string tableName)
    //     {
    //         Console.WriteLine("\n*** Retrieving table information ***");
    //         var request = new DescribeTableRequest
    //         {
    //             TableName = tableName
    //         };

    //         var response = await client.DescribeTableAsync(request);

    //         TableDescription description = response.Table;
    //         string LatestStreamArn = description.LatestStreamArn;
    //         if(!String.IsNullOrEmpty(LatestStreamArn))
    //         {
    //             AmazonDynamoDBStreamsConfig streamsConfig = new AmazonDynamoDBStreamsConfig{
    //             ServiceURL = "http://localhost:8000"
    //             };
    //             AmazonDynamoDBStreamsClient streamsClient = new AmazonDynamoDBStreamsClient("test","test",streamsConfig);
                
    //             string lastEvaluatedShardId = String.Empty;

    //             do {
    //                 var describeStreamResult = await streamsClient.DescribeStreamAsync(
    //                     new DescribeStreamRequest{
    //                         StreamArn = LatestStreamArn
    //                     });
    //                 var shards = describeStreamResult.StreamDescription.Shards;
    //                 foreach (var shard in shards)
    //                 {
    //                     Console.WriteLine("Shard: " + shard.ShardId);
    //                     var shardIteratorRequest = new GetShardIteratorRequest {
    //                         StreamArn = LatestStreamArn,
    //                         ShardId = shard.ShardId,
    //                         ShardIteratorType = ShardIteratorType.TRIM_HORIZON
    //                     };
    //                     GetShardIteratorResponse shardIteratorResponse = await streamsClient.GetShardIteratorAsync(shardIteratorRequest);
    //                     string currentShardIter = shardIteratorResponse.ShardIterator;
    //                     int processedRecordCount = 0;
    //                     while(currentShardIter != null && processedRecordCount < 100){
    //                         try{
    //                         var getRecordsResult = await streamsClient.GetRecordsAsync(new GetRecordsRequest{ ShardIterator = currentShardIter});
    //                         var records = getRecordsResult.Records;
    //                         foreach (var record in records)
    //                         {
    //                             Console.WriteLine("Record: " + record.Dynamodb);
    //                         }
    //                         processedRecordCount += records.Count;
    //                         currentShardIter = getRecordsResult.NextShardIterator;
    //                         }
    //                         catch(Exception ex)
    //                         {
    //                             Console.WriteLine(ex.ToString());
    //                         }
    //                     }
    //                 }
    //             } while(lastEvaluatedShardId != null);
    //             Console.WriteLine("yeah");
    //         }
            

    //         Console.WriteLine("Name: {0}", description.TableName);
    //         Console.WriteLine("# of items: {0}", description.ItemCount);
    //         Console.WriteLine("Provision Throughput (reads/sec): {0}",
    //                   description.ProvisionedThroughput.ReadCapacityUnits);
    //         Console.WriteLine("Provision Throughput (writes/sec): {0}",
    //                   description.ProvisionedThroughput.WriteCapacityUnits);
    //     }


    public static IServiceCollection AddDynamoDB(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        
        var updateTableRequest = new UpdateTableRequest();
        updateTableRequest.StreamSpecification = new StreamSpecification {
            StreamEnabled = true,
            StreamViewType = StreamViewType.NEW_IMAGE
        };
        updateTableRequest.TableName = "write_events";

        try
        {
            client.UpdateTableAsync(updateTableRequest).Wait();
        }
        catch (Exception)
        {
            
        }
        

        //ListMyTables();

        var context = new DynamoDbContext(
                new DynamoDbContextConfig(
                        RegionEndpoint.Create(RegionEndpoint.EUWest1, "http://localhost:8000"),
                        new AwsCredentials("test", "test")
                    )
                );

        services
            .AddSingleton(
                context
            )
            .AddSingleton(client)
            .AddDynamoDBAppendScope()
            .AddTransient<DynamoDBStreamSubscription, DynamoDBStreamSubscription>();
            // .AddMarten(sp => SetStoreOptions(sp, martenConfig, configureOptions))
            // .ApplyAllDatabaseChangesOnStartup()
            // .AddAsyncDaemon(DaemonMode.Solo);

        return services;
    }

    public static IServiceCollection AddDynamoDBStreamSubscription(
        this IServiceCollection services, string tableName)
    {
        return services.AddHostedService(serviceProvider =>
            {
                var logger =
                    serviceProvider.GetRequiredService<ILogger<BackgroundWorker>>();

                var dynamoDBSubscription =
                    serviceProvider.GetRequiredService<DynamoDBStreamSubscription>();

                return new BackgroundWorker(
                    logger,
                    ct =>
                        dynamoDBSubscription.SubscribeToTable(
                            tableName,
                            ct
                        )
                );
            }
        );
    }
}