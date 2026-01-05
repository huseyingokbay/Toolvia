using Microsoft.AspNetCore.Mvc;
using MultiTools.API.Models;
using System.Security.Cryptography;
using System.Text;

namespace MultiTools.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HashController : ControllerBase
{
    [HttpPost("md5")]
    public ActionResult<HashResponse> Md5([FromBody] HashRequest request)
    {
        var bytes = Encoding.UTF8.GetBytes(request.Input);
        var hash = MD5.HashData(bytes);
        var hashString = Convert.ToHexStringLower(hash);
        return Ok(new HashResponse(hashString, "MD5", 32));
    }

    [HttpPost("sha1")]
    public ActionResult<HashResponse> Sha1([FromBody] HashRequest request)
    {
        var bytes = Encoding.UTF8.GetBytes(request.Input);
        var hash = SHA1.HashData(bytes);
        var hashString = Convert.ToHexStringLower(hash);
        return Ok(new HashResponse(hashString, "SHA-1", 40));
    }

    [HttpPost("sha256")]
    public ActionResult<HashResponse> Sha256([FromBody] HashRequest request)
    {
        var bytes = Encoding.UTF8.GetBytes(request.Input);
        var hash = SHA256.HashData(bytes);
        var hashString = Convert.ToHexStringLower(hash);
        return Ok(new HashResponse(hashString, "SHA-256", 64));
    }

    [HttpPost("sha384")]
    public ActionResult<HashResponse> Sha384([FromBody] HashRequest request)
    {
        var bytes = Encoding.UTF8.GetBytes(request.Input);
        var hash = SHA384.HashData(bytes);
        var hashString = Convert.ToHexStringLower(hash);
        return Ok(new HashResponse(hashString, "SHA-384", 96));
    }

    [HttpPost("sha512")]
    public ActionResult<HashResponse> Sha512([FromBody] HashRequest request)
    {
        var bytes = Encoding.UTF8.GetBytes(request.Input);
        var hash = SHA512.HashData(bytes);
        var hashString = Convert.ToHexStringLower(hash);
        return Ok(new HashResponse(hashString, "SHA-512", 128));
    }

    [HttpPost("all")]
    public ActionResult<Dictionary<string, HashResponse>> AllHashes([FromBody] HashRequest request)
    {
        var bytes = Encoding.UTF8.GetBytes(request.Input);

        var result = new Dictionary<string, HashResponse>
        {
            ["md5"] = new HashResponse(Convert.ToHexStringLower(MD5.HashData(bytes)), "MD5", 32),
            ["sha1"] = new HashResponse(Convert.ToHexStringLower(SHA1.HashData(bytes)), "SHA-1", 40),
            ["sha256"] = new HashResponse(Convert.ToHexStringLower(SHA256.HashData(bytes)), "SHA-256", 64),
            ["sha384"] = new HashResponse(Convert.ToHexStringLower(SHA384.HashData(bytes)), "SHA-384", 96),
            ["sha512"] = new HashResponse(Convert.ToHexStringLower(SHA512.HashData(bytes)), "SHA-512", 128)
        };

        return Ok(result);
    }
}
