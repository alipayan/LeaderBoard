using LeaderBoard.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.BrokerConfig();
builder.ApplicationDbContextConfig();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/{topic}", (string Topic, int count, LeaderBoardDbContext dbContext) =>
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

app.MapPost("/game", async (string player, int score, IPublishEndpoint endpoint) =>
{
	var topic = "game";

	await endpoint.Publish(new PlayerScoreChangedEvent(player, score));

});

app.MapPost("/ordering", async (string catalog_id, IPublishEndpoint endpoint) =>
{
	var topic = "order";
	await endpoint.Publish(new SoldProductEvent(catalog_id));

});

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
}