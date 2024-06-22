using Akka.Pathfinder.Managers;
using Akka.Pathfinder;
using Advanced.Pathfinder.Workers;
using MongoDB.Driver;
using Akka.HealthCheck.Hosting.Web;
using Serilog;
using Akka.HealthCheck.Hosting;
using Advanced.Pathfinder.Core;
using Path = Advanced.Pathfinder.Core.Persistence.Data.Path;
using Advanced.Pathfinder.Core.Configs;
using Advanced.Pathfinder.Core.Services;
using Akka.Hosting;
using Akka.Persistence.Sql.Config;
using Akka.Logger.Serilog;
using Akka.Remote.Hosting;
using Akka.Cluster.Hosting;
using Akka.Persistence.Sql.Hosting;
using Akka.Persistence.Hosting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Advanced.Pathfinder.Managers;

Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Debug()
            .MinimumLevel.Information()
            .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(Log.Logger);
RegisterMongoShit();

builder.Services.AddHealthChecks();

builder.Services.WithAkkaHealthCheck(HealthCheckType.All)
.AddSingleton<IMongoClient>(x => new MongoClient(AkkaPathfinder.GetEnvironmentVariable("mongodb")))
.AddScoped(x => x.GetRequiredService<IMongoClient>().GetDatabase("pathfinder"))
.AddScoped(x => x.GetRequiredService<IMongoDatabase>().GetCollection<Path>("path"))
.AddScoped(x => x.GetRequiredService<IMongoDatabase>().GetCollection<MapConfig>("map_config"))
.AddScoped<IPathWriter, PathWriter>()
.AddScoped<IPathReader>(x => x.GetRequiredService<IPathWriter>())
.AddScoped<IMapConfigWriter, MapConfigWriter>(x => new MapConfigWriter(x.GetRequiredService<IMongoCollection<MapConfig>>()))
.AddScoped<IMapConfigReader>(x => x.GetRequiredService<IMapConfigWriter>())
.AddScoped<IPointConfigReader, PointConfigReader>()
.AddAkka("Zeus", (builder, sp) =>
    {
        var connectionString = AkkaPathfinder.GetEnvironmentVariable("postgre");
        var shardingJournalOptions = new Akka.Persistence.Sql.Hosting.SqlJournalOptions(true)
        {
            ConnectionString = connectionString,
            ProviderName = LinqToDB.ProviderName.PostgreSQL15,
            TagStorageMode = TagMode.TagTable,
            AutoInitialize = true,
        };

        var shardingSnapshotOptions = new Akka.Persistence.Sql.Hosting.SqlSnapshotOptions(true)
        {
            ConnectionString = connectionString,
            ProviderName = LinqToDB.ProviderName.PostgreSQL15,
            AutoInitialize = true,
        };

        builder.ConfigureLoggers(setup =>
            {
                // Clear all loggers
                setup.ClearLoggers();

                // Add serilog logger
                setup.AddLogger<SerilogLogger>();
                setup.LogMessageFormatter = typeof(SerilogLogMessageFormatter);
            })
            .AddHocon(hocon: "akka.remote.dot-netty.tcp.maximum-frame-size = 256000b", addMode: HoconAddMode.Prepend)
            .WithHealthCheck(x => x.AddProviders(HealthCheckType.All))
            .WithWebHealthCheck(sp)
            .WithRemoting("0.0.0.0", 1337, "127.0.0.1")
            .WithClustering(new ClusterOptions
            {
                Roles = ["KEKW"],
                SeedNodes = ["akka.tcp://Zeus@127.0.0.1:42000"]
            })
            .WithSqlPersistence(connectionString!, LinqToDB.ProviderName.PostgreSQL15, PersistenceMode.Both, autoInitialize: true, tagStorageMode: TagMode.Both)
            .WithJournalAndSnapshot(shardingJournalOptions, shardingSnapshotOptions)
            .WithShardRegion<PointWorker>("PointWorker", (_, _, dependecyResolver) => x => dependecyResolver.Props<PointWorker>(x), new MessageExtractor(128), new ShardOptions()
            {
                JournalOptions = shardingJournalOptions,
                SnapshotOptions = shardingSnapshotOptions,
                Role = "KEKW",
                ShouldPassivateIdleEntities = true,
                PassivateIdleEntityAfter = TimeSpan.FromSeconds(30),
            })
            .WithShardRegionProxy<PointWorkerProxy>("PointWorker", "KEKW", new MessageExtractor(128))
            .WithShardRegion<PathfinderWorker>("PathfinderWorker", (_, _, dependecyResolver) => x => dependecyResolver.Props<PathfinderWorker>(x), new MessageExtractor(), new ShardOptions()
            {
                JournalOptions = shardingJournalOptions,
                SnapshotOptions = shardingSnapshotOptions,
                Role = "KEKW",
                ShouldPassivateIdleEntities = false,
                //PassivateIdleEntityAfter = TimeSpan.FromSeconds(30)
            })
            .WithShardRegionProxy<PathfinderProxy>("PathfinderWorker", "KEKW", new MessageExtractor())
            .WithSingleton<MapManager>("MapManager", (_, _, dependecyResolver) => dependecyResolver.Props<MapManager>(), new ClusterSingletonOptions() { Role = "KEKW" }, false)
            .WithSingletonProxy<MapManagerProxy>("MapManager", new ClusterSingletonOptions() { Role = "KEKW" })
            .WithSingleton<SenderManager>("SenderManager", (_, _, dependecyResolver) => dependecyResolver.Props<SenderManager>(), new ClusterSingletonOptions() { Role = "KEKW" }, false)
            .WithSingletonProxy<SenderManagerProxy>("SenderManager", new ClusterSingletonOptions() { Role = "KEKW" });
    });

var host = builder.Build();
host.UseSerilogRequestLogging();
await CreateIndexes(host.Services);
host.UseHealthChecks("/health/ready", new HealthCheckOptions() { AllowCachingResponses = false, Predicate = _ => true });
await host.RunAsync().ConfigureAwait(false);

public partial class Program
{
    private static int _registered;
    protected Program()
    {
    }

    private static void RegisterMongoShit()
    {
        if (Interlocked.Increment(ref _registered) == 1)
            BsonShit.Register();
    }

    private static async Task CreateIndexes(IServiceProvider provider)
    {
        var mapConfigs = provider.GetRequiredService<IMongoCollection<MapConfig>>();
        var paths = provider.GetRequiredService<IMongoCollection<Path>>();

        await mapConfigs.CreateIndexAsync(builder => builder.Ascending(item => item.Id), "MapConfig_Id");
        await paths.CreateIndexAsync(builder => builder.Ascending(item => item.Id).Ascending(item => item.PathfinderId), "Path_Id");
    }
}