﻿using RaceElement.Controls.Util.SetupImage;
using RaceElement.HUD.ACC;
using RaceElement.HUD.Overlay.Internal;
using RaceElement.HUD.Overlay.OverlayUtil;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RaceElement.Controls.HUD;

internal class PreviewCache
{
    internal static readonly Dictionary<string, CachedPreview> _cachedPreviews = [];
    private static readonly object[] DefaultOverlayArgs = [new System.Drawing.Rectangle((int)SystemParameters.PrimaryScreenWidth / 2, (int)SystemParameters.PrimaryScreenHeight / 2, 300, 150)];

    public class CachedPreview
    {
        public int Width;
        public int Height;
        public CachedBitmap CachedBitmap;
    }

    internal static void UpdatePreviewImage(ListView listOverlays, Image previewImage, string overlayName)
    {
        if (listOverlays.SelectedIndex >= 0)
        {
            ListViewItem lvi = (ListViewItem)listOverlays.SelectedItem;
            TextBlock tb = (TextBlock)lvi.Content;
            string actualOverlayName = overlayName.Replace("Overlay", "").Trim();
            if (tb.Text.Equals(actualOverlayName))
            {
                PreviewCache.GeneratePreview(actualOverlayName);
                PreviewCache._cachedPreviews.TryGetValue(actualOverlayName, out CachedPreview preview);
                if (preview != null)
                {
                    previewImage.Stretch = Stretch.UniformToFill;
                    previewImage.Width = preview.Width;
                    previewImage.Height = preview.Height;
                    previewImage.Source = ImageControlCreator.CreateImage(preview.Width + 1, preview.Height + 1, preview.CachedBitmap).Source;
                }
                else
                {
                    previewImage.Source = null;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="overlayName"></param>
    /// <param name="cached">Chooses to use a cached version if there is one</param>
    internal static void GeneratePreview(string overlayName, bool cached = false)
    {
        if (cached && _cachedPreviews.ContainsKey(overlayName))
            return;

        OverlaysAcc.AbstractOverlays.TryGetValue(overlayName, out Type overlayType);

        if (overlayType == null)
            return;

        AbstractOverlay abstractOverlay;
        try
        {
            abstractOverlay = (AbstractOverlay)Activator.CreateInstance(overlayType, DefaultOverlayArgs);
        }
        catch (Exception)
        {
            return;
        }

        if (abstractOverlay is ACCOverlay) {
            ACCOverlay overlay = (ACCOverlay) abstractOverlay;

            overlay.pageGraphics = ACCSharedMemory.Instance.ReadGraphicsPageFile(false);
            overlay.pageGraphics.NumberOfLaps = 30;
            overlay.pageGraphics.FuelXLap = 3.012f;
            overlay.pageGraphics.SessionType = ACCSharedMemory.AcSessionType.AC_RACE;
            overlay.pageGraphics.Status = ACCSharedMemory.AcStatus.AC_LIVE;
            overlay.pageGraphics.MandatoryPitDone = false;
            overlay.pageGraphics.NormalizedCarPosition = 0.0472972f;
            //overlay.pageGraphics.DeltaLapTimeMillis = -0137;
            overlay.pageGraphics.IsValidLap = true;
            overlay.pageGraphics.WindDirection = 0.1f;
            overlay.pageGraphics.WindSpeed = 16.92f;
            overlay.pageGraphics.ExhaustTemperature = 325.24f;
            overlay.pageGraphics.currentTyreSet = 3;

            overlay.pagePhysics = ACCSharedMemory.Instance.ReadPhysicsPageFile(false);
            overlay.pagePhysics.SpeedKmh = 272.32f;
            overlay.pagePhysics.Fuel = 76.07f;
            overlay.pagePhysics.Rpms = 8500;
            overlay.pagePhysics.Gear = 3;
            overlay.pagePhysics.WheelPressure = [27.61f, 27.56f, 26.94f, 26.13f];
            overlay.pagePhysics.TyreCoreTemperature = [102.67f, 88.51f, 74.92f, 67.23f];
            overlay.pagePhysics.PadLife = [24f, 24f, 25f, 25f];
            overlay.pagePhysics.BrakeTemperature = [300f, 250f, 450f, 460f];
            overlay.pagePhysics.Gas = 0.78f;
            overlay.pagePhysics.Brake = 0.133f;
            overlay.pagePhysics.SteerAngle = 0.053f;
            overlay.pagePhysics.BrakeBias = 0.88f;
            overlay.pagePhysics.WaterTemp = 98.3f;
            overlay.pagePhysics.RoadTemp = 29.826f;
            overlay.pagePhysics.AirTemp = 36.2326f;

            overlay.pageStatic = ACCSharedMemory.Instance.ReadStaticPageFile(false);
            overlay.pageStatic.MaxFuel = 120f;
            overlay.pageStatic.MaxRpm = 9250;
            overlay.pageStatic.CarModel = "porsche_991ii_gt3_r";
        }

        abstractOverlay.SetupPreviewData();
        abstractOverlay.IsPreviewing = true;

        try
        {
            abstractOverlay.BeforeStart();
            CachedPreview cachedPreview = new()
            {
                Width = abstractOverlay.Width,
                Height = abstractOverlay.Height,
                CachedBitmap = new CachedBitmap(abstractOverlay.Width, abstractOverlay.Height, g => abstractOverlay.Render(g))
            };

            if (_cachedPreviews.ContainsKey(overlayName))
                _cachedPreviews[overlayName] = cachedPreview;
            else
                _cachedPreviews.Add(overlayName, cachedPreview);

            abstractOverlay.BeforeStop();
        }
        catch (Exception) { }
        finally
        {
            abstractOverlay.Dispose();
            abstractOverlay = null;
        }
    }


}
