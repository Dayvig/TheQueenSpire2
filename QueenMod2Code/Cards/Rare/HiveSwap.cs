using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using QueenMod2.QueenMod2Code.Cards.Generated;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class HiveSwap() : QueenMod2Card(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(0)
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(QueenMod2Keywords.HiveTip)
    ];  
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        HiveSwap swap = this;
        IEnumerable<CardModel> draw = CardPile.GetCards(swap.Owner, PileType.Draw);
        IEnumerable<CardModel> hand = CardPile.GetCards(swap.Owner, PileType.Hand);
        IEnumerable<CardModel> discard = CardPile.GetCards(swap.Owner, PileType.Discard);

        await SwapHiveCards(draw, PileType.Draw);
        await SwapHiveCards(hand, PileType.Hand);
        await SwapHiveCards(discard, PileType.Discard);

        if (IsUpgraded)
        {
            await CardPileCmd.Draw(choiceContext, play.Card.Owner);
        }
    }

    private async Task SwapHiveCards(IEnumerable<CardModel> pile, PileType pileType)
    {
        HiveSwap swap = this;
        List<CardModel> toRemove = new List<CardModel>();
        List<CardModel> toAdd = new List<CardModel>();

        foreach (CardModel model in pile)
        {
            if (model.Id.Equals(ModelDb.Card<Hornet>().Id))
            {
                toRemove.Add(model);
                Bumblebee newCard = swap.CombatState.CreateCard<Bumblebee>(swap.Owner);
                if (model.IsUpgraded){CardCmd.Upgrade(newCard);}
                if (model.Enchantment != null && model.Enchantment.CanEnchant(newCard))
                {
                    newCard.EnchantInternal(model.Enchantment, model.Enchantment.Amount);
                }
                toAdd.Add(newCard);
            }
            if (model.Id.Equals(ModelDb.Card<Bumblebee>().Id))
            {
                toRemove.Add(model);
                Hornet newCard = swap.CombatState.CreateCard<Hornet>(swap.Owner);
                if (model.IsUpgraded){CardCmd.Upgrade(newCard);}
                if (model.Enchantment != null && model.Enchantment.CanEnchant(newCard))
                {
                    newCard.EnchantInternal(model.Enchantment, model.Enchantment.Amount);
                }
                toAdd.Add(newCard);
            }
        }

        foreach (CardModel model in toRemove)
        {
            Control playContainer = NCombatRoom.Instance.Ui.PlayContainer;
            NPlayerHand hand = NCombatRoom.Instance.Ui.Hand;
            if (pileType == PileType.Hand)
            {
                NCard? onTable = NCard.FindOnTable(model);
                if (onTable != null && !NodeUtil.IsDescendant((Node)playContainer, (Node)onTable))
                {
                    hand.Remove(model);
                }
                else
                {
                    Node parent = onTable.GetParent();
                    if (parent != null)
                        parent.RemoveChildSafely((Node)onTable);
                }
            }
            model.RemoveFromCurrentPile(true);
        }

        foreach (CardModel newCard in toAdd)
        {
            await CardPileCmd.AddGeneratedCardToCombat(newCard, pileType, this.Owner);
        }
    }
        
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}