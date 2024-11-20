﻿using RaceElement.Data.Common;
using RaceElement.HUD.Overlay.Internal;
using RaceElement.HUD.Overlay.OverlayUtil;
using RaceElement.HUD.Overlay.Util;
using RaceElement.Util.SystemExtensions;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace RaceElement.HUD.Common.Overlays.Driving.ShiftRpm;
[Overlay(
Name = "Shift RPM",
Description = "The current engine RPM as text")]
internal sealed class ShiftRpmOverlay(Rectangle rectangle) : CommonAbstractOverlay(rectangle, "Shift RPM")
{
    private readonly ShiftRpmConfiguration _config = new();

    private CachedBitmap _cachedBackground;
    private RpmBitmaps _bitmaps;

    public override void BeforeStart()
    {
        RefreshRateHz = _config.General.RefreshRate;

        _bitmaps = new RpmBitmaps(_config);
        Width = 5 * _bitmaps.Dimension.Width + _config.General.ExtraDigitSpacing * 4;
        Height = _bitmaps.Dimension.Height;

        _cachedBackground = new(Width, Height, g =>
        {
            RectangleF barArea = new(0, 0, Width - 1, Height - 1);

            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using SolidBrush darkBrush = new(Color.FromArgb(_config.Colors.BackgroundOpacity, _config.Colors.BackgroundColor));
            g.FillRoundedRectangle(darkBrush, Rectangle.Round(barArea), 3);
            using Pen darkPen = new(darkBrush, 1);
            g.DrawRoundedRectangle(darkPen, Rectangle.Round(barArea), 3);
        });
    }

    public override void BeforeStop()
    {
        _cachedBackground?.Dispose();
        _bitmaps?.Dispose();
    }

    public override void Render(Graphics g)
    {
        _cachedBackground?.Draw(g);

        int x = 0;

        int currentRpm = SimDataProvider.LocalCar.Engine.Rpm;
        currentRpm.Clip(0, 99_999);

        string s = $"{currentRpm}".FillStart(5, '0');

        for (int i = 0; i < 5; i++)
        {
            if (byte.TryParse(s.AsSpan(i, 1), out byte number))
                _bitmaps.GetForNumber(number).Draw(g, new(x, 0));

            x += _bitmaps.Dimension.Width + _config.General.ExtraDigitSpacing;
        }
    }
}

internal sealed class RpmBitmaps : IDisposable
{
    private readonly CachedBitmap[] _rpmBitmaps = new CachedBitmap[10];
    public readonly (int Width, int Height) Dimension;
    public RpmBitmaps(ShiftRpmConfiguration config)
    {
        GenerateBitMaps(config);
        Dimension = (_rpmBitmaps[0].Width, _rpmBitmaps[0].Height);
    }

    private void GenerateBitMaps(ShiftRpmConfiguration config)
    {
        Font font = config.General.Font switch
        {
            ShiftRpmConfiguration.RpmTextFont.Conthrax => FontUtil.FontConthrax(config.General.FontSize),
            ShiftRpmConfiguration.RpmTextFont.Obitron => FontUtil.FontOrbitron(config.General.FontSize),
            ShiftRpmConfiguration.RpmTextFont.Roboto => FontUtil.FontRoboto(config.General.FontSize),
            ShiftRpmConfiguration.RpmTextFont.Segoe => FontUtil.FontSegoeMono(config.General.FontSize),
            _ => FontUtil.FontConthrax(config.General.FontSize),
        };

        using StringFormat format = StringFormat.GenericDefault;
        format.Alignment = StringAlignment.Center;
        format.LineAlignment = StringAlignment.Center;

        int bitmapWidth = (int)(config.General.FontSize + 4);
        int bitmapHeight = bitmapWidth + 4;

        if (bitmapHeight < FontUtil.MeasureHeight(font, "0123456789"))
            bitmapHeight = (int)Math.Round(FontUtil.MeasureHeight(font, "0123456789"));

        for (int i = 0; i <= 9; i++)
            _rpmBitmaps[i] = new(bitmapWidth, bitmapHeight, g =>
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                using SolidBrush textBrush = new(Color.FromArgb(config.Colors.TextOpacity, config.Colors.TextColor));
                g.DrawStringWithShadow($"{i}", font, textBrush, new RectangleF(0, 2, bitmapWidth, bitmapHeight - 2), format);
            });
    }

    public CachedBitmap GetForNumber(byte number)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(number, 9);
        return _rpmBitmaps.AsSpan()[number];
    }

    public void Dispose()
    {
        foreach (CachedBitmap cachedBitmap in _rpmBitmaps)
            cachedBitmap?.Dispose();
    }
}
