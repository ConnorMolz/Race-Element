using RaceElement.Data.Games;
using RaceElement.HUD.Overlay.Configuration;

namespace RaceElement.HUD.Common.Overlays.Pitwall.DSX;

internal sealed class DsxConfiguration : OverlayConfiguration
{
    public DsxConfiguration()
    {
        GenericConfiguration.AlwaysOnTop = false;
        GenericConfiguration.Window = false;
        GenericConfiguration.Opacity = 1.0f;
        GenericConfiguration.AllowRescale = false;
    }

    [ConfigGrouping("DSX UDP", "Adjust the port DSX uses, 6969 is default.")]
    public UdpConfig UDP { get; init; } = new UdpConfig();
    public sealed class UdpConfig
    {
        [ToolTip("Adjust the port used by DSX, 6969 is default.")]
        [IntRange(0, 65535, 1)]
        public int Port { get; init; } = 6969;
    }

    [ConfigGrouping("Brake Slip", "Adjust the slip effect whilst applying the brakes.")]
    public BrakeSlipHaptics BrakeSlip { get; init; } = new();
    public sealed class BrakeSlipHaptics
    {
        /// <summary>
        /// The brake in percentage (divide by 100f if you want 0-1 value)
        /// </summary>
        [ToolTip("The minimum brake percentage before any effects are applied. See this like a deadzone.")]
        [FloatRange(0.1f, 99f, 0.1f, 1)]
        public float BrakeThreshold { get; init; } = 3f;

        [ToolTip("Sets the frequency of the slip effect whilst applying the brakes.")]
        [IntRange(10, 150, 1)]
        public int Frequency { get; init; } = 85;

        [FloatRange(0.05f, 6f, 0.01f, 2)]
        public float FrontSlipThreshold { get; init; } = 0.7f;

        [FloatRange(0.05f, 6f, 0.01f, 2)]
        public float RearSlipThreshold { get; init; } = 0.6f;
    }

    [ConfigGrouping("Throttle Slip", "Adjust the slip effect whilst applying the throttle.\nModify the threshold to increase or decrease sensitivity in different situations.")]
    public ThrottleSlipHaptics ThrottleSlip { get; init; } = new();
    public sealed class ThrottleSlipHaptics
    {
        /// <summary>
        /// The throttle in percentage (divide by 100f if you want 0-1 value)
        /// </summary>
        [ToolTip("The minimum throttle percentage before any effects are applied. See this like a deadzone.")]
        [FloatRange(0.1f, 99f, 0.1f, 1)]
        public float ThrottleThreshold { get; init; } = 3f;

        [ToolTip("Sets the frequency of the slip effect whilst applying the throttle.")]
        [IntRange(10, 150, 1)]
        public int Frequency { get; init; } = 85;

        [ToolTip("Decrease this treshold to increase the sensitivity when the front wheels slip (understeer).")]
        [FloatRange(0.05f, 6f, 0.01f, 2)]
        public float FrontSlipThreshold { get; init; } = 0.6f;

        [ToolTip("Decrease this treshold to increase the sensitivity when the rear wheels slip (oversteer).")]
        [FloatRange(0.05f, 10f, 0.01f, 2)]
        public float RearSlipThreshold { get; init; } = 0.5f;
    }
}
