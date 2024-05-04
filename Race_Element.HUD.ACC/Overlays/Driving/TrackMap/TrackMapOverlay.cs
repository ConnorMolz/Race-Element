﻿using RaceElement.HUD.Overlay.Internal;
using RaceElement.Util;

using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System;

namespace RaceElement.HUD.ACC.Overlays.Driving.TrackMap;

internal struct BoundingBox
{
    public float Left, Right;
    public float Bottom, Top;
}

#if DEBUG
[Overlay(Name = "Track Map",
    Description = "Shows a track map",
    OverlayCategory = OverlayCategory.Track,
    OverlayType = OverlayType.Drive)]
#endif
internal sealed class TrackMapOverlay : AbstractOverlay
{
    private  readonly TrackMapConfiguration _config = new();
    private TrackMapCreationJob _miniMapCreationJob;

    private readonly float _margin = 32.0f;
    private List<PointF> _trackPositions;

    public TrackMapOverlay(Rectangle rectangle) : base(rectangle, "Track Map")
    {
        RefreshRateHz = 24;
    }

    public override void BeforeStart()
    {
        if (IsPreviewing)
        {
            return;
        }

        _miniMapCreationJob = new TrackMapCreationJob()
        {
            IntervalMillis = 4,
        };

        _miniMapCreationJob.OnMapPositionsCallback += OnMapPositionsCallback;
        _miniMapCreationJob.Run();
    }

    public override void BeforeStop()
    {
        if (IsPreviewing)
        {
            return;
        }

        _miniMapCreationJob?.CancelJoin();
        _miniMapCreationJob = null;

        _trackPositions.Clear();
        _trackPositions = null;
    }

    public override bool ShouldRender()
    {
        return base.ShouldRender() && _trackPositions != null;
    }

    public override void Render(Graphics g)
    {
        var bitmap = CreateBitmapForCarsAndTrack();

        if (bitmap.Width != Width || bitmap.Height != Height)
        {
            Height = bitmap.Height;
            Width = bitmap.Width;
        }

        g.DrawImage(CreateBitmapForCarsAndTrack(), 0, 0);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void OnMapPositionsCallback(object sender, List<PointF> positions)
    {
        _miniMapCreationJob.Cancel();
        _trackPositions = positions;

        if (!_config.Map.SavePreview)
        {
            return;
        }

        var pageFileStatic = ACCSharedMemory.Instance.PageFileStatic;
        if (pageFileStatic.Track.Length == 0) return;

        var track = ScaleAndRotate(_trackPositions, GetBoundingBox(_trackPositions), _config.Map.Scale, _config.Map.Rotation);
        var minimap = CreateBitmapForCarsAndTrack(new List<PointF>(), track);

        string path = FileUtil.RaceElementTracks + pageFileStatic.Track + ".jpg";
        minimap.Save(path);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private Bitmap CreateBitmapForCarsAndTrack()
    {
        var pageFileGraphic = ACCSharedMemory.Instance.PageFileGraphic;
        var boundaries = GetBoundingBox(_trackPositions);
        List<PointF> cars = new();

        for (int i = 0; i < pageFileGraphic.ActiveCars; ++i)
        {
            float x = pageFileGraphic.CarCoordinates[i].X;
            float y = pageFileGraphic.CarCoordinates[i].Z;

            cars.Add(new PointF(x, y));
        }

        var track = ScaleAndRotate(_trackPositions, boundaries, _config.Map.Scale, _config.Map.Rotation);
        cars = ScaleAndRotate(cars, boundaries, _config.Map.Scale, _config.Map.Rotation);

        return CreateBitmapForCarsAndTrack(cars, track);
    }

    private Bitmap CreateBitmapForCarsAndTrack(List<PointF> cars, List<PointF> track)
    {
        BoundingBox boundaries = GetBoundingBox(track);

        for (int i = 0; i < cars.Count; ++i)
        {
            float x = cars[i].X - boundaries.Left + _margin * 0.5f;
            float y = cars[i].Y - boundaries.Bottom + _margin * 0.5f;

            cars[i] = new PointF(x, y);
        }

        for (int i = 0; i < track.Count; ++i)
        {
            float x = track[i].X - boundaries.Left + _margin * 0.5f;
            float y = track[i].Y - boundaries.Bottom + _margin * 0.5f;

            track[i] = new PointF(x, y);
        }

        float w = (float)Math.Sqrt((boundaries.Right - boundaries.Left) * (boundaries.Right - boundaries.Left));
        float h = (float)Math.Sqrt((boundaries.Top - boundaries.Bottom) * (boundaries.Top - boundaries.Bottom));

        var carsAndTrack = new Bitmap((int)(w + _margin), (int)(h + _margin));
        carsAndTrack.MakeTransparent();

        var g = Graphics.FromImage(carsAndTrack);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.CompositingMode = CompositingMode.SourceOver;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.DrawLines(new Pen(_config.Map.Color, _config.Map.Thickness), track.ToArray());

        for (int i = 0; i < cars.Count; ++i)
        {
            Color color = _config.Car.OthersColor;
            var pageFileGraphic = ACCSharedMemory.Instance.PageFileGraphic;

            if (pageFileGraphic.CarIds[i] == pageFileGraphic.PlayerCarID)
            {
                color = _config.Car.PlayerColor;
            }

            float outBorder = 3.0f;
            var car = cars[i];

            float scale = 18 * _config.Map.Thickness / 10;
            scale = scale < 5 ? 5 : scale;

            g.FillEllipse(new SolidBrush(Color.Black), car.X - outBorder * 0.5f, car.Y - outBorder * 0.5f, scale, scale);
            g.FillEllipse(new SolidBrush(color), car.X, car.Y, scale - outBorder, scale - outBorder);
        }

        return carsAndTrack;
    }

    private List<PointF> ScaleAndRotate(List<PointF> positions, BoundingBox boundings, float scale, float rotation)
    {
        var rot = Double.DegreesToRadians(rotation);
        float centerX = (boundings.Right + boundings.Left) * 0.5f;
        float centerY = (boundings.Top + boundings.Bottom) * 0.5f;

        List<PointF> result = new();
        foreach (var it in positions)
        {
            PointF pos = new();

            pos.X = (it.X - centerX) * scale;
            pos.Y = (it.Y - centerY) * scale;

            var x = pos.X * Math.Cos(rot) - pos.Y * Math.Sin(rot);
            var y = pos.X * Math.Sin(rot) + pos.Y * Math.Cos(rot);

            result.Add(new PointF((float)x, (float)y));
        }

        return result;
    }

    private BoundingBox GetBoundingBox(List<PointF> positions)
    {
        BoundingBox result = new();

        if (positions.Count > 0)
        {
            result.Right = positions[0].X;
            result.Left = positions[0].X;

            result.Top = positions[0].Y;
            result.Bottom = positions[0].Y;
        }

        foreach (var it in positions)
        {
            result.Right = Math.Max(result.Right, it.X);
            result.Left = Math.Min(result.Left, it.X);

            result.Top = Math.Max(result.Top, it.Y);
            result.Bottom = Math.Min(result.Bottom, it.Y);
        }

        return result;
    }
}
