using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Character;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Patches;

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardPlayed))]
class SwarmDistributionPatch
{
    [HarmonyPostfix]
    static void Postfix(ICombatState combatState,
            PlayerChoiceContext choiceContext,
            CardPlay cardPlay, ref Task __result)
    {
        MainFile.Logger.Info("Executing SwarmDistributionPatch.Postfix");
        CardModel card = cardPlay.Card;
        int prevSwarmAmount = SwarmController.TotalSwarmAmount;
        SwarmController.Instance.CalculateSwarm(card);
        MainFile.Logger.Info("Different Swarm Amount" + (prevSwarmAmount != SwarmController.TotalSwarmAmount).ToString());
        __result = SwarmController.Instance.DistributeSwarm(choiceContext, card, prevSwarmAmount != SwarmController.TotalSwarmAmount);
    }
}