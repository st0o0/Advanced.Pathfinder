﻿using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Advanced.Pathfinder.Core;

public static class IMongoQueryableExtensions
{
    public static async Task ThrottleAsync<T>(this IMongoQueryable<T> values, Action<T> action, TimeSpan? initialDelay = null, TimeSpan? interval = null)
    {
        initialDelay ??= TimeSpan.Zero;
        interval ??= TimeSpan.FromMicroseconds(5);

        await Task.Delay(initialDelay.Value);
        var results = values.ToList();
        foreach (var item in results)
        {
            await Task.Delay(interval.Value);
            action.Invoke(item);
        }
    }

    public static async Task CreateIndexAsync<T>(this IMongoCollection<T> collection, Func<IndexKeysDefinitionBuilder<T>, IndexKeysDefinition<T>> selector, string indexName, CancellationToken cancellationToken = default)
    {
        var indexKeysDefinition = selector.Invoke(Builders<T>.IndexKeys);

        await collection.Indexes.CreateOneAsync(new CreateIndexModel<T>(indexKeysDefinition, new CreateIndexOptions() { Name = indexName, }), cancellationToken: cancellationToken);
    }
}
