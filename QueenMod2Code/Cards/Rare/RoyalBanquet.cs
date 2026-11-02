using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Cards.Uncommon;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class RoyalBanquet() : QueenMod2Card(2,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(13M, ValueProp.Move),
        new DynamicVar("HiveBound", 1M)
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(QueenMod2Keywords.HiveTip),
        HoverTipFactory.FromKeyword(QueenMod2Keywords.Hivebound)
    ];  
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        foreach (CardModel card in CardPile.GetCards(Owner, PileType.Hand))
        {
            if (card.Keywords.Contains(QueenMod2Keywords.Hive))
            {
                CardCmd.ApplySingleTurnRetain(card);
            }
        }
    }
    
    public override Task AfterAutoPrePlayPhaseEnteredLate(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (CardPile.GetCards(this.Owner, PileType.Draw).Contains(this))
        {
            return triggerHiveBoundEffect(choiceContext);
        }
        return Task.CompletedTask;
    }

    public async Task triggerHiveBoundEffect(PlayerChoiceContext choiceContext)
    {
        //CardCmd.Preview(stingers, 0.4f, CardPreviewStyle.GridLayout);
        CardModel hiveModel;
        if (CombatState == null)
            return;
        switch (Owner.RunState.Rng.CombatCardGeneration.NextInt(0, 4))
        {
            case 0:
                hiveModel = (CardModel)CombatState.CreateCard<Hornet>(Owner);
                break;
            case 1:
                hiveModel = (CardModel)CombatState.CreateCard<Bumblebee>(Owner);
                break;
            case 2:
                hiveModel = (CardModel)CombatState.CreateCard<Drone>(Owner);
                break;
            case 3:
                hiveModel = (CardModel)CombatState.CreateCard<Workerbee>(Owner);
                break;
            default:
                return;
        }
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(hiveModel, PileType.Hand, Owner));
        CardCmd.Preview(this, 1f, CardPreviewStyle.EventLayout);
    }

        
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3M);
    }
}