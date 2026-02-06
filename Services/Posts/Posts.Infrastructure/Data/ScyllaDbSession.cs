using Cassandra;
using Microsoft.Extensions.Options;

namespace Posts.Infrastructure.Data
{
    public static class ScyllaDbSession
    {
        private static ISession? _session;

        public static ISession Connect(IOptions<ScyllaDbSettings> options)
        {
            if (_session != null) return _session;

            var settings = options.Value;

            var clusterBuilder = Cluster.Builder()
                .AddContactPoints(settings.ContactPoints);

            if (!string.IsNullOrEmpty(settings.Username))
                clusterBuilder = clusterBuilder.WithCredentials(settings.Username, settings.Password);

            var cluster = clusterBuilder.Build();
            _session = cluster.Connect(settings.Keyspace);

            return _session;
        }

    }
}