using System;
using static VillaUtility.StaticDetails;

namespace VillaWeb.Models;

public class APIRequest
{
    public ApiType ApiType { get; set; } = ApiType.GET;

    public string Url { get; set; }
    public object Data { get; set; }
}

