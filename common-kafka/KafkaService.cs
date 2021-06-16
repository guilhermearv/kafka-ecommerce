using Avro.Generic;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace common_kafka
{
    public class KafkaService
    {
        private ConsumeResult<Ignore, GenericRecord> _ConsumeResult { get; set; }
        public string Subscribe { get; set; }
        public string GroupId { get; set; }
        public string KAFKA_SCHEMA_REGISTRY_URL { get; private set; }
        public string KAFKA_SCHEMA_REGISTRY_AUTH { get; private set; }
        public string KAFKA_URL_SERVER_URL { get; private set; }

        public KafkaService()
        {
            KAFKA_SCHEMA_REGISTRY_URL = "localhost:8081";
            KAFKA_SCHEMA_REGISTRY_AUTH = "";
            KAFKA_URL_SERVER_URL = "localhost:9092";
        }

        public Task run()
        {
            try
            {
                CancellationToken stoppingToken;

                var config = new ConsumerConfig
                {
                    BootstrapServers = KAFKA_URL_SERVER_URL,
                    GroupId = this.GroupId,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = false
                };

                using (var schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig { Url = KAFKA_SCHEMA_REGISTRY_URL, BasicAuthUserInfo = KAFKA_SCHEMA_REGISTRY_AUTH }))
                {
                    using (var consumer = new ConsumerBuilder<Ignore, GenericRecord>(config)
                        .SetValueDeserializer(new AvroDeserializer<GenericRecord>(schemaRegistry).AsSyncOverAsync())
                        .Build())
                    {
                        consumer.Subscribe(this.Subscribe);

                        var i = 0;
                        while (!stoppingToken.IsCancellationRequested)
                        {
                            this._ConsumeResult = consumer.Consume(stoppingToken);

                            if (this._ConsumeResult != null)
                            {
                                this.MessageResult?.Invoke(this, this._ConsumeResult);

                                if (i++ % 1000 == 0)
                                {
                                    consumer.Commit();
                                }
                            }
                        }

                        consumer.Close();
                    }

                    schemaRegistry.Dispose();
                }
            }
            catch (ConsumeException e)
            {
                this.MessageError?.Invoke(this, string.Format("Delivery failed: {0}", e.Error.Reason));
            }
            catch (Exception exception)
            {
                KafkaDispatcher kafkaDispatcher = new KafkaDispatcher();
                kafkaDispatcher.MessageError += MessageError;

                ExceptionMessage exceptionMessage = new ExceptionMessage();
                exceptionMessage.Message = string.Format("exception.Message: {0} exception.StackTrace: {1}", exception.Message, exception.StackTrace);
                kafkaDispatcher.Send("ECOMMERCE_DEADLETTER", JsonConvert.SerializeObject(exceptionMessage));
            }

            return Task.CompletedTask;
        }

        public event EventHandler<ConsumeResult<Ignore, GenericRecord>> MessageResult;

        public event EventHandler<string> MessageError;
    }
}
