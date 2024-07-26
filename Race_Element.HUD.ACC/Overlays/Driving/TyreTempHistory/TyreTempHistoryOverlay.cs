﻿using RaceElement.Data.ACC.Database.SessionData;
using RaceElement.Data.ACC.Session;
using RaceElement.HUD.Overlay.Internal;
using RaceElement.HUD.Overlay.OverlayUtil;
using RaceElement.HUD.Overlay.OverlayUtil.Drawing;
using RaceElement.HUD.Overlay.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace RaceElement.HUD.ACC.Overlays.Driving.TyreTempHistory;

[Overlay(Name = "Tyre Temp History",
Description = "A table with the minimum, average and maximum tyre temps for each tyre for the previous lap.",
OverlayCategory = OverlayCategory.Physics,
OverlayType = OverlayType.Drive,
Authors = ["Reinier Klarenberg"],
Version = 1.0)]
internal sealed class TyreTempHistoryOverlay : AbstractOverlay
{
    private readonly TyreTempHistoryConfiguration _config = new();

    private TyreTempHistoryJob _historyJob;
    private readonly List<TyreTempHistoryModel> _tempHistory = [];

    private GraphicsGrid _graphicsGrid;
    private Font _font;
    private CachedBitmap[] _columnBackgrounds;
    private CachedBitmap[] _columnBackgroundsRed;
    private CachedBitmap[] _columnBackgroundsBlue;

    private DrawableTextCell lapTextCell;

    private readonly string[] TyreNames = ["FL", "FR", "RL", "RR"];

    public TyreTempHistoryOverlay(Rectangle rectangle) : base(rectangle, "Tyre Temp History")
    {
        RefreshRateHz = 1 / 3f;
    }

    public override void SetupPreviewData()
    {
        _tempHistory.Add(new()
        {
            Lap = 4,
            Min = [65.9f, 66.6f, 66.9f, 67.0f],
            Averages = [76.2f, 76.7f, 77.0f, 77.5f],
            Max = [96.5f, 97.0f, 97.0f, 107.6f],
            TyreCompound = "" // compound can be "wet_compound", else it's dry 
        });
    }

    public sealed override void BeforeStart()
    {
        CreateGraphicsGrid();

        if (IsPreviewing) return;

        RaceSessionTracker.Instance.OnNewSessionStarted += OnNewSessionStarted;

        _historyJob = new(this) { IntervalMillis = 100 };
        _historyJob.OnNewHistory += OnNewHistory;
        _historyJob.Run();
    }

    private void OnNewSessionStarted(object sender, DbRaceSession e)
    {
        _tempHistory.Clear();
        ClearData();
    }
    private void OnNewHistory(object sender, TyreTempHistoryModel model) => _tempHistory.Add(model);

    public sealed override void BeforeStop()
    {
        if (IsPreviewing) return;


        for (int i = 0; i < _columnBackgrounds.Length; i++)
        {
            _columnBackgrounds[i]?.Dispose();
            _columnBackgroundsBlue[i]?.Dispose();
            _columnBackgroundsRed[i]?.Dispose();
        }

        lapTextCell?.Dispose();
        _graphicsGrid?.Dispose();

        _historyJob.OnNewHistory -= OnNewHistory;
        _historyJob?.CancelJoin();
    }

