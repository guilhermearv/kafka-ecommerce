using common_kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace service_new_order
{
    class Program
    {
        static void Main()
        {
            KafkaDispatcher kafkaDispatcher = new KafkaDispatcher();
            kafkaDispatcher.MessageError += MessageError;

            Order order = new Order();
            order.email = RandomString(8) + "@email.com";

            for (int i = 0; i < 10; i++)
            {
                order.orderId = Guid.NewGuid().ToString();
                order.amount = (random.Next(52) * 5000 + 1);
                var value = JsonConvert.SerializeObject(order);
                kafkaDispatcher.Send("ECOMMERCE_NEW_ORDER", value);

                var value2 = new Dictionary<string, string>();
                value2.Add("message", "Thank you for your order! We are processing your order!");
                value = JsonConvert.SerializeObject(value2);
                kafkaDispatcher.Send("ECOMMERCE_SEND_EMAIL", value);
            }
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        static void MessageError(object sender, string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
