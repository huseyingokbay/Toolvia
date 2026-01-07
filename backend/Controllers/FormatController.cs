using Microsoft.AspNetCore.Mvc;
using Toolvia.API.Models;
using System.Text.Json;
using System.Text.RegularExpressions;
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

    [HttpPost("html/format")]
    public ActionResult<FormatResponse> FormatHtml([FromBody] FormatRequest request)
    {
        try
        {
            var html = request.Input.Trim();
            var indent = new string(' ', request.IndentSize);
            var selfClosingTags = new[] { "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "param", "source", "track", "wbr" };

            // Normalize whitespace between tags
            html = Regex.Replace(html, @">\s+<", "><");

            // Split by tags
            var tokens = Regex.Split(html, @"(<[^>]+>)").Where(t => !string.IsNullOrWhiteSpace(t)).ToList();

            var result = new System.Text.StringBuilder();
            var pad = 0;

            foreach (var token in tokens)
            {
                var isTag = token.StartsWith("<");

                if (isTag)
                {
                    var isClosingTag = token.StartsWith("</");
                    var isSelfClosing = token.EndsWith("/>") ||
                        selfClosingTags.Any(tag => Regex.IsMatch(token, $@"^<{tag}(\s|>|/>)", RegexOptions.IgnoreCase));
                    var isDoctype = token.StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase);
                    var isComment = token.StartsWith("<!--");

                    if (isClosingTag)
                    {
                        pad = Math.Max(0, pad - 1);
                        result.AppendLine(string.Concat(Enumerable.Repeat(indent, pad)) + token);
                    }
                    else if (isSelfClosing || isDoctype || isComment)
                    {
                        result.AppendLine(string.Concat(Enumerable.Repeat(indent, pad)) + token);
                    }
                    else
                    {
                        result.AppendLine(string.Concat(Enumerable.Repeat(indent, pad)) + token);
                        pad++;
                    }
                }
                else
                {
                    var text = token.Trim();
                    if (!string.IsNullOrEmpty(text))
                    {
                        result.AppendLine(string.Concat(Enumerable.Repeat(indent, pad)) + text);
                    }
                }
            }

            return Ok(new FormatResponse(result.ToString().TrimEnd(), true));
        }
        catch (Exception ex)
        {
            return Ok(new FormatResponse("", false, ex.Message));
        }
    }

    [HttpPost("html/minify")]
    public ActionResult<FormatResponse> MinifyHtml([FromBody] FormatRequest request)
    {
        try
        {
            var html = request.Input;

            // Remove newlines, tabs, and excess whitespace
            html = Regex.Replace(html, @"\r\n|\r|\n", "");
            html = Regex.Replace(html, @"\t", "");
            html = Regex.Replace(html, @">\s+<", "><");
            html = Regex.Replace(html, @"\s{2,}", " ");

            return Ok(new FormatResponse(html.Trim(), true));
        }
        catch (Exception ex)
        {
            return Ok(new FormatResponse("", false, ex.Message));
        }
    }
}
