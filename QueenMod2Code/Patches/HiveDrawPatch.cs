using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Character;

namespace QueenMod2.QueenMod2Code.Patches;
[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardDrawn))]
class HiveDrawPatch
{
    [HarmonyPrefix]
    static void Prefix(PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw, ref Task __result)
    {
        if (card.Keywords.Contains(QueenMod2Keywords.Hive))
        {
            __result = CardPileCmd.Draw(choiceContext, 1, card.Owner);
        }
    }
}