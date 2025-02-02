using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using IksAdminApi;

namespace AdminsConvert;

public class PluginConfig : PluginCFG<PluginConfig>
{
    public static PluginConfig Config = new PluginConfig();
    public Dictionary<string, string[]> FlagsConvert {get; set;} = new()
    {
        {"z", ["@css/root", "@css/rcon"]},
        {"b", ["@css/ban"]},
        {"m", ["@css/mute"]},
        {"g", ["@css/gag"]}
    };
    public bool ConvertImmunity {get; set;} = true; // Конвертировать иммунитет?
    public bool ConvertGroup {get; set;} = true; // Конвертировать группу? Группа Admin => #css/Admin

    public static void Set()
    {
        Config = Config.ReadOrCreate(Main.Instance.ModuleDirectory + "/../../configs/plugins/IksAdmin_Modules/convert_flags.json", Config);
    }
}