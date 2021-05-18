using Avro.Generic;
using common_kafka;
using Confluent.Kafka;
using System;

namespace service_email
{
    class Program
    {
        static void Main()
        {
            KafkaService kafkaService = new KafkaService();
            kafkaService.Subscribe = "ECOMMERCE_SEND_EMAIL";
            kafkaService.GroupId = "ECOMMERCE_SEND_EMAIL_GROUP";
            kafkaService.MessageResult += MessageResult;
            kafkaService.MessageError += MessageError;
            kafkaService.run();
        }

        static void MessageResult(object sender, ConsumeResult<Ignore, GenericRecord> record)
        {
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("Send email");
            Console.WriteLine(record.Message.Key);
            Console.WriteLine(record.Message.Value);
            Console.WriteLine(record.Partition);
            Console.WriteLine(record.Offset);            
            Console.WriteLine("Email sent");
        }

        static void MessageError(object sender, string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
