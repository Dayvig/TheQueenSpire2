using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Cards.Rare;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Generated;

public class Honeycomb() : QueenMod2Card(0,
    CardType.Skill, CardRarity.Token,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(1M),
        new CalculationExtraVar(1M),
        new CalculatedVar("EnergyGain").WithMultiplier((card, _) =>
        {
            List<CardModel> weapons = new List<CardModel>();
            List<CardModel> cards = new List<CardModel>();
            foreach (CardModel model in PileType.Hand.GetPile(card.Owner).Cards)
            {
                if (model.Id.Equals(ModelDb.Card<CannonXL>().Id))
                {
                    weapons.Add(model);
                }
            }
            return (weapons.Count + card.Owner.Creature.GetPowerAmount<ExtraChamberPower>());
        })
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        foreach (CardModel model in PileType.Hand.GetPile(Owner).Cards)
        {
            if (model.Id.Equals(ModelDb.Card<CannonXL>().Id))
            {
                MainFile.Logger.Info("Hit");
            }
        }
        CalculatedVar EnergyG = (CalculatedVar)DynamicVars["EnergyGain"];
        Decimal val = EnergyG.Calculate(play.Target);
        await PlayerCmd.GainEnergy(val, this.Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(1);
    }
}