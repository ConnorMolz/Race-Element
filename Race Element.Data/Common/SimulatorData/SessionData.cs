﻿using System.Collections.Concurrent;

namespace RaceElement.Data.Common.SimulatorData;

public sealed record SessionData
{
    public WeatherConditions Weather { get; set; } = new();
    public TrackData Track { get; set; } = new();
    public sealed record TrackData
    {
        /// <summary>
        /// The track name based on the way the game provides it
        /// </summary>
        public string GameName { get; internal set; } = string.Empty;

        /// <summary>
        /// The track temperature in celsius
        /// </summary>
        public float Temperature { get; internal set; }
        public int Length { get; internal set; }
    }

    public sealed record WeatherConditions
    {
        /// <summary>
        /// The air temperature in celsius
        /// </summary>
        public float AirTemperature { get; internal set; }
        public float AirPressure { get; internal set; }

        /// <summary>
        /// The speed of the air in km/h
        /// </summary>
        public float AirVelocity { get; internal set; }

        /// <summary>
        /// The direction of the air in radians
        /// </summary>
        public float AirDirection { get; internal set; }
    }

    // ------------- TODO Refactor and rewrite code.

    // car index -> car info dictionary
    internal readonly ConcurrentDictionary<int, CarInfo> _entryListCars = [];
    public List<KeyValuePair<int, CarInfo>> Cars
    {
        get
        {
            return [.. _entryListCars];
        }
    }


    public void AddOrUpdateCar(int carIndex, CarInfo car)
    {
        _entryListCars.AddOrUpdate(carIndex, car, (key, oldCarInfo) => car);
    }

    private static SessionData _instance;
    public static SessionData Instance
    {
        get
        {
            _instance ??= new SessionData();
            return _instance;
        }
    }

    /// <summary>
    /// Car index of the player. This is game provided that can differ from
    /// session to session. E.g. not the race car number.
    /// </summary>
    public int PlayerCarIndex { get; internal set; }
    /// <summary>
    /// Car index that is focused on. E.g. in a replay or in the pit.
    /// </summary>
    public int FocusedCarIndex { get; set; }
    public RaceSessionType SessionType { get; set; }
    public SessionPhase Phase { get; set; }
    public float LapDeltaToSessionBestLapMs { get; set; }

    public bool IsSetupMenuVisible { get; set; }
    public double SessionTimeLeftSecs { get; set; }
    
    public CurrentFlag CurrentFlag { get; set; }
}


public enum RaceSessionType
{
    Practice = 0,
    Qualifying = 4,
    Superpole = 9,
    Race = 10,
    Hotlap = 11,
    Hotstint = 12,
    HotlapSuperpole = 13,
    Replay = 14
};
public enum SessionPhase
{
    NONE = 0,
    Starting = 1,
    PreFormation = 2,
    FormationLap = 3,
    PreSession = 4,
    Session = 5,
    SessionOver = 6,
    PostSession = 7,
    ResultUI = 8
};

public enum Status : int
{
    OFF,
    REPLAY,
    LIVE,
    PAUSE,
}

public enum CurrentFlag : int
{
    None,
    Green,
    Blue,
    Yellow,
    Red,
    Black,
    White,
    Checkered,
    Finish,
    Max
}
