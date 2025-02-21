using System.Runtime.CompilerServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using IksAdminApi;

namespace AdminsConvert;

public class Main : AdminModule
{
    public override string ModuleName => "FlagsConvert";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "iks__";
    public override string ModuleDescription => "for IksAdmin 3.0";
    Dictionary<CCSPlayerController, List<string>> _deleteFlags = new();
    Dictionary<CCSPlayerController, uint> _defaultImmunities = new();
    public override void Ready()
    {
        Instance = this;
        PluginConfig.Set();
    }

    [GameEventHandler]
    public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (player == null || player.AuthorizedSteamID == null) return HookResult.Continue;
        var admin = player.Admin();
        if (admin == null) return HookResult.Continue;
        if (PluginConfig.Config.ConvertImmunity)
        {
            _defaultImmunities.Add(player, AdminManager.GetPlayerImmunity(player));
            AdminManager.SetPlayerImmunity(player, (uint)admin.CurrentImmunity);
        }
        if (PluginConfig.Config.ConvertGroup && admin.Group != null)
        {
            AdminManager.AddPlayerToGroup(player, [$"#css/{admin.Group.Name}"]);
        }
        foreach (var item in PluginConfig.Config.FlagsConvert)
        {
            var flag = item.Key;
            var cssFlags = item.Value;
            if (admin.CurrentFlags.Contains(flag))
            {
                AdminUtils.LogDebug(cssFlags.ToString()!);
                AdminManager.AddPlayerPermissions(player, cssFlags);
                if (!_deleteFlags.ContainsKey(player))
                    _deleteFlags.Add(player, cssFlags.ToList());
                else {
                    foreach (var f in cssFlags)
                    {
                        _deleteFlags[player].Add(f);
                    }
                }
            }   
        }
        AdminUtils.LogDebug("Immunity:" + AdminManager.GetPlayerImmunity(player).ToString());
        AdminUtils.LogDebug("Flags:" + AdminManager.GetPlayerAdminData(player)!.Flags.Values.ToString());
        return HookResult.Continue;
    }
    
    [GameEventHandler]
    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (player == null || player.IsBot || !player.IsValid || player.AuthorizedSteamID == null) return HookResult.Continue;

        var admin = player.Admin();
        if (admin == null) return HookResult.Continue;
        if (_defaultImmunities.TryGetValue(player, out var defaultImmunity))
        {
            AdminManager.SetPlayerImmunity(player, defaultImmunity);
            _defaultImmunities.Remove(player);
        }
        if (PluginConfig.Config.ConvertGroup && admin.Group != null)
        {
            AdminManager.RemovePlayerFromGroup(player, true, [$"#css/{admin.Group.Name}"]);
        }
        if (_deleteFlags.TryGetValue(player, out var flagsForDelete))
        {
            AdminManager.RemovePlayerPermissions(player, flagsForDelete.ToArray());
            _deleteFlags.Remove(player);
        }   
        return HookResult.Continue;
    }
    
}
