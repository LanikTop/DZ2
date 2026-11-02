using Infrastructure.DAO;
using Presentation.Interceptors;
using Presentation.Middleware;
using Application.Services;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddGrpc(options => options.Interceptors.Add<LoggingInterceptor>())
    .AddJsonTranscoding();

builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddSingleton<IProductDAO, ProductDAO>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);
});
builder.Services.AddGrpcSwagger();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapGrpcService<ProductGrpcService>();
app.Run();