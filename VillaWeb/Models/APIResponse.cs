using System;
using System.Net;

namespace VillaWeb.Models;

public class APIResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSucces { get; set; } = true;
    public List<string> ErrorMessages { get; set; }
    public object Result { get; set; }

}

