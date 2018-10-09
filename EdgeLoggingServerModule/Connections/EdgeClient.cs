using SkunkLab.VirtualRtu.ModBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VirtualRtu.Common.Configuration;

namespace FieldGatewayMicroservice.Connections
{
    public class EdgeClient
    {
        public static event System.EventHandler<MqttErrorEventArgs> OnError;
        public static event System.EventHandler<MessageEventArgs> OnMessage;
        private static HttpClient httpClient;

        public static EdgeMqttClient Client { get; internal set; }
        public static IssuedConfig Config { get; internal set; }
               
        public static async Task Init(IssuedConfig config)
        {
            httpClient = new HttpClient();

            if (config == null)
            {
                Console.WriteLine("Config is null.  Cannot start mqtt client.");
                return;
            }

            Config = config;

            if (Client != null)
            {
                try
                {
                    await Client.CloseAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fault closing MQTT client '{0}'", ex.Message);
                }
            }


            try
            {
                Client = new EdgeMqttClient(config.Hostname, config.Port, config.PskIdentity, Convert.FromBase64String(config.PSK));
                Client.OnError += Client_OnError;
                Client.OnMessage += Client_OnMessage;
                await Client.ConnectAsync(config.SecurityToken);
                Console.WriteLine("MQTT client connected :-)");
                await Client.Subscribe(config.Resources.RtuInputResource);
                Console.WriteLine("Subscribed for input :-)");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fault opening MQTT client '{0}'", ex.Message);
            }
        }

        private static void Client_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine("{0} - Received Piraeus message", DateTime.Now.ToString("hh:MM:ss.ffff"));
            byte[] buffer = new byte[7];
            Buffer.BlockCopy(e.Message, 0, buffer, 0, buffer.Length);
            MbapHeader header = MbapHeader.Decode(buffer);
            Console.WriteLine("{0} - Transaction ID {1}", DateTime.Now.ToString("hh:MM:ss.ffff"), header.TransactionId);
            //forward to Modbus Protocol Adapter

            try
            {
                Console.WriteLine("{0} - Begin echo", DateTime.Now.ToString("hh:MM:ss.ffff"));
                //IPHelper.Queue.Enqueue(e.Message);
                HttpContent content = new System.Net.Http.ByteArrayContent(e.Message);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Headers.ContentLength = e.Message.Length;
                string requestUrl = IPHelper.GetAddress();
                Task<HttpResponseMessage> task = httpClient.PostAsync(requestUrl, content);
                Task.WaitAll(task);
                HttpResponseMessage response = task.Result;
                Console.WriteLine("{0} - Sent echo message with status code '{1}'", DateTime.Now.ToString("hh:MM:ss.ffff"), response.StatusCode);
                //Console.WriteLine("Transaction Id '{0}' sent with status code {1}", header.TransactionId, response.StatusCode);



            }
            catch (WebException we)
            {
                Console.WriteLine("Web exception - {0}", we.Message);
                if (we.InnerException != null)
                {
                    Console.WriteLine("Web inner exception - {0}", we.InnerException.Message);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Post exception - {0}", ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Post inner exception - {0}", ex.InnerException.Message);

                }
            }
        }

        private static void Client_OnError(object sender, MqttErrorEventArgs e)
        {
            Console.WriteLine("Mqtt client has faulted and will restart after 10 seconds - {0}", e.Error.Message);
            Task delayTask = Task.Delay(10000);
            Task.WaitAll(delayTask);

            Task task = Init(Config);
            Task.WhenAll(task);
        }
    }
}
