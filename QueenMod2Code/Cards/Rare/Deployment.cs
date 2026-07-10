using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class Deployment() : QueenMod2Card(0,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyAlly)
{ 
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(0)
    ];
    
    public override CardMultiplayerConstraint MultiplayerConstraint
    {
        get => CardMultiplayerConstraint.MultiplayerOnly;
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (play.Target == null)
        {
            return;
        }

        List<CardModel> Hive = new List<CardModel>();
        foreach (CardModel m in CardPile.GetCards(play.Card.Owner, PileType.Hand).Where((model => model.Keywords.Contains(QueenMod2Keywords.Hive))))
        {
            Hive.Add(m);
        }
        foreach (CardModel model in Hive)
        {
            //model.ModifyCardPlayResultPileTypeAndPosition(model, false play.Target.Player, PileType.Hand)
            await Deployment.GiveToAnotherPlayer(model, play.Target.Player, PileType.Hand);
            CardPileAddResult cardPileAddResult = await CardPileCmd.Add(model, PileType.Draw, CardPilePosition.Random);
            //await RemoveAndReplaceHiveCard(model);
            if (IsUpgraded)
            {
                await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, this.Owner);
            }
        }
    }

    public async Task RemoveAndReplaceHiveCard(CardModel m)
    {
        Control playContainer = NCombatRoom.Instance.Ui.PlayContainer;
        NPlayerHand hand = NCombatRoom.Instance.Ui.Hand;
        NCard? onTable = NCard.FindOnTable(m);
        if (onTable != null && !NodeUtil.IsDescendant((Node)playContainer, (Node)onTable))
        {
            hand.Remove(m);
        }
        else
        {
            Node parent = onTable.GetParent();
            if (parent != null)
                parent.RemoveChildSafely((Node)onTable);
        }
        m.RemoveFromCurrentPile(true);
    }

    public static async Task<IEnumerable<CardModel>> CreateInHand(
        Player owner,
        int count,
        ICombatState combatState,
        CardModel model)
    {
        if (count == 0)
            return (IEnumerable<CardModel>) Array.Empty<CardModel>();
        if (CombatManager.Instance.IsOverOrEnding)
            return (IEnumerable<CardModel>) Array.Empty<CardModel>();
        List<CardModel> shivs = new List<CardModel>();
        for (int index = 0; index < count; ++index)
            shivs.Add((CardModel)combatState.CreateCard(model.CreateCloneForPlayer(owner), owner));
        IReadOnlyList<CardPileAddResult> combat = await CardPileCmd.AddGeneratedCardsToCombat((IEnumerable<CardModel>) shivs, PileType.Hand, owner);
        return (IEnumerable<CardModel>) shivs;
    }
    
    protected override void OnUpgrade()
    {
        
    }
    
    public static async Task GiveToAnotherPlayer(
        CardModel card,
        Player player,
        PileType pileType,
        CardPilePosition position = CardPilePosition.Bottom,
        AbstractModel? clonedBy = null)
    {
        var cardNode = NCard.FindOnTable(card);
        card.RemoveFromCurrentPile(true);
        card.GiveToAnotherPlayer(player);
        var isLocalPlayerTheReceivingPlayer = LocalContext.IsMine(card);
        await CardPileCmd.Add([card], pileType.GetPile(player), position, clonedBy, true, true);
        if (cardNode == null || !cardNode.IsValid())
            return;
        
        var vfxContainer = card.Owner.Creature.GetVfxContainer();
        cardNode.Reparent(vfxContainer);
        if (isLocalPlayerTheReceivingPlayer)
        {
            if (card.Pile == null) return;
            var cardPileType = card.Pile.Type;
            var child = NCardFlyVfx.Create(cardNode, cardPileType, true, card.Owner.Character.TrailPath);
            vfxContainer?.AddChildSafely(child);

            if (cardPileType == PileType.Hand)
            {
                var newCardNode = PublicCreateCardNodeAndUpdateVisuals(card, pileType, true);
                var handNode = NCombatRoom.Instance?.Ui.Hand;
                handNode?.Add(newCardNode);
            }
        }
        else
        {
            var child = NCardFlyVfx.Create(cardNode, player.Creature, card.Owner.Character.TrailPath);
            vfxContainer?.AddChildSafely(child);
        }
    }
    
    private static NCard PublicCreateCardNodeAndUpdateVisuals(
        CardModel card,
        PileType targetPileType,
        bool owningPlayerIsLocal)
    {
        NCard andUpdateVisuals = NCard.Create(card);
        NCombatRoom.Instance.Ui.AddChildSafely((Node) andUpdateVisuals);
        andUpdateVisuals.UpdateVisuals(targetPileType, CardPreviewMode.Normal);
        if (!owningPlayerIsLocal)
            andUpdateVisuals.Position = NCombatRoom.Instance.GetCreatureNode(card.Owner.Creature).IntentContainer.GlobalPosition;
        else if (card.Pile != null)
            andUpdateVisuals.Position = card.Pile.Type.GetTargetPosition(andUpdateVisuals);
        else
            andUpdateVisuals.Position = targetPileType.GetTargetPosition(andUpdateVisuals);
        return andUpdateVisuals;
    }
    
}