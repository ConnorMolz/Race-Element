﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RaceElement.HUD.ACC.Overlays.Pitwall.LowFuelMotorsport.API;

internal readonly record struct ApiObject
{
    [JsonProperty("user")] public User User { get; init; }
    [JsonProperty("race")] public IReadOnlyList<Race> Races { get; init; }
    [JsonProperty("drivers")] public int Drivers { get; init; }
    [JsonProperty("sim")] public Sim Sim { get; init; }
    [JsonProperty("licenseclass")] public string Licenseclass { get; init; }
    [JsonProperty("safetyplate")] public string Safetyplate { get; init; }
    [JsonProperty("sof")] public int Sof { get; init; }
}

internal readonly record struct Sim
{
    [JsonProperty("sim_id")] public int SimId { get; init; }
    [JsonProperty("select_order")] public int SelectOrder { get; init; }
    [JsonProperty("name")] public string Name { get; init; }
    [JsonProperty("logo_url")] public string LogoUrl { get; init; }
    [JsonProperty("logo_big")] public string LogoBig { get; init; }
    [JsonProperty("platform")] public string Platform { get; init; }
    [JsonProperty("active")] public int Active { get; init; }
}

internal readonly record struct Race
{
    [JsonProperty("event_name")] public string EventName { get; init; }
    [JsonProperty("race_id")] public int RaceId { get; init; }
    [JsonProperty("split")] public int Split { get; init; }
    [JsonProperty("sim_id")] public int SimId { get; init; }
    [JsonProperty("race_date")] public DateTime RaceDate { get; init; }
    [JsonProperty("sof")] public int Sof { get; init; }
    [JsonProperty("split2_sof")] public int Split2Sof { get; init; }
    [JsonProperty("split3_sof")] public int Split3Sof { get; init; }
    [JsonProperty("split4_sof")] public int Split4Sof { get; init; }
    [JsonProperty("split5_sof")] public int Split5Sof { get; init; }
    [JsonProperty("split6_sof")] public int Split6Sof { get; init; }
    [JsonProperty("split7_sof")] public int Split7Sof { get; init; }
    [JsonProperty("split8_sof")] public int Split8Sof { get; init; }
    [JsonProperty("split9_sof")] public int Split9Sof { get; init; }
    [JsonProperty("split10_sof")] public int Split10Sof { get; init; }
}

internal readonly record struct User
{
    [JsonProperty("id")] public int Id { get; init; }
    [JsonProperty("name")] public string Name { get; init; }
    [JsonProperty("username")] public string UserName { get; init; }
    [JsonProperty("vorname")] public string FirstName { get; init; }
    [JsonProperty("shortname")] public string Shortname { get; init; }
    [JsonProperty("nachname")] public string LastName { get; init; }
    [JsonProperty("steamid")] public string SteamId { get; init; }
    [JsonProperty("avatar")] public string Avatar { get; init; }
    [JsonProperty("admin")] public int Admin { get; init; }
    [JsonProperty("created_at")] public string CreatedAt { get; init; }
    [JsonProperty("updated_at")] public string UpdatedAt { get; init; }
    [JsonProperty("profile_extras")] public string ProfileExtras { get; init; }
    [JsonProperty("origin")] public string Origin { get; init; }
    [JsonProperty("c_rating")] public int CRating { get; init; }
    [JsonProperty("cc_rating")] public int CcRating { get; init; }
    [JsonProperty("license")] public string License { get; init; }
    [JsonProperty("safety_rating")] public string SafetyRating { get; init; }
    [JsonProperty("division")] public int Division { get; init; }
    [JsonProperty("valid_license")] public int ValidLicense { get; init; }
    [JsonProperty("darkmode")] public int Darkmode { get; init; }
    [JsonProperty("fav_sim")] public int FavSim { get; init; }
    [JsonProperty("sr_license")] public string SrLicense { get; init; }
}

/// <summary>
/// LFM entry for the entry list. This is just the basic
/// information used for compute the ELO and the position
/// threshold where the player wins/losses ELO.
/// </summary>
public readonly record struct SplitEntry(int RaceNumber, int Elo, bool IsPlayer);

/// <summary>
/// Information about the race. ELO multiplayer and split entry list.
/// </summary>
public struct RaceInfo
{
    /// <summary>ELO multiplayer.</summary>
    public float kFactor;

    /// <summary>Player split entry list.</summary>
    public List<SplitEntry> entries;
}
