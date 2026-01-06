namespace Toolvia.API.Models;

// Encoding Models
public record EncodeDecodeRequest(string Input, string? Separator = null);
public record EncodeDecodeResponse(string Output, bool Success, string? Error = null);

// Hash Models
public record HashRequest(string Input);
public record HashResponse(string Hash, string Algorithm, int Length);

// Converter Models
public record UnitConvertRequest(double Value, string FromUnit, string ToUnit, string Category);
public record UnitConvertResponse(double Result, string Formula);

public record NumberBaseRequest(string Value, int FromBase, int ToBase);
public record NumberBaseResponse(string Result, string Formatted);

// Format Models
public record FormatRequest(string Input, int IndentSize = 2);
public record FormatResponse(string Output, bool IsValid, string? Error = null);

// Generator Models
public record UuidRequest(int Count = 1, bool Uppercase = false, bool NoDashes = false);
public record UuidResponse(List<string> Uuids);

public record PasswordRequest(
    int Length = 16,
    bool IncludeUppercase = true,
    bool IncludeLowercase = true,
    bool IncludeNumbers = true,
    bool IncludeSymbols = true,
    bool ExcludeAmbiguous = false
);
public record PasswordResponse(string Password, int Strength);

public record LoremRequest(string Type = "paragraphs", int Count = 3, bool StartWithLorem = true);
public record LoremResponse(string Text, int WordCount, int CharCount);

// QR Code Models
public record QrCodeRequest(
    string Content,
    int Size = 256,
    string DarkColor = "#000000",
    string LightColor = "#ffffff",
    string ErrorLevel = "M"
);
public record QrCodeResponse(string DataUrl, string SvgContent);
