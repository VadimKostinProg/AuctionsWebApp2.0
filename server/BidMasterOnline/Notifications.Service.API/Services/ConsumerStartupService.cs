using Azure.Messaging.ServiceBus;
using BidMasterOnline.Core.DTO;
using System.Text.Json;

namespace Notifications.Service.API.Services
{
    public class ConsumerStartupService
    {
        private readonly EmailSender _emailSender;
        private readonly ILogger<ConsumerStartupService> _logger;
        private readonly string _serviceBusConnectionString;
        private readonly string _serviceBusQueueName;

        public ConsumerStartupService(EmailSender emailSender,
            ILogger<ConsumerStartupService> logger,
            IConfiguration configuration)
        {
            _emailSender = emailSender;
            _logger = logger;

            _serviceBusConnectionString = configuration["AzureServiceBus:ConnectionString"]!;
            _serviceBusQueueName = configuration["AzureServiceBus:QueueName"]!;
        }

        public async Task StartConsumerAsync()
        {
            ServiceBusClientOptions clientOptions = new()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            ServiceBusClient client = new(_serviceBusConnectionString, clientOptions);

            ServiceBusProcessor processor = client.CreateProcessor(_serviceBusQueueName);

            try
            {
                _logger.LogInformation("Starting service bus consumer...");

                processor.ProcessMessageAsync += ProcessEmail;
                processor.ProcessErrorAsync += ProcessError;

                await processor.StartProcessingAsync();

                _logger.LogInformation("Service bus consumer has started...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during strting consumer.");

                await processor.StopProcessingAsync();
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }

        private Task ProcessEmail(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();

            EmailNotificationDTO notification = JsonSerializer.Deserialize<EmailNotificationDTO>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            _emailSender.SendEmailAsync(notification);

            return Task.CompletedTask;
        }

        private Task ProcessError(ProcessErrorEventArgs args)
        {
            _logger.LogInformation(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
