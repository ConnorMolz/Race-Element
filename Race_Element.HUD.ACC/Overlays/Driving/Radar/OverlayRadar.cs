﻿using RaceElement.Data.ACC.EntryList;
using RaceElement.Data.ACC.Session;
using RaceElement.HUD.Overlay.Configuration;
using RaceElement.HUD.Overlay.Internal;
using RaceElement.HUD.Overlay.OverlayUtil;
using RaceElement.Util.SystemExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;

namespace RaceElement.HUD.ACC.Overlays.OverlaySpotter;

[Overlay(Name = "Radar",
    Description = "Beta",
    Version = 1.00,
    OverlayType = OverlayType.Drive,
    OverlayCategory = OverlayCategory.Driving,
    Authors = ["Reinier Klarenberg"]
)]
internal sealed class RadarOverlay : AbstractOverlay
{
    private readonly RadarConfiguration _config = new();
    private sealed class RadarConfiguration : OverlayConfiguration
    {
        public RadarConfiguration() => GenericConfiguration.AllowRescale = true;

        [ConfigGrouping("Radar", "General options for the radar")]
        public RadarGrouping Radar { get; init; } = new RadarGrouping();
        public sealed class RadarGrouping
        {

            [IntRange(1, 20, 2)]
            [ToolTip("The refreshrate of this HUD.")]
            public int Herz { get; init; } = 20;

            [IntRange(50, 250, 2)]
            public int Width { get; init; } = 250;

            [IntRange(50, 250, 2)]
            public int Height { get; init; } = 250;

            [ToolTip("Display cars inside of the pits.")]
            public bool ShowPitted { get; init; } = true;
        }

        [ConfigGrouping("Proximity", "Options related to proximity of other cars")]
        public ProximityGrouping Proxmity { get; init; } = new ProximityGrouping();
        public sealed class ProximityGrouping
        {
            [ToolTip("Adjust the distance before the HUD will show.")]
            [FloatRange(4f, 40f, 0.02f, 2)]
            public float ShowDistance { get; init; } = 16f;
        }
    }

    private struct SpottableCar
    {
        public int Index;
        public float Y;
        public float X;
        public float Heading;
        public float Distance;
    }

    private readonly List<SpottableCar> _spottables = [];

    private float _playerY;
    private float _playerX;
    private float _playerHeading;

    private CachedBitmap _cachedBackground;

    private CachedBitmap _cachedLocalCar;
    private CachedBitmap _cachedFarCar;
    private CachedBitmap _cachedCloseCar;
    private CachedBitmap _cachedNearCar;

    private readonly CarDrawingData _carDrawingData = new();
    private sealed class CarDrawingData
    {
        public static readonly int CarWidth = 10;
        public static readonly int CarHeight = 22;
        public float ScaledCarWidth;
        public float ScaledCarHeight;
    }

    public RadarOverlay(Rectangle rectangle) : base(rectangle, "Radar")
    {
        RefreshRateHz = _config.Radar.Herz;
    }

    public sealed override bool ShouldRender()
    {
        if (RaceSessionState.IsFormationLap(pageGraphics.GlobalRed, broadCastRealTime.Phase))
            return false;

        if (RaceSessionState.IsSpectating(pageGraphics.PlayerCarID, broadCastRealTime.FocusedCarIndex))
            return true;

        return base.ShouldRender();
    }

    public sealed override void SetupPreviewData()
    {
        _spottables.Clear();

        _playerY = 0; _playerX = 0; _playerHeading = 0.12f;

        var car1 = new SpottableCar { Index = 2, X = 3, Y = 5.7f, Heading = -0.1f };
        car1.Distance = DistanceBetween(_playerX, _playerY, car1.X, car1.Y);
        _spottables.Add(car1);

        var car2 = new SpottableCar { Index = 3, X = -3f, Y = -3.7f, Heading = 0.1f };
        car2.Distance = DistanceBetween(_playerX, _playerY, car2.X, car2.Y);
        _spottables.Add(car2);

        var car3 = new SpottableCar { Index = 4, X = 3.5f, Y = -3.7f, Heading = 0.1f };
        car3.Distance = DistanceBetween(_playerX, _playerY, car3.X, car3.Y);
        _spottables.Add(car3);

        var car4 = new SpottableCar { Index = 5, X = 0f, Y = -13.7f, Heading = 0.2f };
        car4.Distance = DistanceBetween(_playerX, _playerY, car4.X, car4.Y);
        _spottables.Add(car4);
    }

