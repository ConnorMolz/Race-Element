using System.Drawing;
using System.Drawing.Drawing2D;
using RaceElement.Data.Common;
using RaceElement.Data.Common.SimulatorData;
using RaceElement.Data.Games;
using RaceElement.HUD.Overlay.Configuration;
using RaceElement.HUD.Overlay.Internal;
using RaceElement.HUD.Overlay.OverlayUtil;
using RaceElement.HUD.Overlay.OverlayUtil.InfoPanel;
using RaceElement.HUD.Overlay.Util;

namespace RaceElement.HUD.Common.Overlays.Driving.LapTime;

[Overlay(
    Name = "Lap Time",
    Description = "This Overlay is used to display the current lap time, best lap time and the delta to the best lap time.",
    Authors = ["Connor Molz"],
    Game = Game.Automobilista2  
)]
public class LapTimeOverlay : CommonAbstractOverlay
{
    private readonly LapTimeConfig _config = new();
    private sealed class LapTimeConfig : OverlayConfiguration
    {
        public LapTimeConfig() => GenericConfiguration.AllowRescale = true;
        
        [ConfigGrouping("Behavior", "Configure the behavior of the Lap Time Overlay.")]
        public BehaviorGrouping BehaviorPanel { get; init; } = new BehaviorGrouping();
        public sealed class BehaviorGrouping
        {
            [ToolTip("Hide Overlay in Race")]
            public bool HideInRace { get; init; } = true;
        }
    }

    private Font _font;
    
    private PanelText _sector1Header;
    private PanelText _sector1Value;
    private PanelText _sector1DeltaValue;
    
    private PanelText _sector2Header;
    private PanelText _sector2Value;
    private PanelText _sector2DeltaValue;
    
    private PanelText _sector3Header;
    private PanelText _sector3Value;
    private PanelText _sector3DeltaValue;
    
    private PanelText _lastLapHeader;
    private PanelText _lastLapValue;
    private PanelText _lastLapDeltaValue;
    
    private PanelText _bestLapHeader;
    private PanelText _bestLapValue;

    
    public LapTimeOverlay(Rectangle rectangle) : base(rectangle, "Lap Info")
    {
        RefreshRateHz = 10;
    }

    public sealed override void BeforeStart()
    {
        _font = FontUtil.FontSegoeMono(10f * this.Scale);

        int lineHeight = _font.Height;

        int unscaledHeaderWidth = 66;
        int unscaledValueWidth = 114;
        int unscaledDeltaValueWidth = 66;

        int headerWidth = (int)(unscaledHeaderWidth * this.Scale);
        int valueWidth = (int)(unscaledValueWidth * this.Scale);
        int deltaValueWidth = (int)(unscaledDeltaValueWidth * this.Scale);
        int roundingRadius = (int)(6 * this.Scale);

        RectangleF headerRect = new(0, 0, headerWidth, lineHeight);
        RectangleF valueRect = new(headerWidth, 0, valueWidth, lineHeight);
        RectangleF deltaValueRect = new(headerWidth + valueWidth, 0, deltaValueWidth, lineHeight);
        StringFormat headerFormat = new() { Alignment = StringAlignment.Near };
        StringFormat valueFormat = new() { Alignment = StringAlignment.Far };
        StringFormat deltaValueFormat = new() { Alignment = StringAlignment.Far };

        Color accentColor = Color.FromArgb(25, 255, 0, 0);
        CachedBitmap headerBackground = new(headerWidth, lineHeight, g =>
        {
            Rectangle panelRect = new(0, 0, headerWidth, lineHeight);
            using GraphicsPath path = GraphicsExtensions.CreateRoundedRectangle(panelRect, 0, 0, 0, roundingRadius);
            using LinearGradientBrush brush = new(panelRect, Color.FromArgb(185, 0, 0, 0), Color.FromArgb(255, 10, 10, 10), LinearGradientMode.BackwardDiagonal);
            g.FillPath(brush, path);
            using Pen underlinePen = new(accentColor);
            g.DrawLine(underlinePen, 0 + roundingRadius / 2, lineHeight, headerWidth, lineHeight - 1);
        });
        CachedBitmap valueBackground = new(valueWidth, lineHeight, g =>
        {
            Rectangle panelRect = new(0, 0, valueWidth, lineHeight);
            using GraphicsPath path = GraphicsExtensions.CreateRoundedRectangle(panelRect, 0, roundingRadius, 0, 0);
            using LinearGradientBrush brush = new(panelRect, Color.FromArgb(255, 0, 0, 0), Color.FromArgb(185, 0, 0, 0), LinearGradientMode.ForwardDiagonal);
            g.FillPath(brush, path);
            using Pen underlinePen = new(accentColor);
            g.DrawLine(underlinePen, 0, lineHeight - 1, valueWidth, lineHeight - 1);
        });
        CachedBitmap valueDeltaBackground = new(deltaValueWidth, lineHeight, g =>
        {
            Rectangle panelRect = new(0, 0, valueWidth, lineHeight);
            using GraphicsPath path = GraphicsExtensions.CreateRoundedRectangle(panelRect, 0, roundingRadius, 0, 0);
            using LinearGradientBrush brush = new(panelRect, Color.FromArgb(255, 0, 0, 0), Color.FromArgb(185, 0, 0, 0),
                LinearGradientMode.ForwardDiagonal);
            g.FillPath(brush, path);
            using Pen underlinePen = new(accentColor);
            g.DrawLine(underlinePen, 0, lineHeight - 1, valueWidth, lineHeight - 1);
        });
    }

    public sealed override void BeforeStop()
    {
        
    }
    
    public override bool ShouldRender()
    {
        if(_config.BehaviorPanel.HideInRace && SimDataProvider.Session.SessionType == RaceSessionType.Race)
        {
            return false;
        }
        return base.ShouldRender();
    }
    public sealed override void Render(Graphics g)
    {
        
    }

}