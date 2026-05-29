using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Cards.Generated;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class HornetSwarm() : QueenMod2Card(2,
    CardType.Skill, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        (DynamicVar) new CalculationBaseVar(0M),
        (DynamicVar) new CalculationExtraVar(1M),
        new CalculatedVar("CalculatedHornets").WithMultiplier((((Func<CardModel, Creature, Decimal>) (
            (card, _) =>
            {
                Decimal hornetCount = 0;
                foreach (CardModel model in PileType.Hand.GetPile(card.Owner).Cards.Concat(PileType.Draw.GetPile(card.Owner).Cards))
                {
                    if (model.Id.Equals(ModelDb.Card<Hornet>().Id))
                    {
                        hornetCount++;
                    }
                }
                return hornetCount;
            }
        ))!))
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        HornetSwarm swarm = this;
        Creature target = play.Target;
        List<Hornet> toPlay = new List<Hornet>();
        foreach (CardModel model in CardPile.GetCards(swarm.Owner, PileType.Draw).Concat(CardPile.GetCards(swarm.Owner, PileType.Hand)))
        {
            if (model.Id.Equals(ModelDb.Card<Hornet>().Id))
            {
                toPlay.Add((Hornet)model);
            }
        }
        
        foreach (Hornet model in toPlay)
        {
            await CardCmd.AutoPlay(choiceContext, model, play.Target);
            if (!IsUpgraded)
            {
                await CardCmd.Exhaust(choiceContext, model, false, true);
            }
        }

    }
    
    protected override void OnUpgrade()
    {
    }
}