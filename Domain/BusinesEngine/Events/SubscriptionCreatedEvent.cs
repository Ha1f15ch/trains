using DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.Events
{
    //Специальный тип данных для обработчика события mediatr
    public class SubscriptionCreatedEvent : INotification
    {
        public SubscriptionDto Subscription { get; }

        public SubscriptionCreatedEvent(SubscriptionDto subscription)
        {
            Subscription = subscription;
        }

    }
}
