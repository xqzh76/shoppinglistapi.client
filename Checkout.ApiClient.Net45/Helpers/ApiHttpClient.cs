namespace Checkout.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Checkout.ApiServices.SharedModels;
    using Checkout.Utilities;

    /// <summary>
    /// Handles http requests and responses
    /// </summary>
    public sealed class ApiHttpClient
    {
        private WebRequestHandler requestHandler;
        private HttpClient httpClient;

        public ApiHttpClient()
        {
            this.ResetHandler();
        }

        public void ResetHandler()
        {
            if (this.requestHandler != null)
            {
                this.requestHandler.Dispose();
            }
            this.requestHandler = new WebRequestHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip,
                AllowAutoRedirect = false,
                UseDefaultCredentials = false,
                UseCookies = false
            };

            if (this.httpClient != null)
            {
                this.httpClient.Dispose();
            }

            this.httpClient = new HttpClient(this.requestHandler);
            this.httpClient.MaxResponseContentBufferSize = AppSettings.MaxResponseContentBufferSize;
            this.httpClient.Timeout = TimeSpan.FromSeconds(AppSettings.RequestTimeout);
            this.SetHttpRequestHeader("User-Agent",AppSettings.ClientUserAgentName);
            this.httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("Gzip"));
        }

        public void SetHttpRequestHeader(string name, string value)
        {
            if (this.httpClient.DefaultRequestHeaders.Contains(name))
            {
                this.httpClient.DefaultRequestHeaders.Remove(name);
            }

            if (value != null)
            { this.httpClient.DefaultRequestHeaders.Add(name, value); }
        }

        public string GetHttpRequestHeader(string name)
        {
            IEnumerable<string> values = null;
            this.httpClient.DefaultRequestHeaders.TryGetValues(name, out values);

            if (values != null && values.Any())
            { return values.First(); }

            return null;
        }

       
        /// <summary>
        /// Submits a get request to the given web address with default content type e.g. text/plain
        /// </summary>
        public HttpResponse<T> GetRequest<T>(string requestUri,string authenticationKey)
        {
            var httpRequestMsg = new HttpRequestMessage();

            httpRequestMsg.Method = HttpMethod.Get;
            httpRequestMsg.RequestUri = new Uri(requestUri);
            httpRequestMsg.Headers.Add("Accept", AppSettings.DefaultContentType);

            this.SetHttpRequestHeader("Authorization", authenticationKey);

            if (AppSettings.DebugMode)
            {
                Console.WriteLine(string.Format("\n\n** Request ** Post {0}", requestUri));
            }

            return this.SendRequest<T>(httpRequestMsg).Result;
        }

        /// <summary>
        /// Submits a post request to the given web address
        /// </summary>
        public HttpResponse<T> PostRequest<T>(string requestUri,string authenticationKey, object requestPayload = null)
        {
            var httpRequestMsg = new HttpRequestMessage(HttpMethod.Post, requestUri);
            var requestPayloadAsString = this.GetObjectAsString(requestPayload);

            httpRequestMsg.Content = new StringContent(requestPayloadAsString, Encoding.UTF8, AppSettings.DefaultContentType);
            httpRequestMsg.Headers.Add("Accept", AppSettings.DefaultContentType);
            
            this.SetHttpRequestHeader("Authorization", authenticationKey);
            
            if (AppSettings.DebugMode)
            {
                Console.WriteLine(string.Format("\n\n** Request ** Post {0}", requestUri));
                Console.WriteLine(string.Format("\n\n** Payload ** \n {0} \n", requestPayloadAsString));
            }

            return this.SendRequest<T>(httpRequestMsg).Result;
        }

        /// <summary>
        /// Submits a put request to the given web address
        /// </summary>
        public HttpResponse<T> PutRequest<T>(string requestUri, string authenticationKey, object requestPayload = null)
        {
            var httpRequestMsg = new HttpRequestMessage(HttpMethod.Put, requestUri);
            var requestPayloadAsString = this.GetObjectAsString(requestPayload);

            httpRequestMsg.Content = new StringContent(requestPayloadAsString, Encoding.UTF8, AppSettings.DefaultContentType);
            httpRequestMsg.Headers.Add("Accept", AppSettings.DefaultContentType);

            this.SetHttpRequestHeader("Authorization", authenticationKey);

            if (AppSettings.DebugMode)
            {
                Console.WriteLine(string.Format("\n\n** Request ** Put {0}", requestUri));
                Console.WriteLine(string.Format("\n\n** Payload ** \n {0} \n", requestPayloadAsString));
            }

            return this.SendRequest<T>(httpRequestMsg).Result;
        }

        /// <summary>
        /// Submits a delete request to the given web address
        /// </summary>
        public HttpResponse<T> DeleteRequest<T>(string requestUri, string authenticationKey)
        {
            var httpRequestMsg = new HttpRequestMessage();

            httpRequestMsg.Method = HttpMethod.Delete;
            httpRequestMsg.RequestUri = new Uri(requestUri);
            httpRequestMsg.Headers.Add("Accept", AppSettings.DefaultContentType);

            this.SetHttpRequestHeader("Authorization", authenticationKey);

            if (AppSettings.DebugMode)
            {
                Console.WriteLine(string.Format("\n\n** Request ** Delete {0}", requestUri));
            }

            return this.SendRequest<T>(httpRequestMsg).Result; 
        }

        /// <summary>
        /// Sends a http request with the given object. All headers should be set manually here e.g. content type=application/json
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Task<HttpResponse<T>> SendRequest<T>(HttpRequestMessage request)
        {
            Task<HttpResponse<T>> response = null;
            HttpResponseMessage responseMessage = null;
            string responseAsString = null;
            string responseCode = null;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                responseMessage = this.httpClient.SendAsync(request).Result; 
               
                responseCode = responseMessage.StatusCode.ToString();

                var responseContent = responseMessage.Content.ReadAsByteArrayAsync().Result;

                if (responseContent != null && responseContent.Length > 0)
                {
                    responseAsString = Encoding.UTF8.GetString(responseContent);

                    if (AppSettings.DebugMode)
                    {
                        Console.WriteLine(string.Format("\n** HttpResponse - Status {0}**\n {1}\n", responseMessage.StatusCode, responseAsString));
                    }
                }

                response = Task.FromResult(this.CreateHttpResponse<T>(responseAsString, responseMessage.StatusCode));
            }
            catch (Exception ex)
            {
                if (AppSettings.DebugMode)
                {
                    Console.WriteLine(string.Format(@"\n** Exception - HttpStatuscode:\n{0}**\n\n 
                        ** ResponseString {1}\n ** Exception Messages{2}\n ", (responseMessage != null ? responseMessage.StatusCode.ToString() : string.Empty), responseAsString, ExceptionHelper.FlattenExceptionMessages(ex)));
                }

                responseCode = "Exception" + ex.Message;

                throw;
            }
            finally
            {
                request.Dispose();
                this.ResetHandler();
            }

            return response;
        }

        private HttpResponse<T> CreateHttpResponse<T>(string responseAsString, HttpStatusCode httpStatusCode)
        {
            if (httpStatusCode == HttpStatusCode.OK && responseAsString != null)
            {
                return new HttpResponse<T>(this.GetResponseAsObject<T>(responseAsString))
                {
                    HttpStatusCode = httpStatusCode
                };
            }
            else if (responseAsString != null)
            {
                return new HttpResponse<T>(default(T))
                {
                    Error = this.GetResponseAsObject<ResponseError>(responseAsString),
                    HttpStatusCode = httpStatusCode
                };
            }

            return null;
        }

        private string GetObjectAsString(object requestModel)
        {
            return ContentAdaptor.ConvertToJsonString(requestModel);
        }

        private T GetResponseAsObject<T>(string responseAsString)
        {
            return ContentAdaptor.JsonStringToObject<T>(responseAsString);
        }

    }
}
