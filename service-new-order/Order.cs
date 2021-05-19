namespace service_new_order
{
    public class Order
    {
        public string orderId { get; set; }
        public int amount { get; set; }
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
