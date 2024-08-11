﻿using RaceElement.Broadcast;
using RaceElement.Broadcast.Structs;
using RaceElement.Data;
using RaceElement.Data.ACC.EntryList;
using RaceElement.Data.ACC.EntryList.TrackPositionGraph;
using RaceElement.Data.ACC.Session;
using RaceElement.HUD.Overlay.Configuration;
using RaceElement.HUD.Overlay.Internal;
using RaceElement.HUD.Overlay.OverlayUtil;
using RaceElement.HUD.Overlay.Util;
using RaceElement.Util.SystemExtensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using static RaceElement.Data.ACC.EntryList.EntryListTracker;
using static RaceElement.Data.ACC.Tracks.TrackData;
using static RaceElement.Data.SetupConverter;
using static RaceElement.HUD.Overlay.OverlayUtil.InfoTable;

namespace RaceElement.HUD.ACC.Overlays.OverlayDebugInfo.OverlayEntryList;

//#if DEBUG
[Overlay(Name = "Entrylist Overlay", Version = 1.00, OverlayType = OverlayType.Pitwall,
Description = "(BETA) A table representing a leaderboard.")]
//#endif
internal sealed class EntryListOverlay : AbstractOverlay
{
    private readonly EntryListDebugConfig _config = new();
    private sealed class EntryListDebugConfig : OverlayConfiguration
    {
        [ConfigGrouping("EntryList", "Provides settings for overlay docking.")]
        public EntryListGrouping Entrylist { get; set; } = new EntryListGrouping();
        public sealed class EntryListGrouping
        {
            [ToolTip("Show extended data, adds a new row for each car.")]
            public bool ExtendedData { get; set; } = false;
        }

        [ConfigGrouping("Dock", "Provides settings for overlay docking.")]
        public DockConfigGrouping Dock { get; set; } = new DockConfigGrouping();
        public class DockConfigGrouping
        {
            [ToolTip("Allows you to reposition this debug panel.")]
            public bool Undock { get; set; } = false;
        }

        public EntryListDebugConfig()
        {
            GenericConfiguration.AllowRescale = true;
        }
    }

    private readonly InfoTable _table;

    private readonly Color Gt3Color = Color.FromArgb(255, Color.Black);
    private readonly Color Gt4Color = Color.FromArgb(255, 24, 24, 72);
    private readonly Color CupColor = Color.FromArgb(255, 30, 61, 26);
    private readonly Color TcxColor = Color.FromArgb(255, 0, 96, 136);
    private readonly Color StColor = Color.FromArgb(255, 0, 96, 136);
    private readonly Color ChlColor = Color.FromArgb(255, 112, 110, 0);

    public EntryListOverlay(Rectangle rect) : base(rect, "Entrylist Overlay")
    {
        this.AllowReposition = false;
        this.RefreshRateHz = 4;

        float fontSize = 9;
        var font = FontUtil.FontSegoeMono(fontSize);
        _table = new InfoTable(fontSize, [(int)(font.Size * 18), (int)(font.Size * 9), (int)(font.Size * 10), (int)(font.Size * 30)]);

        this.Width = 650;
        this.Height = 500;
    }

    private void Instance_WidthChanged(object sender, bool e)
    {
        if (e)
            this.X = DebugInfoHelper.Instance.GetX(this);
    }

    public sealed override void BeforeStart()
    {
        if (this._config.Dock.Undock)
            this.AllowReposition = true;
        else
        {
            DebugInfoHelper.Instance.WidthChanged += Instance_WidthChanged;
            DebugInfoHelper.Instance.AddOverlay(this);
            this.X = DebugInfoHelper.Instance.GetX(this);
            this.Y = 0;
        }
    }

    public sealed override void BeforeStop()
    {
        if (!this._config.Dock.Undock)
        {
            DebugInfoHelper.Instance.RemoveOverlay(this);
            DebugInfoHelper.Instance.WidthChanged -= Instance_WidthChanged;
        }

    }

