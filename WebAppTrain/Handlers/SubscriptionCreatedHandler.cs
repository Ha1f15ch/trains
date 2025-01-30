using BusinesEngine.Events;
using MediatR;

namespace WebAppTrain.Handlers
{
    public class SubscriptionCreatedHandler : INotificationHandler<SubscriptionCreatedEvent>
    {
        private readonly ILogger<SubscriptionCreatedHandler> _logger;

        public SubscriptionCreatedHandler(ILogger<SubscriptionCreatedHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(SubscriptionCreatedEvent notification, CancellationToken cancellationToken)
        {
            // Имитация того, что уведомление отправлено на клиент
            await Task.Delay(5000, cancellationToken);

            _logger.LogInformation("Уведомление отправлено пользователю {UserId} о подписке на книгу {BookId}.", notification.Subscription.UserId, notification.Subscription.BookId);
        }
    }
}
