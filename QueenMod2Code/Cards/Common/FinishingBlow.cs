using System.ComponentModel;
using MegaCrit.Sts2.Core.Combat;
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

public class FinishingBlow() : QueenMod2Card(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    public static Decimal hitsThisTurn = 0;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(7M),
        new ExtraDamageVar(3M),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(((Func<CardModel, Creature, Decimal>) ((card, _) => hitsThisTurn))!)
    ];
    
    public override Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        MainFile.Logger.Info("Log: "+ dealer?.LogName + target.LogName + target.CombatState.CurrentSide);
        if (dealer != null && dealer.Equals(Owner.Creature) && !props.Equals(ValueProp.Unblockable) && target.CombatState.CurrentSide == CombatSide.Player)
        {
            hitsThisTurn++;
        }
        return Task.CompletedTask;
    }
    
    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            hitsThisTurn = 0;
        }
    }
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        FinishingBlow blow = this;
        AttackCommand attackCommand = await DamageCmd.Attack(blow.DynamicVars.CalculatedDamage).FromCard((CardModel) blow).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash", tmpSfx: "heavy_attack.mp3").Execute(choiceContext);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.ExtraDamage.UpgradeValueBy(1M);
    }
}