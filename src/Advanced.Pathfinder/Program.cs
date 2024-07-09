using Advanced.Pathfinder;
using Advanced.Pathfinder.Actors;
using Advanced.Pathfinder.Core;
using Advanced.Pathfinder.CostManagement;
using Advanced.Pathfinder.Edge;
using Advanced.Pathfinder.Node;
using Akka.Actor;
using Akka.Cluster.Hosting;
using Akka.HealthCheck.Hosting;
using Akka.HealthCheck.Hosting.Web;
using Akka.Hosting;
using Akka.Logger.Serilog;
using Akka.Persistence.Hosting;
using Akka.Persistence.MongoDb.Hosting;
using Akka.Remote.Hosting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using stuff.graph.net;

Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Debug()
            .MinimumLevel.Information()
            .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(Log.Logger);
RegisterMongoShit();

builder.Services.AddControllers();
builder.Services.AddRouting();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "Advanced.Pathfinder", Version = "v1" }));
builder.Services.AddHealthChecks();

builder.Services.WithAkkaHealthCheck(HealthCheckType.All)
.AddSingleton<IMongoClient>(x => new MongoClient("connection string"))
.AddSingleton(x => x.GetRequiredService<IMongoClient>().GetDatabase("Advanced.Pathfinder"))
.AddScoped(x => x.GetRequiredService<IMongoDatabase>().GetCollection<Edge>("edges"))
.AddScoped(x => x.GetRequiredService<IMongoDatabase>().GetCollection<Node>("nodes"))
.AddAkka("Zeus", (builder, sp) =>
    {
        var connectionString = "";
        var shardingJournalOptions = new MongoDbJournalOptions(true)
        {
            ConnectionString = connectionString,
            Collection = "EventJournal",
            MetadataCollection = "MetaEventJournal",
            CallTimeout = TimeSpan.FromSeconds(15),
            AutoInitialize = true,
        };
        var shardingSnapshotOptions = new MongoDbSnapshotOptions(true)
        {
            ConnectionString = connectionString,
            Collection = "SnapshotStore",
            CallTimeout = TimeSpan.FromSeconds(15),
            AutoInitialize = true,
        };

        builder.ConfigureLoggers(setup =>
            {
                // Clear all loggers
                setup.ClearLoggers();
                // Add serilog logger
                setup.AddLogger<SerilogLogger>();
                setup.WithDefaultLogMessageFormatter<SerilogLogMessageFormatter>();
            })
            .AddHocon(hocon: "akka.remote.dot-netty.tcp.maximum-frame-size = 256000b", addMode: HoconAddMode.Prepend)
            .WithHealthCheck(x => x.AddProviders(HealthCheckType.All))
            .WithWebHealthCheck(sp)
            .WithRemoting("0.0.0.0", 0, "127.0.0.1")
            .WithClustering(new ClusterOptions
            {
                Roles = [AkkaRole.Role],
                SeedNodes = ["akka.tcp://Zeus@127.0.0.1:42000"]
            })
            .WithMongoDbPersistence(connectionString!, PersistenceMode.Both)
            .WithJournalAndSnapshot(shardingJournalOptions, shardingSnapshotOptions)
            .WithShardRegion<NodeWorker>("NodeWorker", (_, _, dependecyResolver) => x => dependecyResolver.Props<NodeWorker>(x), new MessageExtractor(), new ShardOptions()
            {
                JournalOptions = shardingJournalOptions,
                SnapshotOptions = shardingSnapshotOptions,
                Role = AkkaRole.Role,
                ShouldPassivateIdleEntities = true,
                PassivateIdleEntityAfter = TimeSpan.FromSeconds(30)
            })
            .WithShardRegion<EdgeWorker>("EdgeWorker", (_, _, dependecyResolver) => x => dependecyResolver.Props<EdgeWorker>(x), new MessageExtractor(), new ShardOptions()
            {
                JournalOptions = shardingJournalOptions,
                SnapshotOptions = shardingSnapshotOptions,
                Role = AkkaRole.Role,
                ShouldPassivateIdleEntities = true,
                PassivateIdleEntityAfter = TimeSpan.FromSeconds(30)
            })
            .WithShardRegion<RequestWorker>("RequestWorker", (_, _, dependecyResolver) => x => dependecyResolver.Props<RequestWorker>(x), new MessageExtractor(), new ShardOptions()
            {
                JournalOptions = shardingJournalOptions,
                SnapshotOptions = shardingSnapshotOptions,
                Role = AkkaRole.Role,
                ShouldPassivateIdleEntities = true,
                PassivateIdleEntityAfter = TimeSpan.FromSeconds(30)
            })
            .WithShardRegionProxy<NodeWorkerProxy>("NodeWorker", AkkaRole.Role, new MessageExtractor())
            .WithShardRegionProxy<EdgeWorkerProxy>("EdgeWorker", AkkaRole.Role, new MessageExtractor())
            .WithShardRegionProxy<RequestWorkerProxy>("RequestWorker", AkkaRole.Role, new MessageExtractor())
            .WithSingleton<CostManager>("CostManager", (_, _, dependecyResolver) => dependecyResolver.Props<CostManager>())
            .WithSingletonProxy<CostManagerProxy>("CostManager", new ClusterSingletonOptions() { BufferSize = 1000, Role = AkkaRole.Role, TerminationMessage = PoisonPill.Instance });
    });
var host = builder.Build();
host.UseSerilogRequestLogging();
host.UseSwagger();
host.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Advanced.Pathfinder"));
host.UseRouting();
host.UseAuthorization();
host.MapControllers();
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
        var nodes = provider.GetRequiredService<IMongoCollection<Node>>();
        var edges = provider.GetRequiredService<IMongoCollection<Edge>>();

        await nodes.CreateIndexAsync(builder => builder.Ascending(item => item.Id), "node_ids");
        await edges.CreateIndexAsync(builder => builder.Ascending(item => item.Id).Ascending(item => item.EndNode.Id).Ascending(item => item.StartNode.Id), "edge_ids");
    }
}