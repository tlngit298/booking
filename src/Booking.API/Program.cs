using Booking.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddApplicationServices();
builder.AddPersistenceServices();

builder.Services.AddControllers();

// Add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        options =>
        {
            options.Authority = "http://localhost:8080/realms/booking";
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "http://localhost:8080/realms/booking",
                ValidateAudience = true,
                ValidAudience = "net-api"
            };
        }
    );

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // API Information
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Booking API",
        Version = "v1",
        Description = "RESTful API for the Booking platform - a service booking system built with Clean Architecture and DDD principles",
        Contact = new OpenApiContact
        {
            Name = "Booking API Support",
            Email = "lenguyentuan3298@gmail.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Include XML documentation
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlPath);

    // Customize schema IDs to use full type names (avoids conflicts)
    options.CustomSchemaIds(type => type.FullName);

    // Support for polymorphism and inheritance
    options.UseAllOfForInheritance();
    options.UseOneOfForPolymorphism();
});

// Register global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable Swagger JSON endpoint
    app.UseSwagger();

    // Enable Swagger UI
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking API v1");
        options.RoutePrefix = "swagger"; // Access at /swagger

        // UI Enhancements
        options.DocumentTitle = "Booking API - Swagger UI";
        options.DisplayOperationId();
        options.DisplayRequestDuration();
        options.EnableDeepLinking();
        options.EnableFilter();
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        options.DefaultModelsExpandDepth(1);
    });
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
