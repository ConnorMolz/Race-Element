﻿using Octokit;
using RaceElement.Util;
using RaceElement.Util.SystemExtensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace RaceElement.Controls;

/// <summary>
/// Interaction logic for About.xaml
/// </summary>
public partial class Info : UserControl
{
    private bool HasAddedDownloadButton = false;


    public Info()
    {
        InitializeComponent();


        buttonWebsite.Click += (sender, e) => Process.Start(new ProcessStartInfo()
        {
            FileName = "cmd",
            Arguments = $"/c start https://race.elementfuture.com/",
            WindowStyle = ProcessWindowStyle.Hidden,
        });
        buttonDiscord.Click += (sender, e) => Process.Start(new ProcessStartInfo()
        {
            FileName = "cmd",
            Arguments = $"/c start https://discord.gg/26AAEW5mUq",
            WindowStyle = ProcessWindowStyle.Hidden,
        });
        buttonGithub.Click += (sender, e) => Process.Start(new ProcessStartInfo()
        {
            FileName = "cmd",
            Arguments = $"/c start https://github.com/RiddleTime/Race-Element",
            WindowStyle = ProcessWindowStyle.Hidden,
        });
        buttonDonate.Click += (sender, e) => Process.Start(new ProcessStartInfo()
        {
            FileName = "cmd",
            Arguments = $"/c start https://race.elementfuture.com/guide/sponsor",
            WindowStyle = ProcessWindowStyle.Hidden,
        });
        ToolTipService.SetInitialShowDelay(buttonWebsite, 1);
        ToolTipService.SetInitialShowDelay(buttonDiscord, 1);
        ToolTipService.SetInitialShowDelay(buttonGithub, 1);
        ToolTipService.SetInitialShowDelay(buttonDonate, 1);

        new Thread(() => CheckNewestVersion()).Start();

        this.IsVisibleChanged += (s, e) =>
        {
            if ((bool)e.NewValue)
                FillReleaseNotes();
            else
                stackPanelReleaseNotes.Children.Clear();

            ThreadPool.QueueUserWorkItem(x =>
            {
                Thread.Sleep(5 * 1000);
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
            });
        };
    }

    private async void CheckNewestVersion()
    {
        Thread.Sleep(500);

        RemoveTempVersionFile();
#if DEBUG
        TitleBar.Instance.SetAppTitle("Dev");
        return;
#endif
#pragma warning disable CS0162 // Unreachable code detected

        try
        {
            if (HasAddedDownloadButton)
                return;

            GitHubClient client = new(new ProductHeaderValue("Race-Element"), new Uri("https://github.com/RiddleTime/Race-Element.git"));
            var releases = await client.Repository.Release.GetAll("RiddleTime", "Race-Element", new ApiOptions() { PageSize = 5 });

            if (releases != null && releases.Count > 0)
            {
                Release latest = releases.First();
                FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(System.Environment.ProcessPath);

                long localVersion = VersionToLong(new Version(fileVersion.FileVersion));
                long remoteVersion = VersionToLong(new Version(latest.Name));

                var newerVersions = releases.Where(x => VersionToLong(new Version(x.Name)) > localVersion);

                if (localVersion > remoteVersion)
                    TitleBar.Instance.SetAppTitle("Beta");

                if (remoteVersion > localVersion)
                {
                    if (latest != null)
                    {
                        var accManagerAsset = latest.Assets.Where(x => x.Name == "RaceElement.exe").First();

                        StringBuilder releaseNotes = new();
                        foreach (Release newRelease in newerVersions)
                        {
                            releaseNotes.AppendLine(newRelease.Name);
                            releaseNotes.AppendLine(newRelease.Body);
                        }

                        await Dispatcher.BeginInvoke(new Action(() =>
                         {
                             MainWindow.Instance.EnqueueSnackbarMessage($"A new version of Race Element is available: {latest.Name}", " Open About tab ", new Action(() => { MainWindow.Instance.tabAbout.Focus(); }));

                             TitleBar.Instance.SetUpdateButton(latest.Name, releaseNotes.ToString(), accManagerAsset);

                             HasAddedDownloadButton = true;
                         }));
                    }
                }
            }

        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
#pragma warning restore CS0162 // Unreachable code detected
    }

    private void RemoveTempVersionFile()
    {
        try
        {
            string tempTargetFile = $"{FileUtil.RaceElementAppDataPath}AccManager.exe";
            FileInfo tempFile = new(tempTargetFile);

            if (tempFile.Exists)
                tempFile.Delete();
        }
        catch (Exception e)
        {
            LogWriter.WriteToLog(e);
        }
    }

    private long VersionToLong(Version VersionInfo)
    {
        string major = $"{VersionInfo.Major}".FillStart(4, '0');
        string minor = $"{VersionInfo.Minor}".FillStart(4, '0');
        string build = $"{VersionInfo.Build}".FillStart(4, '0');
        string revision = $"{VersionInfo.Revision}".FillStart(4, '0');
        string versionString = major + minor + build + revision;

        long.TryParse(versionString, out long version);
        return version;
    }

    private void FillReleaseNotes()
    {
        Dispatcher.BeginInvoke(new Action(() =>
        {
            stackPanelReleaseNotes.Children.Clear();
            ReleaseNotes.Notes.ToList().ForEach(note =>
            {
                TextBlock noteTitle = new()
                {
                    Text = note.Key,
                    Style = Resources["MaterialDesignBody1TextBlock"] as Style,
                    FontWeight = FontWeights.Bold,
                    FontStyle = FontStyles.Oblique
                };
                TextBlock noteDescription = new()
                {
                    Text = note.Value,
                    TextWrapping = TextWrapping.WrapWithOverflow,
                    Style = Resources["MaterialDesignDataGridTextColumnStyle"] as Style
                };

                StackPanel changePanel = new() { Margin = new Thickness(0, 10, 0, 0) };
                changePanel.Children.Add(noteTitle);
                changePanel.Children.Add(noteDescription);

                stackPanelReleaseNotes.Children.Add(changePanel);
            });
        }));
    }
}
