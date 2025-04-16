using System.Threading;
using System.Threading.Tasks;

namespace FluxMediator.Core.Abstractions
{
    /// <summary>
    /// Mediator that routes requests to appropriate handlers
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Sends a command to the corresponding handler
        /// </summary>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="request">Command to be processed</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Handler response</returns>
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a query to the corresponding handler
        /// </summary>
        /// <typeparam name="TResponse">Response type</typeparam>
        /// <param name="query">Query to be processed</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Handler response</returns>
        Task<TResponse> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes a notification to all interested handlers
        /// </summary>
        /// <typeparam name="TNotification">Notification type</typeparam>
        /// <param name="notification">Notification to be published</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
           where TNotification : INotification;
    }
}

