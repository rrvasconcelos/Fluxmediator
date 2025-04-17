using FluxMediator.Core.Abstractions;
using FluxMediator.Core.Extensions;
using SampleWebApi;
using SampleWebApi.Application.Products.UserCases.Create;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMyMediator(typeof(Program).Assembly);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");


app.MapPost("/products", async (RequestProduct requestProduct, IMediator mediator, CancellationToken token) =>
{
    var command = new CreateProductCommand(requestProduct.Name, requestProduct.Price);

    var result = await mediator.Send(command, token);

    return Results.Ok(result);
});

app.Run();


public sealed record RequestProduct(string Name, decimal Price);