    private void CreateGraphicsGrid()
    {
        float scale = this.Scale;
        if (IsPreviewing) scale = 1f;

        _font = FontUtil.FontSegoeMono(12f * scale);

        int rows = 1; // 1 for header
        rows += 4;// 4 extra rows for tyres
        int columns = 4;    // Lap/Tyre, Avg, Min, Max
        _graphicsGrid = new GraphicsGrid(rows, columns);

        float fontHeight = (int)(_font.GetHeight(120));
        int columnHeight = (int)(Math.Ceiling(fontHeight) + 1 * scale);
        int[] columnWidths = [(int)(50f * scale), (int)(50f * scale), (int)(50f * scale), (int)(50f * scale)];
        int totalWidth = columnWidths.Sum();


        // set up backgrounds of each cell
        Color colorDefault = Color.FromArgb(190, Color.Black);
        Color colorRed = Color.FromArgb(150, Color.Red);
        Color colorBlue = Color.FromArgb(150, Color.Blue);
        using HatchBrush columnBrushDefault = new(HatchStyle.LightUpwardDiagonal, colorDefault, Color.FromArgb(colorDefault.A - 25, colorDefault));
        using HatchBrush columnBrushRed = new(HatchStyle.LightUpwardDiagonal, colorDefault, Color.FromArgb(colorRed.A - 75, colorRed));
        using HatchBrush columnBrushBlue = new(HatchStyle.LightUpwardDiagonal, colorDefault, Color.FromArgb(colorBlue.A - 25, colorBlue));
        _columnBackgrounds = new CachedBitmap[columns];
        _columnBackgroundsRed = new CachedBitmap[columns];
        _columnBackgroundsBlue = new CachedBitmap[columns];

        for (int i = 0; i < columns; i++)
        {
            Rectangle rect = new(0, 0, columnWidths[i], columnHeight);
            _columnBackgrounds[i] = new CachedBitmap(columnWidths[i], columnHeight, g =>
            {
                using LinearGradientBrush brush = new(new PointF(columnWidths[i], columnHeight), new PointF(0, 0), Color.FromArgb(90, 0, 0, 0), Color.FromArgb(colorDefault.A, 10, 10, 10));
                g.FillRoundedRectangle(brush, rect, (int)(_config.Table.Roundness * scale));
                g.FillRoundedRectangle(columnBrushDefault, rect, (int)(_config.Table.Roundness * scale));
            });

            _columnBackgroundsRed[i] = new CachedBitmap(columnWidths[i], columnHeight, g =>
            {
                using LinearGradientBrush brush = new(new PointF(0, 0), new PointF(columnWidths[i], columnHeight), Color.FromArgb(90, 0, 0, 0), Color.FromArgb(colorRed.A, colorRed.R, 10, 10));
                g.FillRoundedRectangle(brush, rect, (int)(_config.Table.Roundness * scale));
                g.FillRoundedRectangle(columnBrushRed, rect, (int)(_config.Table.Roundness * scale));
            });
            _columnBackgroundsBlue[i] = new CachedBitmap(columnWidths[i], columnHeight, g =>
            {
                using LinearGradientBrush brush = new(new PointF(0, 0), new PointF(columnWidths[i], columnHeight), Color.FromArgb(90, 0, 0, 0), Color.FromArgb(colorBlue.A, colorBlue.R, 10, 10));
                g.FillRoundedRectangle(brush, rect, (int)(_config.Table.Roundness * scale));
                g.FillRoundedRectangle(columnBrushBlue, rect, (int)(_config.Table.Roundness * scale));
            });
        }

        lapTextCell = new(new RectangleF(0, 0, columnWidths[0], columnHeight), _font);
        lapTextCell.CachedBackground = _columnBackgrounds[0];
        lapTextCell.UpdateText("?");
        DrawableTextCell col2 = new(new RectangleF(lapTextCell.Rectangle.X + columnWidths[0], 0, columnWidths[1], columnHeight), _font);
        col2.CachedBackground = _columnBackgrounds[1];
        col2.UpdateText("Min°");
        DrawableTextCell col3 = new(new RectangleF(col2.Rectangle.X + columnWidths[1], 0, columnWidths[2], columnHeight), _font);
        col3.CachedBackground = _columnBackgrounds[2];
        col3.UpdateText("Avg°");
        DrawableTextCell col4 = new(new RectangleF(col3.Rectangle.X + columnWidths[2], 0, columnWidths[3], columnHeight), _font);
        col4.CachedBackground = _columnBackgrounds[3];
        col4.UpdateText("Max°");
        _graphicsGrid.Grid[0][0] = lapTextCell;
        _graphicsGrid.Grid[0][1] = col2;
        _graphicsGrid.Grid[0][2] = col3;
        _graphicsGrid.Grid[0][3] = col4;

        this.Width = totalWidth + 1;
        this.Height = columnHeight * 5; // +1 for header

        StringFormat sf = new() { Alignment = StringAlignment.Center };
        // config data rows
        for (int row = 1; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                int x = columnWidths.Take(column).Sum();
                int y = row * columnHeight;
                int width = columnWidths[column];
                RectangleF rect = new(x, y, width, columnHeight);
                DrawableTextCell cell = new(rect, _font);
                if (column > 0) cell.StringFormat = sf;
                cell.CachedBackground = _columnBackgrounds[column];
                _graphicsGrid.Grid[row][column] = cell;

                string text = column > 0 ? string.Empty : TyreNames[row - 1];
                cell.UpdateText(text);
            }
        }
    }

    public override bool ShouldRender()
    {
        if (_config.Behavior.ShowInSetupScreen && pageGraphics.IsSetupMenuVisible)
            return true;

        return base.ShouldRender();
    }

    public sealed override void Render(Graphics g)
    {
        if (_tempHistory.Count == 0)
        {
            ClearData();
        }
        else
        {
            TyreTempHistoryModel last = _tempHistory[^1];
            lapTextCell.UpdateText($"L{last.Lap}");
            for (int i = 0; i < 4; i++)
            {
                AbstractDrawableCell[] row = _graphicsGrid.Grid[1 + i];

                DrawableTextCell min = (DrawableTextCell)row[1];
                min.CachedBackground = GetBackgroundSet(last.Min[i], last.TyreCompound)[1];
                min.UpdateText($"{last.Min[i]:F0}");

                DrawableTextCell avg = (DrawableTextCell)row[2];
                avg.CachedBackground = GetBackgroundSet(last.Averages[i], last.TyreCompound)[2];
                avg.UpdateText($"{last.Averages[i]:F0}");

                DrawableTextCell max = (DrawableTextCell)row[3];
                max.CachedBackground = GetBackgroundSet(last.Max[i], last.TyreCompound)[3];
                max.UpdateText($"{last.Max[i]:F0}");
            }
        }

        _graphicsGrid?.Draw(g);
    }

    private void ClearData()
    {
        lapTextCell.UpdateText("?");
        for (int row = 1; row <= 4; row++)
            for (int column = 1; column < 4; column++)
            {
                DrawableTextCell cell = (DrawableTextCell)_graphicsGrid.Grid[row][column];
                cell.CachedBackground = _columnBackgrounds[column];
                cell.UpdateText($"");
            }
    }

    private CachedBitmap[] GetBackgroundSet(float temp, string compound)
    {
        if (compound == "wet_compound")
        {
            if (temp > 65)
                return _columnBackgroundsRed;
            if (temp <= 24.9)
                return _columnBackgroundsBlue;
        }
        else
        {
            if (temp > 95)
                return _columnBackgroundsRed;
            if (temp < 75)
                return _columnBackgroundsBlue;
        }

        return _columnBackgrounds;
    }
}
