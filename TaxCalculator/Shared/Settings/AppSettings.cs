namespace TaxCalculator.Shared.Settings;

public class AppSettings {
    [Required]
    public required string AppEnvironment { get; set; }
    [Required]
    public required bool IsProduction { get; set; }
    [Required]
    public required string ProjectName { get; set; }
    [Required]
    public required int SwaggerSchemaVersion { get; set; }
    public required string BaseUrl { get; set; }
    public required int ApiVersion { get; set; }
}
