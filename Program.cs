using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using SeaSlugAPI.Context;
using SeaSlugAPI.Services;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

configuration.AddJsonFile("appsettings.json").AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetSection("Secrets")["DBConnectionString"]));

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", builder =>
    {
        builder.SetIsOriginAllowed(x => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueCountLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = long.MaxValue;
    options.MemoryBufferThreshold = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

builder.Services.AddScoped<IAzureService, AzureService>();
builder.Services.AddScoped<ISeaSlugService, SeaSlugService>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //db.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("cors");

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers().RequireCors("cors");
});

app.Run();
