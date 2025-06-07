using Azure.Messaging.ServiceBus;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BidMasterOnline.Core.Services
{
    public class NotificationQueueProducer : INotificationsQueueProducer
    {
        private readonly ILogger<NotificationQueueProducer> _logger;

        private readonly string _serviceBusConnectionString;
        private readonly string _serviceBusQueueName;

        public NotificationQueueProducer(ILogger<NotificationQueueProducer> logger,
            IConfiguration configuration)
        {
            _logger = logger;

            _serviceBusConnectionString = configuration["AzureServiceBus:ConnectionString"]!;
            _serviceBusQueueName = configuration["AzureServiceBus:QueueName"]!;
        }

        public async Task PushNotificationAsync(EmailNotificationDTO notification)
        {
            ServiceBusClientOptions clientOptions = new ()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            ServiceBusClient client = new(_serviceBusConnectionString, clientOptions);
            ServiceBusSender sender = client.CreateSender(_serviceBusQueueName);

            string jsonNotification = JsonSerializer.Serialize(notification);

            try
            {
                await sender.SendMessageAsync(new ServiceBusMessage(jsonNotification));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while pushing notification to the service bus.");
            }
            finally
            {
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
        }
    }
}
