using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Common;

public class Infest() : QueenMod2Card(0,
    CardType.Skill, CardRarity.Common,
    TargetType.None)
{
    protected override bool HasEnergyCostX => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new("Swarm", 3M)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower(ModelDb.Power<Swarm>())
    ];  
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Infest order = this;
        int x = order.ResolveEnergyXValue();
        for (int i = 0; i < x; i++)
        {
            await PowerCmd.Apply<Swarm>(choiceContext, order.Owner.Creature,
                DynamicVars["Swarm"].BaseValue,
                order.Owner.Creature, (CardModel)this);
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["Swarm"].UpgradeValueBy(1M);
    }
}