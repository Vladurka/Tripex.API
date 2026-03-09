using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace Profiles.Infrastructure.Data;

public class ProfilesContextFactory : IDesignTimeDbContextFactory<ProfilesContext>
{
    public ProfilesContext CreateDbContext(string[] args)
    {
        var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");

        if (!File.Exists(envPath))
            envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Profiles.API", ".env");

        var connectionString = ParseConnectionString(envPath);

        var conn = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Host = "localhost",
            Port = 5434,
            TrustServerCertificate = true
        };

        var options = new DbContextOptionsBuilder<ProfilesContext>()
            .UseNpgsql(conn.ConnectionString)
            .Options;

        return new ProfilesContext(options);
    }

    private static string ParseConnectionString(string envPath)
    {
        const string key = "ConnectionStrings__PostgresConnection=";

        foreach (var line in File.ReadLines(envPath))
        {
            if (line.StartsWith(key))
                return line[key.Length..];
        }

        throw new InvalidOperationException($"ConnectionStrings__PostgresConnection not found in {envPath}");
    }
}
