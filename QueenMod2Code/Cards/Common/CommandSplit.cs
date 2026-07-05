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
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Cards;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Common;

public class CommandSplit() : QueenMod2Card(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(8M, ValueProp.Move),
        new ("Pheremones", 2)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<Pheremone>(),
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CommandSplit order = this;
        ArgumentNullException.ThrowIfNull((object) play.Target, "play.Target");
            AttackCommand attackCommand = await DamageCmd.Attack(order.DynamicVars.Damage.BaseValue).FromCard((CardModel) order, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
            
        await PowerCmd.Apply<Pheremone>(choiceContext, play.Target,
            DynamicVars["Pheremones"].IntValue,
            order.Owner.Creature, (CardModel)this);
        
        await PowerCmd.Apply<Pheremone>(choiceContext, order.Owner.Creature,
            DynamicVars["Pheremones"].IntValue,
            order.Owner.Creature, (CardModel)this);

    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
        DynamicVars["Pheremones"].UpgradeValueBy(1);
    }
}