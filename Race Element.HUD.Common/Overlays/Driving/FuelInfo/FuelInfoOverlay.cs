﻿//using RaceElement.Data.ACC.Database.LapDataDB;
//using RaceElement.Data.ACC.Tracker.Laps;
using RaceElement.Data.Common;
using RaceElement.Data.Common.SimulatorData;
using RaceElement.Data.Games;
using RaceElement.HUD.Overlay.Configuration;
using RaceElement.HUD.Overlay.Internal;
using RaceElement.HUD.Overlay.Util;
using RaceElement.Util.SystemExtensions;
using System;
using System.Drawing;

namespace RaceElement.HUD.Common.Overlays.OverlayFuelInfo;

[Overlay(
    Name = "Fuel Info (ALPHA)",
    Description = "A panel showing information about the fuel: laps left, fuel to end of race. Optionally showing stint information." +
    "\nBest to be used in a race, Do not use this before you start it as the game doesn't provide accurate data at that point.",
    Version = 1.00,
    OverlayType = OverlayType.Drive,
    OverlayCategory = OverlayCategory.Car,
    Game = Game.iRacing | Game.AssettoCorsa1 | Game.Automobilista2,
    Authors = ["Kris Vickers", "Reinier Klarenberg", "Dirk Wolf"]
)]
internal sealed class FuelInfoOverlay : CommonAbstractOverlay
{
    private readonly InfoPanel _infoPanel;

    private readonly FuelInfoConfig _config = new();
    private sealed class FuelInfoConfig : OverlayConfiguration
    {

        public enum LapTimeSource
        {
            LastThenBest,
            LastTwoLapsAverage,
            BestOnly,
            LastOnly,
        }

        [ConfigGrouping("Data Source", "Adjust the Source of the data used in the fuel calculation. NOTE: at the moment only last lap supported.")]
        public DataSourceGrouping DataSource { get; init; } = new();
        public sealed class DataSourceGrouping
        {
            [ToolTip("Sets the source of the laptime used in the fuel calculation" +
                     "\nLast Then Best: Uses your last lap until you set a best lap." +
                     "\nLast Two Laps Average: Uses the average laptime of your last 2 laps." +
                     "\nBest Only: Only uses your best valid lap if any was set." +
                     "\nLast Only: Only uses your last lap if any was set.")]
            public LapTimeSource LapTimeSource { get; init; } = LapTimeSource.LastThenBest;
        }

        [ConfigGrouping("Info Panel", "Show or hide additional information in the panel.")]
        public InfoPanelGrouping InfoPanel { get; init; } = new();
        public sealed class InfoPanelGrouping
        {
            [ToolTip("Sets the number of additional laps as a fuel buffer.")]
            [IntRange(0, 3, 1)]
            public int FuelBufferLaps { get; init; } = 0;

            [ToolTip("Displays Fuel time remaining which is green if it's higher than stint time or session time and red if it is not.")]
            public bool FuelTime { get; init; } = true;

            [ToolTip("Displays stint time remaining and the suggested amount of fuel to the end of the stint or the session.")]
            public bool StintInfo { get; init; } = true;

            [ToolTip("When viewing the setup menu it will still display the HUD.\nOverriding the default.")]
            public bool ShowInSetup { get; init; } = false;
        }

        [ConfigGrouping("Colors", "Adjust colors for the fuel bar.")]
        public ColorsGrouping Colors { get; init; } = new();
        public sealed class ColorsGrouping
        {
            [ToolTip("Change the color of the fuel bar when full fuel.")]
            public Color FullColor { get; init; } = Color.FromArgb(255, Color.Green);

            [ToolTip("Change the medium fuel percentage for the fuel bar to change color.")]
            [FloatRange(0.30f, 0.75f, 0.01f, 2)]
            public float MediumPercent { get; init; } = 0.5f;
            [ToolTip("Change the color of the fuel bar when medium fuel.")]
            public Color MediumColor { get; init; } = Color.FromArgb(255, 255, 135, 0);

