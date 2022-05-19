using Core.Events;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Core.DynamoDbEventStore.Serialization;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Core.DynamoDbEventStore.Models;
using Core.DynamoDbEventStore.Events;

namespace Core.DynamoDbEventStore.Subscriptions;
public class DynamoDBStreamSubscription
{
    private readonly IEventBus eventBus;
    private readonly AmazonDynamoDBClient eventStoreClient;
    // private readonly ISubscriptionCheckpointRepository checkpointRepository;
    private readonly ILogger<DynamoDBStreamSubscription> logger;
    // private EventStoreDBSubscriptionToAllOptions subscriptionOptions = default!;
    // private string SubscriptionId => subscriptionOptions.SubscriptionId;
    private readonly object resubscribeLock = new();
    private CancellationToken cancellationToken;

    public DynamoDBStreamSubscription(
        AmazonDynamoDBClient eventStoreClient,
        IEventBus eventBus,
        ILogger<DynamoDBStreamSubscription> logger
    )
    {
        this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        this.eventStoreClient = eventStoreClient ?? throw new ArgumentNullException(nameof(eventStoreClient));
        // this.checkpointRepository =
        //     checkpointRepository ?? throw new ArgumentNullException(nameof(checkpointRepository));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SubscribeToTable(string tableName, CancellationToken ct)
    {
        // see: https://github.com/dotnet/runtime/issues/36063
        await Task.Yield();

        // this.subscriptionOptions = subscriptionOptions;
        cancellationToken = ct;

        // logger.LogInformation("Subscription to all '{SubscriptionId}'", subscriptionOptions.SubscriptionId);

        // var checkpoint = await checkpointRepository.Load(SubscriptionId, ct);

        // await eventStoreClient.SubscribeToAllAsync(
        //     checkpoint == null? FromAll.Start : FromAll.After(new Position(checkpoint.Value, checkpoint.Value)),
        //     HandleEvent,
        //     subscriptionOptions.ResolveLinkTos,
        //     HandleDrop,
        //     subscriptionOptions.FilterOptions,
        //     subscriptionOptions.Credentials,
        //     ct
        // );

        // logger.LogInformation("Subscription to all '{SubscriptionId}' started", SubscriptionId);
        Console.WriteLine("\n*** Retrieving table information ***");
            var request = new DescribeTableRequest
            {
                TableName = tableName
            };

            var response = await eventStoreClient.DescribeTableAsync(request);

            TableDescription description = response.Table;
            string LatestStreamArn = description.LatestStreamArn;
            if(!String.IsNullOrEmpty(LatestStreamArn))
            {
                AmazonDynamoDBStreamsConfig streamsConfig = new AmazonDynamoDBStreamsConfig{
                ServiceURL = "http://localhost:8000"
                };
                AmazonDynamoDBStreamsClient streamsClient = new AmazonDynamoDBStreamsClient("test","test",streamsConfig);
                
                string lastEvaluatedShardId = String.Empty;

                do {
                    var describeStreamResult = await streamsClient.DescribeStreamAsync(
                        new DescribeStreamRequest{
                            StreamArn = LatestStreamArn
                        });
                    var shards = describeStreamResult.StreamDescription.Shards;
                    foreach (var shard in shards)
                    {
                        Console.WriteLine("Shard: " + shard.ShardId);
                        var shardIteratorRequest = new GetShardIteratorRequest {
                            StreamArn = LatestStreamArn,
                            ShardId = shard.ShardId,
                            ShardIteratorType = ShardIteratorType.TRIM_HORIZON
                        };
                        GetShardIteratorResponse shardIteratorResponse = await streamsClient.GetShardIteratorAsync(shardIteratorRequest);
                        string currentShardIter = shardIteratorResponse.ShardIterator;
                        int processedRecordCount = 0;
                        while(!string.IsNullOrEmpty(currentShardIter) && processedRecordCount < 100){
                            try
                            {
                                var getRecordsResult = await streamsClient.GetRecordsAsync(new GetRecordsRequest{ ShardIterator = currentShardIter});
                                var records = getRecordsResult.Records;
                                foreach (var record in records)
                                {
                                    Console.WriteLine("Record: " + record.Dynamodb);
                                    var jsonResult=Document.FromAttributeMap(record.Dynamodb.NewImage).ToJson();
                                    JObject jsonObject = JObject.Parse(jsonResult);
                                    if(jsonObject.ContainsKey("event_type"))
                                    {
                                        var eventRecord = jsonObject.ToObject<EventRecord>()!;
                                        var streamId = Guid.Parse(jsonObject.Value<string>("stream_id")!);
                                        var eventType = jsonObject.Value<string>("event_type")!;
                                        eventRecord.StreamId = streamId;
                                        eventRecord.EventType = eventType;
                                        var eventData = DynamoDBStreamEventExtensions.ToStreamEvent(eventRecord)!;
                                        await eventBus.Publish(eventData, cancellationToken);
                                    }
                                }
                                processedRecordCount += records.Count;
                                currentShardIter = getRecordsResult.NextShardIterator;
                            }
                            catch(Amazon.DynamoDBv2.Model.ResourceNotFoundException)
                            {
                                currentShardIter = String.Empty;
                                //Console.WriteLine(ex.ToString());
                            }
                            catch(Amazon.DynamoDBv2.AmazonDynamoDBException)
                            {
                                currentShardIter = String.Empty;
                            }
                        }
                    }
                } while(lastEvaluatedShardId != null);
                Console.WriteLine("yeah");
            }
            

            Console.WriteLine("Name: {0}", description.TableName);
            Console.WriteLine("# of items: {0}", description.ItemCount);
            Console.WriteLine("Provision Throughput (reads/sec): {0}",
                      description.ProvisionedThroughput.ReadCapacityUnits);
            Console.WriteLine("Provision Throughput (writes/sec): {0}",
                      description.ProvisionedThroughput.WriteCapacityUnits);
    }

    private async Task HandleEvent(EventRecord resolvedEvent,
        CancellationToken ct)
    {
        if(string.IsNullOrEmpty(resolvedEvent.Data)) return;
        try
        {
            // publish event to internal event bus
            await eventBus.Publish(resolvedEvent, ct);
        }
        catch (Exception e)
        {
            logger.LogError("Error consuming message: {ExceptionMessage}{ExceptionStackTrace}", e.Message,
                e.StackTrace);
            // if you're fine with dropping some events instead of stopping subscription
            // then you can add some logic if error should be ignored
            throw;
        }
    }

    // private void HandleDrop(StreamSubscription _, SubscriptionDroppedReason reason, Exception? exception)
    // {
    //     logger.LogError(
    //         exception,
    //         "Subscription to all '{SubscriptionId}' dropped with '{Reason}'",
    //         SubscriptionId,
    //         reason
    //     );

    //     if (exception is RpcException { StatusCode: StatusCode.Cancelled })
    //         return;

    //     Resubscribe();
    // }

    // private void Resubscribe()
    // {
    //     // You may consider adding a max resubscribe count if you want to fail process
    //     // instead of retrying until database is up
    //     while (true)
    //     {
    //         var resubscribed = false;
    //         try
    //         {
    //             Monitor.Enter(resubscribeLock);

    //             // No synchronization context is needed to disable synchronization context.
    //             // That enables running asynchronous method not causing deadlocks.
    //             // As this is a background process then we don't need to have async context here.
    //             using (NoSynchronizationContextScope.Enter())
    //             {
    //                 SubscribeToAll(subscriptionOptions, cancellationToken).Wait(cancellationToken);
    //             }

    //             resubscribed = true;
    //         }
    //         catch (Exception exception)
    //         {
    //             logger.LogWarning(exception,
    //                 "Failed to resubscribe to all '{SubscriptionId}' dropped with '{ExceptionMessage}{ExceptionStackTrace}'",
    //                 SubscriptionId, exception.Message, exception.StackTrace);
    //         }
    //         finally
    //         {
    //             Monitor.Exit(resubscribeLock);
    //         }

    //         if (resubscribed)
    //             break;

    //         // Sleep between reconnections to not flood the database or not kill the CPU with infinite loop
    //         // Randomness added to reduce the chance of multiple subscriptions trying to reconnect at the same time
    //         Thread.Sleep(1000 + new Random((int)DateTime.UtcNow.Ticks).Next(1000));
    //     }
    // }

    // private bool IsEventWithEmptyData(ResolvedEvent resolvedEvent)
    // {
    //     if (resolvedEvent.Event.Data.Length != 0) return false;

    //     logger.LogInformation("Event without data received");
    //     return true;
    // }

    // private bool IsCheckpointEvent(ResolvedEvent resolvedEvent)
    // {
    //     if (resolvedEvent.Event.EventType != EventTypeMapper.ToName<CheckpointStored>()) return false;

    //     logger.LogInformation("Checkpoint event - ignoring");
    //     return true;
    // }
}
