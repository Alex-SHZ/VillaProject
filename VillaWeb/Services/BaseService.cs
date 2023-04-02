using System;
using System.Text;
using Newtonsoft.Json;
using VillaUtility;
using VillaWeb.Models;
using VillaWeb.Services.IServices;

namespace VillaWeb.Services;

public class BaseService : IBaseService
{
    public APIResponse responseModel { get; set; }
    public IHttpClientFactory httpClient { get; set; }

    public BaseService(IHttpClientFactory httpClient)
    {
        responseModel = new();
        this.httpClient = httpClient;
    }

    public async Task<T> SendAsync<T>(APIRequest apiRequest)
    {
        try
        {
            HttpClient client = httpClient.CreateClient("Villa");
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new Uri(apiRequest.Url);
            if (apiRequest.Data != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
                    Encoding.UTF8, "application/json");
            }
            switch (apiRequest.ApiType)
            {
                case StaticDetails.ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;
                case StaticDetails.ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                case StaticDetails.ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            HttpResponseMessage apiResponse = null;

            apiResponse = await client.SendAsync(message);

            string apiContent = await apiResponse.Content.ReadAsStringAsync();

            T APIResponse = JsonConvert.DeserializeObject<T>(apiContent);

            return APIResponse;
        }
        catch (Exception e)
        {
            APIResponse dto = new APIResponse
            {
                ErrorMessages = new List<string> { Convert.ToString(e.Message) },
                IsSucces = false
            };

            string res = JsonConvert.SerializeObject(dto);

            T APIResponse = JsonConvert.DeserializeObject<T>(res);

            return APIResponse;
        }
    }
}

