using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class Coccoon() : QueenMod2Card(3,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<Swarm>(10M),
        new PowerVar<DefensePheremone>(5M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [        
        HoverTipFactory.FromPower(ModelDb.Power<Swarm>()),
        HoverTipFactory.FromPower(ModelDb.Power<DefensePheremone>())
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Coccoon coccoon = this;
        await PowerCmd.Apply<Swarm>(choiceContext, coccoon.Owner.Creature, DynamicVars["Swarm"].BaseValue, coccoon.Owner.Creature, this, false);
        await PowerCmd.Apply<DefensePheremone>(choiceContext, coccoon.Owner.Creature, DynamicVars["DefensePheremone"].BaseValue, coccoon.Owner.Creature, this, false);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["Swarm"].UpgradeValueBy(4M);
        DynamicVars["DefensePheremone"].UpgradeValueBy(1M);
    }
}