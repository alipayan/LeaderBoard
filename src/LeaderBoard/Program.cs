using LeaderBoard.Models;

var builder = WebApplication.CreateBuilder(args);

builder.BrokerConfig();
builder.ApplicationDbContextConfig();
builder.RedisConfig();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<SortedInMemoryDatabase>();
builder.Services.AddScoped<ScoreService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//to get from in memory data base 
app.MapGet("/{topic}/in-memory", (string Topic, int count, LeaderBoardDbContext dbContext) =>
{
	if (Topic == "order")
	{
		var items = dbContext.MostSoldProducts.OrderByDescending(d => d.Score)
											  .Take(count);
		return Results.Ok(items);
	}
	else if (Topic == "game")
	{
		var items = dbContext.PlayerScores.OrderByDescending(d => d.Score)
											  .Take(count);
		return Results.Ok(items);
	}
	throw new InvalidOperationException();
});

//to get from sortedset in memory data base 
app.MapGet("/{topic}/sorted-set", (string Topic, int count, SortedInMemoryDatabase sortedDatabase) =>
{
	if (Topic == "order")
	{
		return Results.Ok(sortedDatabase.MostSoldProducts);
	}
	else if (Topic == "game")
	{
		return Results.Ok(sortedDatabase.PlayerScores);
	}
	throw new InvalidOperationException();
});

//to get from redis
app.MapGet("/{topic}", async (int K, string topic, ScoreService scoreService) =>
{
	var items = await scoreService.GetTopAsync<MostSoldProduct>(topic, K);
	return Results.Ok(items);
});

//to get from redis
app.MapGet("/{topic}/game", async (int K, string topic, ScoreService scoreService) =>
{
	var items = await scoreService.GetTopAsync<PlayerScore>(topic, K);
	return Results.Ok(items);
});

app.MapPost("/game", async (string player, int score, IPublishEndpoint endpoint) =>
	await endpoint.Publish(new PlayerScoreChangedEvent(player, score))
);

app.MapPost("/ordering", async (string catalog_id, IPublishEndpoint endpoint) =>
	await endpoint.Publish(new SoldProductEvent(catalog_id))
);

app.Run();

public static class WebApplicationExtensions
{
	public static void BrokerConfig(this IHostApplicationBuilder builder)
	{
		builder.Services.AddMassTransit(configure =>
		{
			var brokerConfig = builder.Configuration.GetSection(BrokerOptions.SectionName)
													.Get<BrokerOptions>();
			if (brokerConfig is null)
			{
				throw new ArgumentNullException(nameof(BrokerOptions));
			}

			configure.AddConsumers(Assembly.GetExecutingAssembly());
			configure.UsingRabbitMq((context, cfg) =>
			{
				cfg.Host(brokerConfig.Host, hostConfigure =>
				{
					hostConfigure.Username(brokerConfig.Username);
					hostConfigure.Password(brokerConfig.Password);
				});

				cfg.ConfigureEndpoints(context);
			});
		});
	}

	public static void ApplicationDbContextConfig(this WebApplicationBuilder builder)
	{
		builder.Services.AddDbContext<LeaderBoardDbContext>(configure =>
		{
			configure.UseInMemoryDatabase(nameof(LeaderBoardDbContext));
		});
	}

	public static void RedisConfig(this WebApplicationBuilder builder)
	{
		builder.Services.AddSingleton<IConnectionMultiplexer>(PathString =>
		{
			var connectionString = builder.Configuration.GetConnectionString("RedisConnections");
			if (connectionString is null)
				throw new ArgumentNullException(nameof(connectionString));
			return ConnectionMultiplexer.Connect(connectionString);
		});
	}
}