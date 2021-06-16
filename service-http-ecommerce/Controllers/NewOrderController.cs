using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using common_kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using service_http_ecommerce.Models;

namespace service_http_ecommerce.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NewOrderController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                Order order = new Order();
                order.email = "user@email.com";
                order.orderId = Guid.NewGuid().ToString();
                order.amount = (2 * 500 + 1);
                var value = JsonConvert.SerializeObject(order);

                KafkaDispatcher kafkaDispatcher = new KafkaDispatcher();
                kafkaDispatcher.MessageError += MessageError;

                kafkaDispatcher.Send("ECOMMERCE_NEW_ORDER", value);

                EmailMessage emailMessage = new EmailMessage();
                emailMessage.Message = "Thank you for your order! We are processing your order!";
                kafkaDispatcher.Send("ECOMMERCE_SEND_EMAIL", JsonConvert.SerializeObject(emailMessage));


                var item = new { Name = "New order sent" };
                return new OkObjectResult(item);
            }
            catch (Exception e)
            {
                var item = new { Error = e };
                return StatusCode(500);
            }
        }

        private void MessageError(object sender, string e)
        {
            throw new Exception(e);
        }
    }
}