            [ToolTip("Change the low fuel percentage for the fuel bar to change color.")]
            [FloatRange(0.01f, 0.25f, 0.01f, 2)]
            public float LowPercent { get; init; } = 0.15f;
            [ToolTip("Change the color of the fuel bar when low fuel.")]
            public Color LowColor { get; init; } = Color.FromArgb(255, Color.Red);
        }

        public FuelInfoConfig()
        {
            this.GenericConfiguration.AllowRescale = true;
        }
    }

    public FuelInfoOverlay(Rectangle rectangle) : base(rectangle, "Fuel Info")
    {
        this.Width = 222;
        _infoPanel = new InfoPanel(10, this.Width - 1) { FirstRowLine = 1 };
        this.Height = this._infoPanel.FontHeight * 6 + 1;
        RefreshRateHz = 2;
    }

    public override void SetupPreviewData()
    {
        SimDataProvider.Instance.SetupPreviewData();
    }

    public sealed override void BeforeStart()
    {
        if (!_config.InfoPanel.StintInfo)
            this.Height -= _infoPanel.FontHeight * 2;

        if (!_config.InfoPanel.FuelTime)
            this.Height -= _infoPanel.FontHeight;
    }

    public override void BeforeStop()
    {
        _infoPanel?.Dispose();
    }

    public sealed override bool ShouldRender()
    {
        if (_config.InfoPanel.ShowInSetup && SessionData.Instance.IsSetupMenuVisible)
            return true;

        return base.ShouldRender();
    }

    public sealed override void Render(Graphics g)
    {
        float fuelLiters = SimDataProvider.LocalCar.Engine.FuelLiters;
        using SolidBrush fuelBarBrush = new(GetFuelBarColor());
        _infoPanel.AddProgressBarWithCenteredText($"{fuelLiters:F2} L", 0, SimDataProvider.LocalCar.Engine.MaxFuelLiters, fuelLiters, fuelBarBrush);
        // Some global variants
        double lapBufferVar = SimDataProvider.LocalCar.Engine.FuelLitersXLap * this._config.InfoPanel.FuelBufferLaps;
        double bestLapTime = GetLapTimeMS();
        if (bestLapTime <= 0)
        {
            if (!IsPreviewing)
            {
                string header = "No Laptime";
                header = header.FillEnd(10, ' ');
                _infoPanel.AddLine(header, "Waiting...");
                _infoPanel.Draw(g);
                return;
            }

        }

        double fuelTimeLeft = SimDataProvider.LocalCar.Engine.FuelEstimatedLaps * bestLapTime;
        // ACC uses -1 for single driver races. TODO In iRacing we hardcode this the same for now
        // double stintDebug = pageGraphics.DriverStintTimeLeft; stintDebug.ClipMin(-1);
        double stintDebug = -1.0D;
        //**********************
        // Workings
        // TODO ACC uses -1000 in single player races. We use the same for iRacing right now.
        int driverStintTimeLeft = -1000;
        double stintFuel = 0; // TODO not used w/o stints driverStintTimeLeft / bestLapTime * SimDataProvider.LocalCar.Engine.FuelLitersXLap + pageGraphics.UsedFuelSinceRefuel;

        double fuelToEnd = SessionData.Instance.SessionTimeLeftSecs / bestLapTime * SimDataProvider.LocalCar.Engine.FuelLitersXLap;
        double fuelToAdd = FuelToAdd(lapBufferVar, stintDebug, stintFuel, fuelToEnd);
        string fuelTime = $"{TimeSpan.FromMilliseconds(fuelTimeLeft):hh\\:mm\\:ss}";
        string stintTime = $"{TimeSpan.FromMilliseconds(stintDebug):hh\\:mm\\:ss}";
        //**********************
        using SolidBrush fuelTimeBrush = new(GetFuelTimeColor(fuelTimeLeft, stintDebug));
        //Start (Basic)
        _infoPanel.AddLine("Laps Left", $"{SimDataProvider.LocalCar.Engine.FuelEstimatedLaps:F1} @ {SimDataProvider.LocalCar.Engine.FuelLitersXLap:F2}L");
        _infoPanel.AddLine("Fuel-End", $"{fuelToEnd + lapBufferVar:F1} : Add {fuelToAdd:F0}");
        //End (Basic)
        //Magic Start (Advanced)
        if (this._config.InfoPanel.FuelTime)
            _infoPanel.AddLine("Fuel Time", fuelTime, fuelTimeBrush);

        if (_config.InfoPanel.StintInfo)
        {
            _infoPanel.AddLine("Stint Time", stintTime);

            if (stintDebug == -1)
                _infoPanel.AddLine("Stint Fuel", "No Stints");
            else
                _infoPanel.AddLine("Stint Fuel", $"{stintFuel + lapBufferVar:F1}");
        }
        //Magic End (Advanced)
        _infoPanel.Draw(g);
    }

