using Serilog;
using StackExchange.Redis;

public class RedisHelper : IDisposable
{
    private readonly string redisConnectionString = "localhost:6379"; // Replace with your Redis server details
    private ConnectionMultiplexer redis;

    public RedisHelper(string connectionString)
    {
        redisConnectionString = connectionString;

        Connect();
    }

    private void Connect()
    {
        try
        {
            redis = ConnectionMultiplexer.Connect(redisConnectionString);
            Console.WriteLine("Connected to Redis successfully.");
        }
        catch (Exception ex)
        {
            Log.Error($"{ex.Message}");

            throw;
        }
    }

    public IDatabase GetDatabase()
    {
        try
        {
            return redis.GetDatabase();
        }
        catch (Exception ex)
        {
            Log.Error($"{ex.Message}");

            throw;
        }
    }

    public void Dispose()
    {
        redis.Dispose();
    }
}