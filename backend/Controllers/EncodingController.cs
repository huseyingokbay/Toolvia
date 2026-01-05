using Microsoft.AspNetCore.Mvc;
using Toolvia.API.Models;
using System.Text;
using System.Web;

namespace Toolvia.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EncodingController : ControllerBase
{
    [HttpPost("base64/encode")]
    public ActionResult<EncodeDecodeResponse> Base64Encode([FromBody] EncodeDecodeRequest request)
    {
        try
        {
            var bytes = Encoding.UTF8.GetBytes(request.Input);
            var encoded = Convert.ToBase64String(bytes);
            return Ok(new EncodeDecodeResponse(encoded, true));
        }
        catch (Exception ex)
        {
            return Ok(new EncodeDecodeResponse("", false, ex.Message));
        }
    }

    [HttpPost("base64/decode")]
    public ActionResult<EncodeDecodeResponse> Base64Decode([FromBody] EncodeDecodeRequest request)
    {
        try
        {
            var bytes = Convert.FromBase64String(request.Input);
            var decoded = Encoding.UTF8.GetString(bytes);
            return Ok(new EncodeDecodeResponse(decoded, true));
        }
        catch (Exception ex)
        {
            return Ok(new EncodeDecodeResponse("", false, ex.Message));
        }
    }

    [HttpPost("hex/encode")]
    public ActionResult<EncodeDecodeResponse> HexEncode([FromBody] EncodeDecodeRequest request)
    {
        try
        {
            var bytes = Encoding.UTF8.GetBytes(request.Input);
            var separator = request.Separator ?? " ";
            var hex = string.Join(separator, bytes.Select(b => b.ToString("x2")));
            return Ok(new EncodeDecodeResponse(hex, true));
        }
        catch (Exception ex)
        {
            return Ok(new EncodeDecodeResponse("", false, ex.Message));
        }
    }

    [HttpPost("hex/decode")]
    public ActionResult<EncodeDecodeResponse> HexDecode([FromBody] EncodeDecodeRequest request)
    {
        try
        {
            var cleanHex = System.Text.RegularExpressions.Regex.Replace(request.Input, @"[\s,;:\-]", "");
            var bytes = Enumerable.Range(0, cleanHex.Length / 2)
                .Select(i => Convert.ToByte(cleanHex.Substring(i * 2, 2), 16))
                .ToArray();
            var decoded = Encoding.UTF8.GetString(bytes);
            return Ok(new EncodeDecodeResponse(decoded, true));
        }
        catch (Exception ex)
        {
            return Ok(new EncodeDecodeResponse("", false, ex.Message));
        }
    }

    [HttpPost("url/encode")]
    public ActionResult<EncodeDecodeResponse> UrlEncode([FromBody] EncodeDecodeRequest request)
    {
        try
        {
            var encoded = Uri.EscapeDataString(request.Input);
            return Ok(new EncodeDecodeResponse(encoded, true));
        }
        catch (Exception ex)
        {
            return Ok(new EncodeDecodeResponse("", false, ex.Message));
        }
    }

    [HttpPost("url/decode")]
    public ActionResult<EncodeDecodeResponse> UrlDecode([FromBody] EncodeDecodeRequest request)
    {
        try
        {
            var decoded = Uri.UnescapeDataString(request.Input);
            return Ok(new EncodeDecodeResponse(decoded, true));
        }
        catch (Exception ex)
        {
            return Ok(new EncodeDecodeResponse("", false, ex.Message));
        }
    }

    [HttpPost("html/encode")]
    public ActionResult<EncodeDecodeResponse> HtmlEncode([FromBody] EncodeDecodeRequest request)
    {
        try
        {
            var encoded = HttpUtility.HtmlEncode(request.Input);
            return Ok(new EncodeDecodeResponse(encoded, true));
        }
        catch (Exception ex)
        {
            return Ok(new EncodeDecodeResponse("", false, ex.Message));
        }
    }

    [HttpPost("html/decode")]
    public ActionResult<EncodeDecodeResponse> HtmlDecode([FromBody] EncodeDecodeRequest request)
    {
        try
        {
            var decoded = HttpUtility.HtmlDecode(request.Input);
            return Ok(new EncodeDecodeResponse(decoded, true));
        }
        catch (Exception ex)
        {
            return Ok(new EncodeDecodeResponse("", false, ex.Message));
        }
    }
}
