using BaseLib.Extensions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;
using QueenMod2.QueenMod2Code.Cards.Uncommon;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Patches;

[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.AddGeneratedCardsToCombat),
[
    typeof(IEnumerable<CardModel>), typeof(PileType), typeof(Player), typeof(CardPilePosition)
])]
class BusyBeesPatch
{
    [HarmonyPostfix]
    static void Postfix(IEnumerable<CardModel> cards,
        PileType newPileType,
        Player? creator,
        CardPilePosition position = CardPilePosition.Bottom)
    {
        MainFile.Logger.Info("This shit is running");
        if (creator != null && creator.HasPower<BusyPower>() &&
            (newPileType == PileType.Draw || newPileType == PileType.Discard))
        {
            for (int i = 0; i < creator.Creature.GetPowerAmount<BusyPower>(); i++)
            {
                MainFile.Logger.Info("Adding extra copies");
                List<CardModel> newCopies = new List<CardModel>();
                foreach (CardModel model in cards)
                {
                    newCopies.Add(model.CreateClone());
                }
                AddExtraCopies(newCopies, newPileType, creator, position);
            }
        }
    }

    private static async Task<IReadOnlyList<CardPileAddResult>> AddExtraCopies(IEnumerable<CardModel> cards,
        PileType newPileType,
        Player? creator,
        CardPilePosition position = CardPilePosition.Bottom)
    {
        List<CardModel> list = cards.ToList<CardModel>();
        if (list.Count == 0)
            return (IReadOnlyList<CardPileAddResult>)Array.Empty<CardPileAddResult>();
        if (!CombatManager.Instance.IsInProgress)
            return (IReadOnlyList<CardPileAddResult>)Array.Empty<CardPileAddResult>();
        if (list.Any<CardModel>((Func<CardModel, bool>)(c => c.Pile != null)))
            throw new InvalidOperationException("You are not allowed to generate cards that already have a pile");
        if (!newPileType.IsCombatPile())
            throw new InvalidOperationException("You are not allowed to added generated cards to a non combat pile");
        ICombatState combatState = list[0].Owner.Creature.CombatState;
        if (combatState == null)
            return (IReadOnlyList<CardPileAddResult>)Array.Empty<CardPileAddResult>();
        List<CardPileAddResult> results = new List<CardPileAddResult>();
        foreach (CardModel card in list)
        {
            CombatManager.Instance.History.CardGenerated(combatState, card, creator);
            List<CardPileAddResult> cardPileAddResultList = results;
            MainFile.Logger.Info("Adding and previewing copies");
            cardPileAddResultList.Add(await CardPileCmd.Add(card, newPileType.GetPile(card.Owner), position));
            CardCmd.PreviewCardPileAdd(cardPileAddResultList, 0.8f, CardPreviewStyle.HorizontalLayout);
            cardPileAddResultList = (List<CardPileAddResult>)null;
            await Hook.AfterCardGeneratedForCombat(combatState, card, creator);
        }
        return (IReadOnlyList<CardPileAddResult>)results;
    }
}
    