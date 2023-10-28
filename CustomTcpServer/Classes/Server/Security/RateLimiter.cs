using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace InfinityServer.Classes.Server.Security
{
    public class RateLimiter
    {
        // Rate Limiting
        private readonly ConcurrentDictionary<string, int> rateLimits = new ConcurrentDictionary<string, int>();
        private readonly ConcurrentDictionary<string, DateTime> lastRequestTimes = new ConcurrentDictionary<string, DateTime>();

        private int _maxRequests;

        private int _rateLimitDurationSeconds;

        private Timer _evictionTimer;

        public RateLimiter()
        {
            _maxRequests = 50;
            _rateLimitDurationSeconds = 60;
            _evictionTimer = new Timer(EvictOldEntries, null, TimeSpan.FromSeconds(_rateLimitDurationSeconds), TimeSpan.FromSeconds(_rateLimitDurationSeconds));
        }

        public bool IsRateLimited(string clientId)
        {
            if (!rateLimits.ContainsKey(clientId))
            {
                rateLimits.TryAdd(clientId, 0);
                lastRequestTimes.TryAdd(clientId, DateTime.Now);
            }

            // Check if the time window has passed and reset
            if ((DateTime.Now - lastRequestTimes[clientId]).TotalSeconds >= _rateLimitDurationSeconds)
            {
                rateLimits[clientId] = 0;
                lastRequestTimes[clientId] = DateTime.Now;
            }

            // Check rate limit
            if (rateLimits[clientId] >= _maxRequests)
            {
                return true;
            }

            // Increment the rate limit counter
            rateLimits[clientId]++;
            return false;
        }

        public TimeSpan GetTimeUntilNextRequest(string clientId)
        {
            if (lastRequestTimes.TryGetValue(clientId, out DateTime lastRequestTime))
            {
                var timeSinceLastRequest = DateTime.Now - lastRequestTime;
                return TimeSpan.FromSeconds(_rateLimitDurationSeconds) - timeSinceLastRequest;
            }

            return TimeSpan.Zero;
        }

        private void EvictOldEntries(object state)
        {
            DateTime currentTime = DateTime.Now;

            foreach (var entry in lastRequestTimes)
            {
                if ((currentTime - entry.Value).TotalSeconds > _rateLimitDurationSeconds + 15)
                {
                    lastRequestTimes.TryRemove(entry.Key, out _);
                    rateLimits.TryRemove(entry.Key, out _);
                }
            }
        }

        private int GetMaxRequestFromConfig()
        {
            var config = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Configs\\config.json")));
            return int.Parse(config["Server"]["MaxRequests"]);
        }

        private int GetRateLimitDurationFromConfig()
        {
            var config = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Configs\\config.json")));
            return int.Parse(config["Server"]["RateLimitDurationInSeconds"]);
        }
    }
}
