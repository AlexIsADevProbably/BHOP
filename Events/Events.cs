using System;
using System.Collections.Generic;
using CustomPlayerEffects;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
using LabApi.Events.CustomHandlers;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using PlayerRoles.FirstPersonControl; // Required for base game physics
using UnityEngine;
using Logger = LabApi.Features.Console.Logger; // Required for Vector3 math
namespace BHOP;

public class Events : CustomEventsHandler
{
    private Dictionary<string, (byte Combo, float LastJumpTime, int Jumps)> _bhopData = new();

    private string _watermarkText;

    private Dictionary<string, Hint> _playerBhopHints = new();
    private readonly Dictionary<Player, Hint> _activeWatermarks = new();
    
    private void PlayCustomAudio(Player player, string clipName)
    {
        try 
        {
            AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"BHOP_{player.Nickname}", onIntialCreation: (p) =>
            {        
                p.transform.parent = player.GameObject.transform;

                Speaker speaker = p.AddSpeaker("Main", isSpatial: true, minDistance: 5f, maxDistance: 15f);

                speaker.transform.parent = player.GameObject.transform;
                
                speaker.transform.localPosition = Vector3.zero;
            });

            audioPlayer.AddClip(clipName);
        }
        catch (Exception ex)
        {
            LabApi.Features.Console.Logger.Error($"Failed to play custom audio {clipName}: {ex}");
        }
    }
    
    public override void OnPlayerJumped(PlayerJumpedEventArgs ev)
    {
        ev.Player.EnableEffect<SilentWalk>(255, 0);
        ev.Player.StaminaRemaining = 1f; 

        string userId = ev.Player.UserId;
        float currentTime = Time.time;
        byte currentCombo = 0;
        int currentJumps = 0;
        
        if (_bhopData.TryGetValue(userId, out var data))
        {
            if (currentTime - data.LastJumpTime <= 1.0f) 
            {
                currentCombo = data.Combo;
                currentJumps = data.Jumps;
            }
        }
        byte elcombo = (byte)Math.Min(currentCombo + 2, 255);
        int ActualCombo = Math.Min(currentJumps + 1, 50000);

        _bhopData[userId] = (elcombo, currentTime, ActualCombo);

        ev.Player.EnableEffect<MovementBoost>(elcombo, 1.5f);
        float currentSpeed = 0f;
        if (ev.Player.ReferenceHub.roleManager.CurrentRole is IFpcRole fpcRole)
        {
            Vector3 velocity = fpcRole.FpcModule.Motor.Velocity;
            currentSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude;
        }

        string ratingName;
        string ratingColor;

        if (elcombo >= 250)     { ratingName = "OMNIPOTENT";   ratingColor = "#00ffff"; PlayCustomAudio(ev.Player, "aura"); }
        else if (elcombo >= 220){ ratingName = "TRANSCENDENT"; ratingColor = "#ff00ff"; PlayCustomAudio(ev.Player, "aura"); }
        else if (elcombo >= 190){ ratingName = "ASCENDED";     ratingColor = "#9933ff"; PlayCustomAudio(ev.Player, "aura"); }
        else if (elcombo >= 160){ ratingName = "GODLIKE";      ratingColor = "#ffd700"; PlayCustomAudio(ev.Player, "aura"); }
        else if (elcombo >= 130){ ratingName = "UNREAL";       ratingColor = "#39ff14"; PlayCustomAudio(ev.Player, "aura"); }
        else if (elcombo >= 100){ ratingName = "INSANE";       ratingColor = "#ff0000"; PlayCustomAudio(ev.Player, "aura"); }

        else if (elcombo >= 75) { ratingName = "SICK";         ratingColor = "#00ced1"; PlayCustomAudio(ev.Player, "boing"); }
        else if (elcombo >= 50) { ratingName = "FLAWLESS";     ratingColor = "#ff3333"; PlayCustomAudio(ev.Player, "boing"); }
        else if (elcombo >= 35) { ratingName = "PERFECT";      ratingColor = "#ffaa00"; PlayCustomAudio(ev.Player, "boing"); }
        else if (elcombo >= 20) { ratingName = "GREAT";        ratingColor = "#33cc33"; PlayCustomAudio(ev.Player, "boing"); }
        else if (elcombo >= 10) { ratingName = "GOOD";         ratingColor = "#3399ff"; PlayCustomAudio(ev.Player, "boing"); }
        else                     { ratingName = "NICE";         ratingColor = "#ffffff"; PlayCustomAudio(ev.Player, "boing" ); }
        
        PlayerDisplay.Get(ev.Player).ShowHint(new Hint {
            Text = $"<b><color={ratingColor}>{ratingName}</color></b>\n<i>Speed: {currentSpeed:F2} m/s | Combo: {ActualCombo}</i>",
            Alignment = HintAlignment.Center,
            YCoordinate = 600f
        }, 0.55f); 
    }
    
    public override void OnPlayerLeft(PlayerLeftEventArgs ev)
    {
        _bhopData.Remove(ev.Player.UserId);
    }
}
