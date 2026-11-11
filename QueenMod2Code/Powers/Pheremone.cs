using BaseLib.Abstracts;
using BaseLib.Extensions;
using QueenMod2.QueenMod2Code.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Character;

namespace QueenMod2.QueenMod2Code.Powers;

public class Pheremone : QueenMod2Power
{
    public override void setConditionalType(Creature target)
    {
        base.setConditionalType(target);
        conditionalType = target.IsEnemy ? PowerType.Debuff : PowerType.Buff;
    }
    
    //Loads from QueenMod2/images/powers/your_power.png
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new("Decrement", 1)
    ];
    
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == Owner.Side)
            return;
        Pheremone power = this;
            if (power.Owner.IsAlive)
                await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), (PowerModel)power, -power.DynamicVars["Decrement"].BaseValue,
                    (Creature)null, (CardModel)null);
            else
                await Cmd.CustomScaledWait(0.1f, 0.25f);
    }
    
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource){
        await SwarmController.Instance.DistributeSwarm(new ThrowingPlayerChoiceContext(), Owner.CombatState, Owner, null);
    }

    /// <summary>
    /// Hook that runs after this power is removed from its owner.
    /// <param name="oldOwner">The owner of this power before it was removed.</param>
    /// </summary>
    public override async Task AfterRemoved(Creature oldOwner)
    {
        if (oldOwner.IsDead)
        {
            return;
        }
        await SwarmController.Instance.DistributeSwarm(new ThrowingPlayerChoiceContext(), Owner.CombatState, Owner, null);
    }


    public override PowerType Type => conditionalTypeSet ? conditionalType : PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override Color AmountLabelColor => _normalAmountLabelColor;
    
}