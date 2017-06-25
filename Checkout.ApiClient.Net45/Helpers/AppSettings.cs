namespace Checkout.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using CheckoutEnvironment = Checkout.Helpers.Environment;

    /// <summary>
    /// Holds application settings that is read from the app.config or web.config
    /// </summary>
    public sealed class AppSettings
    {
        private static CheckoutEnvironment _environment = CheckoutEnvironment.Undefined;
        private static string _clientId;
        private static string _clientSecret;
        private static string _clientScope;
        private static string _baseApiUri;
        private static string _baseAuthenticationUri;
        private static int? _maxResponseContentBufferSize;
        private static int? _requestTimeout;
        private static bool? _debugMode;
        private const string _authenticationLiveUrl = "http://localhost:5000/";
        private const string _authenticationSandboxUrl = "http://localhost:5000/";
        private const string _apiLiveUrl = "http://localhost:5001/v1/";
        private const string _apiSandboxUrl = "http://localhost:5001/v1/";
        public const string ClientUserAgentName = "Checkout-DotNetLibraryClient/v1.0";
        public const string DefaultContentType = "application/json";

        public static string BaseApiUri
        {
            get { return _baseApiUri; }
            set { _baseApiUri = value; }
        }
        public static string BaseAuthenticationUri
        {
            get { return _baseAuthenticationUri; }
            set { _baseAuthenticationUri = value; }
        }
        public static string ClientId
        {
            get { return _clientId ?? (_clientId = ReadConfig("Client.Id", true)); }
            set { _clientId = value; }
        }
        public static string ClientSecret
        {
            get { return _clientSecret ?? (_clientSecret = ReadConfig("Client.Secret", true)); }
            set { _clientSecret = value; }
        }
        public static string ClientScope
        {
            get { return _clientScope ?? (_clientScope = ReadConfig("Client.Scope", true)); }
            set { _clientScope = value; }
        }

        public static int RequestTimeout
        {
            get
            {
                if (_requestTimeout == null)
                {
                   var value = ReadConfig("Checkout.RequestTimeout");
                   _requestTimeout = (!string.IsNullOrEmpty(value) ? int.Parse(value) : 60);
                }

                return _requestTimeout.Value;
            }
            set { _requestTimeout = value; }
        }
        public static int MaxResponseContentBufferSize { 
            get { 
                
                 if (_maxResponseContentBufferSize == null)
                {
                   var value = ReadConfig("Checkout.MaxResponseContentBufferSize");
                   _maxResponseContentBufferSize = (!string.IsNullOrEmpty(value) ? int.Parse(value) : 1000000);
                }

                return _maxResponseContentBufferSize.Value; 
            }

            set { _maxResponseContentBufferSize = value; } 
        }
        public static bool DebugMode
        {
            get
            {
                if (_debugMode == null)
                {
                    var value = ReadConfig("Checkout.DebugMode");
                    _debugMode = (!string.IsNullOrEmpty(value) ? bool.Parse(value) : false);
                }

                return _debugMode.Value;
            }
            set { _debugMode = value; }
        }
        public static CheckoutEnvironment Environment
        {
            get
            {
                return _environment;
            }

            set
            {
                switch (value)
                {
                    case CheckoutEnvironment.Live:
                        _baseApiUri = _apiLiveUrl;
                        _baseAuthenticationUri = _authenticationLiveUrl;
                        break;
                    case CheckoutEnvironment.Sandbox:
                        _baseApiUri = _apiSandboxUrl;
                        _baseAuthenticationUri = _authenticationSandboxUrl;
                        break;
                };
                _environment = value;
                ApiUrls.ResetApiUrls();

            }
        }

        public static void SetEnvironmentFromConfig()
        {
            CheckoutEnvironment selectedEnvironment;
            if (Enum.TryParse<CheckoutEnvironment>(ReadConfig("Checkout.Environment", true), out selectedEnvironment) && Enum.IsDefined(typeof(CheckoutEnvironment), selectedEnvironment))
            { Environment = selectedEnvironment; }
            else
            { throw new KeyNotFoundException("Config value is invalid for: Environment"); }
        }

        private static string ReadConfig(string key,bool throwIfnotExist=false)
        {
            try
            {
                return ConfigurationManager.AppSettings[key].ToString();
            }
            catch (Exception)
            {
                if (throwIfnotExist)
                {
                    throw new KeyNotFoundException("App settings Key not found for: " + key);
                }

                return null;
            }
        }
    }
}
