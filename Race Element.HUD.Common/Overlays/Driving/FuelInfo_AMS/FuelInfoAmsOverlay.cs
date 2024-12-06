using System.Drawing;
using System.Drawing.Drawing2D;
using RaceElement.Data.Common;
using RaceElement.Data.Games;
using RaceElement.HUD.Overlay.Configuration;
using RaceElement.HUD.Overlay.Internal;
using RaceElement.HUD.Overlay.OverlayUtil;
using RaceElement.HUD.Overlay.OverlayUtil.InfoPanel;
using RaceElement.HUD.Overlay.Util;

namespace RaceElement.HUD.Common.Overlays.Driving.FuelInfo_AMS;

[Overlay(
    Name = "Fuel Info",
    Description = "Fuel Info Overlay for AMS2",
    Game = Game.Automobilista2,
    Authors = ["Connor Molz"]
)]
public class FuelInfoAmsOverlay: CommonAbstractOverlay
{
    // Configuration
    private readonly CarElectronicsConfig _config = new();

    private sealed class CarElectronicsConfig : OverlayConfiguration
    {
        [ConfigGrouping("Info Panel", "Show or hide additional information in the panel.")]
        public InfoPanelGrouping InfoPanel { get; init; } = new InfoPanelGrouping();

        public sealed class InfoPanelGrouping
        {
            [ToolTip("Add the formation lap to the To Finish Value.")]
            public bool FormationLap { get; init; } = true;

            [ToolTip("Add Laps to the To Finish value.")]
            [IntRange(0, 3, 1)]
            public int BufferLaps { get; init; } = 0;
        }
        
        public CarElectronicsConfig()
        {
            this.GenericConfiguration.AllowRescale = true;
        }
    }
    
    public FuelInfoAmsOverlay(Rectangle rectangle) : base(rectangle, "Car Electronics")
    {
        RefreshRateHz = 1;
    }

    // Window Components
    private Font _font;
    
    private PanelText _fuelLastLapHeader;
    private PanelText _fuelLastLapValue;
    
    private PanelText _fuelAvgLapHeader;
    private PanelText _fuelAvgLapValue;
    
    private PanelText _fuelRemainingHeader;
    private PanelText _fuelRemainingValue;
    
    private PanelText _fuelToFinishHeader;
    private PanelText _fuelToFinishValue;
    
    private PanelText _fuelLapsRemainingHeader;
    private PanelText _fuelLapsRemainingValue;
    
    // Fuel data
    private float _fuelLastLap;
    private float _fuelAvgLap;
    private float _fuelRemaining;
    private float _fuelToFinish;
    private float _fuelLapsRemaining;

    private float _bestLap;

    private int _currentlap = -1;
    private float _bufferdFuel;
   
    
    // Before render function to setup the window and size
    public sealed override void BeforeStart()
    {
        _font = FontUtil.FontSegoeMono(10f * this.Scale);

        int lineHeight = _font.Height;

        int unscaledHeaderWidth = 130;
        int unscaledValueWidth = 100;

        int headerWidth = (int)(unscaledHeaderWidth * this.Scale);
        int valueWidth = (int)(unscaledValueWidth * this.Scale);
        int roundingRadius = (int)(6 * this.Scale);

        RectangleF headerRect = new(0, 0, headerWidth, lineHeight);
        RectangleF valueRect = new(headerWidth, 0, valueWidth, lineHeight);
        StringFormat headerFormat = new() { Alignment = StringAlignment.Near };
        StringFormat valueFormat = new() { Alignment = StringAlignment.Far };

        Color accentColor = Color.FromArgb(25, 255, 0, 0);
        CachedBitmap headerBackground = new(headerWidth, lineHeight, g =>
        {
            Rectangle panelRect = new(0, 0, headerWidth, lineHeight);
            using GraphicsPath path = GraphicsExtensions.CreateRoundedRectangle(panelRect, 0, 0, 0, roundingRadius);
            using LinearGradientBrush brush = new(panelRect, Color.FromArgb(185, 0, 0, 0),
                Color.FromArgb(255, 10, 10, 10), LinearGradientMode.BackwardDiagonal);
            g.FillPath(brush, path);
            using Pen underlinePen = new(accentColor);
            g.DrawLine(underlinePen, 0 + roundingRadius / 2, lineHeight, headerWidth, lineHeight - 1);
        });

        CachedBitmap valueBackground = new(valueWidth, lineHeight, g =>
        {
            Rectangle panelRect = new(0, 0, valueWidth, lineHeight);
            using GraphicsPath path = GraphicsExtensions.CreateRoundedRectangle(panelRect, 0, roundingRadius, 0, 0);
            using LinearGradientBrush brush = new(panelRect, Color.FromArgb(255, 0, 0, 0), Color.FromArgb(185, 0, 0, 0),
                LinearGradientMode.ForwardDiagonal);
            g.FillPath(brush, path);
            using Pen underlinePen = new(accentColor);
            g.DrawLine(underlinePen, 0, lineHeight - 1, valueWidth, lineHeight - 1);
        });
        
        _fuelLastLapHeader = new PanelText(_font, headerBackground, headerRect) { StringFormat = headerFormat };
        _fuelLastLapValue = new PanelText(_font, valueBackground, valueRect) { StringFormat = valueFormat };
        headerRect.Offset(0, lineHeight);
        valueRect.Offset(0, lineHeight);
        
        _fuelAvgLapHeader = new PanelText(_font, headerBackground, headerRect) { StringFormat = headerFormat };
        _fuelAvgLapValue = new PanelText(_font, valueBackground, valueRect) { StringFormat = valueFormat };
        headerRect.Offset(0, lineHeight);
        valueRect.Offset(0, lineHeight);
        
        _fuelRemainingHeader = new PanelText(_font, headerBackground, headerRect) { StringFormat = headerFormat };
        _fuelRemainingValue = new PanelText(_font, valueBackground, valueRect) { StringFormat = valueFormat };
        headerRect.Offset(0, lineHeight);
        valueRect.Offset(0, lineHeight);
        
        _fuelToFinishHeader = new PanelText(_font, headerBackground, headerRect) { StringFormat = headerFormat };
        _fuelToFinishValue = new PanelText(_font, valueBackground, valueRect) { StringFormat = valueFormat };
        headerRect.Offset(0, lineHeight);
        valueRect.Offset(0, lineHeight);
        
        _fuelLapsRemainingHeader = new PanelText(_font, headerBackground, headerRect) { StringFormat = headerFormat };
        _fuelLapsRemainingValue = new PanelText(_font, valueBackground, valueRect) { StringFormat = valueFormat };
        headerRect.Offset(0, lineHeight);
        valueRect.Offset(0, lineHeight);
        
    }

