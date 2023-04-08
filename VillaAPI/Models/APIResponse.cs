using System;
using System.Net;

namespace VillaAPI.Models;

public class APIResponse
{
    public APIResponse()
    {
        ErrorMessages = new List<string>();
    }

    public HttpStatusCode StatucCode { get; set; }
    public bool IsSucces { get; set; } = true;
    public List<string> ErrorMessages { get; set; } 
    public object Result { get; set; }

}

