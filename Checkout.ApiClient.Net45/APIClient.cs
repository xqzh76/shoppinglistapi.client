namespace Checkout
{
    using Checkout.ApiServices.ShoppingList;
    using Checkout.Helpers;

    public sealed class APIClient
    {
        private ShoppingListService _shoppingListService;

        public ShoppingListService ShoppingListService { get {return _shoppingListService ?? (_shoppingListService = new ShoppingListService());} }

        public APIClient()
        {
            if (AppSettings.Environment == Environment.Undefined)
            {
                AppSettings.SetEnvironmentFromConfig();
            }

            ContentAdaptor.Setup();
        }

        public APIClient(string clientId, string clientSecret, string clientScope, Environment env, bool debugMode, int connectTimeout)
            : this(clientId, clientSecret, clientScope, env, debugMode)
        {
            AppSettings.RequestTimeout = connectTimeout;
        }

        public APIClient(string clientId, string clientSecret, string clientScope, Environment env, bool debugMode)
            : this(clientId, clientSecret, clientScope, env)
        {
            AppSettings.DebugMode = debugMode;
        }

        public APIClient(string clientId, string clientSecret, string clientScope, Environment env)
        {
            AppSettings.ClientId = clientId;
            AppSettings.ClientScope = clientSecret;
            AppSettings.ClientSecret = clientScope;
            AppSettings.Environment = env;
            ContentAdaptor.Setup();
        }

        public APIClient(string clientId, string clientSecret, string clientScope, bool debugMode)
            : this(clientId, clientSecret, clientScope)
        {
            AppSettings.DebugMode = debugMode;
        }

        public APIClient(string clientId, string clientSecret, string clientScope) :this()
        {
            AppSettings.ClientId = clientId;
            AppSettings.ClientScope = clientSecret;
            AppSettings.ClientSecret = clientScope;
        }
    }
}
