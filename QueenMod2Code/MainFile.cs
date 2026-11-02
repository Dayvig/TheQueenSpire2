using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using QueenMod2.QueenMod2Code.Extensions;
using QueenUI = QueenMod2.QueenMod2Code.Nodes.QueenUI;

namespace QueenMod2.QueenMod2Code;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "QueenMod2"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public QueenUI QueenUI { get; private set; }
    
    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        harmony.PatchAll();
    }

    public enum DanceStep
    {
        ATTACK,
        SKILL,
        POWER
    }
    
    public enum StrategizeType
    {
        ATTACK,
        BLOCKSKILL,
        UTILITYSKILL,
        POWER
    }
    
    
}