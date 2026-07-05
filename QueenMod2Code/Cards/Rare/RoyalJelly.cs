using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class RoyalJelly() : QueenMod2Card(1,
    CardType.Power, CardRarity.Rare,
    TargetType.AnyEnemy)
{    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<RoyalJellyPower>(2M),
        new PowerVar<Pollinated>(1M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<Pollinated>()
    ];  
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        PowerCmd.Apply<Pollinated>(choiceContext, play.Target, DynamicVars["Pollinated"].BaseValue,
            Owner.Creature, this, false);
        PowerCmd.Apply<RoyalJellyPower>(choiceContext, Owner.Creature, DynamicVars["RoyalJellyPower"].BaseValue,
            Owner.Creature, this, false);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["RoyalJellyPower"].UpgradeValueBy(1M);
        DynamicVars["Pollinated"].UpgradeValueBy(1M);
    }
}