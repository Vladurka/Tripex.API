using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace Auth.API.Data;

public class AuthContextFactory : IDesignTimeDbContextFactory<AuthContext>
{
    public AuthContext CreateDbContext(string[] args)
    {
        var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
        var connectionString = ParseConnectionString(envPath);

        var conn = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Host = "localhost",
            Port = 5433,
            TrustServerCertificate = true
        };

        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseNpgsql(conn.ConnectionString)
            .Options;

        return new AuthContext(options);
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
