﻿using RaceElement.Core.Jobs.Loop;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using static RaceElement.ACCSharedMemory;

namespace RaceElement.HUD.ACC.Overlays.Driving.RainPrediction;
internal record struct RealtimeWeather
{
    public RealtimeWeather() { }
    public AcRainIntensity Now { get; set; }
    public AcRainIntensity In10 { get; set; }
    public AcRainIntensity In30 { get; set; }
}

internal sealed class RainPredictionJob(RainPredictionOverlay Overlay) : AbstractLoopJob
{
    private readonly ConcurrentDictionary<DateTime, RealtimeWeather> WeatherChanges = [];
    public readonly ConcurrentDictionary<DateTime, AcRainIntensity> UpcomingChanges = [];
    private RealtimeWeather _lastWeather;

    /// <summary>-1 if multiplier not set, happens initially and when <see cref="ResetData"/> </summary>
    public int Multiplier { get; private set; } = -1;

    //public override void BeforeRun() => RaceSessionTracker.Instance.OnMultiplierChanged += OnSessionTimeMultiplierChanged;
    //private void OnSessionTimeMultiplierChanged(object sender, int e) => Multiplier = e;
    //public override void AfterCancel() => RaceSessionTracker.Instance.OnMultiplierChanged -= OnSessionTimeMultiplierChanged;

    public sealed override void RunAction()
    {
        try
        {
            if (Overlay.pageGraphics.Status == AcStatus.AC_OFF)
            {
                ResetData();
                return;
            }

            //if (Multiplier == -1) return;

            while (UpcomingChanges.Count > 100 && UpcomingChanges.Keys.First() < DateTime.UtcNow)
                UpcomingChanges.TryRemove(UpcomingChanges.Keys.First(), out AcRainIntensity _);

            if (WeatherChanges.IsEmpty)
                _lastWeather = new()
                {
                    Now = Overlay.pageGraphics.rainIntensity,
                    In10 = Overlay.pageGraphics.rainIntensityIn10min,
                    In30 = Overlay.pageGraphics.rainIntensityIn30min
                };

            RealtimeWeather newScan = new()
            {
                Now = Overlay.pageGraphics.rainIntensity,
                In10 = Overlay.pageGraphics.rainIntensityIn10min,
                In30 = Overlay.pageGraphics.rainIntensityIn30min
            };

            if (newScan != _lastWeather || UpcomingChanges.Count == 0)
            {
                DateTime change = DateTime.UtcNow;
                WeatherChanges.TryAdd(change, newScan);
                if (Overlay._config.Time.TimeMultiplier == 0) return;

                UpcomingChanges.TryAdd(change.AddMinutes(10d / Overlay._config.Time.TimeMultiplier), newScan.In10);
                UpcomingChanges.TryAdd(change.AddMinutes(30d / Overlay._config.Time.TimeMultiplier), newScan.In30);
                Debug.WriteLine("Added new weather change");
                _lastWeather = newScan;
            }
        }
        catch (Exception)
        {
            // let's not break something for a new release, just in case.
        }
    }
    internal void ResetData()
    {
        Multiplier = -1;
        WeatherChanges.Clear();
        UpcomingChanges.Clear();
    }

    private double Get10MinutesWithMultiplier()
    {
        double countDown = 10d / Multiplier;
        return countDown < 1d ? 0.5d : countDown;
    }

    private double Get30MinutesWithMultiplier()
    {
        double countDown = 30d / Multiplier;
        return countDown < 1d ? 1d : countDown;
    }
}