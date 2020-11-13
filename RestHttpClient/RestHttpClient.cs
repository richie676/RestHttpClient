using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;

namespace RestClient
{
    public class RestHttpClient
    {
        public double TimeoutSeconds { get; set; } = 90;

        public T Get<T>(string url)
        {
            return SendRequest<T>(HttpMethod.Get, null, url);
        }

        public T Post<T>(string url, object requestObject)
        {

            return SendRequest<T>(HttpMethod.Post, requestObject, url);

        }

        public T Put<T>(string url, object requestObject)
        {

            return SendRequest<T>(HttpMethod.Put, requestObject, url);
        }

        public T Head<T>(string url, object requestObject)
        {

            return SendRequest<T>(HttpMethod.Head, requestObject, url);

        }

        public T Delete<T>(string url)
        {
            return SendRequest<T>(HttpMethod.Delete, null, url);
        }

        private string GetContentFromRequest(HttpResponseMessage response)
        {
            var contentArray = response.Content?.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            return Encoding.UTF8.GetString(contentArray, 0, contentArray.Length);
        }

        protected string SerializeObjectToJson(object toSerialize)
        {
            if (toSerialize == null)
            {
                return null;
            }

            if (toSerialize is string)
            {
                return (string)toSerialize;
            }

            return JsonConvert.SerializeObject(toSerialize);
        }

        private T SendRequest<T>(HttpMethod method, object data, string url)
        {
            try
            {
                HttpResponseMessage httpResponse = null;
                HttpRequestMessage request = BuildRequest(method, data, url);
                httpResponse = SendRequest(request);

                T response = JsonConvert.DeserializeObject<T>(GetContentFromRequest(httpResponse));
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private HttpRequestMessage BuildRequest(HttpMethod method, object data, string url)
        {
            var request = new HttpRequestMessage(method, url);

            request.Headers.Add("Accept", "application/json");

            if (data != null)
            {
                string content = SerializeObjectToJson(data);
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }

            //should be removed from SetHeaders, but still there for old Clients without retry
            request.Headers.Remove("IRIS-Request-Id");
            request.Headers.Add("IRIS-Request-Id", Guid.NewGuid().ToString());

            return request;
        }

        private HttpResponseMessage SendRequest(HttpRequestMessage request)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);
                return client.SendAsync(request, HttpCompletionOption.ResponseContentRead).GetAwaiter().GetResult();
            }
        }
    }

}

