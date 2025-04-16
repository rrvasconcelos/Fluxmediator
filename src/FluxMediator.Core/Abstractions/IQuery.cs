namespace FluxMediator.Core.Abstractions
{
    /// <summary>
    /// Marks a class as a query for the mediator that returns a value of type TResponse
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    public interface IQuery<TResponse> { }
}