    public sealed override void BeforeStart()
    {
        Width = _config.Radar.Width;
        Height = _config.Radar.Height;

        _carDrawingData.ScaledCarWidth = CarDrawingData.CarWidth * Scale;
        _carDrawingData.ScaledCarHeight = CarDrawingData.CarHeight * Scale;

        int scaledWidth = (int)(Width * Scale);
        int scaledHeight = (int)(Height * Scale);
        _cachedBackground = new CachedBitmap(scaledWidth + 1, scaledHeight + 1, g =>
        {
            using GraphicsPath gradientPath = new();
            gradientPath.AddEllipse(0, 0, scaledWidth, scaledHeight);
            using PathGradientBrush pthGrBrush = new(gradientPath);
            pthGrBrush.CenterColor = Color.FromArgb(172, 0, 0, 0);
            pthGrBrush.SurroundColors = [Color.FromArgb(5, 0, 0, 0)];
            g.FillRoundedRectangle(pthGrBrush, new Rectangle(0, 0, scaledWidth, scaledHeight), (int)(3 * this.Scale));

            using Pen pen = new(new SolidBrush(Color.FromArgb(160, Color.White)));
            pen.DashPattern = [2 / this.Scale, 4 / this.Scale];
            pen.Width = 2f * Scale;
            g.DrawLine(pen, new PointF(0, scaledHeight / 2), new PointF(scaledWidth, scaledHeight / 2));
            g.DrawLine(pen, new PointF(scaledWidth / 2, 0), new PointF(scaledWidth / 2, scaledHeight));
        });


        Rectangle carRectangle = new(0, 0, (int)Math.Ceiling(_carDrawingData.ScaledCarWidth), (int)Math.Ceiling(_carDrawingData.ScaledCarHeight));
        _cachedLocalCar = new CachedBitmap(carRectangle.Width + 1, carRectangle.Height + 1, g =>
        {
            g.FillRoundedRectangle(Brushes.LimeGreen, carRectangle, (int)(3 * Scale));
            g.DrawRoundedRectangle(Pens.White, carRectangle, (int)(3 * Scale));
        });
        _cachedFarCar = new CachedBitmap(carRectangle.Width + 1, carRectangle.Height + 1, g =>
        {
            g.FillRoundedRectangle(Brushes.White, carRectangle, (int)(3 * Scale));
            g.DrawRoundedRectangle(Pens.Black, carRectangle, (int)(3 * Scale));
        });
        _cachedNearCar = new CachedBitmap(carRectangle.Width + 1, carRectangle.Height + 1, g =>
            {
                g.FillRoundedRectangle(Brushes.Yellow, carRectangle, (int)(3 * Scale));
                g.DrawRoundedRectangle(Pens.Black, carRectangle, (int)(3 * Scale));
            });
        _cachedCloseCar = new CachedBitmap(carRectangle.Width + 1, carRectangle.Height + 1, g =>
        {
            g.FillRoundedRectangle(Brushes.Red, carRectangle, (int)(3 * Scale));
            g.DrawRoundedRectangle(Pens.Black, carRectangle, (int)(3 * Scale));
        });
    }

    public sealed override void BeforeStop()
    {
        _cachedBackground?.Dispose();

        _cachedLocalCar?.Dispose();
        _cachedFarCar?.Dispose();
        _cachedNearCar?.Dispose();
        _cachedCloseCar?.Dispose();
    }

