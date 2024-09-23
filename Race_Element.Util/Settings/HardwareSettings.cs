﻿namespace RaceElement.Util.Settings;

public sealed class HardwareSettingsJson : IGenericSettingsJson
{
    public bool UseHardwareSteeringLock = false;
}

public sealed class HardwareSettings : AbstractSettingsJson<HardwareSettingsJson>
{
    public override string Path => FileUtil.RaceElementSettingsPath;

    public override string FileName => "Hardware.json";

    public override HardwareSettingsJson Default()
    {
        var settings = new HardwareSettingsJson()
        {
            UseHardwareSteeringLock = false
        };
        return settings;
    }
}
