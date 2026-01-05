using Microsoft.AspNetCore.Mvc;
using MultiTools.API.Models;
using System.Security.Cryptography;
using System.Text;

namespace MultiTools.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneratorController : ControllerBase
{
    [HttpPost("uuid")]
    public ActionResult<UuidResponse> GenerateUuid([FromBody] UuidRequest request)
    {
        var uuids = new List<string>();

        for (int i = 0; i < request.Count; i++)
        {
            var uuid = Guid.NewGuid().ToString();

            if (request.NoDashes)
                uuid = uuid.Replace("-", "");

            if (request.Uppercase)
                uuid = uuid.ToUpper();

            uuids.Add(uuid);
        }

        return Ok(new UuidResponse(uuids));
    }

    [HttpPost("password")]
    public ActionResult<PasswordResponse> GeneratePassword([FromBody] PasswordRequest request)
    {
        var charSets = new StringBuilder();

        if (request.IncludeUppercase)
            charSets.Append(request.ExcludeAmbiguous ? "ABCDEFGHJKLMNPQRSTUVWXYZ" : "ABCDEFGHIJKLMNOPQRSTUVWXYZ");

        if (request.IncludeLowercase)
            charSets.Append(request.ExcludeAmbiguous ? "abcdefghjkmnpqrstuvwxyz" : "abcdefghijklmnopqrstuvwxyz");

        if (request.IncludeNumbers)
            charSets.Append(request.ExcludeAmbiguous ? "23456789" : "0123456789");

        if (request.IncludeSymbols)
            charSets.Append("!@#$%^&*()_+-=[]{}|;:,.<>?");

        if (charSets.Length == 0)
            return BadRequest("At least one character set must be selected");

        var chars = charSets.ToString();
        var password = new StringBuilder();

        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[request.Length];
        rng.GetBytes(bytes);

        for (int i = 0; i < request.Length; i++)
        {
            password.Append(chars[bytes[i] % chars.Length]);
        }

        var strength = CalculatePasswordStrength(password.ToString());

        return Ok(new PasswordResponse(password.ToString(), strength));
    }

    private static int CalculatePasswordStrength(string password)
    {
        int score = 0;

        if (password.Length >= 8) score++;
        if (password.Length >= 12) score++;
        if (password.Length >= 16) score++;
        if (password.Any(char.IsLower)) score++;
        if (password.Any(char.IsUpper)) score++;
        if (password.Any(char.IsDigit)) score++;
        if (password.Any(c => !char.IsLetterOrDigit(c))) score++;

        return score switch
        {
            <= 2 => 1,
            <= 4 => 2,
            <= 5 => 3,
            _ => 4
        };
    }

    [HttpPost("lorem")]
    public ActionResult<LoremResponse> GenerateLorem([FromBody] LoremRequest request)
    {
        var words = new[]
        {
            "lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit",
            "sed", "do", "eiusmod", "tempor", "incididunt", "ut", "labore", "et", "dolore",
            "magna", "aliqua", "enim", "ad", "minim", "veniam", "quis", "nostrud",
            "exercitation", "ullamco", "laboris", "nisi", "aliquip", "ex", "ea", "commodo",
            "consequat", "duis", "aute", "irure", "in", "reprehenderit", "voluptate"
        };

        var random = new Random();

        string GenerateSentence(int? wordCount = null)
        {
            var count = wordCount ?? random.Next(5, 15);
            var sentence = string.Join(" ", Enumerable.Range(0, count).Select(_ => words[random.Next(words.Length)]));
            return char.ToUpper(sentence[0]) + sentence[1..] + ".";
        }

        string GenerateParagraph()
        {
            var sentenceCount = random.Next(3, 7);
            return string.Join(" ", Enumerable.Range(0, sentenceCount).Select(_ => GenerateSentence()));
        }

        var result = request.Type.ToLower() switch
        {
            "words" => string.Join(" ", Enumerable.Range(0, request.Count).Select(_ => words[random.Next(words.Length)])),
            "sentences" => string.Join(" ", Enumerable.Range(0, request.Count).Select(_ => GenerateSentence())),
            _ => string.Join("\n\n", Enumerable.Range(0, request.Count).Select(_ => GenerateParagraph()))
        };

        if (request.StartWithLorem)
        {
            var loremStart = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            result = loremStart + " " + result;
        }

        var wordCountResult = result.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

        return Ok(new LoremResponse(result, wordCountResult, result.Length));
    }
}
