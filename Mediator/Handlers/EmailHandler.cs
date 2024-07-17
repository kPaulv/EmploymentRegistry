using Contracts.Interfaces;
using MediatorService.Notifications;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediatorService.Handlers
{
    internal sealed class EmailHandler : INotificationHandler<CompanyDeletedNotification>
    {
        private readonly ILoggerManager _logger;

        public EmailHandler(ILoggerManager logger) => _logger = logger;
        
        public async Task Handle(CompanyDeletedNotification notification, 
            CancellationToken cancellationToken)
        {
            _logger.Warning($"A Delete command for company with " +
                $"Id = {notification.Id} was sent to API.");

            // TODO: add sending this notification to email logic

            await Task.CompletedTask;
        }
    }
}
