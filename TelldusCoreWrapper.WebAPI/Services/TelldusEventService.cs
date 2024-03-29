﻿using Microsoft.Extensions.Logging;
using TelldusCoreWrapper.Entities;

namespace TelldusCoreWrapper.WebAPI.Services
{
    public interface ITelldusEventService
    {
        void Initialize();
    }

    public class TelldusEventService : ITelldusEventService
    {
        private readonly IWebhookService webHookService;
        private readonly ILogger<TelldusEventService> logger;
        private readonly ITelldusCoreService telldusCoreService;

        public TelldusEventService(ITelldusCoreService telldusCoreService, IWebhookService webHookService, ILogger<TelldusEventService> logger)
        {
            this.logger = logger;
            this.webHookService = webHookService;
            this.telldusCoreService = telldusCoreService;
        }

        public void Initialize()
        {
            telldusCoreService.SensorUpdated += TelldusCoreService_SensorUpdated;
            telldusCoreService.CommandReceived += TelldusCoreService_CommandReceived;
            telldusCoreService.RawCommandReceived += TelldusCoreService_RawCommandReceived;
        }

        private void TelldusCoreService_SensorUpdated(object sender, SensorUpdateEventArgs e)
        {
            logger.LogInformation($"Received sensor update from sensor with ID '{e?.Value?.SensorID}'.");
            webHookService.SendSensorWebHook(e.Value);
        }

        private void TelldusCoreService_RawCommandReceived(object sender, RawCommandReceivedEventArgs e)
        {
            // always check if webhook is configured
            webHookService.SendRawCommandWebHook(e);

            // filter for the log
            if (e.Values.TryGetValue("class", out string value) && value == "sensor")
                return;

            logger.LogInformation($"Raw data recieved: {e.RawData}");
        }

        private void TelldusCoreService_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            logger.LogInformation($"Device '{e.Device.Name}' ({e.Device.ID}) had {e.Command} sent to it.");
            webHookService.SendDeviceEventWebHook(e.Device.ID, e.Command, e.Parameter);
        }
    }
}
