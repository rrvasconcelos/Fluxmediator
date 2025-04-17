using FluxMediator.Core.Abstractions;

namespace SampleWebApi.Application.Products.UserCases.Create;

public record CreateProductCommand(string Name, decimal Price) : IRequest<CreateProductResponse>
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = Name;
    public decimal Price { get; init; } = Price;
}

public record CreateProductResponse(string Id, string Name, decimal Price)
{
    public string Id { get; init; } = Id;
    public string Name { get; init; } = Name;
    public decimal Price { get; init; } = Price;
}
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    public Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Creating product with name: {request.Name} and price: {request.Price}");
        var productId = Guid.NewGuid().ToString();
        var response = new CreateProductResponse(productId, request.Name, request.Price);

        return Task.FromResult(response);
    }
}