    public sealed override void Render(Graphics g)
    {
        // table titles
        switch (broadCastRealTime.SessionType)
        {
            case RaceSessionType.Race:
                {
                    _table.AddRow("P", ["#    Driver", $"Previous", "Interval", "Lap| Turn"]);
                    break;
                }
            default:
                {
                    _table.AddRow("P", ["#    Driver", $"Previous", "Gap", "Lap| Turn"]);
                    break;
                }
        }

        List<KeyValuePair<int, CarData>> cars = EntryListTracker.Instance.Cars;

        if (cars.Count == 0)
            return;

        SortEntryList(cars);

        CarData firstCar = GetFirstPositionCar();
        Car carAhead = null;

        if (firstCar.CarInfo != null)
            carAhead = PositionGraph.Instance.GetCar(firstCar.CarInfo.CarIndex);


        foreach (KeyValuePair<int, CarData> kv in cars)
        {
            if (kv.Value.CarInfo != null)
            {
                AddCarFirstRow(kv, carAhead);

                if (_config.Entrylist.ExtendedData)
                {
                    string speed = $"{kv.Value.RealtimeCarUpdate.Kmh} km/h".FillStart(8, ' ');

                    string distanceText = string.Empty;

                    if (carAhead != null)
                    {
                        if (kv.Value.RealtimeCarUpdate.Position != 1 && kv.Key != carAhead.CarIndex)
                        {
                            var carAheadData = EntryListTracker.Instance.GetCarData(carAhead.CarIndex);
                            if (kv.Value.RealtimeCarUpdate.Laps == carAheadData.RealtimeCarUpdate.Laps)
                            {
                                float timeGapToAhead = GapTracker.Instance.TimeGapBetween(kv.Key, kv.Value.RealtimeCarUpdate.SplinePosition, carAhead.CarIndex);
                                distanceText = $" +{timeGapToAhead:F3}";
                            }
                            else
                            {
                                distanceText = $"+{carAheadData.RealtimeCarUpdate.Laps - kv.Value.RealtimeCarUpdate.Laps}L";
                            }
                        }

                    }

                    string currentLap = "|----- NO LAP";

                    if (kv.Value.RealtimeCarUpdate.CurrentLap != null)
                        currentLap = kv.Value.RealtimeCarUpdate.CurrentLap.LaptimeMS.HasValue ? $"|----- {kv.Value.RealtimeCarUpdate.CurrentLap.LaptimeMS.Value / 1000}" : "|----- ";
                    _table.AddRow(String.Empty, [String.Empty, $"{distanceText}", speed, currentLap]);
                }
                carAhead = PositionGraph.Instance.GetCar(kv.Value.CarInfo.CarIndex);

            }
        }

        _table._headerWidthSet = false;

        _table.Draw(g);
    }

