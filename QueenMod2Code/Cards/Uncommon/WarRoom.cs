using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Formatters;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class WarRoom() : QueenMod2Card(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(6M, ValueProp.Move),
        new ("StrategizeValue", 2),
        new StrategizeVar(0, true)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(QueenMod2Keywords.Strategize)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        WarRoom room = this;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        List<CardModel> eligableCards = CardPile.GetCards(Owner, PileType.Draw)
            .Where((card) => MeetsCriteria(card, (StrategizeVar)DynamicVars["Strategize"])).ToList();
        for (int i = 0; i < DynamicVars["StrategizeValue"].IntValue; i++)
        {
            if (eligableCards.Count <= 0)
            {
                return;
            }
            int nextRng = RunState.Rng.Niche.NextInt(0, eligableCards.Count());
            CardPileAddResult cardPileAddResult = await CardPileCmd.Add(eligableCards[nextRng], PileType.Draw, CardPilePosition.Top);
            eligableCards.Remove(eligableCards[nextRng]);
            await CardPileCmd.Draw(choiceContext, 1, room.Owner);

        }
    }

    private bool MeetsCriteria(CardModel card, StrategizeVar? var)
    {
        if (var == null)
        {
            return false;
        }
        switch (var.TypeList[var.place])
        {
            case MainFile.StrategizeType.ATTACK:
                return card.Type.Equals(CardType.Attack);
            case MainFile.StrategizeType.BLOCKSKILL:
                return card.Type.Equals(CardType.Skill) && card.GainsBlock;
            case MainFile.StrategizeType.UTILITYSKILL:
                return card.Type.Equals(CardType.Skill) && !card.GainsBlock;
            case MainFile.StrategizeType.POWER:
                return card.Type.Equals(CardType.Power);
        }

        return false;
    }
    
    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!cardPlay.Card.Owner.Equals(Owner))
            return Task.CompletedTask;
       
        StrategizeVar strat = (StrategizeVar)DynamicVars["Strategize"];
        strat.place++;
        if (strat.place >= strat.TypeList.Count)
        {
            strat.place = 0;
        }
        return Task.CompletedTask;
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2M);
    }
}