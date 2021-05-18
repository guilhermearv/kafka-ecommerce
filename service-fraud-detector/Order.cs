using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace service_fraud_detector
{
    public class Order
    {
        [JsonProperty("orderId")]
        public string orderId { get; set; }

        [JsonProperty("amount")]
        public int amount { get; set; }

        [JsonProperty("email")]
        public string email { get; set; }

        public string toString()
        {
            return "Order{" +
                    "orderId='" + orderId + '\'' +
                    ", amount=" + amount.ToString() +
                    ", email='" + email + '\'' +
                    '}';
        }
    }
}
