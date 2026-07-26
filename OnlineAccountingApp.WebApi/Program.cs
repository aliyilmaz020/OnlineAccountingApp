using OnlineAccountingApp.WebApi.Configurations;
using OnlineAccountingApp.WebApi.DependencyInjections.Application;
using OnlineAccountingApp.WebApi.DependencyInjections.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Update endpoints take the id from the route and assign it after model binding, so the
// implicit "required" that MVC infers for non-nullable properties would reject valid bodies.
builder.Services.AddControllers(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

builder.Services.ConfigureApi();
builder.Services.AddPersistence(builder.Configuration); 
builder.Services.AddApplication();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
