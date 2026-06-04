using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Hooks;
using QueenMod2.QueenMod2Code.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace QueenMod2.QueenMod2Code.Powers;

public class Swarm : QueenMod2Power
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

    public Decimal getTotalAmount(Creature owner)
    {
        Decimal modifiedAmount = this.Amount;
        if (owner.HasPower(ModelDb.Power<Pheremone>().Id))
        {
            modifiedAmount += owner.GetPower<Pheremone>().Amount;
        }
        
        return modifiedAmount;
    }

    public override bool ShouldPlayVfx
    {
        get
        {
            return false;
        }
    }

    public override PowerType Type => conditionalTypeSet ? conditionalType : PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override Color AmountLabelColor => _normalAmountLabelColor;
    
    public override async Task BeforeSideTurnEndEarly(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        Swarm swarmPower = this;
        if (side != swarmPower.Owner.Side)
            return;
        if (side == CombatSide.Player)
        {
            swarmPower.Flash();
            Decimal num = await CreatureCmd.GainBlock(swarmPower.Owner,
                (Decimal)swarmPower.getTotalAmount(swarmPower.Owner), ValueProp.Unpowered, (CardPlay)null);
        }
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        Swarm power = this;
        if (side == CombatSide.Enemy && participants.Contains<Creature>(power.Owner))
        {
            await CreatureCmd.Damage((PlayerChoiceContext)new ThrowingPlayerChoiceContext(), power.Owner,
                (Decimal)power.getTotalAmount(power.Owner), ValueProp.Unblockable | ValueProp.Unpowered, (Creature)null,
                (CardModel)null);
            if (power.Owner.IsAlive)
                await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), (PowerModel)power, -power.DynamicVars["Decrement"].BaseValue,
                    (Creature)null, (CardModel)null);
            else
                await Cmd.CustomScaledWait(0.1f, 0.25f);
        }
        if (side == CombatSide.Player)
        {
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), (PowerModel)power, -power.DynamicVars["Decrement"].BaseValue,
                (Creature)null, (CardModel)null);
        }
    }
    
    public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(
        HealthBarForecastContext context)
    {
        if (Type.Equals(PowerType.Debuff))
        {
            return
            [
                new HealthBarForecastSegment(Amount, new Color(0.9f, 0.85f, 0f), HealthBarForecastDirection.FromRight, 10,
                    null, null)
            ];
        }
        return (IEnumerable<HealthBarForecastSegment>) Array.Empty<HealthBarForecastSegment>();
    }

    
    public override async Task AfterDeath(
        PlayerChoiceContext choiceContext,
        Creature target,
        bool wasRemovalPrevented,
        float deathAnimLength)
    {
        Swarm power = this;
        if (power.Applier.IsDead || target != power.Owner)
            return;
        power.Flash();
        MainFile.Logger.Info("Attempting to apply"+power.Amount+" Swarm to" + power.Applier.LogName);
        await PowerCmd.Apply<Swarm>(choiceContext, power.Applier, power.Amount, power.Applier, (CardModel) null);
    }
}