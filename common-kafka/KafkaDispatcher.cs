using System;
using System.Collections.Generic;
using System.Text;

using Avro;
using Avro.Generic;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Newtonsoft.Json;

namespace common_kafka
{
    public class KafkaDispatcher
    {
        public string KAFKA_SCHEMA_REGISTRY_URL { get; private set; }
        public string KAFKA_SCHEMA_REGISTRY_AUTH { get; private set; }
        public string KAFKA_URL_SERVER_URL { get; private set; }

        public KafkaDispatcher()
        {
            KAFKA_SCHEMA_REGISTRY_URL = "localhost:8081";
            KAFKA_SCHEMA_REGISTRY_AUTH = "";
            KAFKA_URL_SERVER_URL = "localhost:9092";
        }

        public void Send(string Topic, string value)
        {
            try
            {
                using (var schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig { Url = KAFKA_SCHEMA_REGISTRY_URL, BasicAuthUserInfo = KAFKA_SCHEMA_REGISTRY_AUTH }))
                {
                    using (var producer =
                        new ProducerBuilder<string, GenericRecord>(new ProducerConfig { BootstrapServers = KAFKA_URL_SERVER_URL })
                            .SetValueSerializer(new AvroSerializer<GenericRecord>(schemaRegistry))
                            .Build())
                    {
                        var schema = schemaRegistry.GetLatestSchemaAsync($"{Topic}-value").GetAwaiter().GetResult();
                        var schemaParser = (RecordSchema)RecordSchema.Parse(schema.SchemaString);
                        var record = new GenericRecord(schemaParser);
                        var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
                        foreach (var item in values)
                        {
                            record.Add(item.Key.ToString(), item.Value);
                        }

                        var message = new Message<string, GenericRecord> { Value = record };
                        producer.ProduceAsync(Topic, message).GetAwaiter().GetResult();
                    };
                }
            }
            catch (ProduceException<Null, string> e)
            {
                this.MessageError?.Invoke(this, string.Format("Delivery failed: {0}", e.Error.Reason));
            }
            catch (Exception exception)
            {
                this.MessageError?.Invoke(this, string.Format("exception.Message: {0} exception.StackTrace: {1}", exception.Message, exception.StackTrace));
            }
        }

        public event EventHandler<string> MessageError;
    }
}
