using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using QueenMod2.QueenMod2Code.Cards;

namespace QueenMod2.QueenMod2Code.Patches;

[HarmonyPatch(typeof(NHandCardHolder), nameof(NHandCardHolder.UpdateCard))]
class CardGlowPatch
{
    [HarmonyPrefix]
    static bool Prefix(ref NHandCardHolder __instance)
    {
        if ((__instance.CardModel is QueenMod2Card))
        {
            QueenMod2Card card = (QueenMod2Card)__instance.CardModel;
            if (card.HasCustomGlowColor)
            {
                if (!__instance.IsNodeReady() || __instance.CardNode == null)
                    return false;
                __instance.CardNode.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
                if (!CombatManager.Instance.IsInProgress)
                    return false;
                __instance.CardNode.CardHighlight.AnimShow();
                __instance.CardNode.CardHighlight.Modulate = card.customGlowColor;
                return false;
            }
        }
        return true;
    }
}
