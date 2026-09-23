using System.Text.Json.Serialization;
using EmployeeManager.Application;
using EmployeeManager.Infrastructure;
using EmployeeManager.Web.Api.Employees;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapEmployeeEndpoints();

app.Run();
