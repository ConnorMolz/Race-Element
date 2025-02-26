﻿using RaceElement.HUD.Overlay.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Input;

namespace RaceElement.HUD.ACC;

public sealed class OverlaysAcc
{
    private static readonly Lock _lock = new();

    public static readonly SortedDictionary<string, Type> AbstractOverlays = [];
    public static readonly List<CommonAbstractOverlay> ActiveOverlays = [];

    protected OverlaysAcc() { }

    public static void GenerateDictionary()
    {
        AbstractOverlays.Clear();

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsDefined(typeof(OverlayAttribute))))
        {
            var overlayType = type.GetCustomAttribute<OverlayAttribute>();
            if (overlayType != null && !AbstractOverlays.ContainsKey(overlayType.Name))
                AbstractOverlays.Add(overlayType.Name, type);
        }
    }

    public static void CloseAll()
    {
        var closeAllSTa = new Thread(() =>
        {
            Mouse.SetCursor(Cursors.Wait);
            int activeCount = AbstractOverlays.Count;

            lock (_lock)
                while (ActiveOverlays.Count > 0)
                {
                    ActiveOverlays[0].EnableReposition(false);
                    ActiveOverlays[0].Stop();
                    ActiveOverlays.Remove(ActiveOverlays[0]);
                }

            if (activeCount > 0)
                Thread.Sleep(500);


            Mouse.SetCursor(Cursors.Wait);
        });
        closeAllSTa.SetApartmentState(ApartmentState.STA);
        closeAllSTa.Start();
        closeAllSTa.Join();
    }
}
