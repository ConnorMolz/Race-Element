﻿using RaceElement.Util.DataTypes;
using System;
using System.Collections.Generic;
using static RaceElement.Data.ACC.Tracks.TrackData;

namespace ACCManager.Data.ACC.Tracks.Data
{
    internal class Zolder : AbstractTrackData
    {
        public override Guid Guid => new Guid("eaca1a4d-aa7e-4c31-bfc5-6035bfa30395");
        public override string FullName => "Circuit Zolder";
        public override int TrackLength => 4011;

        public override Dictionary<FloatRangeStruct, (int, string)> CornerNames => new Dictionary<FloatRangeStruct, (int, string)>()
        {
            // TODO Split up some corners into seperate Turn numbers: http://www.racb-track-marshals.be/images/circuit_zolder.jpg
            { new FloatRangeStruct(0.04245049f, 0.0946003f), (1, "Earste Links") },
            { new FloatRangeStruct(0.1296046f, 0.1782782f), (2, "Sterrenwachtbocht") },
            { new FloatRangeStruct(0.17827821f, 0.2351831f), (3, "Kanaalbocht") },
            { new FloatRangeStruct(0.2595046f, 0.3127186f), (4, "Lucien Bianchibocht") },
            { new FloatRangeStruct(0.4122366f, 0.4614663f), (5, "Kleine Chicane") },
            { new FloatRangeStruct(0.4122366f, 0.4614663f), (6, "Kleine Chicane") },
            { new FloatRangeStruct(0.5095074f, 0.5512679f), (7, "Butte") },
            { new FloatRangeStruct(0.5667552f, 0.6171156f), (8, "Gille Villeneuve Chicane") },
            { new FloatRangeStruct(0.6240816f, 0.6695773f), (1, "Terlamenbocht") },
            { new FloatRangeStruct(0.7329645f, 0.7745231f), (1, "Bolderberghaarspeldbocht") },
            { new FloatRangeStruct(0.7820684f, 0.8041771f), (1, "Jochen Rindtbocht") },
            { new FloatRangeStruct(0.8749502f, 0.9376088f), (1, "Jackie Ickxbocht") }
        };
    }
}
