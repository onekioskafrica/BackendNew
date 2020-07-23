using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Helpers
{
    public static class ApiHelper
    {
        public static async Task<T> DoWebRequestAsync<T>(string url, object request, string requestType, Dictionary<string, string> headers = null, string authUserName = null, string authPword = null) where T : new()
        {
            T result = new T();
            Method method = requestType.ToLower() == "post" ? Method.POST : Method.GET;
            var client = new RestClient(url);
            var restRequest = new RestRequest(method);
            if (method == Method.POST)
            {
                restRequest.RequestFormat = DataFormat.Json;
                restRequest.JsonSerializer = NewtonsoftJsonSerializer.Default;
                restRequest.AddJsonBody(request);
            }

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    restRequest.AddHeader(item.Key, item.Value);
                }
            }

            if (!string.IsNullOrEmpty(authUserName) && !string.IsNullOrEmpty(authPword))
            {
                client.Authenticator = new HttpBasicAuthenticator(authUserName, authPword);
            }

            try
            {
                //log.Info("Request {@request} ", restRequest);
                IRestResponse<T> response = await client.ExecuteAsync<T>(restRequest);

                result = JsonConvert.DeserializeObject<T>(response.Content);

                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }
    }

    public class NewtonsoftJsonSerializer : ISerializer, IDeserializer
    {
        private Newtonsoft.Json.JsonSerializer serializer;

        public NewtonsoftJsonSerializer(Newtonsoft.Json.JsonSerializer serializer)
        {
            this.serializer = serializer;
        }

        public string ContentType
        {
            get { return "application/json"; } // Probably used for Serialization?
            set { }
        }

        public string DateFormat { get; set; }

        public string Namespace { get; set; }

        public string RootElement { get; set; }

        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    serializer.Serialize(jsonTextWriter, obj);

                    return stringWriter.ToString();
                }
            }
        }

        public T Deserialize<T>(RestSharp.IRestResponse response)
        {
            var content = response.Content;

            using (var stringReader = new StringReader(content))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    return serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }

        public static NewtonsoftJsonSerializer Default
        {
            get
            {
                return new NewtonsoftJsonSerializer(new Newtonsoft.Json.JsonSerializer()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });
            }
        }
    }
}
