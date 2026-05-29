using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using QueenMod2.QueenMod2Code.Cards;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Generated;

public class Honeycomb() : QueenMod2Card(0,
    CardType.Skill, CardRarity.Token,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(1M),
        new CalculationExtraVar(1M),
        new CalculatedVar("EnergyGain").WithMultiplier((card, _) => card.Owner.Creature.GetPowerAmount<ExtraChamberPower>())
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PlayerCmd.GainEnergy(DynamicVars["EnergyGain"].BaseValue, this.Owner);
    }

    protected override void OnUpgrade()
    {

    }
}