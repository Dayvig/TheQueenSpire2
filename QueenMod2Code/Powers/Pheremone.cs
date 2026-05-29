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
        new("Decrement", 2)
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
    
    public override PowerType Type => conditionalTypeSet ? conditionalType : PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override Color AmountLabelColor => _normalAmountLabelColor;
    
}