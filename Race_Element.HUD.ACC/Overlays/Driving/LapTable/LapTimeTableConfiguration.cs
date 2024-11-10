﻿using RaceElement.HUD.Overlay.Configuration;

namespace RaceElement.HUD.ACC.Overlays.OverlayLapTimeTable;

internal sealed class LapTimeTableConfiguration : OverlayConfiguration
{
    [ConfigGrouping("Table", "Change the behavior of the table")]
    public TableGrouping Table { get; init; } = new();
    public sealed class TableGrouping
    {
        [ToolTip("Display Columns with sector times for each lap in the table.")]
        public bool ShowSectors { get; init; } = true;

        [ToolTip("Change the amount of visible rows in the table.")]
        [IntRange(1, 12, 1)]
        public int Rows { get; init; } = 4;

        [ToolTip("Changed the amount of corner roundering for each cell in the table.")]
        [IntRange(0, 12, 2)]
        public int Roundness { get; init; } = 2;
    }

    [ConfigGrouping("Behavior", "Adjust behavorial settings")]
    public BehaviorGrouping Behavior { get; init; } = new();
    public sealed class BehaviorGrouping
    {
        public bool HideInRace { get; init; } = false;
    }

    public LapTimeTableConfiguration() => GenericConfiguration.AllowRescale = true;
}
