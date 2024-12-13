using RaceElement.HUD.Overlay.Configuration;

namespace RaceElement.HUD.ACC.Overlays.Pitwall.LowFuelMotorsport;

internal sealed class LowFuelMotorsportConfiguration : OverlayConfiguration
{
    public enum FontFamilyConfig
    {
        SegoeMono,
        Conthrax,
        Orbitron,
        Roboto,
    }

    public LowFuelMotorsportConfiguration() => GenericConfiguration.AllowRescale = false;

    [ConfigGrouping("Connection", "LFM user information and fetch interval")]
    public ConnectionGrouping Connection { get; init; } = new();
    public sealed class ConnectionGrouping
    {
        [ToolTip("User identifier (https://lowfuelmotorsport.com/profile/[HERE_IS_THE_ID])")]
        public string User { get; init; } = "";

        [ToolTip("Server fetch interval in seconds")]
        [IntRange(30, 120, 10)]
        public int Interval { get; init; } = 30;
    }

    [ConfigGrouping("Font", "Font configuration")]
    public FontGrouping Font { get; init; } = new();
    public sealed class FontGrouping
    {
        [ToolTip("Font family")]
        public FontFamilyConfig FontFamily { get; init; } = FontFamilyConfig.Roboto;

        [ToolTip("Font size")]
        [IntRange(5, 32, 1)]
        public int Size { get; init; } = 10;
    }

    [ConfigGrouping("Others", "Other options")]
    public OtherGrouping Others { get; init; } = new();
    public sealed class OtherGrouping
    {
        [ToolTip("Show always")]
        public bool ShowAlways { get; init; } = false;

        [ToolTip("Enables speech warnings ahead of the race.\n5 Minutes ahead, 3 minutes ahead and 1 minute ahead.")]
        public bool SpeechWarnings { get; init; } = false;

    }
}
