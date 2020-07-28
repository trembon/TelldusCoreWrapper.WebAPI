using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TelldusCoreWrapper.Entities;
using TelldusCoreWrapper.Enums;

namespace TelldusCoreWrapper.WebAPI.Services
{
    public interface IWebhookService
    {
        Task SendSensorWebHook(SensorValue sensorValue);

        Task SendDeviceEventWebHook(int deviceId, DeviceMethods command, string parameter);

        Task SendRawCommandWebHook(RawCommandReceivedEventArgs rawCommand);
    }

    public class WebhookService : IWebhookService, IDisposable
    {
        private HttpClient httpClient;
        
        private IConfiguration configuration;
        private ILogger<WebhookService> logger;

        public WebhookService(IConfiguration configuration, ILogger<WebhookService> logger)
        {
            httpClient = new HttpClient();

            this.logger = logger;
            this.configuration = configuration;
        }

        public void Dispose()
        {
            try
            {
                httpClient?.Dispose();
                httpClient = null;
            }catch { }
        }

        public async Task SendDeviceEventWebHook(int deviceId, DeviceMethods command, string parameter)
        {
            string jsonData = JsonConvert.SerializeObject(new { deviceId, command, parameter });

            foreach (string url in configuration.GetSection("Webhooks:DeviceEvents").Get<string[]>())
            {
                try
                {
                    bool result = await SendData(url, jsonData);
                    if (!result)
                        throw new Exception("Invalid response from url");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed to send device event to webhook: {url}");
                }
            }
        }

        public async Task SendSensorWebHook(SensorValue sensorValue)
        {
            string jsonData = JsonConvert.SerializeObject(sensorValue);

            foreach (string url in configuration.GetSection("Webhooks:SensorUpdates").Get<string[]>())
            {
                try
                {
                    bool result = await SendData(url, jsonData);
                    if (!result)
                        throw new Exception("Invalid response from url");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed to send sensor data to webhook: {url}");
                }
            }
        }

        public async Task SendRawCommandWebHook(RawCommandReceivedEventArgs rawCommand)
        {
            string jsonData = JsonConvert.SerializeObject(new { rawCommand.ControllerID, rawCommand.RawData });

            foreach (string url in configuration.GetSection("Webhooks:RawCommands").Get<string[]>())
            {
                try
                {
                    bool result = await SendData(url, jsonData);
                    if (!result)
                        throw new Exception("Invalid response from url");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed to send raw command to webhook: {url}");
                }
            }
        }

        private async Task<bool> SendData(string url, string jsonData)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.SendAsync(request);
            return response.StatusCode == HttpStatusCode.OK;
        }
    }
}
