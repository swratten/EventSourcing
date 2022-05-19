namespace Core.DynamoDbEventStore.OptimisticConcurrency;

public class DynamoDBExpectedStreamVersionProvider
{
    public long? Value { get; private set; }

    public bool TrySet(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !long.TryParse(value, out var expectedVersion))
            return false;

        Value = expectedVersion;
        return true;
    }
}

public class DynamoDBNextStreamVersionProvider
{
    public long? Value { get; private set; }

    public void Set(long nextVersion)
    {
        Value = nextVersion;
    }
}
