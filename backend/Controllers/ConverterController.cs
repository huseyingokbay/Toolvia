using Microsoft.AspNetCore.Mvc;
using MultiTools.API.Models;

namespace MultiTools.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConverterController : ControllerBase
{
    private static readonly Dictionary<string, Dictionary<string, double>> UnitFactors = new()
    {
        ["length"] = new()
        {
            ["mm"] = 0.001, ["cm"] = 0.01, ["m"] = 1, ["km"] = 1000,
            ["in"] = 0.0254, ["ft"] = 0.3048, ["yd"] = 0.9144, ["mi"] = 1609.344
        },
        ["weight"] = new()
        {
            ["mg"] = 0.000001, ["g"] = 0.001, ["kg"] = 1, ["t"] = 1000,
            ["oz"] = 0.0283495, ["lb"] = 0.453592
        },
        ["area"] = new()
        {
            ["mm2"] = 0.000001, ["cm2"] = 0.0001, ["m2"] = 1, ["km2"] = 1000000,
            ["ha"] = 10000, ["ac"] = 4046.86, ["ft2"] = 0.092903
        },
        ["volume"] = new()
        {
            ["ml"] = 0.001, ["l"] = 1, ["m3"] = 1000,
            ["gal"] = 3.78541, ["qt"] = 0.946353, ["pt"] = 0.473176,
            ["cup"] = 0.236588, ["floz"] = 0.0295735
        },
        ["data"] = new()
        {
            ["b"] = 0.125, ["B"] = 1, ["KB"] = 1024, ["MB"] = 1048576,
            ["GB"] = 1073741824, ["TB"] = 1099511627776
        },
        ["time"] = new()
        {
            ["ms"] = 0.001, ["s"] = 1, ["min"] = 60, ["h"] = 3600,
            ["d"] = 86400, ["wk"] = 604800, ["mo"] = 2629746, ["yr"] = 31556952
        }
    };

    [HttpPost("unit")]
    public ActionResult<UnitConvertResponse> ConvertUnit([FromBody] UnitConvertRequest request)
    {
        var category = request.Category.ToLower();

        if (category == "temperature")
        {
            var result = ConvertTemperature(request.Value, request.FromUnit.ToLower(), request.ToUnit.ToLower());
            return Ok(new UnitConvertResponse(result, $"{request.Value} {request.FromUnit} = {result:F6} {request.ToUnit}"));
        }

        if (!UnitFactors.TryGetValue(category, out var factors))
            return BadRequest($"Unknown category: {category}");

        if (!factors.TryGetValue(request.FromUnit, out var fromFactor))
            return BadRequest($"Unknown unit: {request.FromUnit}");

        if (!factors.TryGetValue(request.ToUnit, out var toFactor))
            return BadRequest($"Unknown unit: {request.ToUnit}");

        var result2 = (request.Value * fromFactor) / toFactor;
        return Ok(new UnitConvertResponse(result2, $"{request.Value} {request.FromUnit} = {result2:F6} {request.ToUnit}"));
    }

    private static double ConvertTemperature(double value, string from, string to)
    {
        double celsius = from switch
        {
            "f" => (value - 32) * 5 / 9,
            "k" => value - 273.15,
            _ => value
        };

        return to switch
        {
            "f" => celsius * 9 / 5 + 32,
            "k" => celsius + 273.15,
            _ => celsius
        };
    }

    [HttpPost("number-base")]
    public ActionResult<NumberBaseResponse> ConvertNumberBase([FromBody] NumberBaseRequest request)
    {
        try
        {
            var decimalValue = Convert.ToInt64(request.Value, request.FromBase);
            var result = Convert.ToString(decimalValue, request.ToBase).ToUpper();

            var formatted = request.ToBase switch
            {
                2 => string.Join(" ", Enumerable.Range(0, (result.Length + 3) / 4)
                    .Select(i => result.PadLeft(((result.Length + 3) / 4) * 4, '0')
                        .Substring(i * 4, 4))),
                16 => string.Join(" ", Enumerable.Range(0, (result.Length + 1) / 2)
                    .Select(i => result.PadLeft(((result.Length + 1) / 2) * 2, '0')
                        .Substring(i * 2, 2))),
                _ => result
            };

            return Ok(new NumberBaseResponse(result, formatted));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("case")]
    public ActionResult<Dictionary<string, string>> ConvertCase([FromBody] HashRequest request)
    {
        var input = request.Input;

        var results = new Dictionary<string, string>
        {
            ["lowercase"] = input.ToLower(),
            ["uppercase"] = input.ToUpper(),
            ["titlecase"] = ToTitleCase(input),
            ["sentencecase"] = ToSentenceCase(input),
            ["camelcase"] = ToCamelCase(input),
            ["pascalcase"] = ToPascalCase(input),
            ["snakecase"] = ToSnakeCase(input),
            ["kebabcase"] = ToKebabCase(input),
            ["constantcase"] = ToConstantCase(input)
        };

        return Ok(results);
    }

    private static string ToTitleCase(string input) =>
        System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());

    private static string ToSentenceCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        var result = input.ToLower();
        return char.ToUpper(result[0]) + result[1..];
    }

    private static string ToCamelCase(string input)
    {
        var words = SplitIntoWords(input);
        if (words.Length == 0) return "";
        return words[0].ToLower() + string.Concat(words.Skip(1).Select(w => char.ToUpper(w[0]) + w[1..].ToLower()));
    }

    private static string ToPascalCase(string input)
    {
        var words = SplitIntoWords(input);
        return string.Concat(words.Select(w => char.ToUpper(w[0]) + w[1..].ToLower()));
    }

    private static string ToSnakeCase(string input) =>
        string.Join("_", SplitIntoWords(input).Select(w => w.ToLower()));

    private static string ToKebabCase(string input) =>
        string.Join("-", SplitIntoWords(input).Select(w => w.ToLower()));

    private static string ToConstantCase(string input) =>
        string.Join("_", SplitIntoWords(input).Select(w => w.ToUpper()));

    private static string[] SplitIntoWords(string input)
    {
        return System.Text.RegularExpressions.Regex
            .Replace(input, "([a-z])([A-Z])", "$1 $2")
            .Split(new[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(w => !string.IsNullOrWhiteSpace(w))
            .ToArray();
    }
}
