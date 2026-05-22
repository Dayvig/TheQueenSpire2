using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Character;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Patches;

[HarmonyPatch(typeof(PowerCmd), nameof(PowerCmd.Apply), [typeof(PlayerChoiceContext), typeof(PowerModel), typeof(Creature), typeof(Decimal), typeof(Creature), typeof(CardModel), typeof(bool)])]
class ConditionalPowerTypeChange
{
    [HarmonyPrefix]
    static void Prefix(PowerModel power, Creature target,
        Decimal amount,
        Creature? applier,
        CardModel? cardSource,
        bool silent)
    {
        if (power is QueenMod2Power targetPower)
        {
            targetPower.setConditionalType(target);
        }
    }
}