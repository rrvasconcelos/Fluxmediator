namespace FluxMediator.Core.Abstractions
{
    /// <summary>
    /// Marks a class as a command for the mediator that returns a value of type TResponse
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    public interface IRequest<TResponse> { }
}

