using System.ComponentModel;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Common;

public class Mark() : QueenMod2Card(0,
    CardType.Skill, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ("Vulnerable", 1)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower(ModelDb.Power<VulnerablePower>()),
        HoverTipFactory.FromPower(ModelDb.Power<Pheremone>())
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Mark order = this;
        await PowerCmd.Apply<VulnerablePower>(choiceContext, play.Target,
            DynamicVars["Vulnerable"].BaseValue,
            order.Owner.Creature, (CardModel)this);
        
        await PowerCmd.Apply<Pheremone>(choiceContext, play.Target,
            DynamicVars["Vulnerable"].BaseValue,
            order.Owner.Creature, (CardModel)this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars["Vulnerable"].UpgradeValueBy(1);
    }
}