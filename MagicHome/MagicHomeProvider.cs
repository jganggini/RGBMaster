﻿using Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace MagicHome
{
    public class MagicHomeProvider : BaseProvider
    {
        private const int DISCOVERY_PORT = 48899;
        private const string DISCOVERY_MESSAGE = "HF-A11ASSISTHREAD";
        private UdpClient discoveryUdpClient;

        public MagicHomeProvider(): base(new MagicHomeProviderMetadata())
        {

        }

        public override async Task<List<Device>> Discover()
        {
            var lights = new List<Device>();

            var data = Encoding.UTF8.GetBytes(DISCOVERY_MESSAGE);
            await discoveryUdpClient.SendAsync(data, data.Length, "255.255.255.255", DISCOVERY_PORT);
            bool keepReceiving = true;

            while (keepReceiving)
            {
                using (var timeoutCancellationTokenSource = new CancellationTokenSource())
                {
                    var socketReceiveTask = discoveryUdpClient.ReceiveAsync();
                    socketReceiveTask.ConfigureAwait(false);

                    var completedTask = await Task.WhenAny(socketReceiveTask, Task.Delay(1000, timeoutCancellationTokenSource.Token));

                    if (completedTask != socketReceiveTask)
                    {
                        keepReceiving = false;
                    }
                    else
                    {
                        timeoutCancellationTokenSource.Cancel();
                        var payload = await socketReceiveTask;

                        string message = Encoding.UTF8.GetString(payload.Buffer);

                        if (message != DISCOVERY_MESSAGE)
                        {
                            string address = message.Split(',')[0];
                            lights.Add(new MagicHomeDevice(address, new MagicHomeDeviceMetadata(ProviderMetadata.ProviderGuid, address)));
                        }
                    }
                }
            }

            return lights;
        }

        protected override Task InternalRegister()
        {
            discoveryUdpClient = new UdpClient(DISCOVERY_PORT);

            return Task.CompletedTask;
        }

        protected override Task InternalUnregister()
        {
            discoveryUdpClient.Close();
            discoveryUdpClient.Dispose();

            return Task.CompletedTask;
        }
    }
}
