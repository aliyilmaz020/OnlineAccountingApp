using OnlineAccountingApp.Grpc.DependencyInjections;
using OnlineAccountingApp.Grpc.Interceptors;
using OnlineAccountingApp.Grpc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(options => options.Interceptors.Add<BusinessExceptionInterceptor>());
builder.Services.AddGrpcReflection();

// Must precede AddGrpcAuthentication(): AddIdentity (inside AddGrpcPersistence) registers cookie
// handlers as the default scheme, and the later AddAuthentication() registration is the one that
// wins - same ordering constraint as WebApi's Program.cs.
builder.Services.AddGrpcPersistence(builder.Configuration);
builder.Services.AddGrpcAuthentication(builder.Configuration);
builder.Services.AddGrpcInfrastructure(builder.Configuration);
builder.Services.AddGrpcApplication();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<UniformChartOfAccountsGrpcService>();
app.MapGrpcService<AuthGrpcService>();
app.MapGrpcService<CompaniesGrpcService>();
app.MapGrpcService<RolesGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
