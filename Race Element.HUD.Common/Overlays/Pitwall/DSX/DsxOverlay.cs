﻿using Newtonsoft.Json;
using RaceElement.Data.Games;
using RaceElement.HUD.Overlay.Internal;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static RaceElement.HUD.Common.Overlays.Pitwall.DSX.Resources;

namespace RaceElement.HUD.Common.Overlays.Pitwall.DSX;

[Overlay(Name = "DSX",
    Description = "Adds active triggers for the DualSense 5 controller using DSX on steam.\n See Guide in the Discord of Race Element for instructions.",
    OverlayCategory = OverlayCategory.Inputs,
    OverlayType = OverlayType.Pitwall,
    Game = Game.RaceRoom | Game.AssettoCorsa1,
    Authors = ["Reinier Klarenberg"]
)]
internal sealed class DsxOverlay : CommonAbstractOverlay
{
    internal readonly DsxConfiguration _config = new();
    private DsxJob _dsxJob;

    internal UdpClient _client;
    internal IPEndPoint _endPoint;
    private DateTime _timeSent;

    public DsxOverlay(Rectangle rectangle) : base(rectangle, "DSX")
    {
        Width = 1; Height = 1;
        RefreshRateHz = 1;
        AllowReposition = false;
    }

    public override void BeforeStart()
    {
        if (IsPreviewing) return;

        _dsxJob = new DsxJob(this) { IntervalMillis = 1000 / 200 };
        _dsxJob.Run();
    }
    public override void BeforeStop()
    {
        if (IsPreviewing) return;

        _dsxJob?.CancelJoin();
        _client?.Close();
        _client?.Dispose();
    }

    public override bool ShouldRender() => DefaultShouldRender() && !IsPreviewing;

    public override void Render(Graphics g) { }

    internal void SetLighting()
    {
        Debug.WriteLine("Changing RGB");

        int controllerIndex = 0;

        DsxPacket p = new()
        {
            Instructions = [new() { Type = InstructionType.RGBUpdate, Parameters = [controllerIndex, 255, 69, 0] }]
        };

        Send(p);
        ServerResponse lightingReponse = Receive();
        if (lightingReponse != null)
        {
            HandleResponse(lightingReponse);
        }
    }

    internal void CreateEndPoint()
    {
        _client = new UdpClient();
        _endPoint = new IPEndPoint(Triggers.localhost, _config.UDP.Port);
    }

    internal void Send(DsxPacket data)
    {
        var RequestData = Encoding.ASCII.GetBytes(Triggers.PacketToJson(data));
        _client?.Send(RequestData, RequestData.Length, _endPoint);
        _timeSent = DateTime.Now;
    }

    private ServerResponse? Receive()
    {
        byte[] bytesReceivedFromServer = _client.Receive(ref _endPoint);

        if (bytesReceivedFromServer.Length > 0)
        {
            ServerResponse ServerResponseJson = JsonConvert.DeserializeObject<ServerResponse>($"{Encoding.ASCII.GetString(bytesReceivedFromServer, 0, bytesReceivedFromServer.Length)}");
            return ServerResponseJson;
        }

        return null;
    }

    private void HandleResponse(ServerResponse response)
    {
        if (response != null)
        {
            Debug.WriteLine("===================================================================");

            Debug.WriteLine($"Status: {response.Status}");
            TimeSpan Timespan = DateTime.Now - _timeSent;
            // First send shows high Milliseconds response time for some reason
            Debug.WriteLine($"Time Received: {response.TimeReceived}, took: {Timespan.TotalMilliseconds} to receive response from DSX");
            Debug.WriteLine($"isControllerConnected: {response.isControllerConnected}");
            Debug.WriteLine($"BatteryLevel: {response.BatteryLevel}");

            Debug.WriteLine("===================================================================\n");
        }
    }
}
