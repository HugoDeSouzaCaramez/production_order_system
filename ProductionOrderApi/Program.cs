using Microsoft.EntityFrameworkCore;
using ProductionOrderApi.Data;
using ProductionOrderApi.Repositories;
using ProductionOrderApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:4173")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductionOrderRepository, ProductionOrderRepository>();
builder.Services.AddScoped<IProductionOrderService, ProductionOrderService>();
builder.Services.AddScoped<IProductionLogRepository, ProductionLogRepository>();
builder.Services.AddScoped<IProductionOrderRepository, ProductionOrderRepository>();
builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
builder.Services.AddScoped<IProductionLogService, ProductionLogService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<DatabaseSeeder>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.UnmappedMemberHandling = 
            System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow;
    });


var app = builder.Build();

if (args.Contains("--seed"))
{
    using var scope = app.Services.CreateScope();
    await DatabaseSeeder.RunSeed(scope.ServiceProvider);
    return;
}

app.UseCors("AllowFrontend");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var seeder = services.GetRequiredService<DatabaseSeeder>();
        
        await context.Database.EnsureCreatedAsync();
        
        await seeder.SeedDatabase();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao inicializar banco: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
