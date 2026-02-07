namespace Posts.Infrastructure.Data;

public class ScyllaDbSettings
{
    public List<string> ContactPoints { get; set; } = [];
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string Keyspace { get; set; } = string.Empty;
}
