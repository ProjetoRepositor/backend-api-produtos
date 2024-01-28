using System.Net;
using App.Models;

namespace App.Util;

public class Cosmos : WebClient
{
    protected override WebRequest GetWebRequest(Uri uri)
    {
        
        var request = base.GetWebRequest(uri);
        request.Headers["User-Agent"] = "Cosmos-API-Request";
        request.Headers["X-Cosmos-Token"] = "dFR4oQ-1Nc4z9n_mmJ7NWw";
        Encoding = System.Text.Encoding.UTF8;
        return request;
    }
}