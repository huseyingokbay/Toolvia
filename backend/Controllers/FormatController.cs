using Microsoft.AspNetCore.Mvc;
using Toolvia.API.Models;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace Toolvia.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FormatController : ControllerBase
{
    [HttpPost("json/format")]
    public ActionResult<FormatResponse> FormatJson([FromBody] FormatRequest request)
    {
        try
        {
            var jsonDoc = JsonDocument.Parse(request.Input);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var formatted = JsonSerializer.Serialize(jsonDoc, options);
            return Ok(new FormatResponse(formatted, true));
        }
        catch (JsonException ex)
        {
            return Ok(new FormatResponse("", false, ex.Message));
        }
    }

    [HttpPost("json/minify")]
    public ActionResult<FormatResponse> MinifyJson([FromBody] FormatRequest request)
    {
        try
        {
            var jsonDoc = JsonDocument.Parse(request.Input);
            var minified = JsonSerializer.Serialize(jsonDoc);
            return Ok(new FormatResponse(minified, true));
        }
        catch (JsonException ex)
        {
            return Ok(new FormatResponse("", false, ex.Message));
        }
    }

    [HttpPost("json/validate")]
    public ActionResult<FormatResponse> ValidateJson([FromBody] FormatRequest request)
    {
        try
        {
            JsonDocument.Parse(request.Input);
            return Ok(new FormatResponse(request.Input, true));
        }
        catch (JsonException ex)
        {
            return Ok(new FormatResponse("", false, ex.Message));
        }
    }

    [HttpPost("json/unescape")]
    public ActionResult<FormatResponse> UnescapeJson([FromBody] FormatRequest request)
    {
        try
        {
            var input = request.Input.Trim();
            string unescaped;

            // If wrapped in quotes, it's a JSON string containing escaped JSON
            if ((input.StartsWith("\"") && input.EndsWith("\"")) ||
                (input.StartsWith("'") && input.EndsWith("'")))
            {
                try
                {
                    // Try to parse as JSON string first
                    unescaped = JsonSerializer.Deserialize<string>(input) ?? input;
                }
                catch
                {
                    // Manual unescape if JSON parsing fails
                    unescaped = input[1..^1]
                        .Replace("\\\"", "\"")
                        .Replace("\\'", "'")
                        .Replace("\\\\", "\\")
                        .Replace("\\n", "\n")
                        .Replace("\\r", "\r")
                        .Replace("\\t", "\t");
                }
            }
            else
            {
                // Not wrapped in quotes, just unescape
                unescaped = input
                    .Replace("\\\"", "\"")
                    .Replace("\\'", "'")
                    .Replace("\\\\", "\\")
                    .Replace("\\n", "\n")
                    .Replace("\\r", "\r")
                    .Replace("\\t", "\t");
            }

            // Validate and format the result
            var jsonDoc = JsonDocument.Parse(unescaped);
            var options = new JsonSerializerOptions { WriteIndented = true };
            var formatted = JsonSerializer.Serialize(jsonDoc, options);
            return Ok(new FormatResponse(formatted, true));
        }
        catch (JsonException ex)
        {
            return Ok(new FormatResponse("", false, $"Failed to unescape: {ex.Message}"));
        }
    }

    [HttpPost("xml/format")]
    public ActionResult<FormatResponse> FormatXml([FromBody] FormatRequest request)
    {
        try
        {
            var doc = XDocument.Parse(request.Input);
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = new string(' ', request.IndentSize),
                OmitXmlDeclaration = !request.Input.TrimStart().StartsWith("<?xml")
            };

            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);
            doc.Save(xmlWriter);
            return Ok(new FormatResponse(stringWriter.ToString(), true));
        }
        catch (XmlException ex)
        {
            return Ok(new FormatResponse("", false, ex.Message));
        }
    }

    [HttpPost("xml/minify")]
    public ActionResult<FormatResponse> MinifyXml([FromBody] FormatRequest request)
    {
        try
        {
            var doc = XDocument.Parse(request.Input);
            var settings = new XmlWriterSettings
            {
                Indent = false,
                OmitXmlDeclaration = !request.Input.TrimStart().StartsWith("<?xml")
            };

            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);
            doc.Save(xmlWriter);
            return Ok(new FormatResponse(stringWriter.ToString(), true));
        }
        catch (XmlException ex)
        {
            return Ok(new FormatResponse("", false, ex.Message));
        }
    }

    [HttpPost("xml/validate")]
    public ActionResult<FormatResponse> ValidateXml([FromBody] FormatRequest request)
    {
        try
        {
            XDocument.Parse(request.Input);
            return Ok(new FormatResponse(request.Input, true));
        }
        catch (XmlException ex)
        {
            return Ok(new FormatResponse("", false, ex.Message));
        }
    }
}
