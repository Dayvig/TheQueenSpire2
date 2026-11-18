using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Cards;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.DepreciatedCards;

public class Strategic() : QueenMod2Card(1,
    CardType.Skill, CardRarity.None,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0M),
        new CalculationExtraVar(1M),
        new CalculatedVar("TemporaryStrength").WithMultiplier((Func<CardModel, Creature, Decimal>) ((card, _) => CardPile.Get(PileType.Hand, card.Owner).Cards.Count))
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<StrategicGeniusPower>(choiceContext, Owner.Creature, ((CalculatedVar) DynamicVars["TemporaryStrength"]).Calculate(Owner.Creature), Owner.Creature, (CardModel) this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

}