    public override void Render(Graphics g)
    {
        
        _fuelLastLapHeader.Draw(g, "Last Lap", this.Scale);
        _fuelLastLapValue.Draw(g, "1:23.456", this.Scale);
        
        _fuelAvgLapHeader.Draw(g, "Avg Lap", this.Scale);
        _fuelAvgLapValue.Draw(g, "1:23.456", this.Scale);
        
        _fuelRemainingHeader.Draw(g, "Remaining", this.Scale);
        _fuelRemainingValue.Draw(g, "10.0 L", this.Scale);
        
        _fuelToFinishHeader.Draw(g, "To Finish", this.Scale);
        _fuelToFinishValue.Draw(g, "10.0 L", this.Scale);
        
        _fuelLapsRemainingHeader.Draw(g, "Laps in Tank", this.Scale);
        _fuelLapsRemainingValue.Draw(g, "10", this.Scale);
    }
    
    public override void BeforeStop()
    {
        _font.Dispose();
        
        _fuelLastLapHeader.Dispose();
        _fuelLastLapValue.Dispose();
        _fuelAvgLapHeader.Dispose();
        _fuelAvgLapValue.Dispose();
        _fuelRemainingHeader.Dispose();
        _fuelRemainingValue.Dispose();
        _fuelToFinishHeader.Dispose();
        _fuelToFinishValue.Dispose();
        _fuelLapsRemainingHeader.Dispose();
        _fuelLapsRemainingValue.Dispose();
        
    }

    private void Calculate()
    {
        // Data form game which need no calculation
        _bestLap = SimDataProvider.LocalCar.Timing.LapTimeBestMs;
        _fuelRemaining = SimDataProvider.LocalCar.Engine.FuelLiters;
        _fuelAvgLap = SimDataProvider.LocalCar.Engine.FuelLitersXLap;
        _fuelLapsRemaining = SimDataProvider.LocalCar.Engine.FuelEstimatedLaps;
        
        // Calculate the fuel used in the last lap
        if(_currentlap != SimDataProvider.LocalCar.Race.LapsDriven)
        {
            _fuelLastLap = _bufferdFuel - _fuelRemaining;
            _bufferdFuel = _fuelRemaining;
            _currentlap = SimDataProvider.LocalCar.Race.LapsDriven;
        }
        
        // Calculate the fuel to finish
        double remainingLaps = SimDataProvider.Session.SessionTimeLeftSecs / _bestLap;
        _fuelToFinish = Convert.ToSingle(remainingLaps * _fuelAvgLap + _config.InfoPanel.BufferLaps * _fuelAvgLap);
        if (_config.InfoPanel.FormationLap)
        {
            _fuelToFinish += _fuelAvgLap;
        }

    }
    
}