using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class PheremoneSwap() : QueenMod2Card(0,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(3M, ValueProp.Move),
        new DamageVar(3M, ValueProp.Move)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<Pheremone>(),
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        AttackCommand attackCommand = await DamageCmd.Attack(DynamicVars.Damage.IntValue).FromCard(this, play).
            Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        int DefenseToApply = 0;
        int OffenseToApply = 0;
        if (Owner.HasPower<Pheremone>())
        {
            OffenseToApply = Owner.Creature.GetPowerAmount<Pheremone>();
            await PowerCmd.Remove<Pheremone>(Owner.Creature);
        }
        if (play.Target.HasPower<Pheremone>())
        {
            DefenseToApply = play.Target.GetPowerAmount<Pheremone>();
            await PowerCmd.Remove<Pheremone>(play.Target);
        }
        if (DefenseToApply > 0)
        {
            await PowerCmd.Apply<Pheremone>(choiceContext, Owner.Creature,
                DefenseToApply, Owner.Creature, this, false);
        }
        if (OffenseToApply > 0)
        {
            await PowerCmd.Apply<Pheremone>(choiceContext, play.Target,
                OffenseToApply, Owner.Creature, this, false);
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1M);
        DynamicVars.Block.UpgradeValueBy(1M);
    }
}