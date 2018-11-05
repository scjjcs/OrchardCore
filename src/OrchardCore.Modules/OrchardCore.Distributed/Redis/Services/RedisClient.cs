using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrchardCore.Distributed.Redis.Options;
using OrchardCore.Environment.Shell;
using StackExchange.Redis;

namespace OrchardCore.Distributed.Redis.Services
{
    public class RedisClient : IRedisClient, IDisposable
    {
        private readonly string _tenant;
        private readonly IOptions<RedisOptions> _options;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1);
        private bool _initialized;

        public RedisClient(ShellSettings shellSettings, IOptions<RedisOptions> options, ILogger<RedisClient> logger)
        {
            _tenant = shellSettings.Name;
            _options = options;
            Logger = logger;
        }

        public ILogger Logger { get; set; }

        public bool IsConnected => Connection?.IsConnected ?? false;
        public IConnectionMultiplexer Connection { get; private set; }
        public IDatabase Database { get; private set; }

        public async Task ConnectAsync()
        {
            if (_initialized)
            {
                return;
            }

            await _connectionLock.WaitAsync();

            try
            {
                if (!_initialized)
                {
                    Connection = await ConnectionMultiplexer.ConnectAsync(_options.Value.ConfigurationOptions);
                    Database = Connection.GetDatabase();
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "'{TenantName}' is unable to connect to Redis.", _tenant);
            }
            finally
            {
                _initialized = true;
                _connectionLock.Release();
            }
        }

        public void Dispose()
        {
            if (Connection != null)
            {
                Connection.Close();
            }
        }
    }
}