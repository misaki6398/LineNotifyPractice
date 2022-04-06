using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LineNotifyPractice.Enums;

namespace LineNotifyPractice.Utilitys
{
    public static class HttpClientUtility
    {
        /// <summary>
        /// 共用 POST 工具
        /// </summary>
        /// <param name="url"></param>
        /// <param name="contentType"></param>
        /// <param name="requestModel"></param>
        /// <param name="requestType"></param>
        /// <param name="token"></param>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        public static async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest requestModel, RequestType requestType, string token = null)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                if (token is not null)
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }

                HttpResponseMessage response;
                string jsonString = string.Empty;
                switch (requestType)
                {
                    case RequestType.Json:
                        jsonString = JsonSerializer.Serialize(requestModel);
                        response = await httpClient.PostAsync(url, new StringContent(jsonString, Encoding.UTF8, "application/json"));
                        break;
                    case RequestType.FormUrlEncoded:
                        jsonString = JsonSerializer.Serialize(requestModel);
                        var request = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
                        var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(request) };
                        response = await httpClient.SendAsync(req);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                TResponse? model = default(TResponse);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    model = JsonSerializer.Deserialize<TResponse>(responseString);
                }
                else
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    throw new Exception($@"Error detect status: {response.StatusCode}, request:{jsonString}, response:{responseString}");
                }

                return model;
            }
        }

        /// <summary>
        /// 共用 GET 工具
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        public static async Task<TResponse> GetAsync<TResponse>(string url, string token = null)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string getProfileUrl = url;
                if (token is not null)
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }
                var response = await httpClient.GetAsync(getProfileUrl);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseModel = JsonSerializer.Deserialize<TResponse>(responseString);
                    return responseModel;
                }
                else
                {
                    throw new Exception($"Token not valid, please relogin status code is {response.StatusCode.ToString()}");
                }
            }
        }
    }
}