    private void AddCarFirstRow(KeyValuePair<int, CarData> kv, Car carAhead)
    {
        string[] firstRow = [String.Empty, String.Empty, String.Empty, String.Empty, String.Empty];
        Color[] firstRowColors = [Color.White, Color.White, Color.White, Color.White, Color.White];

        DriverInfo currentDriver = kv.Value.CarInfo.Drivers[kv.Value.CarInfo.CurrentDriverIndex];
        firstRow[0] = $"{kv.Value.CarInfo.RaceNumber.ToString().FillEnd(5, ' ')}";
        string firstName = currentDriver.FirstName;
        if (firstName.Length > 0) firstName = firstName.First() + "";
        string name = $"{firstName}. {currentDriver.LastName}";
        firstRow[0] += new string(name.Take(15).ToArray());


        int bestSessionLapMS = -1;
        if (broadCastRealTime.BestSessionLap != null)
            bestSessionLapMS = broadCastRealTime.BestSessionLap.LaptimeMS.GetValueOrDefault(-1);
        switch (broadCastRealTime.SessionType)
        {
            case RaceSessionType.Race:
                {
                    if (kv.Value.RealtimeCarUpdate.LastLap != null)
                        if (kv.Value.RealtimeCarUpdate.LastLap.LaptimeMS.HasValue)
                        {
                            if (broadCastRealTime.BestSessionLap != null)
                                if (kv.Value.RealtimeCarUpdate.LastLap.LaptimeMS == bestSessionLapMS)
                                    firstRowColors[1] = Color.FromArgb(255, 207, 97, 255);

                            TimeSpan fastestLapTime = TimeSpan.FromMilliseconds(kv.Value.RealtimeCarUpdate.LastLap.LaptimeMS.Value);
                            firstRow[1] = $"{fastestLapTime:mm\\:ss\\.fff}";
                        }
                        else
                            firstRow[1] = $"--:--.---";

                    if (carAhead != null)
                    {
                        if (kv.Value.RealtimeCarUpdate.Position != 1 && kv.Key != carAhead.CarIndex)
                        {
                            var carAheadData = EntryListTracker.Instance.GetCarData(carAhead.CarIndex);
                            if (kv.Value.RealtimeCarUpdate.Laps == carAheadData.RealtimeCarUpdate.Laps)
                            {
                                float timeGapToAhead = GapTracker.Instance.TimeGapBetween(kv.Key, kv.Value.RealtimeCarUpdate.SplinePosition, carAhead.CarIndex);
                                if (timeGapToAhead > 0)
                                    firstRow[2] = $"+{timeGapToAhead:F3}";

                                if (kv.Value.RealtimeCarUpdate.CarLocation != CarLocationEnum.Pitlane)
                                {
                                    firstRowColors[2] = timeGapToAhead switch
                                    {
                                        <= .05f => Color.Red,
                                        > .05f and <= .15f => Color.Orange,
                                        > .15f and < .4f => Color.Yellow,

                                        _ => Color.White,
                                    }; ;
                                }
                            }
                            else
                            {
                                var firstPostionCar = EntryListTracker.Instance.Cars.Where(x => x.Value.RealtimeCarUpdate.Position == 1).FirstOrDefault();
                                if (firstPostionCar.Value != null)
                                {
                                    int lapDiff = firstPostionCar.Value.RealtimeCarUpdate.Laps - kv.Value.RealtimeCarUpdate.Laps;
                                    firstRow[2] = $"+{lapDiff} Lap{(lapDiff > 1 ? "s" : "")}";
                                }
                            }
                        }
                        else
                        {
                            firstRow[2] = $"Lap {kv.Value.RealtimeCarUpdate.Laps}";
                        }
                    }
                    break;
                }

            case RaceSessionType.Practice:
            case RaceSessionType.Qualifying:
                {
                    if (kv.Value.RealtimeCarUpdate.BestSessionLap != null)
                        if (kv.Value.RealtimeCarUpdate.BestSessionLap.LaptimeMS.HasValue)
                        {
                            if (broadCastRealTime.BestSessionLap != null)
                                if (kv.Value.RealtimeCarUpdate.LastLap.LaptimeMS == bestSessionLapMS)
                                    firstRowColors[1] = Color.FromArgb(255, 207, 97, 255);

                            TimeSpan fastestLapTime = TimeSpan.FromMilliseconds(kv.Value.RealtimeCarUpdate.BestSessionLap.LaptimeMS.Value);
                            firstRow[1] = $"{fastestLapTime:mm\\:ss\\.fff}";

                            var firstPostionCar = EntryListTracker.Instance.Cars.Where(x => x.Value.RealtimeCarUpdate.Position == 1).FirstOrDefault();
                            if (firstPostionCar.Value != null && kv.Key != firstPostionCar.Key)
                            {
                                var timeDiff = TimeSpan.FromMilliseconds(firstPostionCar.Value.RealtimeCarUpdate.BestSessionLap.LaptimeMS.Value).Subtract(fastestLapTime);
                                firstRow[2] = $"+{timeDiff:s\\.fff}";
                            }
                        }
                        else
                        {
                            if (kv.Value.RealtimeCarUpdate.LastLap != null && kv.Value.RealtimeCarUpdate.LastLap.LaptimeMS.HasValue)
                            {
                                TimeSpan fastestLapTime = TimeSpan.FromMilliseconds(kv.Value.RealtimeCarUpdate.LastLap.LaptimeMS.Value);
                                firstRow[1] = $"{fastestLapTime:mm\\:ss\\.fff}";
                                firstRowColors[1] = Color.OrangeRed;
                            }
                            else
                            {
                                firstRow[1] = $"--:--.---";
                            }
                        }
                    break;
                }
            default: break;
        }

        switch (kv.Value.RealtimeCarUpdate.CarLocation)
        {
            case CarLocationEnum.PitEntry:
                {
                    firstRow[2] += " (PI)";
                    break;
                }
            case CarLocationEnum.PitExit:
                {
                    firstRow[2] += " (PE)";
                    break;
                }
            case CarLocationEnum.Pitlane:
                {
                    firstRow[2] += " (P)";
                    break;
                }

            case CarLocationEnum.Track:
                {
                    //firstRow[2] = $"{kv.Value.RealtimeCarUpdate.Delta / 1000f:F2}".FillStart(6, ' ');
                    //firstRowColors[2] = kv.Value.RealtimeCarUpdate.Delta > 0 ? Color.OrangeRed : Color.LimeGreen;

                    //if (kv.Value.RealtimeCarUpdate.BestSessionLap.LaptimeMS.HasValue && broadCastRealTime.BestSessionLap != null)
                    //{
                    //    if (kv.Value.RealtimeCarUpdate.BestSessionLap.LaptimeMS.Value + kv.Value.RealtimeCarUpdate.Delta < broadCastRealTime.BestSessionLap.LaptimeMS)
                    //    {
                    //        // purple if delta is faster than server best lap
                    //        firstRowColors[2] = Color.FromArgb(255, 207, 97, 255);
                    //    }
                    //}

                    firstRow[3] = $"L{kv.Value.RealtimeCarUpdate.Laps + 1} | ";

                    var matchingTracks = Tracks.Where(x => x.FullName == broadCastTrackData.TrackName).ToList();
                    if (matchingTracks.Any())
                    {
                        var currentTrack = matchingTracks[0];
                        if (currentTrack.CornerNames.Any())
                        {
                            var items = currentTrack.CornerNames.Where(x => x.Key.IsInRange(kv.Value.RealtimeCarUpdate.SplinePosition));
                            if (items.Any() && items.First().Value.Item1 != 0)
                            {
                                (int, string) corner = items.First().Value;
                                if (corner.Item1 != 0)
                                {
                                    StringBuilder builder = new();
                                    builder.Append("T");
                                    builder.Append(corner.Item1.ToString().FillEnd(2, ' ') + ' ');
                                    builder.Append(corner.Item2);
                                    firstRow[3] += $"{builder}";
                                }
                            }
                        }
                    }
                    break;
                }
            default: break;
        }

        var carModel = ConversionFactory.GetCarModels(kv.Value.CarInfo.CarModelType);
        var carClass = ConversionFactory.GetConversion(carModel).CarClass;

        Color headerBackgroundColor = Color.FromArgb(90, Color.Black);
        switch (carClass)
        {
            case CarClasses.GT3:
                headerBackgroundColor = Gt3Color;
                break;
            case CarClasses.GT4:
                headerBackgroundColor = Gt4Color;
                break;
            case CarClasses.CUP:
                headerBackgroundColor = CupColor;
                break;
            case CarClasses.ST:
                headerBackgroundColor = StColor;
                break;
            case CarClasses.TCX:
                headerBackgroundColor = TcxColor;
                break;
            case CarClasses.CHL:
                headerBackgroundColor = ChlColor;
                break;
            default:
                break;

        }

        string cupPosition = $"{kv.Value.RealtimeCarUpdate.Position}";    // changed to position, Cup position is working againt since start of May.
        TableRow row = new()
        {
            Header = $"{cupPosition}".FillStart(3, ' '),
            Columns = firstRow,
            ColumnColors = firstRowColors,
            HeaderBackground = headerBackgroundColor
        };


        // config??
        //if (kv.Key == pageGraphics.PlayerCarID)
        //    row.HeaderBackground = Color.FromArgb(120, Color.Red);
        //else
        if (kv.Key == broadCastRealTime.FocusedCarIndex) row.HeaderBackground = Color.FromArgb(90, Color.Red);

        _table.AddRow(row);
    }

