namespace Tests
{
    using Checkout;
    using NUnit.Framework;

    public class BaseServiceTests
    {
        protected APIClient CheckoutClient;

        [SetUp]
        public void Init()
        {
            this.CheckoutClient = new APIClient(); 
        }
    }
}
