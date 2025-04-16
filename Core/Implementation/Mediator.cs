using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluxMediator.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace FluxMediator.Core.Implementation
{
    /// <summary>
    /// Default implementation of the mediator
    /// </summary>
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Creates a new instance of the mediator
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the service provider is null</exception>
        /// <remarks>
        /// The service provider is used to resolve the handlers for commands, queries, and notifications.
        /// </remarks>
        /// <example>
        /// var mediator = new Mediator(serviceProvider);
        /// </example>
        /// <param name="serviceProvider">
        ///     The service provider to resolve handlers
        /// </param>
        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Sends a command to the corresponding handler
        /// </summary>
        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var requestType = request.GetType();
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

            var handler = _serviceProvider.GetService(handlerType) ??
                          throw new InvalidOperationException($"Handler not found for {requestType.Name}");
            return await InvokeHandler<TResponse>(request, handler, cancellationToken);
        }

        /// <summary>
        /// Sends a query to the corresponding handler
        /// </summary>
        public async Task<TResponse> Query<TResponse>(IQuery<TResponse> query,
            CancellationToken cancellationToken = default)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var queryType = query.GetType();

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse));
            var handler = _serviceProvider.GetService(handlerType) ??
                          throw new InvalidOperationException($"Handler not found for {queryType.Name}");

            return await InvokeHandler<TResponse>(query, handler, cancellationToken);
        }

        /// <summary>
        /// Publishes a notification to all interested handlers
        /// </summary>
        public async Task Publish<TNotification>(TNotification notification,
            CancellationToken cancellationToken = default) where TNotification : INotification
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));

            var notificationType = notification.GetType();
            var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);
            var handlers = _serviceProvider.GetServices(handlerType).ToList();
            
            if (handlers == null || !handlers.Any())
                throw new InvalidOperationException($"No handlers found for {notificationType.Name}");

            var tasks = handlers
                .Select(handler => InvokeNotificationHandler(notification, handler, cancellationToken))
                .ToList();

            await Task.WhenAll(tasks);
        }

        private static async Task<TResponse> InvokeHandler<TResponse>(object request, object handler,
            CancellationToken cancellationToken)
        {
            var method = handler.GetType().GetMethod("Handle");

            if (method == null)
                throw new InvalidOperationException($"Handler method not found for {handler.GetType().Name}");

            return await (Task<TResponse>)method.Invoke(handler, new[] { request, cancellationToken });
        }

        private async Task InvokeNotificationHandler<TNotification>(TNotification notification, object? handler,
            CancellationToken cancellationToken)
        {
            var method = handler?.GetType().GetMethod("Handle");

            if (method == null)
                throw new InvalidOperationException($"Handler method not found for {handler?.GetType().Name}");

            if (notification == null)
                throw new ArgumentNullException(nameof(notification));

            await (Task)method.Invoke(handler, new object[] { notification, cancellationToken });
        }
    }
}