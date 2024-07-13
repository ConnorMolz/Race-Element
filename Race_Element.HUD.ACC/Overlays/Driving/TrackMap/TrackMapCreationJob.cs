﻿using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using RaceElement.Core.Jobs.LoopJob;
using RaceElement.Data.ACC.Core;
using RaceElement.Util;
using System.Reflection;
using System.Linq;

namespace RaceElement.HUD.ACC.Overlays.Driving.TrackMap;

public class TrackMapCreationJob : AbstractLoopJob
{
    private enum CreationState
    {
        Start,
        TraceTrack,
        LoadFromFile,
        NotifySubscriber,
        Error,
        End
    }

    public EventHandler<List<TrackPoint>> OnMapPositionsCallback;
    public EventHandler<string> OnMapProgressCallback;

    private CreationState _mapTrackingState = CreationState.Start;
    private readonly List<TrackPoint> _trackedPositions = [];

    private int _completedLaps = ACCSharedMemory.Instance.PageFileGraphic.CompletedLaps;
    private int _prevPacketId = ACCSharedMemory.Instance.PageFileGraphic.PacketId;

    private float _trackingPercentage;

    private const short Version = 1;
    private readonly byte[] _magic = [(byte)'r', (byte)'e', (byte)'t', (byte)'m'];

    public override void RunAction()
    {
        switch (_mapTrackingState)
        {
            case CreationState.Start:
                {
                    _mapTrackingState = InitialState();
                }
                break;

            case CreationState.LoadFromFile:
                {
                    try { _mapTrackingState = LoadMapFromFile(); }
                    catch (Exception e) { Debug.WriteLine(e); }
                }
                break;

            case CreationState.TraceTrack:
                {
                    _mapTrackingState = PositionTracking();
                }
                break;

            case CreationState.NotifySubscriber:
                {
                    {
                        const string msg = "Map tracked. Enjoy it!";
                        OnMapProgressCallback?.Invoke(null, msg);
                    }

                    OnMapPositionsCallback?.Invoke(this, _trackedPositions);
                    _mapTrackingState = CreationState.End;
                }
                break;

            case CreationState.Error:
                {
                    Thread.Sleep(10 * 1000);
                    _mapTrackingState = CreationState.Start;
                }
                break;

            default:
                {
                    OnMapProgressCallback?.Invoke(null, null);
                    Thread.Sleep(10);
                }
                break;
        }
    }

    private CreationState InitialState()
    {
        var laps = ACCSharedMemory.Instance.PageFileGraphic.CompletedLaps;
        var trackName = ACCSharedMemory.Instance.PageFileStatic.Track;
        //string path = FileUtil.RaceElementTracks + trackName + ".bin";  // used when checking if track data from file exists. not used anymore, but kept in place for dev.

        if (trackName.Length > 0 && TrackDataExists(trackName))
        {
            return CreationState.LoadFromFile;
        }

        {
            const string msg = "Tracking state -> waiting for lap counter to change.\n"
                               + "For a better mapping we recommend to drive at\n"
                               + "constant speed by enabling pit speed limiter,\n"
                               + "and at the center of the track.";
            OnMapProgressCallback?.Invoke(null, msg);
        }

        if (!AccProcess.IsRunning)
        {
            return CreationState.Start;
        }

        if (laps != _completedLaps)
        {
            _completedLaps = laps;
            return CreationState.TraceTrack;
        }

        return CreationState.Start;
    }

