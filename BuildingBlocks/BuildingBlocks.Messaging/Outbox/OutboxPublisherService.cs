using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Messaging.Outbox;

public class OutboxPublisherService<T>(IServiceScopeFactory scopeFactory) : BackgroundService where T : DbContext, IOutboxContext
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<T>();
            
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            var messages = await db.OutboxMessages
                .Where(x => !x.IsPublished)
                .OrderBy(x => x.CreatedAt)
                .Take(20)
                .ToListAsync(cancellationToken);

            foreach (var message in messages)
            {
                var eventType = Type.GetType(message.Type);
                if (eventType == null) continue;

                var evt = JsonSerializer.Deserialize(message.Payload, eventType);
                if (evt == null) continue;

                await publishEndpoint.Publish(evt, cancellationToken);

                message.IsPublished = true;
                message.PublishedAt = DateTime.UtcNow;
            }

            var threshold = DateTime.UtcNow.AddDays(-7);
            var stale = await db.OutboxMessages
                .Where(x => x.IsPublished && x.PublishedAt < threshold)
                .Take(100)
                .ToListAsync(cancellationToken);

            if (stale.Count > 0)
                db.OutboxMessages.RemoveRange(stale);

            await db.SaveChangesAsync(cancellationToken);
            await Task.Delay(30000, cancellationToken);
        }
    }
}