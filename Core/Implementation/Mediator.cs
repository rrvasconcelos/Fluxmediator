using System;
using System.Collections.Generic;
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

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Sends a command to the corresponding handler
        /// </summary>
        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var requestType = request.GetType();
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

            var handler = _serviceProvider.GetService(handlerType) ?? throw new InvalidOperationException($"Handler not found for {requestType.Name}");
            return await InvokeHandler<TResponse>(request, handler, cancellationToken);
        }

        /// <summary>
        /// Sends a query to the corresponding handler
        /// </summary>
        public async Task<TResponse> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var queryType = query.GetType();

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse));
            var handler = _serviceProvider.GetService(handlerType) ?? throw new InvalidOperationException($"Handler not found for {queryType.Name}");

            return await InvokeHandler<TResponse>(query, handler, cancellationToken);
        }

        /// <summary>
        /// Publishes a notification to all interested handlers
        /// </summary>
        public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));

            var notificationType = notification.GetType();
            var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);
            var handlers = _serviceProvider.GetServices(handlerType);

            var tasks = new List<Task>();
            foreach (var handler in handlers)
            {
                tasks.Add(InvokeNotificationHandler(notification, handler, cancellationToken));
            }

            await Task.WhenAll(tasks);
        }

        private async Task<TResponse> InvokeHandler<TResponse>(object request, object handler, CancellationToken cancellationToken)
        {
            var method = handler.GetType().GetMethod("Handle");
            return await (Task<TResponse>)method.Invoke(handler, new[] { request, cancellationToken });
        }

        private async Task InvokeNotificationHandler<TNotification>(TNotification notification, object handler, CancellationToken cancellationToken)
        {
            var method = handler.GetType().GetMethod("Handle");
            await (Task)method.Invoke(handler, new object[] { notification, cancellationToken });
        }
    }
}