    private void SortEntryList(List<KeyValuePair<int, CarData>> cars)
    {

        switch (broadCastRealTime.SessionType)
        {
            case RaceSessionType.Race:
                {
                    switch (broadCastRealTime.Phase)
                    {
                        case SessionPhase.SessionOver:
                        case SessionPhase.PreSession:
                        case SessionPhase.PreFormation:
                            {
                                cars.Sort((a, b) =>
                                {
                                    if (a.Value.CarInfo == null)
                                        return -1;

                                    if (b.Value.CarInfo == null)
                                        return 1;


                                    if (a.Value.RealtimeCarUpdate.Position == b.Value.RealtimeCarUpdate.Position)
                                        return a.Value.CarInfo.RaceNumber.CompareTo(b.Value.CarInfo.RaceNumber);

                                    return a.Value.RealtimeCarUpdate.Position.CompareTo(b.Value.RealtimeCarUpdate.Position);
                                });
                                break;
                            }
                        default:
                            {
                                cars.Sort((a, b) =>
                                {
                                    if (a.Value.RealtimeCarUpdate.Position == b.Value.RealtimeCarUpdate.Position) return 1;
                                    if (a.Value.CarInfo == null || b.Value.CarInfo == null)
                                        return -1;

                                    if (a.Value.RealtimeCarUpdate.Laps > b.Value.RealtimeCarUpdate.Laps)
                                        return 1;

                                    Car carCarA = PositionGraph.Instance.GetCar(a.Value.CarInfo.CarIndex);
                                    Car carCarb = PositionGraph.Instance.GetCar(b.Value.CarInfo.CarIndex);

                                    if (carCarA == null) return -1;
                                    if (carCarb == null) return 1;

                                    var aSpline = carCarA.SplinePosition;
                                    if (a.Value.RealtimeCarUpdate.CarLocation == CarLocationEnum.NONE)
                                        aSpline = 0;
                                    var bSpline = carCarb.SplinePosition;
                                    if (b.Value.RealtimeCarUpdate.CarLocation == CarLocationEnum.NONE)
                                        bSpline = 0;


                                    var aLaps = a.Value.RealtimeCarUpdate.Laps;
                                    var bLaps = b.Value.RealtimeCarUpdate.Laps;

                                    if (a.Value.RealtimeCarUpdate.CarLocation == CarLocationEnum.Pitlane && b.Value.RealtimeCarUpdate.CarLocation == CarLocationEnum.Pitlane)
                                    {
                                        if (aLaps == bLaps)
                                        {
                                            return a.Value.RealtimeCarUpdate.Position < b.Value.RealtimeCarUpdate.Position ? 1 : -1;
                                        }
                                    }

                                    if (aSpline > 0.9f || aSpline < 0.1f)
                                    {
                                        return a.Value.RealtimeCarUpdate.Position < b.Value.RealtimeCarUpdate.Position ? 1 : -1;
                                    }

                                    float aPosition = aLaps + aSpline / 10;
                                    float bPosition = bLaps + bSpline / 10;
                                    return aPosition.CompareTo(bPosition);
                                });
                                cars.Reverse();
                                break;
                            };
                    }
                    break;
                }

            case RaceSessionType.Practice:
            case RaceSessionType.Qualifying:
                {
                    cars.Sort((a, b) =>
                    {
                        if (a.Value.CarInfo == null)
                            return -1;

                        if (b.Value.CarInfo == null)
                            return 1;


                        if (a.Value.RealtimeCarUpdate.CarLocation == CarLocationEnum.Pitlane && b.Value.RealtimeCarUpdate.CarLocation == CarLocationEnum.Pitlane)
                        {
                            if (a.Value.RealtimeCarUpdate.Position == b.Value.RealtimeCarUpdate.Position)
                                return a.Value.CarInfo.RaceNumber.CompareTo(b.Value.CarInfo.RaceNumber);
                            return a.Value.RealtimeCarUpdate.Position.CompareTo(b.Value.RealtimeCarUpdate.Position);
                        }

                        if (a.Value.RealtimeCarUpdate.Position == b.Value.RealtimeCarUpdate.Position)
                            return a.Value.CarInfo.RaceNumber.CompareTo(b.Value.CarInfo.RaceNumber);

                        return a.Value.RealtimeCarUpdate.Position.CompareTo(b.Value.RealtimeCarUpdate.Position);
                    });
                    break;
                }

            default: break;
        }
    }

