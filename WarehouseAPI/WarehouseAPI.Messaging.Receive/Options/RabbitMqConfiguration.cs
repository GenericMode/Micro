using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WarehouseAPI.Messaging.Receive.Options;
using WarehouseAPI.Service.Models;
using WarehouseAPI.Service.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WarehouseAPI.Messaging.Receive.Options
{
        public class RabbitMqConfiguration
        {
            public string Hostname { get; set; }

            public string QueueName { get; set; }

            public string UserName { get; set; }

            public string Password { get; set; }

            public bool Enabled { get; set; }
        }



}