    private int GetLapTimeMS()
    {
        int lapTime = -1;
        switch (_config.DataSource.LapTimeSource)
        {
            /* TODO: port the LapTracker to be independent of ACC
            case FuelInfoConfig.LapTimeSource.LastTwoLapsAverage:
                {
                    lapTime = LapTracker.Instance.Laps.GetAverageLapTime(2);
                    break;
                }
            case FuelInfoConfig.LapTimeSource.LastOnly:
                {
                    lapTime = LapTracker.Instance.Laps.GetLastLapTime();
                    break;
                }
            case FuelInfoConfig.LapTimeSource.BestOnly:
                {
                    lapTime = LapTracker.Instance.Laps.GetBestLapTime();
                    break;
                }
            case FuelInfoConfig.LapTimeSource.LastThenBest:
            */
            default:
                {
                    CarInfo carInfo = SessionData.Instance.Cars[SessionData.Instance.PlayerCarIndex].Value;
                    lapTime = (int)carInfo.FastestLap.LaptimeMS;
                    if (lapTime > TimeSpan.FromMinutes(12).TotalMilliseconds || lapTime == 0)
                    {
                        if (carInfo.LastLap.LaptimeMS < TimeSpan.FromMinutes(12).TotalMilliseconds && carInfo.LastLap.LaptimeMS != 0)
                            lapTime = (int)carInfo.LastLap.LaptimeMS;
                    }
                    break;
                }

        }

        return lapTime;
    }

    private double FuelToAdd(double lapBufferVar, double stintDebug, double stintFuel, double fuelToEnd)
    {
        double fuel;
        if (stintDebug == -1)
            fuel = Math.Min(Math.Ceiling(fuelToEnd - SimDataProvider.LocalCar.Engine.FuelLiters), SimDataProvider.LocalCar.Engine.FuelLiters) + lapBufferVar;
        else
            fuel = Math.Min(stintFuel - SimDataProvider.LocalCar.Engine.FuelLiters, SimDataProvider.LocalCar.Engine.MaxFuelLiters) + lapBufferVar;
        fuel.ClipMin(0);
        return fuel;
    }

    private Color GetFuelBarColor()
    {
        float percentage = SimDataProvider.LocalCar.Engine.FuelLiters / SimDataProvider.LocalCar.Engine.MaxFuelLiters;

        Color color = _config.Colors.FullColor;
        if (percentage <= _config.Colors.MediumPercent) color = _config.Colors.MediumColor;
        if (percentage <= _config.Colors.LowPercent) color = _config.Colors.LowColor;

        return color;
    }

    private Color GetFuelTimeColor(double fuelTimeLeft, double stintDebug)
    {
        Color color;
        if (stintDebug > -1)
            color = fuelTimeLeft <= stintDebug ? Color.Red : Color.LimeGreen;
        else
            color = fuelTimeLeft <= SessionData.Instance.SessionTimeLeftSecs ? Color.Red : Color.LimeGreen;
        return color;
    }
}