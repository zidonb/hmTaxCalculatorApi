namespace TaxCalculator.Shared.Settings;

public sealed class ImpersonateSettings {
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public int Logon32ProviderDefault { get; set; }
    public int Logon32LogonInteractive { get; set; }
}