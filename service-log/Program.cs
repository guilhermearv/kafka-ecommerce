using Avro.Generic;
using common_kafka;
using Confluent.Kafka;
using System;

namespace service_log
{
    class Program
    {
        static void Main()
        {
            KafkaService kafkaService = new KafkaService();
            kafkaService.Subscribe = "ECOMMERCE.*";
            kafkaService.GroupId = "ECOMMERCE_LOG_GROUP";
            kafkaService.MessageResult += MessageResult;
            kafkaService.MessageError += MessageError;
            kafkaService.run();
        }

        static void MessageResult(object sender, ConsumeResult<Ignore, GenericRecord> record)
        {
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("LOG: " + record.Topic);
            Console.WriteLine(record.Message.Key);
            Console.WriteLine(record.Message.Value);
            Console.WriteLine(record.Partition);
            Console.WriteLine(record.Offset);
        }

        static void MessageError(object sender, string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
