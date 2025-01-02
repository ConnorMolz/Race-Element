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
        [ConfigGrouping("Info Panel", "Show or hide additional information in the panel.")]
        public InfoPanelGrouping InfoPanel { get; init; } = new InfoPanelGrouping();
        
        [ConfigGrouping("Behavior", "Configure the behavior of the Lap Time Overlay.")]
        public BehaviorGrouping BehaviorPanel { get; init; } = new BehaviorGrouping();
        
        public sealed class InfoPanelGrouping
        {
            [ToolTip("Show Sector Times")]
            public bool SectorTimes { get; init; } = true;
            
            [ToolTip("Show Last Lap Time")]
            public bool LastLapTime { get; init; } = true;
            
            [ToolTip("Show Best Lap Time")]
            public bool BestLapTime { get; init; } = true;
            
            [ToolTip("Show Deltas to best lap time")]
            public bool DeltaToBest { get; init; } = true;
        }
        public sealed class BehaviorGrouping
        {
            [ToolTip("Hide Overlay in Race")]
            public bool HideInRace { get; init; } = false;
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

        if (_config.InfoPanel.SectorTimes)
        {
            _sector1Header = new PanelText(_font, headerBackground, headerRect) { StringFormat = headerFormat };
            _sector1Value = new PanelText(_font, valueBackground, valueRect) { StringFormat = valueFormat };
            if (_config.InfoPanel.DeltaToBest)
            {
                _sector1DeltaValue = new PanelText(_font, valueDeltaBackground, deltaValueRect) { StringFormat = deltaValueFormat };
            }
            
            _sector2Header = new PanelText(_font, headerBackground, headerRect) { StringFormat = headerFormat };
            _sector2Value = new PanelText(_font, valueBackground, valueRect) { StringFormat = valueFormat };
            if (_config.InfoPanel.DeltaToBest)
            {
                _sector2DeltaValue = new PanelText(_font, valueDeltaBackground, deltaValueRect) { StringFormat = deltaValueFormat };
            }
            
            _sector3Header = new PanelText(_font, headerBackground, headerRect) { StringFormat = headerFormat };
            _sector3Value = new PanelText(_font, valueBackground, valueRect) { StringFormat = valueFormat };
            if (_config.InfoPanel.DeltaToBest)
            {
                _sector3DeltaValue = new PanelText(_font, valueDeltaBackground, deltaValueRect) { StringFormat = deltaValueFormat };
            }
        }

        if (_config.InfoPanel.LastLapTime)
        {
            _lastLapHeader = new PanelText(_font, headerBackground, headerRect) { StringFormat = headerFormat };
            _lastLapValue = new PanelText(_font, valueBackground, valueRect) { StringFormat = valueFormat };
            if (_config.InfoPanel.DeltaToBest)
            {
                _lastLapDeltaValue = new PanelText(_font, valueDeltaBackground, deltaValueRect) { StringFormat = deltaValueFormat };
            }
        }

        if (_config.InfoPanel.BestLapTime)
        {
            _bestLapHeader = new PanelText(_font, headerBackground, headerRect) { StringFormat = headerFormat };
            _bestLapValue = new PanelText(_font, valueBackground, valueRect) { StringFormat = valueFormat };
        }
    }

    public sealed override void BeforeStop()
    {
        _sector1Header?.Dispose();
        _sector1Value?.Dispose();
        _sector1DeltaValue?.Dispose();
        
        _sector2Header?.Dispose();
        _sector2Value?.Dispose();
        _sector2DeltaValue?.Dispose();
        
        _sector3Header?.Dispose();
        _sector3Value?.Dispose();
        _sector3DeltaValue?.Dispose();
        
        _lastLapHeader?.Dispose();
        _lastLapValue?.Dispose();
        _lastLapDeltaValue?.Dispose();
        
        _bestLapHeader?.Dispose();
        _bestLapValue?.Dispose();
        
        _font?.Dispose();
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
        int playerId = SimDataProvider.Session.PlayerCarIndex;
        if (_config.InfoPanel.SectorTimes)
        {
            int? sector1 = SimDataProvider.Session.Cars[playerId].Value.CurrentLap.Splits[0];
            int? bestSector1 = SimDataProvider.Session.Cars[playerId].Value.FastestLap.Splits[0];
            _sector1Header.Draw(g,"S1", this.Scale);
            _sector1Value.Draw(g, sector1.HasValue ? TimeSpan.FromMilliseconds(sector1.Value).ToString("mm\\:ss\\.fff") : "--:--.---", this.Scale);
            
            if(_config.InfoPanel.DeltaToBest)
            {
                _sector1DeltaValue.Draw(g, bestSector1.HasValue ? TimeSpan.FromMilliseconds(sector1.Value - bestSector1.Value).ToString("mm\\:ss\\.fff") : "--:--.---", this.Scale);
            }        
        }
        
        if (_config.InfoPanel.SectorTimes)
        {
            int? sector2 = SimDataProvider.Session.Cars[playerId].Value.CurrentLap.Splits[1];
            int? bestSector2 = SimDataProvider.Session.Cars[playerId].Value.FastestLap.Splits[1];
            _sector2Header.Draw(g,"S2", this.Scale);
            _sector2Value.Draw(g, sector2.HasValue ? TimeSpan.FromMilliseconds(sector2.Value).ToString("mm\\:ss\\.fff") : "--:--.---", this.Scale);
            
            if(_config.InfoPanel.DeltaToBest)
            {
                _sector2DeltaValue.Draw(g, bestSector2.HasValue ? TimeSpan.FromMilliseconds(sector2.Value - bestSector2.Value).ToString("mm\\:ss\\.fff") : "--:--.---", this.Scale);
            }        
        }
        
        if (_config.InfoPanel.SectorTimes)
        {
            int? sector3 = SimDataProvider.Session.Cars[playerId].Value.CurrentLap.Splits[2];
            int? bestSector3 = SimDataProvider.Session.Cars[playerId].Value.FastestLap.Splits[2];
            _sector3Header.Draw(g,"S3", this.Scale);
            _sector3Value.Draw(g, sector3.HasValue ? TimeSpan.FromMilliseconds(sector3.Value).ToString("mm\\:ss\\.fff") : "--:--.---", this.Scale);
            
            if(_config.InfoPanel.DeltaToBest)
            {
                _sector3DeltaValue.Draw(g, bestSector3.HasValue ? TimeSpan.FromMilliseconds(sector3.Value - bestSector3.Value).ToString("mm\\:ss\\.fff") : "--:--.---", this.Scale);
            }        
        }

        if (_config.InfoPanel.LastLapTime)
        {
            int? lastLap = SimDataProvider.Session.Cars[playerId].Value.LastLap.LaptimeMS;
            int? bestLap = SimDataProvider.Session.Cars[playerId].Value.FastestLap.LaptimeMS;
            _lastLapHeader.Draw(g,"Last", this.Scale);
            _lastLapValue.Draw(g, lastLap.HasValue ? TimeSpan.FromMilliseconds(lastLap.Value).ToString("mm\\:ss\\.fff") : "--:--.---", this.Scale);

            if (_config.InfoPanel.DeltaToBest)
            {
                _lastLapDeltaValue.Draw(g, bestLap.HasValue ? TimeSpan.FromMilliseconds(lastLap.Value - bestLap.Value).ToString("mm\\:ss\\.fff") : "--:--.---", this.Scale);
            }
        }

        if (_config.InfoPanel.BestLapTime)
        {
            int? bestLap = SimDataProvider.Session.Cars[playerId].Value.FastestLap.LaptimeMS;
            _bestLapHeader.Draw(g,"Best", this.Scale);
            _bestLapValue.Draw(g, bestLap.HasValue ? TimeSpan.FromMilliseconds(bestLap.Value).ToString("mm\\:ss\\.fff") : "--:--.---", this.Scale);
        }
    }

}