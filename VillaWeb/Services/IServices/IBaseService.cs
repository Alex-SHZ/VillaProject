using System;
using VillaWeb.Models;

namespace VillaWeb.Services.IServices;

public interface IBaseService
{
    APIResponse responseModel { get; set; }

    Task<T> SendAsync<T>(APIRequest apiRequest);
}

