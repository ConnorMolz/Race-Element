﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace RaceElement.HUD.Overlay.OverlayUtil;

public static class GraphicsExtensions
{
    #region // Draw String With Shadow in at Location
    public static void DrawStringWithShadow(this Graphics g, string text, Font font, Color color, PointF location, StringFormat format = null)
    {
        DrawStringWithShadow(g, text, font, color, Color.FromArgb(60, Color.Black), location, 0.75f, format);
    }

    public static void DrawStringWithShadow(this Graphics g, string text, Font font, Brush brush, PointF location, StringFormat format = null)
    {
        DrawStringWithShadow(g, text, font, brush, Color.FromArgb(60, Color.Black), location, 0.75f, format);
    }

    public static void DrawStringWithShadow(this Graphics g, string text, Font font, Color color, PointF location, float shadowDistance, StringFormat format = null)
    {
        DrawStringWithShadow(g, text, font, color, Color.FromArgb(60, Color.Black), location, shadowDistance, format);
    }

    public static void DrawStringWithShadow(this Graphics g, string text, Font font, Color color, Color shadowColor, PointF location, StringFormat format = null)
    {
        DrawStringWithShadow(g, text, font, color, shadowColor, location, 0.75f, format);
    }

    public static void DrawStringWithShadow(this Graphics g, string text, Font font, Brush brush, Color shadowColor, PointF location, float shadowDistance, StringFormat format = null)
    {
        if (format == null)
        {
            format ??= StringFormat.GenericDefault;
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Near;
        }

        using SolidBrush shadowBrush = new(shadowColor);
        g.DrawString(text, font, shadowBrush, new PointF(location.X + shadowDistance, location.Y + shadowDistance), format);
        g.DrawString(text, font, brush, location, format);
    }

    public static void DrawStringWithShadow(this Graphics g, string text, Font font, Color color, Color shadowColor, PointF location, float shadowDistance, StringFormat format = null)
    {
        if (format == null)
        {
            format ??= StringFormat.GenericDefault;
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Near;
        }

        using SolidBrush shadowBrush = new(shadowColor);
        using SolidBrush foregroundBrush = new(color);
        g.DrawString(text, font, shadowBrush, new PointF(location.X + shadowDistance, location.Y + shadowDistance), format);
        g.DrawString(text, font, foregroundBrush, location, format);
    }
    #endregion


    #region // Draw String With Shadow in Rectangle.
    public static void DrawStringWithShadow(this Graphics g, string text, Font font, Color color, RectangleF rectangle, StringFormat format = null)
    {
        DrawStringWithShadow(g, text, font, color, Color.FromArgb(60, Color.Black), rectangle, 0.75f, format);
    }

    public static void DrawStringWithShadow(this Graphics g, string text, Font font, Brush brush, RectangleF rectangle, StringFormat format = null)
    {
        using SolidBrush shadowBrush = new(Color.FromArgb(60, Color.Black));
        DrawStringWithShadow(g, text, font, brush, shadowBrush, rectangle, 0.75f, format);
    }

    public static void DrawStringWithShadow(this Graphics g, string text, Font font, Color color, Color shadowColor, RectangleF rectangle, float shadowDistance, StringFormat format = null)
    {
        if (format == null)
        {
            format ??= StringFormat.GenericDefault;
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Near;
        }

        rectangle.Y += shadowDistance;
        using SolidBrush shadowBrush = new(shadowColor);
        g.DrawString(text, font, shadowBrush, rectangle, format);

        rectangle.Y -= shadowDistance;
        using SolidBrush foregroundBrush = new(color);
        g.DrawString(text, font, foregroundBrush, rectangle, format);
    }

    public static void DrawStringWithShadow(this Graphics g, string text, Font font, Brush color, Brush shadowColor, RectangleF rectangle, float shadowDistance, StringFormat format = null)
    {
        if (format == null)
        {
            format ??= StringFormat.GenericDefault;
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Near;
        }

        rectangle.Y += shadowDistance;
        g.DrawString(text, font, shadowColor, rectangle, format);
        rectangle.Y -= shadowDistance;
        g.DrawString(text, font, color, rectangle, format);
    }
    #endregion

    public static void DrawEllipse(this Graphics graphics, Pen pen,
                              float centerX, float centerY, float radius)
    {
        if (graphics == null)
            throw new ArgumentNullException("graphics");
        if (pen == null)
            throw new ArgumentNullException("pen");

        graphics.DrawEllipse(pen, centerX - radius, centerY - radius,
                  radius + radius, radius + radius);
    }

