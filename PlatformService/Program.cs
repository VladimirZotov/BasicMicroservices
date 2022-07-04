using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

if (builder.Environment.IsProduction())
{
    Console.WriteLine("--> Using MsSQL Db");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}
else
{
    Console.WriteLine("--> Using InMem Db");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
}

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddGrpc();

var app = builder.Build();

app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints => { 
    endpoints.MapControllers();
    endpoints.MapGrpcService<GrpcPlatformService>();
    endpoints.MapGet("/protos/platforms.proto", async context => {
        await context.Response.WriteAsync(System.IO.File.ReadAllText("Protos/platforms.proto"));
    });
});

PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

app.Run();