    public CarData GetFirstPositionCar()
    {
        List<KeyValuePair<int, CarData>> cars = EntryListTracker.Instance.Cars;
        cars.Sort((a, b) =>
        {
            if (a.Value.CarInfo == null)
                return -1;

            if (b.Value.CarInfo == null)
                return 1;

            if (a.Value.RealtimeCarUpdate.CarLocation == CarLocationEnum.Pitlane && b.Value.RealtimeCarUpdate.CarLocation == CarLocationEnum.Pitlane)
            {
                if (a.Value.RealtimeCarUpdate.CupPosition == b.Value.RealtimeCarUpdate.CupPosition)
                    return a.Value.CarInfo.RaceNumber.CompareTo(b.Value.CarInfo.RaceNumber);
                return a.Value.RealtimeCarUpdate.CupPosition.CompareTo(b.Value.RealtimeCarUpdate.CupPosition);
            }

            if (a.Value.RealtimeCarUpdate.CupPosition == b.Value.RealtimeCarUpdate.CupPosition)
                return a.Value.CarInfo.RaceNumber.CompareTo(b.Value.CarInfo.RaceNumber);


            var aSpline = PositionGraph.Instance.GetCar(a.Value.CarInfo.CarIndex)?.SplinePosition;
            var bSpline = PositionGraph.Instance.GetCar(b.Value.CarInfo.CarIndex)?.SplinePosition;

            var aLaps = PositionGraph.Instance.GetCar(a.Value.CarInfo.CarIndex)?.LapIndex;
            var bLaps = PositionGraph.Instance.GetCar(b.Value.CarInfo.CarIndex)?.LapIndex;

            float aPosition = aLaps.GetValueOrDefault(0) * 10 + aSpline.GetValueOrDefault(0);
            float bPosition = bLaps.GetValueOrDefault(0) * 10 + bSpline.GetValueOrDefault(0);
            return aPosition.CompareTo(bPosition);
        });
        cars.Reverse();


        return cars.First().Value;
    }

    public sealed override bool ShouldRender() => true;
}
