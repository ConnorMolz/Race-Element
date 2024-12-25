using RaceElement.HUD.Overlay.Configuration;
using RaceElement.HUD.Overlay.Internal;
using RaceElement.HUD.Overlay.OverlayUtil;
using RaceElement.HUD.Overlay.Util;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace RaceElement.HUD.ACC.Overlays.OverlayStartScreen;

[Overlay(Name = "Start Screen",
 Description = "Shows a start screen",
 Version = 1.00,
 OverlayType = OverlayType.Pitwall)]
public sealed class StartScreenOverlay : AbstractOverlay
{
    private readonly StartScreenConfig _config = new();
    private sealed class StartScreenConfig : OverlayConfiguration
    {
        public StartScreenConfig()
        {
            this.GenericConfiguration.AllowRescale = false;
        }
    }

    private string Version = "0.0.0.0";

    private GraphicsPath _clippingPath;
    private CachedBitmap _cachedBackground;
    private CachedBitmap _cachedText;
    private CachedBitmap _slider;
    private Stopwatch stopwatch;

    private const int SliderWidth = 280;
    private const float SliderPixelSpeed = 9f;
    private int sliderX = -SliderWidth;

    public StartScreenOverlay(Rectangle rectangle) : base(rectangle, "Start Screen")
    {
        if (rectangle.Width > 0 && (rectangle.X != 0 || rectangle.Y != 0)) // if there's no preset, lets not set it.
        {
            this.X = rectangle.X;
            this.Y = rectangle.Y;
        }

        this.Width = 610;
        this.Height = 72;
        this.RefreshRateHz = 60;
        this.SubscribeToACCData = false;
    }

    public override void BeforeStart()
    {
        this.Version = FileVersionInfo.GetVersionInfo(Environment.ProcessPath).FileVersion;

        float rounding = 19f;
        int intRounding = (int)rounding;
        _clippingPath = GraphicsExtensions.CreateRoundedRectangle(new(1, 0, Width - 2, Height - 1), intRounding, intRounding, intRounding, intRounding);

        _cachedBackground = new CachedBitmap(this.Width, this.Height, g =>
        {
            Rectangle rectangle = new(0, 0, Width - 1, Height - 1);
            using LinearGradientBrush solidBackgroundBrush = new(PointF.Empty, new PointF(Width, Height), Color.FromArgb(255, 0, 0, 0), Color.FromArgb(170, 8, 0, 0));
            using HatchBrush hatchBrush = new(HatchStyle.LightUpwardDiagonal, Color.FromArgb(3, Color.Red), Color.FromArgb(230, Color.Black));
            g.FillRoundedRectangle(solidBackgroundBrush, rectangle, intRounding);
            g.FillRoundedRectangle(hatchBrush, rectangle, intRounding);

            int crimpAmount = -1;
            Rectangle crimpRect = new(rectangle.X + crimpAmount, rectangle.Y + crimpAmount, rectangle.Width - crimpAmount * 2, rectangle.Height - crimpAmount * 2);

            using SolidBrush additionalBrush = new(Color.FromArgb(8, Color.OrangeRed));
            using Pen additionalPen = new(additionalBrush, 5f);
            g.DrawLine(additionalPen, new(0, 0), new(Width, 0));
        }, opacity: 1f);

        _cachedText = new CachedBitmap(this.Width, this.Height, g =>
        {
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.TextContrast = 1;

            using Matrix transform = g.Transform;
            transform.Shear(-0.3f, 0);
            transform.Translate(7f, 0);
            g.Transform = transform;

            string header = $"Race Element {Version}";
            using Font font16 = FontUtil.FontConthrax(32);
            int font16Height = (int)font16.GetHeight(g);
            int halfStringLength = (int)(g.MeasureString(header, font16).Width / 2);
            int x = this.Width / 2 - halfStringLength;

            g.DrawStringWithShadow(header, font16, Color.FromArgb(170, Color.Black), new PointF(x + 6.2f, 6.6f), 7f, new StringFormat() { LineAlignment = StringAlignment.Near });
            g.DrawStringWithShadow(header, font16, Color.FromArgb(170, Color.Red), new PointF(x + 2.7f, 3.7f), 7f, new StringFormat() { LineAlignment = StringAlignment.Near });
            g.DrawStringWithShadow(header, font16, Color.FromArgb(215, Color.White), new Point(x, 2), 5f, new StringFormat() { LineAlignment = StringAlignment.Near });

            font16.Dispose();

            transform.Shear(0.3f, 0);
            transform.Translate(-12f, -3f);
            g.Transform = transform;
            string subHeader = $"Solutions for Simulators © 2022 - {DateTime.UtcNow.Year} Reinier Klarenberg";
            using Font font11 = FontUtil.FontConthrax(10.8f);
            g.DrawStringWithShadow(subHeader, font11, Color.FromArgb(185, Color.Red), new PointF(x + 8f, font16Height + 0.5f));
            g.DrawStringWithShadow(subHeader, font11, Color.FromArgb(185, Color.White), new PointF(x + 7f, font16Height));
            font11.Dispose();
        }, opacity: 1);

        _slider = new CachedBitmap(SliderWidth, Height, g =>
        {
            RectangleF rect = new(0, 0, SliderWidth, Height / 1.2f);
            using GraphicsPath gradientPath = new();
            gradientPath.AddEllipse(rect);
            using PathGradientBrush pthGrBrush = new(gradientPath);
            pthGrBrush.SurroundColors = [Color.FromArgb(45, 255, 0, 0)];
            pthGrBrush.CenterColor = Color.FromArgb(255, 255, 0, 0);

            // draw diagonal lines
            using Pen pen = new(pthGrBrush, 1.15f);
            int spacing = 8;
            int lines = (int)Math.Floor(SliderWidth / (double)spacing);
            for (int i = 0; i < lines * 2; i++)
            {
                int baseX = -SliderWidth / 3 + i * spacing;
                g.DrawLine(pen, new Point(baseX, 0), new Point(baseX + SliderWidth / 2, Height));
            }

            float width = 28f;
            // draw top horizontal line
            PointF topLeft = new(SliderWidth / 2 - width / 2, 0);
            PointF topRight = new(SliderWidth / 2 + width / 2, 0);
            PointF innerTopLeft = new(SliderWidth / 2 - width / 7f, 0);
            PointF innerTopRight = new(SliderWidth / 2 + width / 7f, 0);

            using SolidBrush topLineBrush = new(Color.FromArgb(90, 255, 0, 0));
            using Pen topLinePen = new(topLineBrush, 2f);
            g.DrawLine(topLinePen, topLeft, topRight);
            g.DrawLine(topLinePen, innerTopLeft, innerTopRight);
        }, opacity: 1);

    }

    public override void BeforeStop()
    {
        _clippingPath?.Dispose();

        _cachedBackground?.Dispose();
        _cachedText?.Dispose();
        _slider?.Dispose();
        stopwatch?.Stop();
    }

    public override bool ShouldRender() => true;

    public override void Render(Graphics g)
    {
        g.SetClip(_clippingPath);

        _cachedBackground?.Draw(g);

        if (sliderX > Width - SliderPixelSpeed)
            sliderX = (int)(-SliderWidth + SliderPixelSpeed);
        _slider?.Draw(g, new Point(sliderX, 0));
        sliderX += (int)SliderPixelSpeed;

        _cachedText?.Draw(g);
    }
}
