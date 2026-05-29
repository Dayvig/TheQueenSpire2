using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class AbsolutePower() : QueenMod2Card(2,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<StrengthPower>(5M)
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, DynamicVars.Strength.BaseValue,
            Owner.Creature, this, false);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Strength.UpgradeValueBy(2M);
    }
}