    public static void FillEllipse(this Graphics graphics, Brush brush,
                              float centerX, float centerY, float radius)
    {
        if (graphics == null)
            throw new ArgumentNullException("graphics");
        if (brush == null)
            throw new ArgumentNullException("pen");

        graphics.FillEllipse(brush, centerX - radius, centerY - radius,
                      radius + radius, radius + radius);
    }

    public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
    {
        if (graphics == null)
            throw new ArgumentNullException("graphics");
        if (pen == null)
            throw new ArgumentNullException("pen");

        using (GraphicsPath path = CreateRoundedRectangle(bounds, cornerRadius))
        {
            graphics.DrawPath(pen, path);
        }
    }

    public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
    {
        if (graphics == null)
            throw new ArgumentNullException("graphics");
        if (brush == null)
            throw new ArgumentNullException("brush");

        using (GraphicsPath path = CreateRoundedRectangle(bounds, cornerRadius))
        {
            graphics.FillPath(brush, path);
        }
    }

    /// <summary>
    /// Returns the path for a rounded rectangle specified by a bounding <see cref="Rectangle"/> structure and four corner radius values.
    /// </summary>
    /// <param name="bounds">A <see cref="Rectangle"/> structure that bounds the rounded rectangle.</param>
    /// <param name="radiusTopLeft">Size of the top-left radius.</param>
    /// <param name="radiusTopRight">Size of the top-right radius.</param>
    /// <param name="radiusBottomRight">Size of the bottom-right radius.</param>
    /// <param name="radiusBottomLeft">Size of the bottom-left radius.</param>
    public static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radiusTopLeft, int radiusTopRight, int radiusBottomRight, int radiusBottomLeft)
    {
        var size = new Size(radiusTopLeft, radiusTopLeft);
        var arc = new Rectangle(bounds.Location, size);
        var path = new GraphicsPath();

        // top left arc
        if (radiusTopLeft == 0)
            path.AddLine(arc.Location, arc.Location);
        else
            path.AddArc(arc, 180, 90);

        // top right arc
        if (radiusTopRight != radiusTopLeft)
        {
            size = new Size(radiusTopRight, radiusTopRight);
            arc.Size = size;
        }

        arc.X = bounds.Right - size.Width;
        if (radiusTopRight == 0)
            path.AddLine(arc.Location, arc.Location);
        else
            path.AddArc(arc, 270, 90);

        // bottom right arc
        if (radiusTopRight != radiusBottomRight)
        {
            size = new Size(radiusBottomRight, radiusBottomRight);
            arc.X = bounds.Right - size.Width;
            arc.Size = size;
        }

        arc.Y = bounds.Bottom - size.Height;
        if (radiusBottomRight == 0)
            path.AddLine(arc.Location, arc.Location);
        else
            path.AddArc(arc, 0, 90);

        // bottom left arc
        if (radiusBottomRight != radiusBottomLeft)
        {
            arc.Size = new Size(radiusBottomLeft, radiusBottomLeft);
            arc.Y = bounds.Bottom - arc.Height;
        }

        arc.X = bounds.Left;
        if (radiusBottomLeft == 0)
            path.AddLine(arc.Location, arc.Location);
        else
            path.AddArc(arc, 90, 90);

        path.CloseFigure();
        return path;
    }

    #region Private Methods

    /// <summary>
    /// Returns the path for a rounded rectangle specified by a bounding <see cref="Rectangle"/> structure and a common corner radius value for each corners.
    /// </summary>
    /// <param name="bounds">A <see cref="Rectangle"/> structure that bounds the rounded rectangle.</param>
    /// <param name="radius">Size of the corner radius for each corners.</param>
    private static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        if (radius == 0)
        {
            path.AddRectangle(bounds);
            return path;
        }

        int diameter = radius * 2;
        var size = new Size(diameter, diameter);
        var arc = new Rectangle(bounds.Location, size);

        // top left arc
        path.AddArc(arc, 180, 90);

        // top right arc
        arc.X = bounds.Right - diameter;
        path.AddArc(arc, 270, 90);

        // bottom right arc
        arc.Y = bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);

        // bottom left arc
        arc.X = bounds.Left;
        path.AddArc(arc, 90, 90);

        path.CloseFigure();
        return path;
    }

    #endregion
}
