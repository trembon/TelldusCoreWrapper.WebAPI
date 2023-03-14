using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelldusCoreWrapper.Enums;
using TelldusCoreWrapper.WebAPI.Extensions;

namespace TelldusCoreWrapper.WebAPI.Services
{
    public interface ITelldusCommandService
    {
        Task SendCommand(int id, DeviceMethods command, string parameter = null);
    }

    public class TelldusCommandService : ITelldusCommandService
    {
        private readonly IConfiguration configuration;
        private readonly ITelldusCoreService telldusCoreService;

        private readonly object queueLock = new();
        private Queue<QueueItem> sendQueue;
        private bool isQueueProcessing = false;

        public TelldusCommandService(IConfiguration configuration, ITelldusCoreService telldusCoreService)
        {
            this.configuration = configuration;
            this.telldusCoreService = telldusCoreService;

            sendQueue = new Queue<QueueItem>();
        }

        public Task SendCommand(int id, DeviceMethods command, string parameter = null)
        {
            lock (queueLock)
            {
                // if same device is already in queue, remove it
                if (sendQueue.Any(i => i.DeviceID == id))
                    sendQueue = sendQueue.Where(i => i.DeviceID != id).ToQueue();

                QueueItem item = new(){ DeviceID = id, Command = command, Parameter = parameter };

                // add one first to queue, so it will be executed next
                var list = sendQueue.ToList();
                list.Insert(0, item);
                sendQueue = list.ToQueue();

                // and then the other last in the queue, so total 3 tries or the configured amount
                for (int i = 1; i < configuration.GetValue("Telldus:RetryCount", 3); i++)
                    sendQueue.Enqueue(item);

                // if the queue is not running, start it
                if (!isQueueProcessing)
                {
                    isQueueProcessing = true;
                    Task.Run(ExecuteSendCommand);
                }
            }

            return Task.FromResult(true);
        }

        private void ExecuteSendCommand()
        {
            // get the item from the queue
            QueueItem item = null;
            lock (queueLock)
            {
                item = sendQueue.Dequeue();
            }

            var result = telldusCoreService.SendCommand(item.DeviceID, item.Command, item.Parameter);
            if (result != ResultCode.Success)
                throw new InvalidOperationException();

            // when done sending command, check if more is in the queue and then continue processing
            lock (queueLock)
            {
                if (sendQueue.Count > 0)
                {
                    // get delay, should be about 3 second delay or the configured value between each command
                    int delay = configuration.GetValue("Telldus:RetryWait", 3000);
                    Task.Delay(delay).ContinueWith(t => ExecuteSendCommand());
                }
                else
                {
                    isQueueProcessing = false;
                }
            }
        }

        private class QueueItem
        {
            public int DeviceID { get; set; }

            public DeviceMethods Command { get; set; }

            public string Parameter { get; set; }

            public override string ToString()
            {
                return $"ID: {DeviceID} - {Command}";
            }
        }
    }
}
