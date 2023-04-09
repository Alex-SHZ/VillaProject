using System;
using System.Net.Http.Headers;
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

            if (!string.IsNullOrEmpty(apiRequest.Token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.Token);
            }

            apiResponse = await client.SendAsync(message);

            string apiContent = await apiResponse.Content.ReadAsStringAsync();

            try
            {
                APIResponse ApiResponse = JsonConvert.DeserializeObject<APIResponse>(apiContent);
                if (ApiResponse != null
                    && apiResponse.StatusCode == System.Net.HttpStatusCode.BadRequest
                    || apiResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ApiResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    ApiResponse.IsSucces = false;

                    string res = JsonConvert.SerializeObject(ApiResponse);

                    T returnObj = JsonConvert.DeserializeObject<T>(res);

                    return returnObj;
                }
            }
            catch (Exception e)
            {
                T exceptionResponse = JsonConvert.DeserializeObject<T>(apiContent);
                return exceptionResponse;
            }

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

