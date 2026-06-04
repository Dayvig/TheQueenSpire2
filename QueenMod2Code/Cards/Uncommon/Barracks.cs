using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class Barracks() : QueenMod2Card(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard(ModelDb.Card<Hornet>())
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new("Hornets", 2)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        for (int i = 0; i < DynamicVars["Hornets"].IntValue; i++)
        {
            await CardPileCmd.AddGeneratedCardToCombat((CardModel)CombatState.CreateCard<Hornet>(Owner),
                PileType.Hand, play.Card.Owner);
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

    protected override void OnUpgrade()
    {
        this.RemoveKeyword(CardKeyword.Exhaust);
    }

    public async Task triggerHiveBoundEffect(PlayerChoiceContext choiceContext)
    {
        CardModel model = (CardModel)this;
        //CardCmd.Preview(model, 0.4f, CardPreviewStyle.GridLayout);
        await PowerCmd.Apply<BarracksPower>(choiceContext, Owner.Creature, 1M, Owner.Creature, model, false);
    }
}