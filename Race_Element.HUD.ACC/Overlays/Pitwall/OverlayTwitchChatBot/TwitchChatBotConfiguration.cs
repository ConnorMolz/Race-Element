﻿using RaceElement.HUD.Overlay.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceElement.HUD.ACC.Overlays.Pitwall.OverlayTwitchChatBot;

internal sealed class TwitchChatBotConfiguration : OverlayConfiguration
{
    public TwitchChatBotConfiguration() => GenericConfiguration.AllowRescale = false;

    [ConfigGrouping("Connection", "Set up the username and O Auth token")]
    public CredentialsGrouping Credentials { get; init; } = new();
    public sealed class CredentialsGrouping
    {
        [ToolTip("Your channel name")]
        public string TwitchUser { get; init; } = "";

        [ToolTip("Create an O Auth token at https://twitchtokengenerator.com/, click Bot Chat Token and connect and copy the generated ACCESS TOKEN." +
            "\n(This is required for Race Element to connect to your chat using the twitch api, " +
            "this Access token can expire for security reasons).")]
        [StringOptions(isPassword: true)]
        public string OAuthToken { get; init; } = "";
    }

    [ConfigGrouping("Links", "Various links on websites used for this HUD.")]
    public LinksGrouping Links { get; init; } = new();
    public sealed class LinksGrouping
    {
        [ToolTip("Opens the website to generate an Access token.")]
        [LinkText("Generate Access token at https://twitchtokengenerator.com/")]
        public LinkOption GetToken { get; init; } = new("https://twitchtokengenerator.com/");

        [ToolTip("Opens up a list of all available commands for the race element chat bot.")]
        [LinkText("Open list of Chat Bot Commands")]
        public LinkOption AvailableCommands { get; init; } = new("https://race.elementfuture.com/guide/twitch-chat-bot-commands");
    }


    //[ConfigGrouping("Tasks", "Add additional behavior that is for example periodically actived.")]
    //public TasksGrouping Tasks { get; init; } = new();
    //public class TasksGrouping
    //{
    //    [ToolTip("Automatically shout out the +commands periodically")]
    //    public bool ShoutCommands { get; init; } = true;

    //    [ToolTip("Set the amount of minutes between each shoutout of '+commands' in chat.")]
    //    [IntRange(1, 60, 1)]
    //    public int CommandsInterval { get; init; } = 20;
    //}

}
