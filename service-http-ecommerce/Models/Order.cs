using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace service_http_ecommerce.Models
{
    public class Order
    {
        public string orderId { get; set; }
        public int amount { get; set; }
        public string email { get; set; }
    }
}
