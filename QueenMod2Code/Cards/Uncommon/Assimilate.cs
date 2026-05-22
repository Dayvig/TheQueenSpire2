using System.ComponentModel;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class Assimilate() : QueenMod2Card(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ("Swarm", 5M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower(ModelDb.Power<Swarm>()),
        HoverTipFactory.FromKeyword(QueenMod2Keywords.HiveTip)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Assimilate order = this;
        List<CardModel> toExhaust = new List<CardModel>();
        foreach (CardModel model in CardPile.GetCards(play.Card.Owner, PileType.Hand))
        {
            if (model.Keywords.Contains(QueenMod2Keywords.Hive))
            {
                toExhaust.Add(model);
            }
        }
        await PowerCmd.Apply<Swarm>(choiceContext, order.Owner.Creature, toExhaust.Count * DynamicVars["Swarm"].IntValue, order.Owner.Creature, (CardModel) order);
        foreach (CardModel model in toExhaust)
        {
            CardCmd.Exhaust(choiceContext, model);
        }
    }
    protected override void OnUpgrade()
    {
        DynamicVars["Swarm"].UpgradeValueBy(1M);
    }
}