using Avro.Generic;
using common_kafka;
using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace service_fraud_detector
{
    class Program
    {
        private static KafkaDispatcher kafkaDispatcher = new KafkaDispatcher();

        static void Main()
        {
            KafkaService kafkaService = new KafkaService();
            kafkaService.Subscribe = "ECOMMERCE_NEW_ORDER";
            kafkaService.GroupId = "ECOMMERCE_NEW_ORDER_GROUP";
            kafkaService.MessageResult += MessageResult;
            kafkaService.MessageError += MessageError;
            kafkaService.run();
        }

        public static void MessageResult(object sender, ConsumeResult<Ignore, GenericRecord> record)
        {
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("Processing new order, checking for fraud");
            Console.WriteLine(record.Message.Key);
            Console.WriteLine(record.Message.Value);
            Console.WriteLine(record.Partition);
            Console.WriteLine(record.Offset);

            Order order = new Order();
            order.orderId = record.Message.Value["orderId"].ToString();
            order.amount = Convert.ToInt32(record.Message.Value["amount"]);
            order.email = record.Message.Value["email"].ToString();
            if (isFraud(order))
            {
                Console.WriteLine("Order is a fraud!!!!!" + order);
                kafkaDispatcher.Send("ECOMMERCE_ORDER_REJECTED", JsonConvert.SerializeObject(order));
            }
            else 
            {
                Console.WriteLine("Approved: " + order);
                kafkaDispatcher.Send("ECOMMERCE_ORDER_APPROVED", JsonConvert.SerializeObject(order));
            }
            
        }

        public static void MessageError(object sender, string msg)
        {
            Console.WriteLine(msg);
        }

        private static bool isFraud(Order order)
        {
            return order.amount.CompareTo(4500) >= 0;
        }
    }
}
