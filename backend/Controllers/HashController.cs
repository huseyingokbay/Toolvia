using Microsoft.AspNetCore.Mvc;
using Toolvia.API.Models;
using System.Security.Cryptography;
using System.Text;

namespace Toolvia.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HashController : ControllerBase
{
    // Text hashing endpoints
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

    [HttpPost("sha224")]
    public ActionResult<HashResponse> Sha224([FromBody] HashRequest request)
    {
        var bytes = Encoding.UTF8.GetBytes(request.Input);
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(bytes);
        // SHA-224 is SHA-256 truncated to 224 bits (28 bytes)
        var hashString = Convert.ToHexStringLower(hash[..28]);
        return Ok(new HashResponse(hashString, "SHA-224", 56));
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

    [HttpPost("sha3-256")]
    public ActionResult<HashResponse> Sha3_256([FromBody] HashRequest request)
    {
        var bytes = Encoding.UTF8.GetBytes(request.Input);
        var hash = SHA3_256.HashData(bytes);
        var hashString = Convert.ToHexStringLower(hash);
        return Ok(new HashResponse(hashString, "SHA3-256", 64));
    }

    [HttpPost("sha3-384")]
    public ActionResult<HashResponse> Sha3_384([FromBody] HashRequest request)
    {
        var bytes = Encoding.UTF8.GetBytes(request.Input);
        var hash = SHA3_384.HashData(bytes);
        var hashString = Convert.ToHexStringLower(hash);
        return Ok(new HashResponse(hashString, "SHA3-384", 96));
    }

    [HttpPost("sha3-512")]
    public ActionResult<HashResponse> Sha3_512([FromBody] HashRequest request)
    {
        var bytes = Encoding.UTF8.GetBytes(request.Input);
        var hash = SHA3_512.HashData(bytes);
        var hashString = Convert.ToHexStringLower(hash);
        return Ok(new HashResponse(hashString, "SHA3-512", 128));
    }

    // File hashing endpoint
    [HttpPost("file")]
    public async Task<ActionResult<HashResponse>> HashFile(IFormFile file, [FromQuery] string algorithm = "sha256")
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "No file provided" });

        using var stream = file.OpenReadStream();
        byte[] hash;
        string algorithmName;
        int hashLength;

        switch (algorithm.ToLower())
        {
            case "md5":
                hash = await MD5.HashDataAsync(stream);
                algorithmName = "MD5";
                hashLength = 32;
                break;
            case "sha1":
                hash = await SHA1.HashDataAsync(stream);
                algorithmName = "SHA-1";
                hashLength = 40;
                break;
            case "sha256":
                hash = await SHA256.HashDataAsync(stream);
                algorithmName = "SHA-256";
                hashLength = 64;
                break;
            case "sha384":
                hash = await SHA384.HashDataAsync(stream);
                algorithmName = "SHA-384";
                hashLength = 96;
                break;
            case "sha512":
                hash = await SHA512.HashDataAsync(stream);
                algorithmName = "SHA-512";
                hashLength = 128;
                break;
            case "sha3-256":
                hash = await SHA3_256.HashDataAsync(stream);
                algorithmName = "SHA3-256";
                hashLength = 64;
                break;
            case "sha3-384":
                hash = await SHA3_384.HashDataAsync(stream);
                algorithmName = "SHA3-384";
                hashLength = 96;
                break;
            case "sha3-512":
                hash = await SHA3_512.HashDataAsync(stream);
                algorithmName = "SHA3-512";
                hashLength = 128;
                break;
            default:
                return BadRequest(new { error = $"Unknown algorithm: {algorithm}" });
        }

        var hashString = Convert.ToHexStringLower(hash);
        return Ok(new HashResponse(hashString, algorithmName, hashLength));
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
            ["sha512"] = new HashResponse(Convert.ToHexStringLower(SHA512.HashData(bytes)), "SHA-512", 128),
            ["sha3-256"] = new HashResponse(Convert.ToHexStringLower(SHA3_256.HashData(bytes)), "SHA3-256", 64),
            ["sha3-384"] = new HashResponse(Convert.ToHexStringLower(SHA3_384.HashData(bytes)), "SHA3-384", 96),
            ["sha3-512"] = new HashResponse(Convert.ToHexStringLower(SHA3_512.HashData(bytes)), "SHA3-512", 128)
        };

        return Ok(result);
    }
}
