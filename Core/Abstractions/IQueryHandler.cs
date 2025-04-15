using System.Threading;
using System.Threading.Tasks;

namespace FluxMediator.Core.Abstractions
{
    /// <summary>
    /// Defines a handler for a query
    /// </summary>
    /// <typeparam name="TQuery">Query type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    public interface IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        /// <summary>
        /// Handles a query and returns a response
        /// </summary>
        Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
    }
}

