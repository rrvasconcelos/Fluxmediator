using System.Threading;
using System.Threading.Tasks;

namespace FluxMediator.Core.Abstractions
{
    /// <summary>
    /// Defines a handler for a command with response
    /// </summary>
    /// <typeparam name="TRequest">Command type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    public interface IRequestHandler<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Handles a command and returns a response
        /// </summary>
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}

