using StackExchange.Redis;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using AljasAuthApi.Config;

namespace AljasAuthApi.Services
{
    public class RedisService
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly string _streamKey;

        public RedisService(RedisSettings settings)
        {
            _redis = ConnectionMultiplexer.Connect(settings.ConnectionString);
            _db = _redis.GetDatabase();
            _streamKey = settings.StreamKey;
        }

        public async Task PublishEventAsync(string action, object data)
        {
            var eventData = new
            {
                Action = action,
                Timestamp = DateTime.UtcNow,
                Data = data
            };

            string jsonData = JsonConvert.SerializeObject(eventData);
            await _db.StreamAddAsync(_streamKey, new NameValueEntry[] { new NameValueEntry("message", jsonData) });
        }
    }
}
