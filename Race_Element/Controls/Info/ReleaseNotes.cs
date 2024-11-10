﻿using System.Collections.Generic;

namespace RaceElement.Controls;

public static class ReleaseNotes
{
    internal static readonly Dictionary<string, string> Notes = new()
    {
        {"2.1.0.2", "Assetto Corsa Competizione:"+
                    "\n- Tyre Temp History HUD can now be automatically hidden in race sessions."+
                    "\n- Brake Temp History HUD can now be automatically hidden in race sessions."+
                    "\n- Tyre Pressure History HUD can now be automatically hidden in race sessions."+
                    "\n- Lap Table HUD can now be automatically hidden in race sessions."+
                    "\n- Fixed Track Map HUD (ACC)." },
        {"2.1.0.0", "Race Element:"+
                    "\n- When dragging titlebar when maximized, the app now returns to normal mode."+
                    "\n\nAll HUDs:"+
                    "\n- Options with sliders now have got an inline editor to type. Hover an option with a slider to make it appear, leave the option with your mouse to save."+
                    "\n- Slider options now save when adjusting them with the arrow keys."+
                    "\n- Can if needed refresh up and beyond 500 Hz."+
                    "\n- That have the scaling option can now be scaled to 0.1% instead of 0.2%."+
                    "\n\nMulti-Sim:"+
                    "\n- Added new Shift Bar HUD: refreshes up to 200 Hz."+
                    "\n- G-Force Trace HUD now allows g forces up to 6G, this to support single seater series."+
                    "\n\niRacing:"+
                    "\n- Fixed Local Acceleration(G-Force) data mapping."+
                    "\n\nAssetto Corsa Competizione:"+
                    "\n- Added new Shift Bar HUD: refreshes up to 200 Hz."
                    },
        {"2.0.0.4", "Race Element:"+
                    "\n- Minimum height of the app is now smaller at 720p."+
                    "\n- Added New Game selector! It's located at the left-bottom of the app."+
                    "\n\nACC HUDs:"+
                    "\n- Fixed a crash in track map hud."
                    },
        {"2.0.0.2", "Multi-Sim:"+
                    "\n- Improved speed of switching between simulators."+
                    "\n- You might have to select the current simulator after the update."+
                    "\n- Support added for Automobilista 2 (By Andrei Jianu)."+
                    "\n\niRacing:"+
                    "\n- HUDs should now only show once you're actually driving the car." +
                    "\n\nGeneral:"+
                    "\n- Updated System.Net.Json package due to vulnerability."
                    },
        {"2.0.0.0", "To Restore your ACC HUD Settings, Open the main Settings tab and Click the Migrate Button, it will start a new instance and then shuts down the already running instance."+
                    "\n\n Multi-Sim Support for HUDS:" +
                    "\n  - Supported added for Assetto Corsa 1, iRacing and RaceRoom."+
                    "\n  - To switch simulators, select one in the top-right of the app. This will reload the HUD Tab, this may take a few seconds."+
                    "\n  - Some HUDs only work on specific simulators, so you will see the HUD list change as you change to a different simulator."+
                    "\n  - Report any feedback in the Race Element discord server."+
                    "\n\n HUD Tab:"+
                    "\n  - Checked Checkboxes and their labels are now highlighted in a more visible color."+
                    "\n\n ACC HUDs:"+
                    "\n  - Added Track Map HUD(BETA) by Andrei Jianu."+
                    "\n  - Added Track Distance HUD: Shows the current percentual distance and the stint distance in meters." },
        {"1.1.1.2", "- Input Trace, Oversteer Trace, G Force Trace HUDs: Lower cpu usage by roughly 20%."+
                    "\n- Added new Tyre Temp History HUD: showing minimum, average and maximum tyre temps for the last lap."},
        {"1.1.1.0", "- Corner Data HUD: Fixed collection of data on tracks that had straights."+
                    "\n- Brake Temp History HUD: Added option to show HUD whilst setup menu is visible in-game."+
                    "\n- Pressure History HUD: Added option to show HUD whilst setup menu is visible in-game."+
                    "\n- Update .net json package due to vulnerability, recommend to update the app."},
        {"1.1.0.8", "- Wheel Slip HUD: Adjusted default option for WheelSize from 92 to 68."+
                    "\n- HUD Config: Now hides Scaling option when HUD does not allow scaling."+
                    "\n- Twitch Chat Bot: Fixed time delimiter character for +gap."+
                    "\n- Race Element should now be able to collect data during a session which wasn't accessed before."},
        {"1.1.0.6", "- Twitch Chat Bot:"+
                    "\n  - Added new command +diff: Shows the difference in lap times and sectors for the currently viewed car vs the selected. Use like(+diff ahead, +diff behind, +diff p 1, +diff # 1)."+
                    "\n  - Adjusted link to commands list on new website." },
        {"1.1.0.4", "- Updated SetupLinks to use the new website format, read the news article on the website for migrating your pre 1.1.0.4 SetupLinks links." },
        {"1.1.0.2", "- Setup Importer: disabled multi-track import when opened from SetupLink."+
                    "\n- Setup Browser: Added Copy As SetupLink for Discord, this automatically creates a nice readable link in markdown. It contains the name of the setup, the car and the track."},
        {"1.1.0.0", "- Rain Prediction HUD: Added configurable time multiplier."+
                    "\n- Track Info HUD: Increased precision of air and track temps."+
                    "\n- Brake Temp History HUD: Updated background set min and max values."+
                    "\n- Setups Tab: Added \"Copy As Link\", if someone clicks this link, race element will open with the setup importer. The setup is the link."+
                    "\n- Added option to include Links in Hud configuration. Twitch chat bot and hud are now using this option."},
        {"1.0.8.6", "- Damage HUD: Added configurable colors for each level of body damage"+
                    "\n- Internal optimizations to rendering."+
                    "\n- Improved data detection for cars leaving the server by Andrei Jianu."+
                    "\n- Added hud to benchmark rendering optimizations (Pitwall->CB Benchmark)."+
                    "\n- Added Brake Temp History HUD: showing minimum, average and maximum brake temps for the last lap."+
                    "\n- Entrylist overlay HUD(pitwall section): Shows time interval between cars during race session, alpha test."},
        {"1.0.8.4", "- Fuel Info HUD: Added option to set which laptime is used in the fuel calculation."+
                    "\n- Damage HUD: Added additional colors for various amounts of damage."+
                    "\n- Countless of internal optimizations."},
        {"1.0.8.2", "- Added Pressure History HUD: showing minimum, average and maximum tyre pressures for the last lap (beta)."+
                    "\n- Radar HUD is now hiding again when there aren't any cars close enough."},
        {"1.0.8.0", "- Added Ford Mustang GT3 support."+
                    "\n- Radar HUD is functional again."+
                    "\n- Damage HUD: Now shows different colors based on the amount of damage."},
        {"1.0.7.2", "- Fuel Info HUD: Reworked some internals."+
                    "\n- Boost Gauge HUD: Added customizable bar color."+
                    "\n- Speedometer HUD: Added customizable bar color."+
                    "\n- Added Positions Timestamp HUD: Can be used during a race session whilst calling a safety car, you will have all the positions (according to when drivers crossed a sector)."},
        {"1.0.7.0", "- hotfix for fuel info hud, drawing things were disposed when they shouldn't have been." },
        {"1.0.6.8", "- Fuel Info HUD:" +
                    "\n  - Now calculates laptimes above 3 minutes. It will also use your last lap as laptime if you don't haven't set a valid best lap yet." +
                    "\n  - No data will be shown if there are no known laptimes(best or last)."+
                    "\n- Setup Viewer: Fix Camber value for Lambo Huracan ST Evo 2."},
        {"1.0.6.6", "- Twitch Chat Bot:"+
                    "\n  - Added Fuel Calculator: +fuel [minutes] [liters/lap] [laptime]. Use like +fuel 60 3 2:16."+
                    "\n  - Car info commands now also show whether a car is in the pitlane."+
                    "\n  - Added: +help, does the same as +commands."+
                    "\n- Revert Speedometer HUD to old version due to it being more minimalistic whilst providing more info."},
        {"1.0.6.4", "- Rain Prediction HUD: Drizzle is now called Dew."+
                    "\n- Twitch Chat Bot:"+
                    "\n  - +commands now links to the guide on the website, this guide provides a bit more detail than just a commands list."+
                    "\n  - +pos is now +p."+
                    "\n  - Added +# to look up a car using it's entry number, for example: +# 992."},
        {"1.0.6.2", "- Twitch Chat HUD:"+
                    "\n  - Added configurable color for when someone tags you(the broadcaster) in the chat."+
                    "\n- Rain Prediction HUD: Fixed, will now show weather predictions again."},
        {"1.0.6.0", "- Twitch Chat Bot:"+
                    "\n  - Added +session, responds with information about the current session."+
                    "\n  - +temps now also shows rain condition when it's not dry."+
                    "\n  - +pos, +ahead and +behind now all use the same response, including the lap index of the last lap."+
                    "\n- Twitch Chat HUD: Added behaviour category to options: Always Visible and Hide In Qualifying."+
                    "\n- Rain Prediction HUD: Smaller dimension."+
                    "\n- Lap Table HUD: Sector times will now show a minute marker if higher than 59.999. The HUD is now slightly wider than before."},
        {"1.0.5.6", "- Added preliminary support for ACC Nordschleife 24H DLC, expect an update for corner names and sector data."+
                    "\n- Corner Data HUD: Increased width of speed delta columns."+
                    "\n- Rain Prediction HUD: \"Heavy Rain\" will now show as \"Heavy\"."},
        {"1.0.5.4", "- Add Rain Prediction HUD: Shows predicted rain intensity and counts down." },
        {"1.0.5.2", "- Dual Sense X: Improved responsiveness by increasing ffb rate from 70 to 100 Herz." },
        {"1.0.5.0", "- Rework of several Live Trace HUDs:"+
                    "\n  - G-Force Trace HUD:"+
                    "\n    - Added option to set hud refresh rate and faster data collection."+
                    "\n  - Oversteer Trace HUD:"+
                    "\n    - Added option to set hud refresh rate and faster data collection."+
                    "\n    - Added option to set enable horizontal grid lines."+
                    "\n    - Decrease cpu usage."+
                    "\n  - Input Trace HUD:"+
                    "\n    - Added option to set hud refresh rate and faster data collection."+
                    "\n    - Decrease cpu usage."},
        {"1.0.4.4", "- Lap Info HUD: Predicted laptime now shows as Estimated time(Est)."+
                    "\n- Added G-Force Trace HUD: a live graph of lateral and longitudinal G-forces."+
                    "\n- Updated tooltip for movement button in HUD tab, will now show mouse and keyboard shortcut."},
        {"1.0.4.2", "- Low Fuel Motorsport HUD: fixed app crash and corrected SOF to match driver." },
        {"1.0.4.0", "- Twitch Chat Bot:"+
                    "\n  - Corrected last name during driver swap races for +pos command."+
                    "\n- Added Refuel Info HUD by FG - Shows session progress bar, pit stop window and stint timer if available to help with the refuel strategy. With option to only show the progress bar or to display additional information."+
                    "\n- Added Low Fuel Motorsport HUD by Andrei Jianu - Shows drivers license and the upcoming race."},
        {"1.0.3.6", "- Setup Importer now detects Discord download links again, drag it straight from the discord download button on-top of the app, discord changed the way they build up the URLs."+
                    "\n- Twitch Chat HUD: Increased max width from 500 to 800."+
                    "\n- Twitch Chat Bot: Added +pos command, it provides information regarding the car in the requested race position. Use it like +pos 1 and you will gain information about the car in P1!"},
        {"1.0.3.4", "- Added Brake Pressure HUD: Shows the applied brake pressure for front and rear brakes."+
                    "\n- Sector Data HUD: Added option to show the current sector live, will increase refresh rate thus cpu usage."},
        {"1.0.3.2", "- Add Sector Data HUD: Shows you the Min and Max velocity after each sector."+
                    "\n- Add Twitch Chat Bot: Removed it from the chat HUD itself, this way you can only enable the bot without using the twitch chat HUD. There is still an option to display the bot commands in the chat hud itself." +
                    " You can use the same credentials as you have used in the twitch chat hud."+
                    "\n- Fix Mouse Position HUD disposal at shutdown of app when movement mode was still active."+
                    "\n- Fix DualSenseX Free, now places the trigger states text file in correct directory."},
        {"1.0.3.0", "- Decrease shutdown time of app."+
                    "\n- HUD Tab:"+
                    "\n  - Added buttons to reset the position or the configuration for a HUD."+
                    "\n  - Added tooltips when hovering a hud in the list, it shows the description of the HUD."+
                    "\n- Internal changes for Multi-Sim support, added option to select iRacing."},
        {"1.0.2.2", "- Internal changes for future multi-sim support."+
                    "\n- Twitch Chat HUD: fixed a few commands related to lap times."+
                    "\n- New HUD: Average Laptime HUD by goeflo."},
        {"1.0.2.0", "- Added Track corner names for Red Bull Ring."+
                    "\n- Added Setup Converters for all 6 new GT2 cars(for viewing and comparing setups)."+
                    "\n- Added Steering Locks for all 6 new GT2 cars."},
        {"1.0.1.6", "- Twitch chat HUD:"+
                    "\n  - Added +potential best command, shows the potential best lap if you've set any valid laps."+
                    "\n  - Removed +pos command, (you can literally see this in the Vanilla ACC Widgets)."+
                    "\n- Added basic support for Red Bull Ring, corner data will come soon." +
                    "\n- Once I am able to buy the GT2 DLC, the data for the new cars will have to be collected so expect another update soon."},
        {"1.0.1.4", "- Twitch Chat HUD:"+
                    "\n  - Added option to enable or disable chat bot."+
                    "\n  - Added option to only display the hud when the engine is running, disabling this option also hides the HUD during spectator."+
                    "\n  - Added option to display bot responses in hud."+
                    "\n  - ahead and behind commands now have a parameter called 'best', instead of showing the last lap it will show the best lap.(for example: +behind best)"},
        {"1.0.1.2", "- Twitch Chat HUD:"+
                    "\n  - Connected message is now only visible to you."+
                    "\n  - Added option to make new messages appear at the bottom of the hud."+
                    "\n  - Added more chat bot commands: ahead and behind, both showing the last lap."},
        {"1.0.1.0", "- Twitch Chat HUD:"+
                    "\n  - Added support for subscriptions alerts, they will now show up in one form or another."+
                    "\n  - Added more chat bot commands: purple, green, position."+
                    "\n- Speedometer HUD: Decreased total size by 10%."},
        {"1.0.0.8", "- Twitch Chat HUD: Added support for bits/raids and added several chat commands (type +commands for the list of commands)."},
        {"1.0.0.6", "- Redesigned speedometer, it is now a circular gauge with more customization options."+
                    "\n- Setup Browser now lazy loads."+
                    "\n- Add simple Race Element Process HUD."},
        {"1.0.0.4", "- Add Twitch Chat HUD to Pitwall HUDs, read the description of the HUD on how to configure it."},
        {"1.0.0.2", "- Adjusted tyre pressure range up to 27.3 instead of up to 27.0"+
                    "\n- Corner Data HUD: Added Corner Names Column by mreininger23."+
                    "\n- Prevent Race Element from crashing whilst Content Manager(AC) is still running after AC started and was exited (please shut down content manager before starting ACC and Race Element)."+
                    "\n- Adjusted publish profile to include dds generation api dll."},
        {"1.0.0.0", "- Upgraded to from .NET 4.8.2 -> .NET 8"+
                    "\n- Added Track Bar HUD: Alpha version for now."+
                    "\n- Added Track Circle HUD: Alpha version for now."+
                    "\n- Minimize to tray now also hides the app in the windows alt-tab menu."},
        {"0.3.0.6", "- Shift Indicator HUD: Now flashes after upshift percentage has been reached." },
        {"0.3.0.4", "- Added Input Values HUD: Showing raw throttle and brake values"+
                    "\n- Wind Direction HUD: Now shows wind speed in km/h in the center."},
        {"0.3.0.2", "- Lap Table HUD:"+
                    "\n   - Clear graphics grid when a new session starts."+
                    "\n   - Add sector based lap invalidation."+
                    "\n - DualSenseX: adjusted default ffb frequencies for TC and ABS."},
        {"0.3.0.0", "- Added Corner Data HUD(Alpha version/early access), showing corner delta and optionally: minimum speed, average speed and max lateral g force."+
                    "\n- DualSenseX module now allows to set a custom port for DSX(6969 is default)."+
                    "\n- Lap table HUD: Reworked design and functionality, now shows personal fastest and best sectors based on valid laps."+
                    "\n- HUD Tab: Switched movement mode and always visible buttons around"+
                    "\n- Revamped internals of several HUDs to lower cpu usage:"+
                    "\n  - Lap Table HUD"+
                    "\n  - Car Info HUD"+
                    "\n  - Shared Memory page HUDs"+
                    "\n- Wind Direction HUD: Added Wind Threshold slider for only showing the hud above a certain wind speed."},
        {"0.2.4.2", "- Removed affiliate links:"+
                    "\nRace Element is free to run software, I do not want to encourage anyone ever again to pay to become faster."+
                    "\nLearn to create your own setups, there are tons of guides out there, even from one of the developers of Assetto Corsa Competizione."+
                    "\nI dare you to create fast setups and share them for free!"+
                    "\nIf you want to support Race Element, provide code, ideas.. support.. whatever you like.. I appreciate it all."+
                    "\nBe honest about your performance, is it the setup or is it you?"},
        {"0.2.4.0", "- Added Active Triggers support for Playstation DualSense Controller (see discord for guide)."+
                    "\n- Input Trace HUD can have a smaller width."+
                    "\n- Added better logging for auto-updater."+
                    "\n- Fix app manifest not being embedded."},
        {"0.2.3.8", "- App hotkeys: Added Ctrl + (F4/W) to shut down the app when the gui is open."+
                    "\n- Adaptive text scaling by windows is now permanently disabled for the app."+
                    "\n- Add HUDs hud in pitwall tab to display all active huds and some extra information about them."},
        {"0.2.3.6", "- Fix Setup browser from intercepting right clicks." },
        {"0.2.3.4", "- Added Clock HUD to Pitwall Tab: Displays your system time in either 24h or am/pm format."+
                    "\n- Setup Browser: When refreshing the livery browser, the current car and track combo will open in the tree."+
                    "\n- Race Element Settings Tab: Added option to by default generate 4K dds_1 files instead of downsizing them to 2K resolution like the game does."+
                    "\n- ACC Settings Tab: Added ACC Settings only available through json editing."+
                    "\n  - ACC HUD Settings: Allows you to toggle settings like the rating widget."+
                    "\n  - ACC Camera Settings: Allows you to modify helicam settings."+
                    "\n  - ACC Livery Settings: Allows you to toggle settings like texDDS."},
        {"0.2.3.2", "- Fixed Lap Delta Bar HUD Activation."+
                    "\n- Lap Delta Trace HUD: Better data for preview image and more precise adjustment of data collection rate(Herz)."+
                    "\n- Slight rework of Info tab design."},
        {"0.2.3.0", "- Track Corners HUD: Added Circuit Ricardo Tormo Valencia."+
                    "\n- Added Lap Delta Trace HUD: Shows a history of your laptime delta over time."+
                    "\n- Added option for horizontal grid lines for the input trace HUD."+
                    "\n- Main menu: Info tab is now at the bottom."+
                    "\n- Title bar: Added button to access Info tab." },
        {"0.2.2.2", "- Current Gear HUD: Gear shifts are now animated."+
                    "\n- Livery tab: Add Create Livery button, creates files and folders required for a new custom livery."},
        {"0.2.2.0", "- Laptime Table HUD: Invalid laps now show in red."+
                    "\n- Electronics HUD: Repositioned ABS after BB instead of after TC2."+
                    "\n- Tyre Info HUD: Add configurable refreshrate."+
                    "\n- Fix app crash at startup." },
        {"0.2.1.8", "- Track Corner Names: Adjusted Mount Panorama."+
                    "\n- Data Tab: New Design of Laptime Table."+
                    "\n- New Design of Left Side Menu."},
        {"0.2.1.6", "- Radar HUD: Updated scaling."+
                    "\n- Track Corners HUD: Updated trigger that updates the track."+
                    "\n- Fix Brake bias data for Honda NSX GT3 Evo (2019)."+
                    "\n- Fix cleaning of setup importer download cache intervening with importing of setups."},
        {"0.2.1.4", "- Radar HUD: more progress on and design, beta version now."+
                    "\n- Shift Indicator HUD: Pit limiter indication is now animated."},
        {"0.2.1.2", "- Fix Current Gear HUD."+
                    "\n- Add Spotter HUD (Alpha Version)." },
        {"0.2.1.0", "- HUD: Add spectator mode to: Track Corners, Laptime Delta and Current Gear."+
                    "\n- Startup screen now shows version number."},
        {"0.2.0.8", "- Fix HUD Activation panel behaving like a non-clickable area." },
        {"0.2.0.6", "- Shift Indicator: Add adjustable percentages for early and upshift."+
                    "\n- Accelerometer: Increased background color brightness."+
                    "\n- Race Element: You can drag to move the app from empty places within the app."},
        {"0.2.0.4", "- Wheel Slip HUD: Better fidelity between US and OS, both can now be adjusted separately."+
                    "\n- Setup Tab: Copy to other track now uses multi-select."},
        {"0.2.0.2", "- Laptime Delta HUD: text turns red on invalidated lap."+
                    "\n- All steering locks are now displayed in the Settings -> Hardware tab."+
                    "\n- Wheel Slip HUD: Add slip offset to configuration, this allows you to configure the amount of over/under-steer before colors indicate either of them."},
        {"0.2.0.0", "- Fix 296 GT3 steering, now 800." },
        {"0.1.9.8", "- Steering Locks: Added McLaren 720s Evo GT3 and updated Ferrari 296 GT3."+
                    "\n- Setup Viewer: Added support for new McLaren 720s Evo GT3."},
        {"0.1.9.6", "- Reset tyre pressure loss in free practice sessions when visiting the setup screen in-game."+
                    "\n- Fuel Info HUD: Added option to show the hud when viewing the setup screen in-game."+
                    "\n- Tyre Info HUD: Set default amount of decimals to 2."+
                    "\n- Data Tab: Now opens current month automatically in the tree view."},
        {"0.1.9.4", "- Race Element GUI: Set Default Window Opacity to 100%, now only transparent whilst dragging."+
                    "\n- You can now drag and drop setups from the web directly ontop of race element(for example from discord). It will download it for you and show it in the importer."+
                    "\n- Lap Info HUD: removed lap delta bar as there is a lap delta HUD."},
        {"0.1.9.2", "- Add Setup Conversion for Porsche 992 R GT3, Lamborghini Huracán GT3 Evo2 2023 and Ferrari 296 GT3 2023 by andreasmaier." },
        {"0.1.9.0", "- Input Bars HUD: Add configurable refresh rate."+
                    "\n- Added steering locks for porsche 992 gt3, huracan evo2 gt3 and ferrari 296 gt3."+
                    "\n- Added base data for Valencia track."+
                    "\n- Updated tyre pressure HUD."+
                    "\n- Track corners and setup viewer for the new dlc will come soon."},
        {"0.1.8.0", "- Reverted both old and new huracan evo back to gt3 tyres."+
                    "\n- Reworked Accelerometer."+
                    "\n- (BETA) Added traction circle to extended telemetry."},
        {"0.1.7.8", "- Added Wheel Slip HUD: Displaying live wheel slip amount and angle for each wheel."+
                    "\n- Added Track Corners HUD: Displays corner numbers and names for each corner of each track."+
                    "\n- Added Tyre Pressure Loss to Tyre Info HUD, by Mominon."+
                    "\n- (BETA) Added extended telemetry recording. Try this by enabling it in Race Element settings."+
                    "\n- (BETA) Added charts for each lap to Data tab, this for race weekends with extended telemetry enabled."},
        {"0.1.7.6", "- Upgraded to net framework to 4.8.1."+
                    "\n- Reworked usability of HUD Tab."+
                    "\n- Add Electronics HUD showing TC1, TC2, ABS, BB and chosen engine map."+
                    "\n- Added configurable assists color on Input Bars."+
                    "\n- Lap Delta: added option to hide the HUD during race sessions."+
                    "\n- Update button is moved to the titlebar."+
                    "\n- Hovering the update button will now show all the missing releases."},
        {"0.1.7.4", "- HUDs: Centered value texts in Info Panels"+
                    "\n- Track Info HUD: Configurable track temp."+
                    "\n- HUDs should now hide when in menu during Hotlap."+
                    "\n- New font for HUDs."},
        {"0.1.7.3", "- Fix: Input Bars HUD." },
        {"0.1.7.2", "- HUD Tab: Adjusted toggle-button colors when main options are activated, blue for always visible and green for movement mode."+
                    "\n- Input Bars HUD: Add adjustable colors."},
        {"0.1.7.1", "- HUD Tab: Added HUD Categories, you can filter the HUDs with the drop-down menu."+
                    "\n- HUD Tab: Moved the redesigned movement and demo mode toggles to the left bottom."+
                    "\n- HUDs: increased decimals for scale, up to 3 now."+
                    "\n- Steering HUD: Reworked design, added ring thickness option."+
                    "\n- Tyre Info HUD: Tyre temps are now default enabled, added option to hide pressures."+
                    "\n- Added Wind Info HUD: Shows wind direction relative to the car."},
        {"0.1.7.0", "- Data tab: Grouped race weekends by year and month."+
                    "\n- HUDs: Improved overal CPU usage."+
                    "\n- Input Trace HUD: Added configurable line thickness."+
                    "\n- Oversteer Trace HUD: Added configurable line thickness."},
        {"0.1.6.9", "- Fuel Info HUD: Add option to change fuel bar color based on fuel percentage."+
                    "\n- Add Laptime Table HUD: configurable rows and information."},
        {"0.1.6.8", "- HUD:"+
                    "\n  - Fix: Dropdown menus not saving."+
                    "\n  - Input Trace: customizable height."+
                    "\n  - Oversteer Trace:"+
                    "\n    - Customizable height."+
                    "\n    - Increase selectable max slip angle."},
        {"0.1.6.7", "- Added Laptime Delta HUD: a customizable lap delta bar." },
        {"0.1.6.6", "- Data tab: Added context menu to copy race weekends to clipboard and show them in windows explorer."+
                    "\n- Input Bars HUD: Add proper option to either set the bars horizontal of vertical. Vertical is default."+
                    "\n- Inputs HUD is now known as Steering HUD: Added additional text display options and setting color."},
        {"0.1.6.5", "- Setup Browser: Added option to copy a setup json directly to the clipboard."+
                    "\n- Reworked all custom context menus in the app: Added icons and improved design."+
                    "\n- Tyre Info HUD: Reworked tyre temperature text."},
        {"0.1.6.4", "- Internal optimizations."+
                    "\n- Prevent HUDs from showing during replay whilst being in a session."},
        {"0.1.6.3", "- Added color picker to Current Gear HUD."},
        {"0.1.6.2", "- Added splash screen when starting the app."+
                    "\n- Added Current Gear HUD."},
        {"0.1.6.1", "- Hotfix: Fix app crash during movement mode activation." },
        {"0.1.6.0", "- Better precision and clarity for Shift Indicator HUD."+
                    "\n- Improved visibility of IMO/OMI coloring for Tyre Info HUD."+
                    "\n- Improved memory management of HUDs."},
        {"0.1.5.9", "- Shift Indicator HUD:" +
                    "\n  - Added configurable opacity for each color state."+
                    "\n  - Added background for RPM Text where as the font size now scales with bar height."},
        {"0.1.5.8", "- Damage HUD will now display during movement mode even if autohide is enabled."+
                    "\n- Colors of Shift Indicator HUD can now be adjusted."},
        {"0.1.5.7", "- Improved rendering quality of pressures in tyre info hud." },
        {"0.1.5.6", "- Shift Indicator HUD: Revert original base color." },
        {"0.1.5.5", "- HUDs: When the Window toggle is enabled the HUD windows will now be always enabled and allow for better detection by Streaming and VR apps."+
                    "\n- Reworked Tyre Info HUD, added guidelines when repositioning and always display psi."+
                    "\n   - Added option to show 2 decimals for tyre pressures."},
        {"0.1.5.4", "- Reworked Input and Oversteer trace."+
                    "\n- Reworked background of shift indicator."+
                    "\n- Reworked internal structure of the app."+
                    "\n- Updated About Tab."+
                    "\n- Open livery json now opens a new explorer window and selects the livery json file."},
        {"0.1.5.3", "- HUDs: Added Opacity Slider, this will allow you to set the overal transparency for each HUD separately."+
                    "\n- Fix setup hider." },
        {"0.1.5.2", "- Liveries can now be dragged and dropped ontop of the app."+
                    "\n- Liveries Viewer: Added competitor name to viewer if it's used in the livery."+
                    "\n- Further design changes according to rebrand."},
        {"0.1.5.1", "- Reworked Titlebar, Icon and About Tab."+
                    "\n- HUD: Added boost gauge, showing boost percentage."},
        {"0.1.5.0", "- ACC Manager is now known as Race Element."+
                    "\n- Removed request for admin rights, if you have run as admin enabled, disable it."+
                    "\n- You can copy all the entire contents of the ACC Manager folder to the Race Element folder. This to keep all your data and settings."},
        {"0.1.4.1", "- Added request for admin rights, the app uses this to detect whether ACC is running. Without these rights you may experience unexpected app crashes." },
        {"0.1.4.0", "- Reworked hud internals."+
                    "\n- Reduced idle cpu usage to almost 0% when ACC is not running."},
        {"0.1.3.2", "- Fix bug when repositioning huds."},
        {"0.1.3.1", "- Internal optimizations."+
                    "\n- Improved contrast of hud text panels."+
                    "\n- HUDs now show when the engine is not running."},
        {"0.1.3.0", "- Design of Damage HUD, blends in better. Increased Font Size." },
        {"0.1.2.2", "- HUD: Added Damage HUD, displaying body damage in repair time and suspension damage in percentage."+
                    "\n- Added track condition to basic lap telemetry recording."},
        {"0.1.2.1", "- Reworked design of several HUDs."+
                    "\n   - Input bars: Added option to switch around throttle and brake bars."+
                    "\n   - Inputs: Added gradual contrast."+
                    "\n   - Input and Oversteer Traces: Added gradual contrast."},
        {"0.1.2.0", "- Main folder changed from My Documents/ACC Manager to %AppData%/ACC Manager."+
                    "\n   - You can copy the old Data folder containing your race weekends to the new location."+
                    "\n- HUD Configuration:" +
                    "\n   - Redesign of HUD Configuration Controls: Grouped Controls."+
                    "\n   - This resets your HUD Configuration. Unfortunately ;)"+
                    "\n   - Right clicking inside of the configuration controls toggles activation of viewed HUD."+
                    "\n   - Scroll the sliders to change the value."+
                    "\n- Added button to ACC Manager settings to open the ACC Manager folder."},
        {"0.1.1.2", "- HUD: Input bars overlay now has the option to set the bars to horizontal mode."+
                    "\n- HUD: Added beta version of Race Info Overlay."},
        {"0.1.1.1", "- Changed dry tyres for Lamborghini Super Trofeo cars to GT4." },
        {"0.1.1.0", "- Auto Updater added: in the about tab when a new version is released there will be a 1 click button to update the app."+
                    "\n- HUDs: Improvements to maintaining rendering frequency (Thank you FBalazs)."+
                    "\n- ACC Manager: Fixed persistence of minimize app to system tray option."},
        {"0.1.0.5", "- Liveries Tab: Added button to refresh the livery trees."+
                    "\n- ACC Manager: Added option to minimize the app to the system tray (default off)."+
                    "\n- Decrease startup time."+
                    "\n- Added startup counter (global)."},
        {"0.1.0.3", "- HUDs: Added Always On top option to each overlay, allowing you to hide the overlay in the game whilst streaming using the Window option."+
                    "\n- HUD: Added Input Bars Overlay, displaying live throttle and brake inputs with vertical progress bars."+
                    "\n- Liveries: Removed generating, import and export of dds_0 files."},
        {"0.1.0.1", "- OBS Websocket updated to version 5, using the setup hider now requires OBS version 28 or higher."+
                    "\n- Updated design theme."+
                    "\n- HUD tab: the title & description is now 1 big button to toggle hud activation."},
        {"0.0.9.0", "- Added Race Weekends tab to Telemetry tab."+
                    "\n- ACC Manager now saves race weekend data(lap and sector times)."+
                    "\n- Titlebar: Added icons for automatic steering lock and the stream setup hider."+
                    "\n- Setup Importer drag and drop now works on any part of the app."+
                    "\n- Changed order of main menu tabs." },
        {"0.0.8.2", "- HUD: Added Oversteer trace overlay. Displaying under and oversteer."+
                    "\n- HUDs tab: Double click overlays on the list items to toggle overlays on and off."+
                    "\n- Setups tab: Add strategy section to viewer and comparison."+
                    "\n- Internal memory usage enhancements."},
        {"0.0.8.1", "- Setups tab: Right click to copy a setup to another track."+
                    "\n- Setups Viewer: Reworked the table cell alignment."},
        {"0.0.8.0", "- HUD tab: New Design showing previews of overlays."+
                    "\n- Setup Comparison: Highlights differences for each individual value with colors."+
                    "\n- ECU Maps: Updated (By Mominon)."},
        {"0.0.7.9", "- TreeViews: Fixed open/close bug when clicking the leaves."+
                    "\n- Livery Viewer: Increased snappiness and decreased memory usage."+
                    "\n- ACC Settings: Added serverlist so you can manage unlisted servers and quickly enable or disable the serverList.json."+
                    "\n- HUDs: Overlay options will only show when hovering else the description of the overlay will be displayed."+
                    "\n- HUDs: Add a Window option to each overlay, allowing it to be captured by streaming applications."},
        {"0.0.7.8", "- Setups: Added Setup Importer, drag and drop your setup json file in the Setup Page/Tab."+
                    "\n- HUDs: Added a preview mode."+
                    "\n- HUD: Increased performance for Shift Indicator."},
        {"0.0.7.7", "- HUDs: Multiple monitors are now supported." +
                    "\n- HUDs: Increased performance for info tables/panels."},
        {"0.0.7.6", "- ACC Manager now remembers the last main tab you opened."+
                    "\n- ACC Manager now remembers the last position of the GUI."+
                    "\n- HUD: Added Debug Output overlay showing all kind of logging."+
                    "\n- HUD: Increased performance for several overlays."+
                    "\n- HUD: When hovering an overlay in the GUI, spacebar or enter key enabled/disables the overlay."+
                    "\n- Lap tracker: Fixed an issue with finalizing lap data."},
        {"0.0.7.5", "- Setup Conversion: Fixed for McLaren 720s."+
                    "\n- HUD: Shift indicator now draws vertical lines for every 1000 rpm available."},
        {"0.0.7.4", "- Hardware Settings: Added automatic steering hard lock based per car."+
                    "\n- HUD: Made stint info configurable for Fuel Info overlay."+
                    "\n- HUD: Input trace overlay can now be sized up to 800 datapoints (be aware of cpu usage)."},
        {"0.0.7.3", "- Stream settings: Fixed a crash (sigh..)." },
        {"0.0.7.2","- HUD: Current tyre set is now configurable for the Car Info Overlay."+
                   "\n- Stream settings: Added Setup Hider for OBS and Streamlabs."},
        {"0.0.7.1", "- HUDs: Improve rendering performance."+
                    "\n- HUDs: Prevent huds from rendering when pausing during hotlap mode."+
                    "\n- HUD: Added Shift Indicator overlay, including an option to display when the pit limiter is activated."},
        {"0.0.7.0", "- HUD: Added configurable data collection/refresh rate for input trace."+
                    "\n- HUD: Lap delta overlay's recorded laps are now reset in between practice/qualifying/race."},
        {"0.0.6.9", "- HUD: Added configurable potentional best lap time to lap delta overlay."+
                    "\n- HUD: Fixed brake temps multiplication in tyre info overlay."},
        {"0.0.6.8", "- HUDs: Overlays now also render when reposition is enabled."+
                    "\n- HUD: Fixed fuel progress bar for Fuel Info Overlay."},
        {"0.0.6.7", "- HUDs: Improved 'should render' status detection."+
                    "\n- HUD: Fixed average brake temps in Tyre Info Overlay."+
                    "\n- HUD: Added Inputs Overlay by Floriwan."+
                    "\n- Livery exporter: Added option to exclude dds files."},
        {"0.0.6.6", "- HUD: Added configurable data point amount for the Input Trace Overlay."+
                    "\n- HUD: Lap Delta bar outlines positive or negative delta color."+
                    "\n- HUD: Added subtle value background and row lines to info table and panel."+
                    "\n- HUD: Added mouse cursor when repositioning overlays."},
        {"0.0.6.5", "- Liveries tab: Added tree item in tag tree showing all cars that aren't tagged yet."+
                    "\n- HUD: Reposition hotkey has been changed to (Ctrl + Home), it was conflicting with other global hotkeys."+
                    "\n- HUD: Changed font for values in the info panel and added drop shadow."+
                    "\n- HUD: Lap Delta overlay now displays sector delta based on your fastest sectors."},
        {"0.0.6.4", "- HUD: Added hotkey (Ctrl + Shift + M) for enabling the reposition of overlays"+
                    "\n- HUD: Overlays can now be scaled on with 1% steps, providing more precision."+
                    "\n- HUD: Overlays can now be moved with the arrow keys as well for finetuning of position."+
                    "\n- Added startup argument to start in minimized window state: ( /StartMinimized )."+
                    "\n- Added Tyre Info overlay, this can be placed on top of the kunos tyre info."},
        {"0.0.6.3", "- Lap Data Collector: Fix Average Fuel usage calculation after adding new fuel." },
        {"0.0.6.2", "- Integrate broadcast data, available for overlay developers."+
                    "\n- HUD: Added Car Info Overlay."+
                    "\n- HUD: Accelerometer Overlay is now scalable."+
                    "\n- HUD: Fuel Info Overlay:"+
                    "\n    - Fuel bar turns red when fuel tank reaches 15% or lower."+
                    "\n    - Fuel Time is green if you have enough for stint/session and red if not."+
                    "\n    - Suggested fuel no longer shows negative values."+
                    "\n    - Fuel buffer laps now user selectable. Choice of 0-3 laps (Default: 0)"+
                    "\n- HUD: Lap Delta Overlay:"+
                    "\n    - Added configurable Lap Type (in/out/regular)."+
                    "\n    - Added configurable Max Delta value."+
                    "\n    - Added tooltips."+
                    "\n- HUD: Track Info Overlay:" +
                    "\n    - Added configurable Time of Day information."+
                    "\n    - Added tooltips."},
        {"0.0.6.1", "- Improve logging for overlays." },
        {"0.0.6.0", "- HUD Tab:"+
                    "\n    - Scroll the scale control to adjust the scale."+
                    "\n    - Click Scroll/middle mouse button overlay lines to enable reposition."+
                    "\n    - Left Click overlay line to toggle."+
                    "\n- HUD: ECU Map Overlay can now be scaled and repositioned."+
                    "\n- Trees in the GUI now expand on single click."},
        {"0.0.5.9", "- About Tab: Added information about HUDs."+
                    "\n- Added Play ACC Button, launches Assetto Corsa Competizione using the steam link."+
                    "\n- HUD: Added Fuel Info Overlay (By KrisV147)."+
                    "\n- HUD: Added Lap Delta Overlay."+
                    "\n- HUD Tab: Some overlays are now scalable."},
        {"0.0.5.8", "- HUD Tab: Added design."+
                    "\n- HUD: Added option to input trace for disabling steering input."},
        {"0.0.5.7", "- HUD: Added Track Info overlay."+
                    "\n- HUD: Added Accelerometer overlay."+
                    "\n- HUD: Some overlays can be configured."},
        {"0.0.5.6", "- HUD: Certain overlays can now be repositioned, this position will be automatically saved."+
                    "\n- HUD: Enabled overlays will be re-enabled during the next start of ACC Manager."},
        {"0.0.5.5", "- The Livery browser will re-expand previously expanded items after refreshing."+
                    "\n- Added hotkey(Delete) to quickly open up the delete livery interface."},
        {"0.0.5.4", "- Decrease draw rate for tyre pressure trace by 90%."+
                    "\n- Updated system.net.http reference due to possible vulnerability."},
        {"0.0.5.3", "- Added screen that pops-up after importing liveries, allowing you to tag them."+
                    "\n- Setup conversions added by KrisV147:"+
                    "\n - BMW M2 Cup 2020."+
                    "\n - Lamborghini Huracán ST Evo2 2021."+
                    "\n - Porsche 992 GT3 Cup 2021."+
                    "\n - All existing in-game cars are now supported for the setup viewer/comparison."},
        {"0.0.5.2", "- Setup conversion added: Ferrari 488 Challenge Evo 2020(By KrisV147)."+
                    "\n- Tyre pressure trace: now auto-detects gt3/gt4(gtc)/wet tyres."+
                    "\n- Tyre pressure trace: draws specific parts in the trace by color, green = good, red is too high, blue is too low."},
        {"0.0.5.1", "- Setup conversions added:"+
                    "\n - Audi R8 LMS Evo 2019."+
                    "\n - Bentley Continental GT3 2015."+
                    "\n - BMW M6 GT3 2017."+
                    "\n - Lamborghini Gallardo G3 Reiter 2017."+
                    "\n - Lamborghini Huracán ST 2015."+
                    "\n - McLaren 650S GT3 2015."+
                    "\n - Mercedes-AMG GT3 2015."+
                    "\n - Porsche 911 II GT3 Cup 2017." },
        {"0.0.5.0", "- Added ECU map HUD (bottom-right), still very simple look."+
                    "\n- Repositioned Input Trace HUD."+
                    "\n- Added upper and lower bounds to Tires Traces (working for gt3 dry only atm)."},
        {"0.0.4.9", "- Setup conversions added by KrisV147:"+
                    "\n - Audi R8 LMS GT4 2016."+
                    "\n - BMW M4 GT4 2018."+
                    "\n - Chevrolet Camaro GT4 R 2017."+
                    "\n - Ginetta G55 GT4 2012."+
                    "\n - Ktm Xbow GT4 2016."+
                    "\n - Maserati Gran Turismo MC GT4 2016."+
                    "\n - McLaren 570s GT4 2016."+
                    "\n - Mercedes AMG GT4 2016."+
                    "\n - Porsche 718 Cayman GT4 MR 2019."+
                    "\n- Livery Browser: displays sub-item count in tree header." +
                    "\n- HUD: Tire pressure input trace changed."},
        {"0.0.4.8", "- HUD tab: Added overlays: Very very simple tire pressure trace overlay and a shared memory static data overlay." },
        {"0.0.4.7", "- Added HUD tab: Simple checkbox to enable the input trace overlay." },
        {"0.0.4.6", "- Setup conversion added: Aston Martin V12 Vantage GT3 2013(by KrisV147), Alpine A110 GT4(by KrisV147) and Aston Martin Vantage AMR GT4 2018(by KrisV147)." },
        {"0.0.4.5", "- Input Trace: Should be drawing less gpu cost." },
        {"0.0.4.4", "- Telemetry Debug: Add test overlay for input traces. Enabled with Draw On Game checkbox in Telemetry Debug (doesn't work with full-screen mode)." },
        {"0.0.4.3", "- Livery Displayer: Sponsors Image is now displayed ontop of Decals image."+
                    "\n- Livery Displayer: Revamped layout to increase size of livery preview."+
                    "\n- Livery Importer: Improved feedback for user when importing an unsupported archive."+
                    "\n- Livery Tagger: When adding tags to cars, only the Tags tab in the Livery Browser will be updated."+
                    "\n- Setup conversion added: Emil Frey Jaguar G3 2012."},
        {"0.0.4.2", "- Livery Tags: Cars under a tag are now alphabetically sorted."+
                    "\n- ACCM: Allow window resizing."+
                    "\n- Livery Displayer: preview images scale with window resizing."},
        {"0.0.4.1", "- Livery Tags: Cannot enter an empty tag name anymore."+
                    "\n- Livery Browser: Now displays liveries without a team name set."},
        {"0.0.4.0", "- Livery Browser: Add livery tagging system." },
        {"0.0.3.9", "- Livery Deleter: shows the to be deleted skin in the livery displayer."+
                    "\n- Add new app icon."+
                    "\n- Fix issue with importer which caused acc manager to keep a file stream open."+
                    "\n- Add more botched import scenarios for livery archives that only contain 1 livery."},
        {"0.0.3.8", "- Setup conversion added: Aston Martin V8 Vantage GT3 2019(by FBalazs)."+
                    "\n- Livery Browser: added context menu option to delete liveries."},
        {"0.0.3.7", "- Setups tab: Car names have the year added."+
                    "\n- Setups tab: Added button to right click menu to open car directory."+
                    "\n- Setups tab: Added button to browser tab to refresh the setup list."+
                    "\n- Setup conversion added: Lamborghini Huracán GT3 2015, Nissan GT-R Nismo GT3 2015, Ferrari 488 GT3 2018."+
                    "\n- Livery Importer: Added 1 file filter for all supported types."+
                    "\n- Livery Importer: Add strategy for botched archives (containing no separate folders)."},
        {"0.0.3.6", "- About tab: When a new release is available a new button allows you to visit the latest release and download the newest version." },
        {"0.0.3.5", "- About tab: Added buttons for Discord and GitHub."+
                    "\n- Bulk DDS Generator: generate button is disabled when all DDS files have been generated."},
        {"0.0.3.4", "- Added Bulk DDS Generator: Got many skins without dds files? this will generate the dds files. Sit back and relax." },
        {"0.0.3.3", "- Livery browser: added two tabs for grouping style, Car model type and Teams."+
                    "\n- Livery viewer: fix crash when generating dds files."},
        {"0.0.3.2", "- Fuel calcalator: added button to fetch data from game."+
                    "\n- Livery browser: fix for fetching the liveries to display them in the list."},
        {"0.0.3.1", "- Telemetry: Display Array data" },
        {"0.0.3.0", "- Add Telemetry tab, very basic for now, might still have some data bugs, also not all data is shown as data yet."+
                    "\n- Add car model name to livery viewer."},
        {"0.0.2.10","- Add features panel to about tab" },
        {"0.0.2.9", "- Setup conversion added: Bentley Continental GT3 2018, Lamborghini Huracán GT3 Evo, Lexus RC F GT3, Nissan GT-R Nismo GT3 2018." },
        {"0.0.2.8", "- Added exception handling so the manager will not crash for windows version lower than 11." },
        {"0.0.2.7", "- Livery exporter: Added skin pack exporter, right click teams or skins to add."+
                    "\n- ACC Manager Folder: Added dedicated folder for the acc Manager, My Documents/ACC Manager."+
                    "\n- Added logger which logs to the dedicated folder/Log."},
        {"0.0.2.6", "- Icon: Something not too fancy, but it shows what the tool does."+
                    "\n- Setup conversion: fixed Porsche 911 II GT3."},
        {"0.0.2.5", "- Livery Displayer: Added button to generate dds files."+
                    "\n- Livery Exporter: Now includes dds files if available."},
        {"0.0.2.4", "- Livery Displayer: Added buttons to open livery json and livery folder."+
                    "\n- Livery Displayer: Added nationality, Paint materials for body and rims."},
        {"0.0.2.3", "- Add setup conversion for: Mercedes-AMG GT3 2020, BMW M4 GT3, Ferrari 488 GT3 Evo."+
                    "\n- Fuel calculator: fixed fuel value parsing when windows language is set differently."},
        {"0.0.2.2", "- Livery exporter: exported zips are now according to acc folder structure."+
                    "\n- Livery exporter: export team with all the skins inside in a single zip."+
                    "\n- Livery importer: added support for archives with multiple liveries."},
        {"0.0.2.1", "- Livery Browser: Skins are grouped by team name." },
        {"0.0.2.0", "- Add Livery Browser, displays: team name, display name, car number, decals image and sponsors image."+
                    "\n- Add Livery exporter: right click livery to save as zip."+
                    "\n- Add Livery importer: supports (.7z, .rar, .zip), can import multiple archives at once, though multiple liveries per archive is not supported yet."},
        {"0.0.1.5", "- Right click track in setup browser to open folder in file explorer." },
        {"0.0.1.4", "- Add Simple Fuel Calculator." },
        {"0.0.1.3", "- Setup comparison now highlights setup when different."+
                    "\n- Add setup conversion for: Audi R8 LMS, Honda NSX GT3, Porsche 911 GT3 R."},
        {"0.0.1.2", "- Added more icons."+
            "\n- Added custom Titlebar."},
        {"0.0.1.1", "- Setup Browser added."+
                    "\n- Setup Comparison added: Right click setups in the setup browser to add them to the comparison."+
                    "\n- Add setup conversion for: Audi R8 LMS evo II(by Jubka), Honda NSX GT3 Evo, Mclaren 720S GT3, Porsche II GT3 R."},
        {"0.0.1.0", "- Material Design added." },
    };
}
