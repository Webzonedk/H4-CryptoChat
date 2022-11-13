using Microsoft.AspNetCore.ResponseCompression;
using API.Hubs;

var builder = WebApplication.CreateBuilder(args);


/// <summary>
/// Adding Cors, to be able to share data over multiple IP's and instances
/// Cors is added as a nugget
/// </summary>
var _corsPolicy = "CorsPolicy";
//Adding Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: _corsPolicy,
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            //builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200", "http://localhost:80");
        });
});

/// <summary>
/// Adding signalR to be able to send live data to the website
/// </summary>
builder.Services.AddSignalR();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Added to compress messages, to minimize size
builder.Services.AddResponseCompression(options =>
{
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/// <summary>
/// Needed for signalR to be able to send handshakes
/// </summary>
app.UseRouting();

/// <summary>
/// Applying the Cors
/// </summary>
app.UseCors(_corsPolicy);



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

/// <summary>
/// Initialising the SignalR server
/// </summary>
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chathub");
});


app.Run();
