using CleanArchitecture.API.Middleware;
using CleanArchitecture.Application;
using CleanArchitecture.Infrastructure;
using Swashbuckle.AspNetCore.Annotations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.TagActionsBy(api =>
    {
        var swaggerOperation = api.ActionDescriptor.EndpointMetadata
            .OfType<SwaggerOperationAttribute>()
            .FirstOrDefault();

        if (swaggerOperation?.Tags is { Length: > 0 })
        {
            return swaggerOperation.Tags;
        }

        return new[]
        {
            api.GroupName
                ?? api.ActionDescriptor.RouteValues["controller"]
                ?? "Endpoints"
        };
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
