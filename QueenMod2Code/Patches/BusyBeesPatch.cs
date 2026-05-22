using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Patches;

[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.AddGeneratedCardsToCombat))]
class BusyBeesPatch
{
    [HarmonyPrefix]
    static void Prefix(IEnumerable<CardModel> cards,
        PileType newPileType,
        Player? creator,
        CardPilePosition position, ref Task __result)
    {
        AddAdditionalCards(cards, newPileType, creator, position);
    }



    private static async Task<IReadOnlyList<CardPileAddResult>> AddAdditionalCards(IEnumerable<CardModel> cards,
        PileType newPileType, Player? creator,
        CardPilePosition position = CardPilePosition.Bottom)
    {
        if (creator != null && creator.HasPower<BusyPower>() && newPileType == PileType.Draw)
        {
            List<CardModel> extraCards = new List<CardModel>();
            for (int i = 0; i < creator.Creature.GetPowerAmount<BusyPower>(); i++)
            {
                foreach (CardModel model in cards)
                {
                    extraCards.Add(model.CreateClone());
                }
            }

            List<CardModel> list = extraCards;
                if (list.Count == 0)
                    return (IReadOnlyList<CardPileAddResult>)Array.Empty<CardPileAddResult>();
                if (!CombatManager.Instance.IsInProgress)
                    return (IReadOnlyList<CardPileAddResult>)Array.Empty<CardPileAddResult>();
                if (list.Any<CardModel>((Func<CardModel, bool>)(c => c.Pile != null)))
                    throw new InvalidOperationException(
                        "You are not allowed to generate cards that already have a pile");
                if (!newPileType.IsCombatPile())
                    throw new InvalidOperationException(
                        "You are not allowed to added generated cards to a non combat pile");
                ICombatState combatState = creator.Creature.CombatState;
                if (combatState == null)
                    return (IReadOnlyList<CardPileAddResult>)Array.Empty<CardPileAddResult>();
                List<CardPileAddResult> results = new List<CardPileAddResult>();
                foreach (CardModel card in list)
                {
                    CombatManager.Instance.History.CardGenerated(combatState, card, creator);
                    List<CardPileAddResult> cardPileAddResultList = results;
                    cardPileAddResultList.Add(await CardPileCmd.Add(card, newPileType.GetPile(card.Owner), position));
                    await Hook.AfterCardGeneratedForCombat(combatState, card, creator);
                }
        }
        return (IReadOnlyList<CardPileAddResult>)Array.Empty<CardPileAddResult>();
    }
}