    private CreationState PositionTracking()
    {
        {
            const string msg = "Tracking state -> tracking map ({0:0.0}%),\nif you invalidate the lap you will have to start again.";
            OnMapProgressCallback?.Invoke(null, String.Format(msg, _trackingPercentage * 100));
        }

        var pageGraphics = ACCSharedMemory.Instance.PageFileGraphic;
        if (pageGraphics.PacketId == _prevPacketId)
        {
            return CreationState.TraceTrack;
        }

        if (Math.Abs(pageGraphics.NormalizedCarPosition - _trackingPercentage) <= float.Epsilon)
        {
            return CreationState.TraceTrack;
        }

        _trackingPercentage = pageGraphics.NormalizedCarPosition;
        _prevPacketId = pageGraphics.PacketId;

        if (!pageGraphics.IsValidLap)
        {
            _trackingPercentage = 0;
            _trackedPositions.Clear();
            return CreationState.Start;
        }

        for (var i = 0; i < pageGraphics.ActiveCars; ++i)
        {
            if (pageGraphics.CarIds[i] != pageGraphics.PlayerCarID)
            {
                continue;
            }

            TrackPoint pos = new()
            {
                X = pageGraphics.CarCoordinates[i].X,
                Y = pageGraphics.CarCoordinates[i].Z,
                Spline = pageGraphics.NormalizedCarPosition,
            };

            _trackedPositions.Add(pos);
            break;
        }

        if (pageGraphics.CompletedLaps != _completedLaps)
        {
            WriteMapToFile();
            return CreationState.NotifySubscriber;
        }

        return CreationState.TraceTrack;
    }

    private bool TrackDataExists(string trackName)
    {
        var founds = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(x => x.EndsWith($"{trackName.ToLower()}.bin"));
        return founds.Any();
    }

    private CreationState LoadMapFromFile()
    {
        {
            const string msg = "Tracking state -> Map found on disk, loading it.";
            OnMapProgressCallback?.Invoke(null, msg);
        }

        var trackName = ACCSharedMemory.Instance.PageFileStatic.Track.ToLower();
        var founds = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(x => x.EndsWith($"{trackName}.bin"));
        if (!founds.Any())
            return CreationState.Error;

        using Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(founds.First());
        using BinaryReader binaryReader = new(resourceStream);

        {
            var magic = binaryReader.ReadBytes(4);  // Magic number
            binaryReader.ReadInt16();               // Version
            binaryReader.ReadInt32();               // Reserved 01 (future use)
            binaryReader.ReadInt32();               // Reserved 02 (future use)

            if (magic[0] != _magic[0] || magic[1] != _magic[1] || magic[2] != _magic[2] || magic[3] != _magic[3])
            {
                binaryReader.Close();
                resourceStream.Close();

                {
                    string msg = "Tracking state -> Corrupt map file. Delete the file and track again the track.\n" + trackName; //path;
                    OnMapProgressCallback?.Invoke(null, msg);
                }

                return CreationState.Error;
            }
        }

        while (resourceStream.Position < resourceStream.Length)
        {
            {
                const string msg = "Tracking state -> loading from disk ({0:0.0}%)";
                OnMapProgressCallback?.Invoke(null, String.Format(msg, ((float)resourceStream.Position / resourceStream.Length) * 100.0f));
            }

            TrackPoint pos = new()
            {
                X = binaryReader.ReadSingle(),
                Y = binaryReader.ReadSingle(),
                Spline = binaryReader.ReadSingle(),
            };

            _trackedPositions.Add(pos);
        }

        binaryReader.Close();
        resourceStream.Close();

        return CreationState.NotifySubscriber;
    }

    private void WriteMapToFile()
    {
        {
            const string msg = "Tracking state -> write map to file.";
            OnMapProgressCallback?.Invoke(null, msg);
        }

        DirectoryInfo directory = new(FileUtil.RaceElementTracks);
        if (!directory.Exists) directory.Create();

        var trackName = ACCSharedMemory.Instance.PageFileStatic.Track.ToLower();
        string path = FileUtil.RaceElementTracks + trackName + ".bin";

        using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        using BinaryWriter binaryWriter = new(fileStream);

        binaryWriter.Write(_magic);     // Magic number
        binaryWriter.Write(Version);   // Version
        binaryWriter.Write(0);          // Reserved 01 (future use)
        binaryWriter.Write(0);          // Reserved 02 (future use)

        for (var i = 0; i < _trackedPositions.Count; ++i)
        {
            {
                const string msg = "Tracking state -> writing from disk ({0:0.0}%)";
                OnMapProgressCallback?.Invoke(null, String.Format(msg, ((float)i / _trackedPositions.Count) * 100.0f));
            }

            binaryWriter.Write(_trackedPositions[i].X);
            binaryWriter.Write(_trackedPositions[i].Y);
            binaryWriter.Write(_trackedPositions[i].Spline);
        }

        binaryWriter.Close();
        fileStream.Close();
    }
}
