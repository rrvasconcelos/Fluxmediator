using System.Threading;
using System.Threading.Tasks;

namespace FluxMediator.Core.Abstractions
{
    /// <summary>
    /// Defines a handler for a notification
    /// </summary>
    /// <typeparam name="TNotification">Notification type</typeparam>
    public interface INotificationHandler<TNotification>
        where TNotification : INotification
    {
        /// <summary>
        /// Handles a notification
        /// </summary>
        Task Handle(TNotification notification, CancellationToken cancellationToken);
    }
}

