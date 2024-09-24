﻿using RaceElement.HUD.Overlay.Internal;
using RaceElement.Util.SystemExtensions;
using System.Drawing;

namespace RaceElement.HUD.Common.Overlays.Driving.InputTrace;

[Overlay(
    Name = "Input Trace",
    Description = "Live graph of steering, throttle and brake inputs.",
    Version = 1.00,
    Authors = ["Reinier Klarenberg, Dirk Wolf"],
    OverlayType = OverlayType.Drive,
    OverlayCategory = OverlayCategory.Inputs
)]
internal sealed class InputTraceOverlay(Rectangle rectangle) : CommonAbstractOverlay(rectangle, "Input Trace")
{
    private readonly InputTraceConfiguration _config = new();

    private InputGraph? _graph;
    private InputDataJob? _dataJob;

    public sealed override void BeforeStart()
    {
        this.Width = _config.Chart.Width;
        this.Height = _config.Chart.Height;
        this.RefreshRateHz = _config.Chart.HudRefreshRate;
        this.RefreshRateHz.ClipMax(_config.Data.Herz);

        _dataJob = new(this, _config.Chart.Width - 1) { IntervalMillis = (int)(1000f / _config.Data.Herz) };
        _graph = new InputGraph(0, 0, _config.Chart.Width - 1, _config.Chart.Height - 1, _dataJob, this._config);

        if (!IsPreviewing)
            _dataJob.Run();
    }

    public sealed override void BeforeStop()
    {
        _graph?.Dispose();

        if (!IsPreviewing)
            _dataJob?.CancelJoin();
    }

    public sealed override void Render(Graphics g) => _graph?.Draw(g);
}