    public sealed override void Render(Graphics g)
    {
        try
        {
            int playerID = pageGraphics.PlayerCarID;
            var playerCar = EntryListTracker.Instance.Cars.FirstOrDefault(car => car.Key == playerID);
            if (playerCar.Value == null || EntryListTracker.Instance.Cars.Count == 0) return;

            _playerX = playerCar.Value.RealtimeCarUpdate.WorldPosX;
            _playerY = playerCar.Value.RealtimeCarUpdate.WorldPosY;
            _playerHeading = playerCar.Value.RealtimeCarUpdate.Heading;


            CollectSpottableCars(playerID);

            if (!_spottables.Any() && !IsRepositioning)
                return;

            //Debug.WriteLine($"{_spottables.Count}");

            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.AntiAlias;


            _cachedBackground?.Draw(g, 0, 0, _config.Radar.Width, _config.Radar.Height);

            // draw spottable cars
            float centerX = _config.Radar.Width / 2f;
            float centerY = _config.Radar.Height / 2f;

            _cachedLocalCar?.Draw(g, (int)(centerX - CarDrawingData.CarWidth / 2), (int)(centerY - CarDrawingData.CarHeight / 2), CarDrawingData.CarWidth, CarDrawingData.CarHeight);

            Matrix originalTransform = g.Transform;
            foreach (SpottableCar car in CollectionsMarshal.AsSpan(_spottables))
            {
                if (car.Index == playerID) continue;

                float multiplier = 5f;
                float xOffset = (_playerX - car.X);
                float yOffset = (_playerY - car.Y);

                float heading = (float)(_playerHeading);
                float newX = (float)(xOffset * Math.Cos(heading) + yOffset * Math.Sin(heading)) * multiplier;
                float newY = (float)(-xOffset * Math.Sin(heading) + yOffset * Math.Cos(heading)) * multiplier;

                Rectangle otherCar = new((int)(centerX - CarDrawingData.CarWidth / 2f + newX), (int)(centerY - CarDrawingData.CarHeight / 2f + newY), CarDrawingData.CarWidth, CarDrawingData.CarHeight);
                var transform = g.Transform;
                transform.RotateAt((float)(-(_playerHeading - car.Heading) * 180f / Math.PI), new PointF(otherCar.Left, otherCar.Top));
                g.Transform = transform;

                float opacity = 1f - car.Distance / _config.Proxmity.ShowDistance * 0.75f;
                opacity.ClipMin(0.25f);

                CachedBitmap selected = car.Distance switch
                {
                    float x when (x < 6f) => _cachedCloseCar,
                    float x when (x >= 6f && x < 8f) => _cachedNearCar,
                    _ => _cachedFarCar
                };
                selected.Opacity = opacity;
                selected.Draw(g, otherCar.Left, otherCar.Top, CarDrawingData.CarWidth, CarDrawingData.CarHeight);

                g.Transform = originalTransform;
            }

            _spottables.Clear();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.ToString());
        }

    }

    private void CollectSpottableCars(int playerID)
    {
        // find spottable cars
        var data = EntryListTracker.Instance.Cars;

        foreach (var car in data)
        {
            if (car.Key == playerID) continue;

            float x = car.Value.RealtimeCarUpdate.WorldPosX;
            float y = car.Value.RealtimeCarUpdate.WorldPosY;

            float distance = DistanceBetween(_playerX, _playerY, x, y);

            if (distance < _config.Proxmity.ShowDistance)
            {
                var spottable = new SpottableCar()
                {
                    Index = car.Key,
                    X = x,
                    Y = y,
                    Distance = distance,
                };

                if (car.Value != null)
                {
                    spottable.Heading = car.Value.RealtimeCarUpdate.Heading;
                    if (!_config.Radar.ShowPitted && car.Value.RealtimeCarUpdate.CarLocation == Broadcast.CarLocationEnum.Pitlane)
                        continue;
                }

                if (!_spottables.Contains(spottable))
                    _spottables.Add(spottable);
            }
        }

    }

    private float DistanceBetween(float x1, float y1, float x2, float y2) => (float)Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));

}
